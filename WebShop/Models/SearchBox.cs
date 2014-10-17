using Syrilium.CommonInterface.Caching;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;
using S = Syrilium.Modules.BusinessObjects;

namespace WebShop.Models
{
    public class SearchBox
    {
        public int DepartmentId { get; set; }
        public int? GroupId { get; set; }
        public NameValueCollection QueryString { get; set; }
        public List<IItem> Products { get; private set; }

        public IEnumerable<SelectListItem> GroupList
        {
            get
            {
                return WebShop.Models.Group.GetGroupItems(DepartmentId);
            }
        }

        private IAttributeTemplate attributeTemplate { get; set; }
        public IAttributeTemplate AttributeTemplate
        {
            get
            {
                if (attributeTemplate == null)
                {
                    attributeTemplate = Module.I<ICache>(CacheNames.MainCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(itemId: GroupId ?? DepartmentId).FirstOrDefault();
                }

                return attributeTemplate;
            }
        }

        public ItemSearch SearchObject { get; set; }

        private List<int> attributesToExcludeFromResults;
        private List<int> attributesToExcludeFromSearch;

        private IAttributeLocator attributeLocator;
        public IAttributeLocator AttributeLocator
        {
            get
            {
                if (attributeLocator == null)
                {
                    attributeLocator = Module.I<IAttributeLocator>();
                }
                return attributeLocator;
            }
        }

        public List<SortParameter> SortParameters { get; set; }

        public SearchBox()
        {
            attributesToExcludeFromResults = new List<int> { 
				AttributeLocator[AttributeKeyEnum.Image]??0,
				AttributeLocator[AttributeKeyEnum.Top]??0,
				AttributeLocator[AttributeKeyEnum.Show]??0,
				AttributeLocator[AttributeKeyEnum.Name]??0,
                AttributeLocator[AttributeKeyEnum.Premium]??0};
            attributesToExcludeFromResults.RemoveAll(a => a == 0);

            attributesToExcludeFromSearch = new List<int> { 
				AttributeLocator[AttributeKeyEnum.Image]??0,
				AttributeLocator[AttributeKeyEnum.Partner]??0,
				AttributeLocator[AttributeKeyEnum.County]??0,
				AttributeLocator[AttributeKeyEnum.DistrictCity]??0,
				AttributeLocator[AttributeKeyEnum.Price]??0,
				AttributeLocator[AttributeKeyEnum.Location]??0,
				AttributeLocator[AttributeKeyEnum.Top]??0,
				AttributeLocator[AttributeKeyEnum.Show]??0,
		};
            attributesToExcludeFromSearch.RemoveAll(a => a == 0);

            SearchObject = new ItemSearch();
        }

        public string GetSortedAttributeName()
        {
            SortParameter sp = SortParameters.FirstOrDefault();
            if (sp != null)
                return string.Concat(sp.Attribute.Id, "_sort");
            else
                return string.Concat(AttributeLocator[AttributeKeyEnum.DateTimeEntry], "_sort");
        }

        public string GetSortedDirection()
        {
            SortParameter sp = SortParameters.FirstOrDefault();
            if (sp != null)
                return sp.SortDirection == SortDirection.Ascending ? "a" : "d";
            else
                return "d";
        }

        public void SetDepartmentAndGroupId(int groupId)
        {
            DepartmentId = Module.I<IGroup>().GetDepartmentId(groupId);
            if (DepartmentId != groupId)
                GroupId = groupId;
        }

        public List<IAttributeDefinition> GetFirstXAttributes()
        {
            List<IAttributeDefinition> attributes = new List<IAttributeDefinition>();
            if (AttributeTemplate == null) return attributes;
            foreach (var element in AttributeTemplate.GetSortedAttributesAndGroups())
            {
                if (element is IAttributeTemplateAttribute)
                {
                    IAttributeTemplateAttribute attr = (IAttributeTemplateAttribute)element;
                    if (!attributesToExcludeFromResults.Contains(attr.AttributeId))
                    {
                        attributes.Add(((IAttributeTemplateAttribute)element).Attribute);
                    }
                }
            }

            return attributes;
        }

        public bool AttributeHasValue(IItem product, IAttributeDefinition attr)
        {
            return !string.IsNullOrWhiteSpace(product.GetValueFormated(attr.Id));
        }

        public List<dynamic> GetElements()
        {
            if (AttributeTemplate == null) return new List<dynamic>();
            return GetElements(AttributeTemplate.GetSortedAttributesAndGroups());
        }

        public List<dynamic> GetElements(IAttributeTemplateAttributeGroup group)
        {
            return GetElements(group.GetSortedAttributesAndGroups());
        }

        public List<dynamic> GetElements(IEnumerable<dynamic> elements)
        {
            List<dynamic> retElements = new List<dynamic>();
            foreach (var element in elements)
            {
                if (element is IAttributeTemplateAttributeGroup)
                {
                    if (GroupHasElements(element))
                        retElements.Add(element);
                }
                else if (!attributesToExcludeFromSearch.Contains(((IAttributeTemplateAttribute)element).AttributeId))
                    retElements.Add(element);
            }
            return retElements;
        }

        public bool GroupHasElements(IAttributeTemplateAttributeGroup templateGroup)
        {
            if (templateGroup.Attributes.Exists(a => !attributesToExcludeFromSearch.Contains(a.AttributeId))) return true;

            foreach (var group in templateGroup)
            {
                if (GroupHasElements(group)) return true;
            }
            return false;
        }

        public List<SelectListItem> GetPartnersDropDownItems()
        {
            List<S.Partner> partners = Module.I<ICache>(CacheNames.MainCache).I<S.Partner>().GetPartners();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var partner in partners)
            {
                items.Add(new SelectListItem
                {
                    Text = partner.Name,
                    Value = partner.Id.ToString()
                });
            }
            return items;
        }

        public void Init(HttpRequestBase request)
        {
            NameValueCollection queryString = request.Form.AllKeys.Count() > 0 ? request.Form : request.QueryString;
            SortParameters = SortParameter.Get(queryString);
        }

        public void Search()
        {
            List<SearchParameter> searchParams = SearchParameter.Get(QueryString);
            List<SortParameter> sortParams = SortParameter.Get(QueryString);

            if (sortParams.Count == 0)
                sortParams.Add(new SortParameter
                {
                    Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.DateTimeEntry),
                    SortDirection = System.Web.Helpers.SortDirection.Descending
                });

            searchParams.Add(new SearchParameter
            {
                Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.Show),
                ComparisonOperator = ComparisonOperator.NotExistsOrEquals,
                Value = true
            });

            Products = SearchObject.Find(searchParams, sortParams, parentId: GroupId ?? DepartmentId, page: ItemSearch.GetPage(QueryString));
        }
    }
}