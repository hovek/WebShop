using Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using System.Data.SqlClient;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class EditItemAttribute
    {
        public int? AttributeDefinitionId { get; set; }
        public string Name { get; set; }
        public string Display { get; set; }
        public string Format { get; set; }
        public AttributeDataTypeEnum DataType { get; set; }
        public AttributeDataSystemListReferenceEnum? SystemListReferenceType { get; set; }
        public string LatDisplay { get; set; }
        public string LngDisplay { get; set; }

        private IAttributeDefinition attributeDefinition;
        public IAttributeDefinition AttributeDefinition
        {
            get
            {
                if (attributeDefinition == null)
                {
                    IAttributeLocator al = Module.I<IAttributeLocator>();
                    al.Cache = Module.I<ICache>(CacheNames.AdminCache);
                    attributeDefinition = al.Find(a => a.Id == AttributeDefinitionId);
                }
                return attributeDefinition;
            }
        }

        public bool IsAttributeInUse
        {
            get
            {
                if (!AttributeDefinitionId.HasValue) return false;
                return WebShopDb.I.Database.SqlQuery<bool>(string.Concat(SPNames.IsAttributeInUse, " @AttributeDefinitionId"), new SqlParameter("@AttributeDefinitionId", AttributeDefinitionId.Value)).First();
            }
        }

        public bool DataTypeChangeNotAllowed
        {
            get
            {
                if (AttributeDefinition != null) return AttributeDefinition.DataTypeChangeNotAllowed;
                return false;
            }
        }

        public void Save()
        {
            int languageId = SessionState.I.LanguageId;

            WebShopDb context = WebShopDb.I;
            IAttributeDefinition attributeDefinition;
            if (AttributeDefinition == null)
            {
                attributeDefinition = Module.I<IAttributeDefinition>();
                context.ItemDefinition.Add((ItemDefinition)attributeDefinition.RawItem);
            }
            else
            {
                attributeDefinition = AttributeDefinition;
                Module.I<IItemManager>().AssociateWithDbContext(context, attributeDefinition.RawItem);
            }

            IItemDefinitionValue<string> nameValue = attributeDefinition.NameValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
            if (nameValue == null)
            {
                nameValue = Module.I<IItemDefinitionValue<string>>();
                nameValue.LanguageId = languageId;
                attributeDefinition.NameValues.Add(nameValue);
            }
            nameValue.Value = Name;

            IItemDefinitionValue<string> displayValue = attributeDefinition.DisplayValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
            if (displayValue == null)
            {
                displayValue = Module.I<IItemDefinitionValue<string>>();
                displayValue.LanguageId = languageId;
                attributeDefinition.DisplayValues.Add(displayValue);
            }
            displayValue.Value = Display;

            IItemDefinitionValue<string> formatValue = attributeDefinition.FormatValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
            if (formatValue == null)
            {
                formatValue = Module.I<IItemDefinitionValue<string>>();
                formatValue.LanguageId = languageId;
                attributeDefinition.FormatValues.Add(formatValue);
            }
            formatValue.Value = Format;

            if (DataTypeChangeNotAllowed) DataType = attributeDefinition.DataType;
            else attributeDefinition.DataType = DataType;

            if (DataType == AttributeDataTypeEnum.Reference)
                attributeDefinition.SystemListReferenceId = SystemListReferenceType;
            else
            {
                IItemData<int> itemData = attributeDefinition.RawItem.IntData.Find(i => i.TypeId == (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId);
                if (itemData != null)
                    context.Database.ExecuteSqlCommand("delete from ItemDefinitionDataInt where Id={0}", itemData.Id);
            }

            IAttributeDefinition lngAttr = attributeDefinition.Find(a => a.Key == AttributeKeyEnum.Longitude);
            IAttributeDefinition latAttr = attributeDefinition.Find(a => a.Key == AttributeKeyEnum.Latitude);
            if (DataType == AttributeDataTypeEnum.Coordinates)
            {
                if (lngAttr == null)
                {
                    lngAttr = Module.I<IAttributeDefinition>();
                    lngAttr.DataType = AttributeDataTypeEnum.Decimal;
                    lngAttr.Key = AttributeKeyEnum.Longitude;
                    displayValue = Module.I<IItemDefinitionValue<string>>();
                    displayValue.LanguageId = languageId;
                    lngAttr.DisplayValues.Add(displayValue);
                    attributeDefinition.Attributes.Add(lngAttr);
                }
                else displayValue = lngAttr.DisplayValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
                displayValue.Value = LngDisplay;

                if (latAttr == null)
                {
                    latAttr = Module.I<IAttributeDefinition>();
                    latAttr.DataType = AttributeDataTypeEnum.Decimal;
                    latAttr.Key = AttributeKeyEnum.Latitude;
                    displayValue = Module.I<IItemDefinitionValue<string>>();
                    displayValue.LanguageId = languageId;
                    latAttr.DisplayValues.Add(displayValue);
                    attributeDefinition.Attributes.Add(latAttr);
                }
                else displayValue = latAttr.DisplayValues.Find(i => i.LanguageId == 0 || i.LanguageId == languageId);
                displayValue.Value = LatDisplay;
            }
            else
            {
                if (lngAttr != null) context.Database.ExecuteSqlCommand("delete from ItemDefinition where Id={0}", lngAttr.Id);
                if (latAttr != null) context.Database.ExecuteSqlCommand("delete from ItemDefinition where Id={0}", latAttr.Id);
            }

            if (context.SaveChanges() > 0)
            {
                ICache cache = Module.I<ICache>(CacheNames.AdminCache);
                cache.AppendClearBuffer(typeof(IAttributeDefinition));
                cache.AppendClearBuffer(typeof(IAttributeTemplate));
                cache.Clear();
            }
        }
    }
}