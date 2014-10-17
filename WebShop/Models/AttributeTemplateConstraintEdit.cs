using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class AttributeTemplateConstraintEdit
    {
        public int TemplateId { get; set; }
        public int LanguageId { get; set; }
        public int TemplateElementId { get; set; }
        public int AttributeDefinitionId { get; set; }
        public List<ItemDefinitionDataTypeEnum> AllowedConstraints { get; private set; }

        public IAttributeTemplate Template
        {
            get
            {
                return Module.I<ICache>(CacheNames.AdminCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(TemplateId).First();
            }
        }

        private IAttributeTemplateAttribute templateAttribute;
        public IAttributeTemplateAttribute TemplateAttribute
        {
            get
            {
                if (templateAttribute == null)
                {
                    templateAttribute = Template.GetAllElements().Find(g => g is IAttributeTemplateAttribute && g.Id == TemplateElementId);
                }
                return templateAttribute;
            }
        }

        private List<KeyValuePair<ItemDefinitionDataTypeEnum, dynamic>> constraints;
        public List<KeyValuePair<ItemDefinitionDataTypeEnum, dynamic>> Constraints
        {
            get
            {
                if (constraints == null)
                {
                    constraints = getConstraints();
                }
                return constraints;
            }
        }

        private IAttributeLocator attributeLocator;
        public IAttributeLocator AttributeLocator
        {
            get
            {
                if (attributeLocator == null)
                {
                    attributeLocator = Module.I<IAttributeLocator>();
                    attributeLocator.Cache = Module.I<ICache>(CacheNames.AdminCache);
                }
                return attributeLocator;
            }
        }

        public AttributeTemplateConstraintEdit()
        {
            AllowedConstraints = new List<ItemDefinitionDataTypeEnum>() {
                ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute
            };
        }

        public List<SelectListItem> GetForeignKeyAttributeLists(AttributeDataSystemListReferenceEnum systemListReference, int? selectedValue = null)
        {
            IEnumerable<AttributeDataSystemListReferenceEnum> foreignLists = SystemListForeignKeyFilter.Filters.Keys.Where(f => f.Value == systemListReference).Select(i => i.Key);
            List<IAttributeTemplateAttribute> templateAttributes = Template.GetAllAttributes();

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = string.Concat("-- ", Translation.Get("Odaberite"), " --"),
                Value = ""
            });

            foreach (var fl in foreignLists)
            {
                IAttributeDefinition attributeDefinition = AttributeLocator.AttributeDefinitions.Find(a => a.SystemListReferenceId == fl);
                if (attributeDefinition != null)
                {
                    items.Add(new SelectListItem
                    {
                        Selected = attributeDefinition.Id == selectedValue,
                        Text = attributeDefinition.GetNameValue(),
                        Value = attributeDefinition.Id.ToString()
                    });
                }
            }

            return items;
        }

        private List<KeyValuePair<ItemDefinitionDataTypeEnum, dynamic>> getConstraints()
        {
            List<KeyValuePair<ItemDefinitionDataTypeEnum, dynamic>> constraints = new List<KeyValuePair<ItemDefinitionDataTypeEnum, dynamic>>();

            IAttributeTemplateAttribute templateAttribute = TemplateAttribute;
            IAttributeDefinition attributeDefinition;

            if (templateAttribute == null || templateAttribute.AttributeId != AttributeDefinitionId)
                attributeDefinition = AttributeLocator.Find(a => a.Id == AttributeDefinitionId);
            else
                attributeDefinition = templateAttribute.Attribute;

            foreach (ItemDefinitionDataTypeEnum ac in AllowedConstraints)
            {
                dynamic value = null;
                dynamic defaultValue = null;

                if (templateAttribute != null)
                {
                    var constraint = templateAttribute.Constraints.Find(c => c.TypeId == ac);
                    defaultValue = constraint == null ? null : constraint.GetRawValue();
                }

                if (ac == ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute)
                {
                    if (!attributeDefinition.SystemListReferenceId.HasValue) continue;
                    value = GetForeignKeyAttributeLists(attributeDefinition.SystemListReferenceId.Value, defaultValue);
                    if (value.Count < 2) continue;
                }
                else value = defaultValue;

                constraints.Add(new KeyValuePair<ItemDefinitionDataTypeEnum, dynamic>(ac, value));
            }


            return constraints;
        }
    }
}