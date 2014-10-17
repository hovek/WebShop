using Syrilium.CommonInterface;
using Syrilium.CommonInterface.Caching;
using Syrilium.DataAccessInterface;
using Syrilium.DataAccessInterface.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjectsInterface.Item
{
    public interface IAttributeTemplate
    {
        int Id { get; set; }
        ICache Cache { get; set; }
        ObservableCollection<IItemDefinitionValue<string>> NameValues { get; set; }
        ObservableCollection<IAttributeTemplateAttribute> Attributes { get; set; }
        ObservableCollection<IAttributeTemplateAttributeGroup> Groups { get; set; }
        S.IItem RawItem { get; set; }
        List<IAttributeTemplate> Get(int? templateId = null, int? itemId = null, bool? allowGetFromNearestParent = null);
        List<IAttributeTemplate> Get(string storedProcedure, List<SySqlParameter> parameters = null);
        List<IAttributeTemplateAttribute> GetAllAttributes();
        List<dynamic> GetAllElements();
        List<dynamic> GetSortedAttributesAndGroups();
        /// <summary>
        /// Get attributes that are constrained by parameter attributeId.
        /// </summary>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        List<int> GetForeignKeyConstrainedAttributes(int attributeId);
        IAttributeTemplateAttribute GetAttribute(int attributeDefinitionId);
    }

    public interface IAttributeTemplateAttribute
    {
        int Id { get; set; }
        IAttributeDefinition Attribute { get; set; }
        int AttributeId { get; set; }
        ObservableCollection<IAttributeTemplateConstraint> Constraints { get; set; }
        S.IItem RawItem { get; set; }
        List<ICommonReferenceListItem> GetConstrainedReferenceList(IList<IItemAttribute> constrainItemAttributes, int? languageId = null);
        List<ICommonReferenceListItem> GetConstrainedReferenceList(Dictionary<int, List<int>> constrainAttributeIdsValues, int? languageId = null);
        List<ICommonReferenceListItem> GetConstrainedReferenceList(Dictionary<IAttributeDefinition, List<int>> constrainAttributesValues, int? languageId = null);
        int Order { get; set; }
        /// <summary>
        /// Gets attributes that constrain this one.
        /// </summary>
        /// <returns></returns>
        List<int> GetForeignKeyConstrainAttributes();
        List<ICommonReferenceListItem> GetConstrainedReferenceList(Dictionary<AttributeDataSystemListReferenceEnum, List<int>> foreignKeyList, int languageId);
    }

    public interface IAttributeTemplateAttributeGroup : IList<IAttributeTemplateAttributeGroup>
    {
        int Id { get; set; }
        S.IItem RawItem { get; set; }
        ObservableCollection<IItemDefinitionValue<string>> DisplayValues { get; set; }
        ObservableCollection<IAttributeTemplateAttribute> Attributes { get; set; }
        string GetDisplayValue(int? languageId = null);
        string GetAnyDisplayValue();
        int Order { get; set; }
        List<dynamic> GetSortedAttributesAndGroups();
    }

    public interface IAttributeTemplateConstraint
    {
        int Id { get; set; }
        ItemDefinitionDataTypeEnum TypeId { get; set; }
        ObservableCollection<IItemDefinitionValue<int>> IntValues { get; set; }
        ObservableCollection<IItemDefinitionValue<string>> StringValues { get; set; }
        ObservableCollection<IItemDefinitionValue<decimal>> DecimalValues { get; set; }
        ObservableCollection<IItemDefinitionValue<DateTime>> DateTimeValues { get; set; }
        S.IItem RawItem { get; set; }
        dynamic GetRawValue(int? languageId = null);
    }
}
