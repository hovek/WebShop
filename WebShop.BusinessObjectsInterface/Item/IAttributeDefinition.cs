using System.Collections.Generic;
using Syrilium.DataAccessInterface;
using System;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface;
using Syrilium.DataAccessInterface.SQL;

namespace WebShop.BusinessObjectsInterface.Item
{
    public interface IAttributeDefinition
    {
        ItemDefinitionDataTypeEnum? ReferenceType { get; set; }
        int? ReferenceId { get; set; }
        AttributeDataSystemListReferenceEnum? SystemListReferenceId { get; set; }
        bool Hidden { get; set; }
        bool DeleteNotAllowed { get; set; }
        bool DataTypeChangeNotAllowed { get; set; }
        AttributeKeyEnum? Key { get; set; }
        AttributeDataTypeEnum DataType { get; set; }
        KeyObservableCollection<AttributeKeyEnum, IAttributeDefinition> Attributes { get; set; }
        ObservableCollection<IItemDefinitionValue<string>> DisplayValues { get; set; }
        ObservableCollection<IItemDefinitionValue<string>> FormatValues { get; set; }
        ObservableCollection<IItemDefinitionValue<string>> NameValues { get; set; }
        int Id { get; set; }
        List<IAttributeDefinition> Get(string storedProcedure = null, List<SySqlParameter> parameters = null);
        string GetDisplayValue();
        string GetFormatValue(int? languageId = null);
        string GetNameValue();
        dynamic GetReferenceObject();
        List<dynamic> GetReferenceListValues(List<int> listValuesId);
        List<ICommonReferenceListItem> GetReferenceList();
        List<IAttributeDefinition> GetAllAttributes();
        IAttributeDefinition Find(Predicate<IAttributeDefinition> match);
        S.IItem RawItem { get; set; }
    }

    public interface ICommonReferenceListItem
    {
        int Id { get; set; }
        dynamic Value { get; set; }
    }
}
