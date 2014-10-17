using Syrilium.CommonInterface.Caching;
using Syrilium.Modules.BusinessObjects.ModuleDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public enum AttributeTemplateEditType
    {
        Group,
        Attribute,
        None
    }

    public class AttributeTemplateEdit
    {
        private dynamic templateElement;
        private int? languageId;

        public string Action { get; set; }
        public int TemplateId { get; set; }
        public int TemplateElementId { get; set; }
        public int GroupId { get; set; }
        public int LanguageId
        {
            get
            {
                if (languageId == null)
                {
                    languageId = SessionState.I.LanguageId;
                }

                return languageId.Value;
            }
            set
            {
                languageId = value;
            }
        }

        private AttributeTemplateConstraintEdit attributeTemplateConstraintEdit;
        public AttributeTemplateConstraintEdit AttributeTemplateConstraintEdit
        {
            get
            {
                if (attributeTemplateConstraintEdit == null)
                {
                    attributeTemplateConstraintEdit = new AttributeTemplateConstraintEdit
                    {
                        LanguageId = this.LanguageId,
                        TemplateId = this.TemplateId,
                        TemplateElementId = this.TemplateElementId
                    };
                    List<SelectListItem> attributeItems = GetAttributes();
                    SelectListItem attrItem = attributeItems.Find(i => i.Selected);
                    if (attrItem == null) attrItem = attributeItems.First();
                    attributeTemplateConstraintEdit.AttributeDefinitionId = int.Parse(attrItem.Value);
                }
                return attributeTemplateConstraintEdit;
            }
            set
            {
                attributeTemplateConstraintEdit = value;
            }
        }

        public IAttributeTemplate Template
        {
            get
            {
                return Module.I<ICache>(CacheNames.AdminCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(TemplateId).First();
            }
        }

        public dynamic TemplateElement
        {
            get
            {
                if (templateElement == null)
                {
                    templateElement = Template.GetAllElements().Find(g => g.Id == TemplateElementId);
                }
                return templateElement;
            }
        }

        public IAttributeTemplateAttributeGroup TemplateGroup
        {
            get
            {
                return TemplateElement is IAttributeTemplateAttributeGroup ? (IAttributeTemplateAttributeGroup)TemplateElement : null;
            }
        }

        public IAttributeTemplateAttribute TemplateAttribute
        {
            get
            {
                return TemplateElement is IAttributeTemplateAttribute ? (IAttributeTemplateAttribute)TemplateElement : null;
            }
        }

        public AttributeTemplateEditType EditType
        {
            get
            {
                if (Action == "edit")
                {
                    if (TemplateElement is IAttributeTemplateAttributeGroup)
                    {
                        return AttributeTemplateEditType.Group;
                    }
                    else
                    {
                        return AttributeTemplateEditType.Attribute;
                    }
                }

                return AttributeTemplateEditType.None;
            }
        }

        public AttributeTemplateEdit()
        {
        }

        public List<SelectListItem> GetAttributes()
        {
            IAttributeTemplateAttribute templateAttribute = TemplateAttribute;
            int selectedAttributeId = templateAttribute == null ? 0 : templateAttribute.Attribute.Id;

            IEnumerable<int> attributesToExclude = Template.GetAllAttributes().ConvertAll(a => a.Attribute.Id).Where(a => a != selectedAttributeId);
            IEnumerable<IAttributeDefinition> attributes = Module.I<ICache>(CacheNames.AdminCache).I<IAttributeDefinition>(Module.I<IAttributeDefinition>().GetType()).Get()
                .Where(a => !a.Hidden && !attributesToExclude.Contains(a.Id));

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var a in attributes)
            {
                list.Add(new SelectListItem
                {
                    Selected = a.Id == selectedAttributeId,
                    Text = a.GetNameValue(),
                    Value = a.Id.ToString()
                });
            }
            list.Sort((i1, i2) => i1.Text.CompareTo(i2.Text));

            return list;
        }
    }
}