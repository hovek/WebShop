using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects.ModuleDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using System.Collections.Specialized;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class EditAttributeTemplateAttribute
    {
        public int GroupId { get; set; }
        public int LanguageId { get; set; }
        public int TemplateId { get; set; }
        public int TemplateElementId { get; set; }
        public string Action { get; set; }
        public int AttributeDefinitionId { get; set; }
        public NameValueCollection QueryString { get; set; }

        public IAttributeTemplate Template
        {
            get
            {
                return Module.I<ICache>(CacheNames.AdminCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(TemplateId).FirstOrDefault();
            }
        }

        public List<IAttributeTemplateConstraint> GetConstraintsFromQueryString()
        {
            List<IAttributeTemplateConstraint> constraints = new List<IAttributeTemplateConstraint>();

            foreach (string key in QueryString.AllKeys)
            {
                if (key.StartsWith("cns_"))
                {
                    string value = QueryString[key];
                    if (string.IsNullOrWhiteSpace(value)) continue;

                    string[] parts = key.Split('_');
                    IAttributeTemplateConstraint constraint = Module.I<IAttributeTemplateConstraint>();
                    constraint.TypeId = (ItemDefinitionDataTypeEnum)int.Parse(parts[1]);
                    if (constraint.TypeId == ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute)
                    {
                        IItemDefinitionValue<int> itemValue = Module.I<IItemDefinitionValue<int>>();
                        itemValue.LanguageId = LanguageId;
                        itemValue.Value = int.Parse(value);
                        constraint.IntValues.Add(itemValue);
                    }
                    constraints.Add(constraint);
                }
            }

            return constraints;
        }

        public void Save()
        {
            if (AttributeDefinitionId < 1) return;

            WebShopDb context = new WebShopDb();
            IAttributeTemplate template = Template;
            Module.I<S.IItemManager>().AssociateWithDbContext(context, template.RawItem);

            IAttributeTemplateAttribute templateAttribute;
            if (Action == "add")
            {
                IAttributeTemplateAttribute newTemplateAttribute = Module.I<IAttributeTemplateAttribute>();

                IAttributeTemplateAttributeGroup templateGroup = template.GetAllElements().Find(e => e.Id == TemplateElementId);
                if (templateGroup != null)
                {
                    newTemplateAttribute.Order = Math.Max((templateGroup.Attributes.Count == 0 ? 0 : templateGroup.Attributes.Max(a => a.Order)),
                    (templateGroup.Count == 0 ? 0 : templateGroup.Max(a => a.Order))) + 1;
                    templateGroup.Attributes.Add(newTemplateAttribute);
                }
                else
                {
                    newTemplateAttribute.Order = Math.Max((template.Attributes.Count == 0 ? 0 : template.Attributes.Max(a => a.Order)),
                        (template.Groups.Count == 0 ? 0 : template.Groups.Max(a => a.Order))) + 1;
                    template.Attributes.Add(newTemplateAttribute);
                }

                templateAttribute = newTemplateAttribute;
            }
            else
            {
                templateAttribute = template.GetAllAttributes().Find(e => e.Id == TemplateElementId);
                context.Database.ExecuteSqlCommand("delete id from ItemDefinition id where id.ParentId = {0} and id.TypeId = 10", templateAttribute.Id);
            }

            templateAttribute.AttributeId = AttributeDefinitionId;
            templateAttribute.Constraints.AddRange(GetConstraintsFromQueryString());

            context.SaveChanges();
            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            cache.AppendClearBuffer(typeof(IAttributeTemplate));
            cache.Clear();
        }
    }
}