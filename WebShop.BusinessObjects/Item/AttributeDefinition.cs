using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.DataAccessInterface;
using WebShop.BusinessObjects.ModuleDefinitions;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface;
using Syrilium.DataAccessInterface.SQL;

namespace WebShop.BusinessObjects.Item
{
    public class AttributeDefinition : IAttributeDefinition
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

        public bool Hidden
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden)
                        return Convert.ToBoolean(i.Value);
                }
                return false;
            }
            set
            {
                int intValue = value ? 1 : 0;
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden);
                if (editValue != null)
                    editValue.Value = intValue;
                else
                {
                    editValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                    editValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden;
                    editValue.Value = intValue;
                    RawItem.IntData.Add(editValue);
                }
            }
        }

        public bool DeleteNotAllowed
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed)
                        return Convert.ToBoolean(i.Value);
                }
                return false;
            }
            set
            {
                int intValue = value ? 1 : 0;
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed);
                if (editValue != null)
                    editValue.Value = intValue;
                else
                {
                    editValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                    editValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed;
                    editValue.Value = intValue;
                    RawItem.IntData.Add(editValue);
                }
            }
        }

        public bool DataTypeChangeNotAllowed
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed)
                        return Convert.ToBoolean(i.Value);
                }
                return false;
            }
            set
            {
                int intValue = value ? 1 : 0;
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed);
                if (editValue != null)
                    editValue.Value = intValue;
                else
                {
                    editValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                    editValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed;
                    editValue.Value = intValue;
                    RawItem.IntData.Add(editValue);
                }
            }
        }

        public AttributeKeyEnum? Key
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeKey)
                        return (AttributeKeyEnum)i.Value;
                }

                return null;
            }
            set
            {
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeKey);
                if (value.HasValue)
                {
                    if (editValue != null)
                        editValue.Value = (int)value;
                    else
                    {
                        editValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                        editValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey;
                        editValue.Value = (int)value;
                        RawItem.IntData.Add(editValue);
                    }
                }
                else if (editValue != null)
                    RawItem.IntData.Remove(editValue);
            }
        }

        public AttributeDataTypeEnum DataType
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeDataType)
                        return (AttributeDataTypeEnum)i.Value;
                }

                throw new ArgumentNullException();
            }
            set
            {
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeDataType);
                if (editValue != null)
                    editValue.Value = (int)value;
                else
                {
                    editValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                    editValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType;
                    editValue.Value = (int)value;
                    RawItem.IntData.Add(editValue);
                }
            }
        }

        public ItemDefinitionDataTypeEnum? ReferenceType
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferencePredefinedListId
                        || i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId)
                        return (ItemDefinitionDataTypeEnum)i.TypeId;
                }

                return null;
            }
            set
            {
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferencePredefinedListId
                                                                    || i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId);
                if (value.HasValue)
                {
                    if (editValue != null)
                        editValue.TypeId = (int)value;
                    else
                    {
                        editValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                        editValue.TypeId = (int)value;
                        RawItem.IntData.Add(editValue);
                    }
                }
                else if (editValue != null)
                    RawItem.IntData.Remove(editValue);
            }
        }

        public int? ReferenceId
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferencePredefinedListId
                        || i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId)
                        return i.Value;
                }

                return null;
            }
            set
            {
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferencePredefinedListId
                                                                    || i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId);
                if (editValue != null)
                    editValue.Value = value.Value;
                else
                    throw new InvalidOperationException("ReferenceType not set.");
            }
        }

        public AttributeDataSystemListReferenceEnum? SystemListReferenceId
        {
            get
            {
                foreach (S.IItemData<int> i in RawItem.IntData)
                {
                    if (i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId)
                        return (AttributeDataSystemListReferenceEnum)i.Value;
                }

                return null;
            }
            set
            {
                S.IItemData<int> editValue = RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId);

                if (value.HasValue)
                {
                    if (editValue != null)
                        editValue.Value = (int)value.Value;
                    else
                    {
                        editValue = Module.I<S.IItemManager>().GetNewItemDefinitionData<int>();
                        editValue.TypeId = (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId;
                        editValue.Value = (int)value.Value;
                        RawItem.IntData.Add(editValue);
                    }
                }
                else if (editValue != null)
                    RawItem.IntData.Remove(editValue);
            }
        }

        public ObservableCollection<IItemDefinitionValue<string>> DisplayValues { get; set; }
        public ObservableCollection<IItemDefinitionValue<string>> FormatValues { get; set; }
        public ObservableCollection<IItemDefinitionValue<string>> NameValues { get; set; }
        public KeyObservableCollection<AttributeKeyEnum, IAttributeDefinition> Attributes { get; set; }

        private S.IItem rawItem;
        public S.IItem RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItemDefinition();
                    rawItem.TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition;
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }

        public AttributeDefinition()
        {
            DisplayValues = new ObservableCollection<IItemDefinitionValue<string>>();
            DisplayValues.CollectionChanged += DisplayValues_CollectionChanged;
            FormatValues = new ObservableCollection<IItemDefinitionValue<string>>();
            FormatValues.CollectionChanged += FormatValues_CollectionChanged;
            NameValues = new ObservableCollection<IItemDefinitionValue<string>>();
            NameValues.CollectionChanged += NameValues_CollectionChanged;
            Attributes = new KeyObservableCollection<AttributeKeyEnum, IAttributeDefinition>();
            Attributes.CollectionChanged += Attributes_CollectionChanged;
            Attributes.Get += Attributes_Get;
        }

        private void Attributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IAttributeDefinition oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IAttributeDefinition newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }

        private void NameValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemDefinitionValue<string> oldItem in e.OldItems)
                {
                    RawItem.StringData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemDefinitionValue<string> newItem in e.NewItems)
                {
                    RawItem.StringData.Add(newItem.RawItem);
                }
            }
        }

        private void FormatValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;

            S.IItem formatItem = RawItem.Children.Find(i => i.TypeId == (int)ItemDefinitionTypeEnum.AttributeDefinitionValueFormat);
            if (formatItem == null)
            {
                formatItem = Module.I<S.IItemManager>().GetNewItemDefinition();
                formatItem.TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionValueFormat;
                RawItem.Children.Add(formatItem);
            }

            if (e.OldItems != null)
            {
                foreach (IItemDefinitionValue<string> oldItem in e.OldItems)
                {
                    formatItem.StringData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemDefinitionValue<string> newItem in e.NewItems)
                {
                    formatItem.StringData.Add(newItem.RawItem);
                }
            }
        }

        private void DisplayValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;

            S.IItem displayItem = RawItem.Children.Find(i => i.TypeId == (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue);
            if (displayItem == null)
            {
                displayItem = Module.I<S.IItemManager>().GetNewItemDefinition();
                displayItem.TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue;
                RawItem.Children.Add(displayItem);
            }

            if (e.OldItems != null)
            {
                foreach (IItemDefinitionValue<string> oldItem in e.OldItems)
                {
                    displayItem.StringData.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemDefinitionValue<string> newItem in e.NewItems)
                {
                    displayItem.StringData.Add(newItem.RawItem);
                }
            }
        }

        private IAttributeDefinition Attributes_Get(AttributeKeyEnum param)
        {
            return Module.I<IAttributeLocator>().Find(Attributes, a => a.Key == param);
        }

        public IAttributeDefinition Find(Predicate<IAttributeDefinition> match)
        {
            return Module.I<IAttributeLocator>().Find(new IAttributeDefinition[] { this }, match);
        }

        public List<IAttributeDefinition> GetAllAttributes()
        {
            List<IAttributeDefinition> attrs = new List<IAttributeDefinition>();
            attrs.Add(this);
            attrs.AddRange(getAllAttributes(this.Attributes));
            return attrs;
        }

        private List<IAttributeDefinition> getAllAttributes(IEnumerable<IAttributeDefinition> attrs)
        {
            List<IAttributeDefinition> retAttrs = new List<IAttributeDefinition>();
            retAttrs.AddRange(attrs);
            foreach (var attr in attrs)
            {
                if (attr.Attributes.Count > 0)
                    retAttrs.AddRange(getAllAttributes(attr.Attributes));
            }
            return retAttrs;
        }

        public string GetDisplayValue()
        {
            return getStringValue(DisplayValues);
        }

        public string GetFormatValue(int? languageId = null)
        {
            if (languageId == null) languageId = Module.LanguageId;
            string value = "";
            foreach (IItemValue<string> val in FormatValues)
            {
                if (val.LanguageId == languageId)
                {
                    value = val.Value;
                    break;
                }
                else if (val.LanguageId == 0)
                {
                    value = val.Value;
                }
            }

            return value;
        }

        public string GetNameValue()
        {
            return getStringValue(NameValues);
        }

        public virtual List<IAttributeDefinition> Get(string storedProcedure = null, List<SySqlParameter> parameters = null)
        {
            if (storedProcedure == null) storedProcedure = SPNames.GetAttributeDefinition;
            List<S.IItem> items = Module.I<S.IItemManager>().GetItemDefinitions(storedProcedure, parameters);
            List<IAttributeDefinition> attDefs = new List<IAttributeDefinition>();
            foreach (S.IItem item in items)
            {
                attDefs.Add(get(item));
            }

            return attDefs;
        }

        public dynamic GetReferenceObject()
        {
            return Reference.I.GetObject(ReferenceType.Value, ReferenceId ?? (int)SystemListReferenceId);
        }

        public List<ICommonReferenceListItem> GetReferenceList()
        {
            return Reference.I.ConvertToCommonReferenceList(GetReferenceObject(), Module.LanguageId);
        }

        public List<dynamic> GetReferenceListValues(List<int> listValuesId)
        {
            return Reference.I.GetListValues(GetReferenceList(), listValuesId);
        }

        private string getStringValue(IEnumerable<IItemValue<string>> itemValue)
        {
            int languageId = Module.LanguageId;
            int preferedLanguageId = Module.I<IConfig>().GetValue(ConfigNames.PreferedLanguageIdIfNoTranslation);
            string value = "";
            foreach (IItemValue<string> val in itemValue)
            {
                if (!string.IsNullOrEmpty(val.Value))
                {
                    if (val.LanguageId == languageId)
                    {
                        value = val.Value;
                        break;
                    }
                    else if (val.LanguageId == preferedLanguageId || string.IsNullOrEmpty(value))
                        value = val.Value;
                }
            }

            return value;
        }

        private IAttributeDefinition get(S.IItem item)
        {
            AttributeDefinition attDef = ModuleDefinitions.Module.I<AttributeDefinition>();
            attDef.inLoadProcess = true;

            attDef.RawItem = item;
            foreach (S.IItemData<string> i in item.StringData)
            {
                IItemDefinitionValue<string> iv = ModuleDefinitions.Module.I<IItemDefinitionValue<string>>();
                iv.RawItem = i;
                attDef.NameValues.Add(iv);
            }

            S.IItem itemD = null;
            S.IItem itemF = null;
            foreach (S.IItem i in item.Children)
            {
                if (i.TypeId == (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue)
                {
                    itemD = i;
                }
                else if (i.TypeId == (int)ItemDefinitionTypeEnum.AttributeDefinitionValueFormat)
                {
                    itemF = i;
                }
                else if (i.TypeId == (int)ItemDefinitionTypeEnum.AttributeDefinition)
                {
                    attDef.Attributes.Add(get(i));
                }
            }

            if (itemD != null)
            {
                foreach (S.IItemData<string> itemDDS in itemD.StringData)
                {
                    IItemDefinitionValue<string> iv = ModuleDefinitions.Module.I<IItemDefinitionValue<string>>();
                    iv.RawItem = itemDDS;
                    attDef.DisplayValues.Add(iv);
                }
            }
            if (itemF != null)
            {
                foreach (S.IItemData<string> itemDDS in itemF.StringData)
                {
                    IItemDefinitionValue<string> iv = ModuleDefinitions.Module.I<IItemDefinitionValue<string>>();
                    iv.RawItem = itemDDS;
                    attDef.FormatValues.Add(iv);
                }
            }
            attDef.inLoadProcess = false;

            return attDef;
        }
    }

    public class CommonReferenceListItem : ICommonReferenceListItem
    {
        public int Id { get; set; }
        public dynamic Value { get; set; }
    }
}
