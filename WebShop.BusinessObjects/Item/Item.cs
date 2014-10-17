using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.DataAccessInterface;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjects.ModuleDefinitions;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.Modules.BusinessObjects;
using System.Globalization;
using System.Data;
using Syrilium.CommonInterface.Caching;
using Syrilium.CommonInterface;
using Syrilium.DataAccessInterface.SQL;

namespace WebShop.BusinessObjects.Item
{
    public class Item : ObservableCollection<IItem>, IItem
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
        public ItemTypeEnum TypeId
        {
            get
            {
                return (ItemTypeEnum)RawItem.TypeId;
            }
            set
            {
                RawItem.TypeId = (int)value;
            }
        }
        public KeyObservableCollection<AttributeKeyEnum, IItemAttribute> Attributes { get; private set; }

        public int? ParentId
        {
            get
            {
                return RawItem.ParentId;
            }
            set
            {
                RawItem.ParentId = value;
            }
        }

        private S.IItem rawItem;
        public S.IItem RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItem();
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }

        public Item()
        {
            this.CollectionChanged += children_CollectionChanged;
            Attributes = new KeyObservableCollection<AttributeKeyEnum, IItemAttribute>();
            Attributes.Get += Attributes_GetItem;
            Attributes.CollectionChanged += attributes_CollectionChanged;
        }

        private IAttributeLocator attributeLocator
        {
            get
            {
                IAttributeLocator attributeLocator = Module.I<IAttributeLocator>();
                attributeLocator.Cache = this.Cache;
                return attributeLocator;
            }
        }

        public IItemAttribute GetItemAttribute(AttributeKeyEnum attributeKey, AttributeKeyEnum? attributeKeyChild = null)
        {
            int? attributeId = attributeLocator[attributeKey];
            if (!attributeId.HasValue) return null;
            return GetItemAttribute(attributeId.Value, attributeKeyChild);
        }

        public IItemAttribute GetItemAttribute(int attributeDefinitionId, AttributeKeyEnum? attributeKey = null)
        {
            IAttributeDefinition attrDef = attributeLocator.Find(a => a.Id == attributeDefinitionId);
            if (attributeKey.HasValue && attrDef != null)
                attrDef = attrDef.Find(a => a.Key == attributeKey.Value);

            if (attrDef == null) return null;

            return Attributes.FirstOrDefault(i => i.Attribute.Id == attrDef.Id);
        }

        public dynamic GetRawValue(AttributeKeyEnum attributeKey, AttributeKeyEnum attributeKeyChild)
        {
            IItemAttribute itemAttr = GetItemAttribute(attributeKey, attributeKeyChild);
            return itemAttr == null ? null : itemAttr.GetRawValue();
        }

        public dynamic GetRawValue(int attributeDefinitionId, AttributeKeyEnum attributeKey)
        {
            IItemAttribute itemAttr = GetItemAttribute(attributeDefinitionId, attributeKey);
            return itemAttr == null ? null : itemAttr.GetRawValue();
        }

        public dynamic GetRawValue(AttributeKeyEnum attributeKey)
        {
            IItemAttribute itemAttr = Attributes[attributeKey];
            return itemAttr == null ? null : itemAttr.GetRawValue();
        }

        public dynamic GetRawValue(int attributeDefinitionId)
        {
            IItemAttribute itemAttr = getItemAttribute(Attributes, attributeDefinitionId);
            return itemAttr == null ? null : itemAttr.GetRawValue();
        }

        public string GetValueFormated(AttributeKeyEnum attributeKey)
        {
            IItemAttribute itemAttr = Attributes[attributeKey];
            return itemAttr == null ? "" : itemAttr.GetValueFormated();
        }

        public string GetValueFormated(int attributeDefinitionId)
        {
            IItemAttribute itemAttr = getItemAttribute(Attributes, attributeDefinitionId);
            return itemAttr == null ? "" : itemAttr.GetValueFormated();
        }

        public IItem Find(IList<IItem> items, Predicate<IItem> match)
        {
            foreach (IItem item in items)
            {
                if (match(item))
                    return item;

                IItem rez = Find(item, match);
                if (rez != null) return rez;
            }

            return null;
        }

        public List<IItem> GetAllItems(IList<IItem> items)
        {
            List<IItem> all = new List<IItem>(items);

            foreach (IItem item in items)
                all.AddRange(GetAllItems(item));

            return all;
        }

        public IItem Get(int itemId, params AttributeKeyEnum[] attributeKeys)
        {
            List<SySqlParameter> parameters = new List<SySqlParameter>();
            SySqlParameter param = new SySqlParameter();
            param.ParameterName = "@itemId";
            param.Value = itemId;
            parameters.Add(param);

            StringBuilder attributesIds = new StringBuilder();
            string delimiter = "";
            foreach (AttributeKeyEnum attrKey in attributeKeys)
            {
                attributesIds.Append(delimiter);
                attributesIds.Append(attributeLocator[attrKey]);
                if (delimiter.Length == 0)
                {
                    delimiter = ",";
                }
            }

            if (attributesIds.Length > 0)
            {
                param = new SySqlParameter();
                param.ParameterName = "@attributeIds";
                param.Value = attributesIds.ToString();
                parameters.Add(param);
            }

            return Get(SPNames.GetItem, parameters).FirstOrDefault();
        }

        public virtual List<IItem> Get(string storedProcedure, List<SySqlParameter> parameters = null)
        {
            List<S.IItem> rawItems = Module.I<S.IItemManager>().GetItems(storedProcedure, parameters);
            return get(rawItems);
        }

        public List<IItem> Get(DataSet ds)
        {
            List<S.IItem> rawItems = Module.I<S.IItemManager>().GetItems(ds);
            return get(rawItems);
        }

        private List<IItem> get(List<S.IItem> rawItems)
        {
            List<IAttributeDefinition> attributeDefinitions = Cache.I<AttributeDefinition>().Get(SPNames.GetAttributeDefinition);
            List<IItem> items = new List<IItem>();
            foreach (S.IItem item in rawItems)
            {
                items.Add(get(item, attributeDefinitions));
            }

            return items;
        }

        private Item get(S.IItem rawItem, List<IAttributeDefinition> attributeDefinitions)
        {
            Item item = new Item();
            item.RawItem = rawItem;
            item.inLoadProcess = true;
            foreach (S.IItem itemC in rawItem.Children)
            {
                if (itemC.TypeId == (int)ItemTypeEnum.Attribute)
                {
                    item.Attributes.Add(ItemAttribute.GetItemAttribute(itemC, attributeDefinitions, Cache));
                }
                else
                {
                    item.Add(get(itemC, attributeDefinitions));
                }
            }
            item.inLoadProcess = false;

            return item;
        }

        public IItem GetChild(int childId)
        {
            return GetChild(childId, this);
        }

        public IItem GetChild(int childId, IList<IItem> items)
        {
            foreach (var i in items)
            {
                if (i.Id == childId)
                {
                    return i;
                }
                IItem subGroup = GetChild(childId, i);
                if (subGroup != null)
                {
                    return subGroup;
                }
            }

            return null;
        }

        private void children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItem oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItem newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }

        private void attributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (inLoadProcess) return;
            if (e.OldItems != null)
            {
                foreach (IItemAttribute oldItem in e.OldItems)
                {
                    RawItem.Children.Remove(oldItem.RawItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (IItemAttribute newItem in e.NewItems)
                {
                    RawItem.Children.Add(newItem.RawItem);
                }
            }
        }

        private IItemAttribute Attributes_GetItem(AttributeKeyEnum param)
        {
            int? attributeId = attributeLocator[param];
            if (!attributeId.HasValue) return null;
            return getItemAttribute(Attributes, attributeId.Value);
        }

        private IItemAttribute getItemAttribute(IList<IItemAttribute> atts, int id)
        {
            foreach (IItemAttribute att in atts)
            {
                if (att.Attribute.Id == id)
                {
                    return att;
                }
            }

            return null;
        }
    }

    public class ItemAttribute : IItemAttribute
    {
        private bool inLoadProcess = false;

        private IAttributeDefinition attribute;
        public IAttributeDefinition Attribute
        {
            get
            {
                return attribute;
            }
            set
            {
                attribute = value;
            }
        }
        public int AttributeId
        {
            get
            {
                foreach (S.IItemData<int> intVal in RawItem.IntData)
                {
                    if (intVal.TypeId == (int)ItemDataTypeEnum.AttributeId)
                        return intVal.Value;
                }

                throw new ArgumentNullException();
            }
            set
            {
                foreach (S.IItemData<int> intVal in RawItem.IntData)
                {
                    if (intVal.TypeId == (int)ItemDataTypeEnum.AttributeId)
                    {
                        intVal.Value = value;
                        return;
                    }
                }

                S.IItemData<int> newVal = Module.I<S.IItemManager>().GetNewItemData<int>();
                newVal.TypeId = (int)ItemDataTypeEnum.AttributeId;
                newVal.Value = value;
                RawItem.IntData.Add(newVal);
            }
        }

        public ObservableCollection<IItemValue<int>> IntValues { get; private set; }
        public ObservableCollection<IItemValue<string>> StringValues { get; private set; }
        public ObservableCollection<IItemValue<decimal>> DecimalValues { get; private set; }
        public ObservableCollection<IItemValue<DateTime>> DateTimeValues { get; private set; }

        private S.IItem rawItem;
        public S.IItem RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItem();
                    rawItem.TypeId = (int)ItemTypeEnum.Attribute;
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }

        #region FORMATED VALUES
        public string GetValueFormated(int? languageId = null)
        {
            return GetValuesFormated(languageId).FirstOrDefault() ?? "";
        }

        public List<string> GetValuesFormated(int? languageId = null)
        {
            List<dynamic> values;
            if (attribute.DataType == AttributeDataTypeEnum.Reference)
            {
                List<int> intValues = new List<int>();
                foreach (var i in GetRawValues(languageId))
                    intValues.Add((int)i);
                values = Attribute.GetReferenceListValues(intValues);
            }
            else
            {
                values = GetRawValues(languageId);
            }

            string formatValue = Attribute.GetFormatValue(languageId);
            List<string> formatedValues = new List<string>();
            foreach (dynamic value in values)
            {
                if (value is string)
                    formatedValues.Add(value);
                else if (value is bool)
                    formatedValues.Add(value.ToString());
                else
                    formatedValues.Add(value.ToString(formatValue, CultureInfo.InvariantCulture));
            }

            return formatedValues;
        }
        #endregion

        #region RAW VALUES
        public dynamic GetRawValue(int? languageId = null)
        {
            return GetRawValues(languageId).FirstOrDefault();
        }

        public List<dynamic> GetRawValues(int? languageId = null)
        {
            if (languageId == null) languageId = Module.LanguageId;
            List<dynamic> values = new List<dynamic>();
            switch (Attribute.DataType)
            {
                case AttributeDataTypeEnum.Bool:
                case AttributeDataTypeEnum.Int:
                case AttributeDataTypeEnum.Reference:
                    foreach (var val in IntValues.Where(i => i.LanguageId == languageId || i.LanguageId == 0))
                    {
                        if (Attribute.DataType == AttributeDataTypeEnum.Bool)
                            values.Add(Convert.ToBoolean(val.Value));
                        else
                            values.Add(val.Value);
                    }
                    break;
                case AttributeDataTypeEnum.String:
                case AttributeDataTypeEnum.Image:
                    foreach (var val in StringValues.Where(i => i.LanguageId == languageId || i.LanguageId == 0))
                        values.Add(val.Value);
                    break;
                case AttributeDataTypeEnum.DateTime:
                    foreach (var val in DateTimeValues.Where(i => i.LanguageId == languageId || i.LanguageId == 0))
                        values.Add(val.Value);
                    break;
                case AttributeDataTypeEnum.Decimal:
                    foreach (var val in DecimalValues.Where(i => i.LanguageId == languageId || i.LanguageId == 0))
                        values.Add(val.Value);
                    break;
            }
            return values;
        }
        #endregion

        #region ANY RAW VALUES
        //public dynamic GetAnyRawValue(int? languageId = null)
        //{
        //    IItemValue value = GetAnyRawItemValues(languageId).FirstOrDefault();
        //    return value != null ? value.ValueDynamic : null;
        //}

        //public List<dynamic> GetAnyRawValues(int? languageId = null)
        //{
        //    List<IItemValue> itemValues = GetAnyRawItemValues(languageId);
        //    List<dynamic> values = new List<dynamic>();
        //    foreach (var i in itemValues)
        //        values.Add(i.ValueDynamic);

        //    return values;
        //}

        //public List<IItemValue> GetAnyRawItemValues(int? languageId = null)
        //{
        //    switch (Attribute.DataType)
        //    {
        //        case AttributeDataTypeEnum.Bool:
        //            List<IItemValue> values = new List<IItemValue>();
        //            foreach (var itemValue in GetAnyRawItemValues(IntValues, languageId))
        //            {
        //                ItemValue newItemValue = new ItemValue
        //                {
        //                    LanguageId = itemValue.LanguageId,
        //                    ValueDynamic = Convert.ToBoolean(itemValue.ValueDynamic)
        //                };
        //                values.Add(newItemValue);
        //            }
        //            return values;
        //        case AttributeDataTypeEnum.Int:
        //        case AttributeDataTypeEnum.Reference:
        //            return GetAnyRawItemValues(IntValues, languageId);
        //        case AttributeDataTypeEnum.String:
        //        case AttributeDataTypeEnum.Image:
        //            return GetAnyRawItemValues(StringValues, languageId);
        //        case AttributeDataTypeEnum.DateTime:
        //            return GetAnyRawItemValues(DateTimeValues, languageId);
        //        case AttributeDataTypeEnum.Decimal:
        //            return GetAnyRawItemValues(DecimalValues, languageId);
        //        default:
        //            throw new NotImplementedException("Attribute.DataType not implemented.");
        //    }
        //}

        //public List<IItemValue> GetAnyRawItemValues(dynamic itemValues, int? languageId = null)
        //{
        //    if (languageId == null) languageId = Module.LanguageId;
        //    int preferedLanguageId = Module.I<IConfig>().GetValue(ConfigNames.PreferedLanguageIdIfNoTranslation);
        //    List<IItemValue> values = new List<IItemValue>();

        //    if (itemValues.Count > 0)
        //    {
        //        int? chosenLanguage = null;

        //        if (itemValues[0].Value is string)
        //        {
        //            foreach (var iv in itemValues)
        //            {
        //                if (!string.IsNullOrEmpty(iv.Value))
        //                {
        //                    if (iv.LanguageId == languageId)
        //                    {
        //                        chosenLanguage = languageId;
        //                        break;
        //                    }
        //                    else if (iv.LanguageId == preferedLanguageId || chosenLanguage == null)
        //                        chosenLanguage = iv.LanguageId;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var iv in itemValues)
        //            {
        //                if (iv.LanguageId == languageId)
        //                {
        //                    chosenLanguage = languageId;
        //                    break;
        //                }
        //                else if (iv.LanguageId == preferedLanguageId || chosenLanguage == null)
        //                    chosenLanguage = iv.LanguageId;
        //            }
        //        }

        //        if (chosenLanguage == null) chosenLanguage = languageId;

        //        foreach (var iv in itemValues)
        //        {
        //            if (iv.LanguageId == chosenLanguage)
        //                values.Add(iv);
        //        }
        //    }

        //    return values;
        //}
        #endregion

        public ItemAttribute()
        {
            IntValues = new ObservableCollection<IItemValue<int>>();
            IntValues.CollectionChanged += intValues_CollectionChanged;
            StringValues = new ObservableCollection<IItemValue<string>>();
            StringValues.CollectionChanged += stringValues_CollectionChanged;
            DecimalValues = new ObservableCollection<IItemValue<decimal>>();
            DecimalValues.CollectionChanged += decimalValues_CollectionChanged;
            DateTimeValues = new ObservableCollection<IItemValue<DateTime>>();
            DateTimeValues.CollectionChanged += dateTimeValues_CollectionChanged;
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

        public static ItemAttribute GetItemAttribute(S.IItem rawItem, List<IAttributeDefinition> attributeDefinitions, ICache cache = null)
        {
            if (cache == null) cache = Module.I<ICache>(CacheNames.MainCache);
            ItemAttribute attribute = new ItemAttribute();
            attribute.inLoadProcess = true;

            int? attributeDefinitionId = null;
            foreach (S.IItemData<int> i in rawItem.IntData)
            {
                if (i.TypeId == (int)ItemDataTypeEnum.AttributeId)
                {
                    attributeDefinitionId = i.Value;
                    break;
                }
            }
            attribute.Attribute = Module.I<IAttributeLocator>().Find(attributeDefinitions, a => a.Id == attributeDefinitionId, cache);
            attribute.RawItem = rawItem;
            foreach (S.IItemData<string> item in rawItem.StringData.Where(i => i.TypeId < 51))
            {
                IItemValue<string> itemValue = ModuleDefinitions.Module.I<IItemValue<string>>();
                itemValue.RawItem = item;
                attribute.StringValues.Add(itemValue);
            }
            foreach (S.IItemData<int> item in rawItem.IntData.Where(i => i.TypeId < 51))
            {
                IItemValue<int> itemValue = ModuleDefinitions.Module.I<IItemValue<int>>();
                itemValue.RawItem = item;
                attribute.IntValues.Add(itemValue);
            }
            foreach (S.IItemData<decimal> item in rawItem.DecimalData.Where(i => i.TypeId < 51))
            {
                IItemValue<decimal> itemValue = ModuleDefinitions.Module.I<IItemValue<decimal>>();
                itemValue.RawItem = item;
                attribute.DecimalValues.Add(itemValue);
            }
            foreach (S.IItemData<DateTime> item in rawItem.DateTimeData.Where(i => i.TypeId < 51))
            {
                IItemValue<DateTime> itemValue = ModuleDefinitions.Module.I<IItemValue<DateTime>>();
                itemValue.RawItem = item;
                attribute.DateTimeValues.Add(itemValue);
            }
            attribute.inLoadProcess = false;

            return attribute;
        }
    }

    public class ItemValue : IItemValue
    {
        public int LanguageId { get; set; }
        public dynamic ValueDynamic { get; set; }
    }

    public class ItemValue<T> : IItemValue<T>
    {
        public int LanguageId
        {
            get
            {
                return RawItem.TypeId;
            }
            set
            {
                RawItem.TypeId = value;
            }
        }
        public T Value
        {
            get
            {
                return RawItem.Value;
            }
            set
            {
                RawItem.Value = value;
            }
        }
        public dynamic ValueDynamic
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }
        private S.IItemData<T> rawItem;
        public S.IItemData<T> RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItemData<T>();
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }
    }

    public class ItemDefinitionValue<T> : IItemDefinitionValue<T>
    {
        public int LanguageId
        {
            get
            {
                return RawItem.TypeId;
            }
            set
            {
                RawItem.TypeId = value;
            }
        }
        public T Value
        {
            get
            {
                return RawItem.Value;
            }
            set
            {
                RawItem.Value = value;
            }
        }
        public dynamic ValueDynamic
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }
        private S.IItemData<T> rawItem;
        public S.IItemData<T> RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = Module.I<S.IItemManager>().GetNewItemDefinitionData<T>();
                }
                return rawItem;
            }
            set
            {
                rawItem = value;
            }
        }
    }
}