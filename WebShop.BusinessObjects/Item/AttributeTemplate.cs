using Syrilium.CommonInterface;
using Syrilium.CommonInterface.Caching;
using Syrilium.DataAccessInterface;
using Syrilium.DataAccessInterface.SQL;
using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjects.ModuleDefinitions;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjects.Item
{
    public class AttributeTemplate : IAttributeTemplate
    {
        private bool inLoadProcess = false;

        private ICache cache;
        public ICache Cache
        {
            get
            {
                if (cache == null)
                {
                    cache = Module.I<ICache>(CacheNames.MainCache);
                }
                return cache;
            }
            set
            {
                cache = value;
            }
        }

        public int Id
        {
            get
            {
                return RawItem.Id;
            }
            set
            {
                RawItem.Id = value;
            }
        }
        public ObservableCollection<IItemDefinitionValue<string>> NameValues { get; set; }
        public ObservableCollection<IAttributeTemplateAttribute> Attributes { get; set; }
        public ObservableCollection<IAttributeTemplateAttributeGroup> Groups { get; set; }

        private S.IItem rawItem;
        public S.IItem RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItemDefinition();
                    rawItem.TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate;
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }

        public AttributeTemplate()
        {
            NameValues = new ObservableCollection<IItemDefinitionValue<string>>();
            NameValues.CollectionChanged += nameValues_CollectionChanged;
            Attributes = new ObservableCollection<IAttributeTemplateAttribute>();
            Attributes.CollectionChanged += attributes_CollectionChanged;
            Groups = new ObservableCollection<IAttributeTemplateAttributeGroup>();
            Groups.CollectionChanged += groups_CollectionChanged;
        }

        public static AttributeTemplate Get(S.IItem rawItem, List<IAttributeDefinition> attributeDefinitions, ICache cache = null)
        {
            AttributeTemplate attributeTemplate = new AttributeTemplate();
            if (cache != null) attributeTemplate.Cache = cache;
            attributeTemplate.RawItem = rawItem;
            attributeTemplate.inLoadProcess = true;
            foreach (S.IItemData<string> rawValue in rawItem.StringData)
            {
                IItemDefinitionValue<string> itemValue = Module.I<IItemDefinitionValue<string>>();
                itemValue.RawItem = rawValue;
                attributeTemplate.NameValues.Add(itemValue);
            }

            foreach (S.IItem rawTemplateChild in rawItem.Children)
            {
                if (rawTemplateChild.TypeId == (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute)
                {
                    attributeTemplate.Attributes.Add(AttributeTemplateAttribute.Get(rawTemplateChild, attributeDefinitions, attributeTemplate.Cache));
                }
                else if (rawTemplateChild.TypeId == (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup)
                {
                    attributeTemplate.Groups.Add(AttributeTemplateAttributeGroup.Get(rawTemplateChild, attributeDefinitions, attributeTemplate.Cache));
                }
            }
            attributeTemplate.inLoadProcess = false;

            return attributeTemplate;
        }

        public List<IAttributeTemplate> Get(int? templateId = null, int? itemId = null, bool? allowGetFromNearestParent = null)
        {
            List<SySqlParameter> parameters = new List<SySqlParameter>();
            if (templateId != null)
            {
                SySqlParameter paramTemplateId = new SySqlParameter();
                paramTemplateId.ParameterName = "@templateID";
                paramTemplateId.Value = templateId;
                parameters.Add(paramTemplateId);
            }
            if (itemId != null)
            {
                SySqlParameter paramItemID = new SySqlParameter();
                paramItemID.ParameterName = "@itemID";
                paramItemID.Value = itemId;
                parameters.Add(paramItemID);
            }
            if (allowGetFromNearestParent != null)
            {
                SySqlParameter paramAllowGetFromNearestParent = new SySqlParameter();
                paramAllowGetFromNearestParent.ParameterName = "@allowGetFromNearestParent";
                paramAllowGetFromNearestParent.Value = allowGetFromNearestParent;
                parameters.Add(paramAllowGetFromNearestParent);
            }
            return Get(SPNames.GetAttributeTemplate, parameters);
        }

        /// <summary>
        /// Get attributes that are constrained by parameter attributeId.
        /// </summary>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public List<int> GetForeignKeyConstrainedAttributes(int attributeId)
        {
            int languageId = Module.LanguageId;
            List<int> attributeIds = new List<int>();
            foreach (IAttributeTemplateAttribute templateAttribute in GetAllAttributes())
            {
                foreach (IAttributeTemplateConstraint constraint in templateAttribute.Constraints)
                {
                    if (constraint.TypeId == ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute)
                    {
                        if (constraint.IntValues.Exists(v => (v.LanguageId == languageId || v.LanguageId == 0) && v.Value == attributeId))
                        {
                            attributeIds.Add(templateAttribute.Attribute.Id);
                            break;
                        }
                    }
                }
            }

            return attributeIds;
        }

        public List<dynamic> GetSortedAttributesAndGroups()
        {
            List<dynamic> sorted = new List<dynamic>();
            sorted.AddRange(Attributes);
            sorted.AddRange(Groups);
            sorted.Sort((i1, i2) => i1.Order.CompareTo(i2.Order));
            return sorted;
        }

        public virtual List<IAttributeTemplate> Get(string storedProcedure, List<SySqlParameter> parameters = null)
        {
            List<S.IItem> rawItems = Module.I<S.IItemManager>().GetItemDefinitions(storedProcedure, parameters);
            List<IAttributeDefinition> attributeDefinitions = this.Cache.I<AttributeDefinition>().Get(SPNames.GetAttributeDefinition);
            List<IAttributeTemplate> itemsTemplates = new List<IAttributeTemplate>();
            foreach (S.IItem item in rawItems)
            {
                itemsTemplates.Add(Get(item, attributeDefinitions));
            }

            return itemsTemplates;
        }

        public List<IAttributeTemplateAttribute> GetAllAttributes()
        {
            List<IAttributeTemplateAttribute> attrs = new List<IAttributeTemplateAttribute>();
            attrs.AddRange(this.Attributes);
            attrs.AddRange(getAllAttributes(Groups));
            return attrs;
        }

        public List<dynamic> GetAllElements()
        {
            List<dynamic> elements = new List<dynamic>();
            elements.AddRange(Attributes);
            elements.AddRange(Groups);
            elements.AddRange(getAllElements(Groups));
            return elements;
        }

        public IAttributeTemplateAttribute GetAttribute(int attributeDefinitionId)
        {
            return GetAllAttributes().Find(a => a.AttributeId == attributeDefinitionId);
        }

        private void nameValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemValue<string> oldItem in e.OldItems)
                {
                    RawItem.StringData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemValue<string> newItem in e.NewItems)
                {
                    RawItem.StringData.Add(newItem.RawItem);
                }
            }
        }

        private void attributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IAttributeTemplateAttribute oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IAttributeTemplateAttribute newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }

        private void groups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IAttributeTemplateAttributeGroup oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IAttributeTemplateAttributeGroup newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }

        private List<IAttributeTemplateAttribute> getAllAttributes(IList<IAttributeTemplateAttributeGroup> groups)
        {
            List<IAttributeTemplateAttribute> attrs = new List<IAttributeTemplateAttribute>();
            foreach (IAttributeTemplateAttributeGroup group in groups)
            {
                attrs.AddRange(group.Attributes);
                attrs.AddRange(getAllAttributes(group));
            }

            return attrs;
        }

        private List<dynamic> getAllElements(IList<IAttributeTemplateAttributeGroup> groups)
        {
            List<dynamic> attrs = new List<dynamic>();
            foreach (IAttributeTemplateAttributeGroup group in groups)
            {
                attrs.AddRange(group.Attributes);
                attrs.AddRange(group);
                attrs.AddRange(getAllElements(group));
            }

            return attrs;
        }
    }

    public class AttributeTemplateAttribute : IAttributeTemplateAttribute
    {
        private bool inLoadProcess = false;

        private ICache cache;
        public ICache Cache
        {
            get
            {
                if (cache == null)
                {
                    cache = Module.I<ICache>(CacheNames.MainCache);
                }
                return cache;
            }
            set
            {
                cache = value;
            }
        }

        public int Id
        {
            get
            {
                return RawItem.Id;
            }
            set
            {
                RawItem.Id = value;
            }
        }
        public IAttributeDefinition Attribute { get; set; }
        public int AttributeId
        {
            get
            {
                foreach (S.IItemData<int> intVal in RawItem.IntData)
                {
                    if (intVal.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId)
                        return intVal.Value;
                }

                throw new ArgumentNullException();
            }
            set
            {
                foreach (S.IItemData<int> intVal in RawItem.IntData)
                {
                    if (intVal.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId)
                    {
                        intVal.Value = value;
                        return;
                    }
                }

                S.IItemData<int> newVal = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                newVal.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId;
                newVal.Value = value;
                RawItem.IntData.Add(newVal);
            }
        }

        public ObservableCollection<IAttributeTemplateConstraint> Constraints { get; set; }
        public int Order
        {
            get
            {
                S.IItemData<int> itemValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder);
                if (itemValue == null) return 0;
                return itemValue.Value;
            }
            set
            {
                S.IItemData<int> itemValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder);
                if (itemValue == null)
                {
                    itemValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                    itemValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder;
                    RawItem.IntData.Add(itemValue);
                }
                itemValue.Value = value;
            }
        }

        private S.IItem rawItem;
        public S.IItem RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItemDefinition();
                    rawItem.TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute;
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }

        public AttributeTemplateAttribute()
        {
            Constraints = new ObservableCollection<IAttributeTemplateConstraint>();
            Constraints.CollectionChanged += constraints_CollectionChanged;
        }

        public List<ICommonReferenceListItem> GetConstrainedReferenceList(IList<IItemAttribute> constrainItemAttributes, int? languageId = null)
        {
            if (languageId == null) languageId = Module.LanguageId;
            Dictionary<IAttributeDefinition, List<int>> constrainAttributesValues = new Dictionary<IAttributeDefinition, List<int>>();
            foreach (var attr in constrainItemAttributes)
            {
                if (!constrainAttributesValues.ContainsKey(attr.Attribute))
                    constrainAttributesValues[attr.Attribute] = new List<int>();
                constrainAttributesValues[attr.Attribute].AddRange(attr.IntValues.Where(v => v.LanguageId == languageId || v.LanguageId == 0).Select(v => v.Value));
            }
            return GetConstrainedReferenceList(constrainAttributesValues);
        }

        public List<ICommonReferenceListItem> GetConstrainedReferenceList(Dictionary<int, List<int>> constrainAttributeIdsValues, int? languageId = null)
        {
            if (languageId == null) languageId = Module.LanguageId;
            Dictionary<IAttributeDefinition, List<int>> constrainAttributesValues = new Dictionary<IAttributeDefinition, List<int>>();
            List<IAttributeDefinition> attributeDefinitions = this.Cache.I<IAttributeDefinition>(Module.I<IAttributeDefinition>().GetType()).Get();
            foreach (var item in constrainAttributeIdsValues)
            {
                IAttributeDefinition attrDef = attributeDefinitions.Find(a => a.Id == item.Key);
                constrainAttributesValues[attrDef] = new List<int>(item.Value);
            }
            return GetConstrainedReferenceList(constrainAttributesValues);
        }

        public List<ICommonReferenceListItem> GetConstrainedReferenceList(Dictionary<IAttributeDefinition, List<int>> constrainAttributesValues, int? languageId = null)
        {
            if (languageId == null) languageId = Module.LanguageId;
            bool hasConstraint = false;
            Dictionary<AttributeDataSystemListReferenceEnum, List<int>> foreignKeyList = new Dictionary<AttributeDataSystemListReferenceEnum, List<int>>();
            foreach (IAttributeTemplateConstraint constraint in Constraints)
            {
                if (constraint.TypeId == ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute)
                {
                    foreach (var constraintValue in constraint.IntValues)
                    {
                        if (constraintValue.LanguageId == languageId || constraintValue.LanguageId == 0)
                        {
                            hasConstraint = true;
                            foreach (KeyValuePair<IAttributeDefinition, List<int>> itemAttribute in constrainAttributesValues)
                            {
                                if (itemAttribute.Key.Id == constraintValue.Value)
                                {
                                    if (!foreignKeyList.ContainsKey(itemAttribute.Key.SystemListReferenceId.Value))
                                        foreignKeyList[itemAttribute.Key.SystemListReferenceId.Value] = new List<int>();
                                    foreignKeyList[itemAttribute.Key.SystemListReferenceId.Value].AddRange(itemAttribute.Value);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (hasConstraint)
            {
                if (foreignKeyList.Count > 0)
                {
                    return GetConstrainedReferenceList(foreignKeyList, Module.LanguageId);
                }
                else
                {
                    return new List<ICommonReferenceListItem>();
                }
            }

            return Reference.I.ConvertToCommonReferenceList(this.Attribute.GetReferenceObject(), Module.LanguageId);
        }

        public List<ICommonReferenceListItem> GetConstrainedReferenceList(Dictionary<AttributeDataSystemListReferenceEnum, List<int>> foreignKeyList, int languageId)
        {
            dynamic referenceList = this.Attribute.GetReferenceObject();
            foreach (var fk in foreignKeyList)
            {
                referenceList = SystemListForeignKeyFilter.Filter(fk.Key, this.Attribute.SystemListReferenceId.Value, fk.Value, referenceList);
            }
            return Reference.I.ConvertToCommonReferenceList(referenceList, languageId);
        }

        /// <summary>
        /// Gets attributes that constrain this one.
        /// </summary>
        /// <returns></returns>
        public List<int> GetForeignKeyConstrainAttributes()
        {
            int languageId = Module.LanguageId;
            List<int> attributeIds = new List<int>();
            foreach (IAttributeTemplateConstraint constraint in Constraints)
            {
                if (constraint.TypeId == ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute)
                {
                    foreach (var constraintValue in constraint.IntValues)
                    {
                        if (constraintValue.LanguageId == languageId || constraintValue.LanguageId == 0)
                        {
                            attributeIds.Add(constraintValue.Value);
                        }
                    }
                }
            }

            return attributeIds;
        }

        public static AttributeTemplateAttribute Get(S.IItem rawItem, List<IAttributeDefinition> attributeDefinitions, ICache cache = null)
        {
            AttributeTemplateAttribute attributeTemplateAttribute = new AttributeTemplateAttribute();
            if (cache != null) attributeTemplateAttribute.Cache = cache;
            attributeTemplateAttribute.RawItem = rawItem;
            attributeTemplateAttribute.inLoadProcess = true;
            int attributeId = 0;
            foreach (S.IItemData<int> rawValue in rawItem.IntData)
            {
                if (rawValue.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId)
                {
                    attributeId = rawValue.Value;
                    break;
                }
            }
            attributeTemplateAttribute.Attribute = attributeDefinitions.Find(i => i.Id == attributeId);

            foreach (S.IItem rawConstraint in rawItem.Children)
            {
                attributeTemplateAttribute.Constraints.Add(AttributeTemplateConstraint.Get(rawConstraint));
            }
            attributeTemplateAttribute.inLoadProcess = false;

            return attributeTemplateAttribute;
        }

        private void constraints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IAttributeTemplateConstraint oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IAttributeTemplateConstraint newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }
    }

    public class AttributeTemplateAttributeGroup : ObservableCollection<IAttributeTemplateAttributeGroup>, IAttributeTemplateAttributeGroup
    {
        private bool inLoadProcess = false;

        private ICache cache;
        public ICache Cache
        {
            get
            {
                if (cache == null)
                {
                    cache = Module.I<ICache>(CacheNames.MainCache);
                }
                return cache;
            }
            set
            {
                cache = value;
            }
        }

        public int Id
        {
            get
            {
                return RawItem.Id;
            }
            set
            {
                RawItem.Id = value;
            }
        }

        public ObservableCollection<IItemDefinitionValue<string>> DisplayValues { get; set; }
        public ObservableCollection<IAttributeTemplateAttribute> Attributes { get; set; }

        public int Order
        {
            get
            {
                S.IItemData<int> itemValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder);
                if (itemValue == null) return 0;
                return itemValue.Value;
            }
            set
            {
                S.IItemData<int> itemValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder);
                if (itemValue == null)
                {
                    itemValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                    itemValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder;
                    RawItem.IntData.Add(itemValue);
                }
                itemValue.Value = value;
            }
        }

        private S.IItem rawItem;
        public S.IItem RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItemDefinition();
                    rawItem.TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup;
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }

        public AttributeTemplateAttributeGroup()
        {
            this.CollectionChanged += this_CollectionChanged;
            DisplayValues = new ObservableCollection<IItemDefinitionValue<string>>();
            DisplayValues.CollectionChanged += displayValues_CollectionChanged;
            Attributes = new ObservableCollection<IAttributeTemplateAttribute>();
            Attributes.CollectionChanged += attributes_CollectionChanged;
        }

        public string GetAnyDisplayValue()
        {
            return getAnyStringValue(DisplayValues);
        }

        public string GetDisplayValue(int? languageId = null)
        {
            if (languageId == null) languageId = Module.LanguageId;
            IItemValue<string> itemValue = DisplayValues.Find(v => v.LanguageId == languageId || v.LanguageId == 0);
            return itemValue == null ? "" : itemValue.Value;
        }

        public List<dynamic> GetSortedAttributesAndGroups()
        {
            List<dynamic> sorted = new List<dynamic>();
            sorted.AddRange(Attributes);
            sorted.AddRange(this);
            sorted.Sort((i1, i2) => i1.Order.CompareTo(i2.Order));
            return sorted;
        }

        public static AttributeTemplateAttributeGroup Get(S.IItem rawItem, List<IAttributeDefinition> attributeDefinitions, ICache cache = null)
        {
            AttributeTemplateAttributeGroup attributeGroup = new AttributeTemplateAttributeGroup();
            if (cache != null) attributeGroup.Cache = cache;
            attributeGroup.RawItem = rawItem;
            attributeGroup.inLoadProcess = true;
            foreach (S.IItemData<string> rawValue in rawItem.StringData)
            {
                IItemDefinitionValue<string> itemValue = Module.I<IItemDefinitionValue<string>>();
                itemValue.RawItem = rawValue;
                attributeGroup.DisplayValues.Add(itemValue);
            }

            foreach (S.IItem rawTemplateChild in rawItem.Children)
            {
                if (rawTemplateChild.TypeId == (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute)
                {
                    attributeGroup.Attributes.Add(AttributeTemplateAttribute.Get(rawTemplateChild, attributeDefinitions, attributeGroup.Cache));
                }
                else if (rawTemplateChild.TypeId == (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup)
                {
                    attributeGroup.Add(AttributeTemplateAttributeGroup.Get(rawTemplateChild, attributeDefinitions, attributeGroup.Cache));
                }
            }
            attributeGroup.inLoadProcess = false;

            return attributeGroup;
        }

        private void attributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IAttributeTemplateAttribute oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IAttributeTemplateAttribute newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }

        private void this_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IAttributeTemplateAttributeGroup oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IAttributeTemplateAttributeGroup newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }

        private void displayValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemValue<string> oldItem in e.OldItems)
                {
                    RawItem.StringData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemValue<string> newItem in e.NewItems)
                {
                    RawItem.StringData.Add(newItem.RawItem);
                }
            }
        }

        private string getAnyStringValue(IEnumerable<IItemValue<string>> itemValue)
        {
            int languageId = Module.LanguageId;
            string value = "";
            foreach (IItemValue<string> val in itemValue)
            {
                if (!string.IsNullOrEmpty(val.Value))
                {
                    if (val.LanguageId == languageId)
                    {
                        value = val.Value;
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            break;
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(value) || val.LanguageId == 0)
                    {
                        value = val.Value;
                    }
                }
            }

            return value;
        }
    }

    public class AttributeTemplateConstraint : IAttributeTemplateConstraint
    {
        private bool inLoadProcess = false;

        public int Id
        {
            get
            {
                return RawItem.Id;
            }
            set
            {
                RawItem.Id = value;
            }
        }

        public ItemDefinitionDataTypeEnum TypeId
        {
            get
            {
                return (ItemDefinitionDataTypeEnum)getType().TypeId;
            }
            set
            {
                S.IItemData<int> typeValue = getType();
                if (typeValue == null)
                {
                    typeValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                    RawItem.IntData.Add(typeValue);
                }
                typeValue.TypeId = (int)value;
            }
        }

        public ObservableCollection<IItemDefinitionValue<int>> IntValues { get; set; }
        public ObservableCollection<IItemDefinitionValue<string>> StringValues { get; set; }
        public ObservableCollection<IItemDefinitionValue<decimal>> DecimalValues { get; set; }
        public ObservableCollection<IItemDefinitionValue<DateTime>> DateTimeValues { get; set; }

        private S.IItem rawItem;
        public S.IItem RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItemDefinition();
                    rawItem.TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint;
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }

        public AttributeTemplateConstraint()
        {
            IntValues = new ObservableCollection<IItemDefinitionValue<int>>();
            IntValues.CollectionChanged += intValues_CollectionChanged;
            StringValues = new ObservableCollection<IItemDefinitionValue<string>>();
            StringValues.CollectionChanged += stringValues_CollectionChanged;
            DecimalValues = new ObservableCollection<IItemDefinitionValue<decimal>>();
            DecimalValues.CollectionChanged += decimalValues_CollectionChanged;
            DateTimeValues = new ObservableCollection<IItemDefinitionValue<DateTime>>();
            DateTimeValues.CollectionChanged += dateTimeValues_CollectionChanged;
        }

        public static AttributeTemplateConstraint Get(S.IItem rawItem)
        {
            AttributeTemplateConstraint attributeTemplateConstraint = new AttributeTemplateConstraint();
            attributeTemplateConstraint.RawItem = rawItem;
            attributeTemplateConstraint.inLoadProcess = true;
            foreach (S.IItemData<int> rawIntValue in rawItem.IntData)
            {
                if (rawIntValue.TypeId < 51)
                {
                    IItemDefinitionValue<int> value = Module.I<IItemDefinitionValue<int>>();
                    value.RawItem = rawIntValue;
                    attributeTemplateConstraint.IntValues.Add(value);
                }
            }
            foreach (S.IItemData<string> rawIntValue in rawItem.StringData)
            {
                IItemDefinitionValue<string> value = Module.I<IItemDefinitionValue<string>>();
                value.RawItem = rawIntValue;
                attributeTemplateConstraint.StringValues.Add(value);
            }
            foreach (S.IItemData<decimal> rawIntValue in rawItem.DecimalData)
            {
                IItemDefinitionValue<decimal> value = Module.I<IItemDefinitionValue<decimal>>();
                value.RawItem = rawIntValue;
                attributeTemplateConstraint.DecimalValues.Add(value);
            }
            foreach (S.IItemData<DateTime> rawIntValue in rawItem.DateTimeData)
            {
                IItemDefinitionValue<DateTime> value = Module.I<IItemDefinitionValue<DateTime>>();
                value.RawItem = rawIntValue;
                attributeTemplateConstraint.DateTimeValues.Add(value);
            }
            attributeTemplateConstraint.inLoadProcess = false;

            return attributeTemplateConstraint;
        }

        public dynamic GetRawValue(int? languageId = null)
        {
            if (!languageId.HasValue) languageId = Module.LanguageId;

            dynamic value = IntValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
            if (value == null) value = StringValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
            if (value == null) value = DecimalValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
            if (value == null) value = DateTimeValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
            if (value != null) return value.Value;

            return value;
        }

        private S.IItemData<int> getType()
        {
            return RawItem.IntData.Find(i =>
                {
                    switch (i.TypeId)
                    {
                        case (int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry:
                        case (int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry:
                        case (int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute:
                            return true;
                    }
                    return false;
                });
        }

        private void intValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemValue<int> oldItem in e.OldItems)
                {
                    RawItem.IntData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemValue<int> newItem in e.NewItems)
                {
                    RawItem.IntData.Add(newItem.RawItem);
                }
            }
        }

        private void stringValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemValue<string> oldItem in e.OldItems)
                {
                    RawItem.StringData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemValue<string> newItem in e.NewItems)
                {
                    RawItem.StringData.Add(newItem.RawItem);
                }
            }
        }

        private void decimalValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemValue<decimal> oldItem in e.OldItems)
                {
                    RawItem.DecimalData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemValue<decimal> newItem in e.NewItems)
                {
                    RawItem.DecimalData.Add(newItem.RawItem);
                }
            }
        }

        private void dateTimeValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemValue<DateTime> oldItem in e.OldItems)
                {
                    RawItem.DateTimeData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemValue<DateTime> newItem in e.NewItems)
                {
                    RawItem.DateTimeData.Add(newItem.RawItem);
                }
            }
        }
    }
}
