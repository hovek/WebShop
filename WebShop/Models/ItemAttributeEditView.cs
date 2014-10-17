using Syrilium.CommonInterface.Caching;
using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class ItemAttributeEditView
    {
        public int? AttributeDefinitionId { get; set; }

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

        public string Name
        {
            get
            {
                if (AttributeDefinition != null)
                    return AttributeDefinition.GetNameValue();
                else
                    return "";
            }
        }

        public string Display
        {
            get
            {
                if (AttributeDefinition != null)
                    return AttributeDefinition.GetDisplayValue();
                else
                    return "";
            }
        }

        public string Format
        {
            get
            {
                if (AttributeDefinition != null)
                    return AttributeDefinition.GetFormatValue();
                else
                    return "";
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

        public bool IsAttributeDataTypeSystemListReference
        {
            get
            {
                return AttributeDefinition != null && AttributeDefinition.SystemListReferenceId.HasValue;
            }
        }

        public AttributeDataTypeEnum? AttributeDataType
        {
            get
            {
                return AttributeDefinition == null ? null : (AttributeDataTypeEnum?)AttributeDefinition.DataType;
            }
        }

        public string LongitudeDisplay
        {
            get
            {
                if (AttributeDefinition != null && AttributeDefinition.DataType == AttributeDataTypeEnum.Coordinates)
                {
                    return AttributeDefinition.Find(a => a.Key == AttributeKeyEnum.Longitude).GetDisplayValue();
                }
                return "";
            }
        }

        public string LatitudeDisplay
        {
            get
            {
                if (AttributeDefinition != null && AttributeDefinition.DataType == AttributeDataTypeEnum.Coordinates)
                {
                    return AttributeDefinition.Find(a => a.Key == AttributeKeyEnum.Latitude).GetDisplayValue();
                }
                return "";
            }
        }

        public List<SelectListItem> GetReferenceTypes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Županija"),
                Value = ((int)AttributeDataSystemListReferenceEnum.County).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Općina/Grad"),
                Value = ((int)AttributeDataSystemListReferenceEnum.DistrictCity).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Partner"),
                Value = ((int)AttributeDataSystemListReferenceEnum.Partner).ToString()
            });

            items.Sort((i1, i2) => i1.Text.CompareTo(i2.Text));

            if (IsAttributeDataTypeSystemListReference)
            {
                string attrReference = ((int)AttributeDefinition.SystemListReferenceId).ToString();
                foreach (SelectListItem item in items)
                {
                    if (item.Value == attrReference)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            return items;
        }

        public List<SelectListItem> GetDataTypes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Da/Ne"),
                Value = ((int)AttributeDataTypeEnum.Bool).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Koordinate"),
                Value = ((int)AttributeDataTypeEnum.Coordinates).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Datum i vrijeme"),
                Value = ((int)AttributeDataTypeEnum.DateTime).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Broj sa decimalama"),
                Value = ((int)AttributeDataTypeEnum.Decimal).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Slike"),
                Value = ((int)AttributeDataTypeEnum.Image).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Broj"),
                Value = ((int)AttributeDataTypeEnum.Int).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Veza"),
                Value = ((int)AttributeDataTypeEnum.Reference).ToString()
            });
            items.Add(new SelectListItem
            {
                Text = Translation.Get("Tekst"),
                Value = ((int)AttributeDataTypeEnum.String).ToString()
            });

            items.Sort((i1, i2) => i1.Text.CompareTo(i2.Text));

            if (AttributeDefinition != null)
            {
                string dataType = ((int)AttributeDefinition.DataType).ToString();
                foreach (SelectListItem item in items)
                {
                    if (item.Value == dataType)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            return items;
        }
    }
}