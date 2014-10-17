using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class Product
    {
        public int? ProductId { get; set; }

        private int? groupId = null;
        public int? GroupId
        {
            get
            {
                if (!groupId.HasValue && ProductId.HasValue)
                {
                    groupId = Module.I<IGroup>().GetClosestParentId(ProductId.Value,
                                    string.Concat((int)ItemTypeEnum.Department, ",", (int)ItemTypeEnum.Group),
                                    Module.I<ICache>(CacheNames.AdminCache));
                }
                return groupId;
            }
            set
            {
                groupId = value;
            }
        }

        public IAttributeTemplate AttributeTemplate
        {
            get
            {
                if (attributeTemplate == null)
                {
                    ICache cache = Module.I<ICache>(CacheNames.AdminCache);
                    attributeTemplate = cache.I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType());
                    attributeTemplate.Cache = cache;
                    attributeTemplate = attributeTemplate.Get(itemId: GroupId).FirstOrDefault();
                }

                return attributeTemplate;
            }
        }
        public IItem ProductObject
        {
            get
            {
                if (product == null && ProductId != null)
                {
                    product = Module.I<IItem>();
                    product.Cache = Module.I<ICache>(CacheNames.AdminCache);
                    product = product.Get(ProductId.Value);
                    product.Cache = Module.I<ICache>(CacheNames.AdminCache);
                }

                return product;
            }
            set
            {
                product = value;
            }
        }

        private IAttributeTemplate attributeTemplate { get; set; }
        private IItem product { get; set; }

        public List<IAttributeTemplateAttribute> GetAllImageTemplateAttributes()
        {
            List<IAttributeTemplateAttribute> templateAttributes = new List<IAttributeTemplateAttribute>();
            if (AttributeTemplate != null)
            {
                templateAttributes.AddRange(AttributeTemplate.Attributes.Where(a => a.Attribute.DataType == AttributeDataTypeEnum.Image));
                templateAttributes.AddRange(getAllImageTemplateAttributes(AttributeTemplate.Groups));
            }
            return templateAttributes;
        }

        private List<IAttributeTemplateAttribute> getAllImageTemplateAttributes(IList<IAttributeTemplateAttributeGroup> groups)
        {
            List<IAttributeTemplateAttribute> templateAttributes = new List<IAttributeTemplateAttribute>();

            foreach (IAttributeTemplateAttributeGroup group in groups)
            {
                templateAttributes.AddRange(group.Attributes.Where(a => a.Attribute.DataType == AttributeDataTypeEnum.Image));
                templateAttributes.AddRange(getAllImageTemplateAttributes(group));
            }

            return templateAttributes;
        }
    }
}