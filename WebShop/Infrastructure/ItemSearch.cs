using Syrilium.CommonInterface;
using Syrilium.DataAccessInterface;
using Syrilium.DataAccessInterface.SQL;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Infrastructure
{
    public enum ComparisonOperator
    {
        [EnumStringValue("1")]
        Exists,
        [EnumStringValue("2")]
        Equals,
        [EnumStringValue("3")]
        Greater,
        [EnumStringValue("4")]
        Less,
        [EnumStringValue("5")]
        GreaterThanOrEqual,
        [EnumStringValue("6")]
        LessThanOrEqual,
        [EnumStringValue("7")]
        Like,
        [EnumStringValue("8")]
        NotExistsOrEquals,
    }

    public class ItemSearch
    {
        public int NumberOfRecords { get; private set; }
        public int RecordsPerPage { get; set; }
        public int Page { get; set; }
        public int RecordFrom
        {
            get
            {
                return RecordTo - RecordsPerPage + 1;
            }
        }
        public int RecordTo
        {
            get
            {
                return RecordsPerPage * Page;
            }
        }
        public int NumberOfPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)NumberOfRecords / (decimal)RecordsPerPage);
            }
        }

        public ItemSearch()
        {
            Page = 1;
            RecordsPerPage = 50;
        }

        public static int? GetPage(NameValueCollection parameters)
        {
            string pageParamValue = CommonHelpers.GetQueryStringParamValue(parameters, "page");
            if (!string.IsNullOrWhiteSpace(pageParamValue))
            {
                int pageFromParams;
                if (int.TryParse(pageParamValue.Trim(), out pageFromParams))
                    return pageFromParams;
            }
            return null;
        }

        public List<IItem> Find(NameValueCollection parameters, int? parentId = null, int? recordsPerPage = null, int? page = null)
        {
            List<SearchParameter> searchParameters = SearchParameter.Get(parameters);
            List<SortParameter> sortParameters = SortParameter.Get(parameters);

            if (page == null)
                page = GetPage(parameters);

            return Find(searchParameters, sortParameters, parentId, recordsPerPage, page);
        }

        public virtual List<IItem> Find(List<SearchParameter> searchParameters = null, List<SortParameter> sortParameters = null, int? parentId = null, int? recordsPerPage = null, int? page = null)
        {
            if (recordsPerPage != null) RecordsPerPage = recordsPerPage.Value;
            if (page != null) Page = page.Value;

            SySqlParameter sqlQueryParam = new SySqlParameter();
            sqlQueryParam.ParameterName = "@statement";
            List<SySqlParameter> queryParameters;
            sqlQueryParam.Value = GenerateSQLQuery(out queryParameters, searchParameters, sortParameters, parentId, RecordFrom, RecordTo);
            queryParameters.Insert(0, sqlQueryParam);
            DataSet ds = Module.I<IQuery>().GetDataSetWithProcedure(SPNames.ExecuteSql, queryParameters);

            NumberOfRecords = (int)ds.Tables[0].Rows[0][0];
            ds.Tables.RemoveAt(0);

            return Module.I<IItem>().Get(ds);
        }

        public static string GenerateSQLQuery(out List<SySqlParameter> queryParameters, List<SearchParameter> searchParameters = null, List<SortParameter> sortParameters = null, int? parentId = null, int from = 1, int to = 1000)
        {
            if (searchParameters == null) searchParameters = new List<SearchParameter>();
            if (sortParameters == null) sortParameters = new List<SortParameter>();

            StringBuilder query = new StringBuilder();

            if (parentId.HasValue)
                query.Append(string.Concat(@"DECLARE @ParentNode HIERARCHYID
								select @ParentNode=Node from Item where Id=", parentId));

            query.Append(@"DECLARE	@tblProductNodes TABLE
						(
						  Node HIERARCHYID,
						  RN INT
						)
						INSERT	INTO @tblProductNodes
						SELECT	Node,
								ROW_NUMBER() OVER ( ORDER BY ");

            if (sortParameters.Count == 0)
            {
                query.Append("Id desc");
            }
            else
            {
                int i = 0;
                foreach (var sp in sortParameters)
                {
                    query.Append(string.Concat((i > 0 ? ", " : ""), "[", sp.Attribute.Id, "] ", (sp.SortDirection == SortDirection.Ascending ? "asc" : "desc")));
                    i++;
                }
            }

            query.Append(@" ) as RN
						FROM	dbo.Item i");

            query.Append(generateJoinsAndWhere(searchParameters, sortParameters, parentId.HasValue, out queryParameters));

            query.Append(@" DECLARE	@tblProductsToSelect TABLE ( Node HIERARCHYID )
						INSERT	INTO @tblProductsToSelect
								SELECT	Node
								FROM	@tblProductNodes");
            query.Append(string.Concat(" where RN between ", from, " and ", to));
            query.Append(@" ORDER BY RN

						SELECT	COUNT(Node)
						FROM	@tblProductNodes

						SELECT	i.Id,
								i.TypeId,
								i.ParentId
						FROM	dbo.Item i
						INNER JOIN @tblProductsToSelect pn ON i.ObjectNode = pn.Node

						SELECT	i.Id,
								i.TypeId,
								i.ItemId,
								i.Value
						FROM	dbo.ItemDataInt i
						INNER JOIN @tblProductsToSelect pn ON i.ObjectNode = pn.Node
															  AND i.TypeId IN (	0,51,");
            int languageId = SessionState.I.LanguageId;
            query.Append(languageId);
            query.Append(@" )
						SELECT	i.Id,
								i.TypeId,
								i.ItemId,
								i.Value
						FROM	dbo.ItemDataString i
						INNER JOIN @tblProductsToSelect pn ON i.ObjectNode = pn.Node
															  AND i.TypeId IN ( 0,");
            query.Append(languageId);
            query.Append(@" )
						SELECT	i.Id,
								i.TypeId,
								i.ItemId,
								i.Value
						FROM	dbo.ItemDataDecimal i
						INNER JOIN @tblProductsToSelect pn ON i.ObjectNode = pn.Node
															  AND i.TypeId IN ( 0,");
            query.Append(languageId);
            query.Append(@" )
						SELECT	i.Id,
								i.TypeId,
								i.ItemId,
								i.Value
						FROM	dbo.ItemDataDateTime i
						INNER JOIN @tblProductsToSelect pn ON i.ObjectNode = pn.Node
															  AND i.TypeId IN ( 0,");
            query.Append(languageId);
            query.Append(@" )");

            return query.ToString();
        }

        private static string generateJoinsAndWhere(List<SearchParameter> searchParameters, List<SortParameter> sortParameters, bool hasParent, out List<SySqlParameter> queryParameters)
        {
            List<IAttributeDefinition> attributes = new List<IAttributeDefinition>();
            searchParameters.ForEach(p =>
            {
                if (!attributes.Contains(p.Attribute))
                    attributes.Add(p.Attribute);
            });
            sortParameters.ForEach(p =>
            {
                if (!attributes.Contains(p.Attribute))
                    attributes.Add(p.Attribute);
            });

            queryParameters = new List<SySqlParameter>();
            StringBuilder query = new StringBuilder();
            StringBuilder parametersDeclarationString = new StringBuilder();

            StringBuilder where = new StringBuilder();
            where.Append(@" WHERE i.TypeId = 3");
            if (hasParent)
                where.Append(@" and i.Node.IsDescendantOf(@ParentNode) = 1");

            int languageId = SessionState.I.LanguageId;

            //ako je department ili grupa skrivena
            where.Append(string.Concat(@" AND NOT EXISTS ( SELECT TOP 1
								                g.Id
						                 FROM	dbo.Item g
						                 INNER JOIN dbo.ItemDataInt a ON a.ObjectNode = g.Node
						                 INNER JOIN dbo.ItemDataInt v ON v.ItemId = a.ItemId
						                 WHERE	i.Node.IsDescendantOf(g.Node) = 1
								                AND g.TypeId IN ( 1, 2 )
								                AND a.TypeId = 51
								                AND a.Value = ", Module.I<IAttributeLocator>()[AttributeKeyEnum.Show].Value, @"
								                AND v.TypeId IN ( 0, ", languageId, @" )
								                AND v.Value = 0 )"));

            foreach (var attr in attributes)
            {
                bool hasSortParameter = sortParameters.Exists(p => p.Attribute == attr);
                List<SearchParameter> searchParametersForAttribute = searchParameters.Where(p => p.Attribute == attr).ToList();

                if (searchParametersForAttribute.Count() > 0)
                {
                    SearchParameter existsSP = searchParametersForAttribute.Find(p => p.ComparisonOperator == ComparisonOperator.Exists || p.ComparisonOperator == ComparisonOperator.NotExistsOrEquals);
                    if (existsSP != null
                        && (existsSP.ComparisonOperator == ComparisonOperator.NotExistsOrEquals || !existsSP.Value))
                    {
                        query.Append(@" OUTER APPLY ( SELECT TOP 1 ");
                        if (existsSP.ComparisonOperator == ComparisonOperator.NotExistsOrEquals)
                        {
                            query.Append(string.Concat("value.Value AS [", attr.Id, "]"));
                            string parameterName = string.Concat("@", attr.Id);
                            where.Append(string.Concat(@" and ([", attr.Id, "] is null or [", attr.Id, "] = ", parameterName, ")"));

                            parametersDeclarationString.Append(string.Concat((parametersDeclarationString.Length > 0 ? ", " : ""), parameterName, " ", GetSqlDataType(attr.DataType)));
                            SySqlParameter queryParam = new SySqlParameter();
                            queryParam.ParameterName = parameterName;
                            queryParam.Value = existsSP.Value;
                            queryParameters.Add(queryParam);
                        }
                        else
                        {
                            if (hasSortParameter)
                                query.Append(string.Concat("value.Value AS [", attr.Id, "]"));
                            else
                                query.Append(string.Concat("1 as [", attr.Id, "]"));
                            where.Append(string.Concat(@" and [", attr.Id, "] is null"));
                        }
                    }
                    else
                    {
                        query.Append(@" CROSS APPLY ( SELECT TOP 1 ");
                        if (hasSortParameter)
                            query.Append(string.Concat("value.Value AS [", attr.Id, "]"));
                        else
                            query.Append(string.Concat("1 as [", attr.Id, "]"));
                    }
                }
                else
                {
                    query.Append(string.Concat(@" OUTER APPLY ( SELECT TOP 1 value.Value AS [", attr.Id, @"]"));
                }

                string tableName = GetItemDataTableName(attr.DataType);
                query.Append(string.Concat(@" FROM		dbo.ItemDataInt attribute
								  INNER JOIN dbo.", tableName, @" value ON attribute.ObjectNode = i.Node
																		 AND attribute.TypeId = 51
																		 AND attribute.Value = ", attr.Id,
                                                                         @" AND value.ItemId = attribute.ItemId
																		 AND value.TypeId IN ( 0, ", languageId, " )"));

                int i = 1;
                foreach (var sp in searchParametersForAttribute)
                {
                    if (sp.ComparisonOperator == ComparisonOperator.Exists || sp.ComparisonOperator == ComparisonOperator.NotExistsOrEquals) continue;

                    string parameterName = string.Concat("@", attr.Id, "_", i);
                    parametersDeclarationString.Append(string.Concat((parametersDeclarationString.Length > 0 ? ", " : ""), parameterName, " ", GetSqlDataType(attr.DataType)));

                    SySqlParameter queryParam = new SySqlParameter();
                    queryParam.ParameterName = parameterName;
                    queryParam.Value = sp.Value;
                    queryParameters.Add(queryParam);
                    query.Append(string.Concat(" and value.Value ", getQueryComparisonWithParameter(sp.ComparisonOperator, parameterName)));
                    i++;
                }

                query.Append(string.Concat(") [", attr.Id, "]"));
            }

            query.Append(where);

            if (parametersDeclarationString.Length > 0)
            {
                SySqlParameter paramsDeclaration = new SySqlParameter();
                paramsDeclaration.ParameterName = "@parameters";
                paramsDeclaration.Value = parametersDeclarationString.ToString();
                queryParameters.Insert(0, paramsDeclaration);
            }

            return query.ToString();
        }

        private static string getQueryComparisonWithParameter(ComparisonOperator co, string parameterName)
        {
            if (co == ComparisonOperator.Like)
                return string.Concat("like '%'+", parameterName, "+'%'");
            else
                return string.Concat(GetQueryComparisonOperator(co), " ", parameterName);
        }

        public static string GetSqlDataType(AttributeDataTypeEnum dataType)
        {
            switch (dataType)
            {
                case AttributeDataTypeEnum.Bool:
                case AttributeDataTypeEnum.Int:
                case AttributeDataTypeEnum.Reference:
                    return "int";
                case AttributeDataTypeEnum.DateTime:
                    return "DateTime";
                case AttributeDataTypeEnum.Decimal:
                    return "decimal(20,6)";
                case AttributeDataTypeEnum.Image:
                case AttributeDataTypeEnum.String:
                    return "nvarchar(max)";
                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetQueryComparisonOperator(ComparisonOperator co)
        {
            switch (co)
            {
                case ComparisonOperator.Equals:
                    return "=";
                case ComparisonOperator.Greater:
                    return ">";
                case ComparisonOperator.Less:
                    return "<";
                case ComparisonOperator.GreaterThanOrEqual:
                    return ">=";
                case ComparisonOperator.LessThanOrEqual:
                    return "<=";
                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetItemDataTableName(AttributeDataTypeEnum dataType)
        {
            switch (dataType)
            {
                case AttributeDataTypeEnum.Bool:
                case AttributeDataTypeEnum.Int:
                case AttributeDataTypeEnum.Reference:
                    return "ItemDataInt";
                case AttributeDataTypeEnum.DateTime:
                    return "ItemDataDateTime";
                case AttributeDataTypeEnum.Decimal:
                    return "ItemDataDecimal";
                case AttributeDataTypeEnum.Image:
                case AttributeDataTypeEnum.String:
                    return "ItemDataString";
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class SortParameter
    {
        public IAttributeDefinition Attribute { get; set; }
        public SortDirection SortDirection { get; set; }

        public static List<SortParameter> Get(NameValueCollection parameters)
        {
            IAttributeLocator attributeLocator = Module.I<IAttributeLocator>();
            List<SortParameter> sortParameters = new List<SortParameter>();
            foreach (var key in parameters.AllKeys)
            {
                if (key == null) continue;

                string[] nameParts = key.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                int attributeId;
                if (nameParts.Length > 1 && int.TryParse(nameParts[0].Trim(), out attributeId))
                {
                    IAttributeDefinition attribute = attributeLocator.Find(a => a.Id == attributeId);
                    if (attribute == null && nameParts[1].Trim().ToLower() != "sort") continue;

                    string sortDirectionString = parameters[key].Trim().ToLower();
                    SortDirection sortDirection;
                    if (sortDirectionString == "a")
                        sortDirection = SortDirection.Ascending;
                    else if (sortDirectionString == "d")
                        sortDirection = SortDirection.Descending;
                    else
                        continue;

                    sortParameters.Add(new SortParameter
                    {
                        Attribute = attribute,
                        SortDirection = sortDirection
                    });
                }
            }
            return sortParameters;
        }

        public override string ToString()
        {
            return string.Concat(Attribute.Id, "_", SortDirection);
        }
    }

    public class SearchParameter
    {
        public IAttributeDefinition Attribute { get; set; }
        public dynamic Value { get; set; }
        public ComparisonOperator ComparisonOperator { get; set; }

        public static List<SearchParameter> Get(NameValueCollection parameters)
        {
            IAttributeLocator attributeLocator = Module.I<IAttributeLocator>();
            List<SearchParameter> searchParameters = new List<SearchParameter>();
            IValueConverter valueConverter = Module.I<IValueConverter>();
            foreach (var key in parameters.AllKeys)
            {
                if (key == null) continue;

                string[] nameParts = key.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                int attributeId;
                if (int.TryParse(nameParts[0].Trim(), out attributeId))
                {
                    IAttributeDefinition attribute = attributeLocator.Find(a => a.Id == attributeId);
                    if (attribute == null) continue;
                    ComparisonOperator comparisonOperator = nameParts.Length > 1 ?
                                                (EnumStringValue.GetEnumValue<ComparisonOperator>(nameParts[1].Trim()) ?? WebShop.Infrastructure.ComparisonOperator.Equals)
                                                : WebShop.Infrastructure.ComparisonOperator.Equals;
                    dynamic value = ConvertValue(attribute.DataType, parameters[key], comparisonOperator);
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        searchParameters.Add(new SearchParameter
                        {
                            Attribute = attribute,
                            ComparisonOperator = comparisonOperator,
                            Value = value
                        });
                    }
                }
            }
            return searchParameters;
        }

        public static dynamic ConvertValue(AttributeDataTypeEnum attributeDataTypeEnum, string value, ComparisonOperator comparisonOperator)
        {
            if (comparisonOperator == ComparisonOperator.Exists)
                attributeDataTypeEnum = AttributeDataTypeEnum.Bool;

            return Module.I<IValueConverter>().Convert(attributeDataTypeEnum, value);
        }

        public override string ToString()
        {
            return string.Concat(Attribute.Id, "_", Value, "_", ComparisonOperator);
        }
    }
}