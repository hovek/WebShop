using System.Collections.Generic;
using Syrilium.DataAccessInterface;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using System;
using System.Data;
using Syrilium.CommonInterface;
using Syrilium.CommonInterface.Caching;
using Syrilium.DataAccessInterface.SQL;
namespace WebShop.BusinessObjectsInterface.Item
{
    public interface IItem : IList<IItem>
    {
        int Id { get; set; }
        KeyObservableCollection<AttributeKeyEnum, IItemAttribute> Attributes { get; }
        ItemTypeEnum TypeId { get; set; }
        S.IItem RawItem { get; set; }
        List<IItem> Get(string storedProcedure, List<SySqlParameter> parameters = null);
        IItem Get(int itemId, params AttributeKeyEnum[] attributeKeys);
        List<IItem> Get(DataSet ds);
        int? ParentId { get; set; }
        IItem GetChild(int childId);
        IItem GetChild(int childId, IList<IItem> items);
        string GetValueFormated(AttributeKeyEnum attributeKey);
        string GetValueFormated(int attributeDefinitionId);
        dynamic GetRawValue(AttributeKeyEnum attributeKey);
        dynamic GetRawValue(int attributeDefinitionId);
        IItemAttribute GetItemAttribute(AttributeKeyEnum attributeKey, AttributeKeyEnum? attributeKeyChild = null);
        IItemAttribute GetItemAttribute(int attributeDefinitionId, AttributeKeyEnum? attributeKey = null);
        dynamic GetRawValue(AttributeKeyEnum attributeKey, AttributeKeyEnum attributeKeyChild);
        dynamic GetRawValue(int attributeDefinitionId, AttributeKeyEnum attributeKey);
        ICache Cache { get; set; }
        IItem Find(IList<IItem> items, Predicate<IItem> match);
        List<IItem> GetAllItems(IList<IItem> items);
    }

    public interface IItemAttribute
    {
        IAttributeDefinition Attribute { get; set; }
        int AttributeId { get; set; }
        ObservableCollection<IItemValue<int>> IntValues { get; }
        ObservableCollection<IItemValue<string>> StringValues { get; }
        ObservableCollection<IItemValue<decimal>> DecimalValues { get; }
        ObservableCollection<IItemValue<DateTime>> DateTimeValues { get; }
        S.IItem RawItem { get; set; }
        string GetValueFormated(int? languageId = null);
        List<string> GetValuesFormated(int? languageId = null);
        dynamic GetRawValue(int? languageId = null);
        List<dynamic> GetRawValues(int? languageId = null);
    }

    public interface IItemValue
    {
        int LanguageId { get; set; }
        dynamic ValueDynamic { get; set; }
    }

    public interface IItemValue<T> : IItemValue
    {
        T Value { get; set; }
        S.IItemData<T> RawItem { get; set; }
    }

    public interface IItemDefinitionValue<T> : IItemValue<T>
    {
    }
}
