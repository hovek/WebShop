using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects.ModuleDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
	public class EditAttributeTemplateGroup
	{
		public int GroupId { get; set; }
		public int LanguageId { get; set; }
		public string GroupName { get; set; }
		public int TemplateId { get; set; }
		public int TemplateElementId { get; set; }
		public string Action { get; set; }

		public IAttributeTemplate Template
		{
			get
			{
				return Module.I<ICache>(CacheNames.AdminCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(TemplateId).FirstOrDefault();
			}
		}

		public void Save()
		{
			WebShopDb context = new WebShopDb();
			Module.I<S.IItemManager>().AssociateWithDbContext(context, Template.RawItem);

			IAttributeTemplate template = Template;
			IAttributeTemplateAttributeGroup templateGroup = template.GetAllElements().Find(e => e.Id == TemplateElementId);
			if (Action == "add")
			{
				IAttributeTemplateAttributeGroup newTemplateGroup = Module.I<IAttributeTemplateAttributeGroup>();
				if (templateGroup != null)
				{
					newTemplateGroup.Order = Math.Max((templateGroup.Attributes.Count == 0 ? 0 : templateGroup.Attributes.Max(a => a.Order)),
						(templateGroup.Count == 0 ? 0 : templateGroup.Max(a => a.Order))) + 1;
					templateGroup.Add(newTemplateGroup);
				}
				else
				{
					newTemplateGroup.Order = Math.Max((template.Attributes.Count == 0 ? 0 : template.Attributes.Max(a => a.Order)),
						(template.Groups.Count == 0 ? 0 : template.Groups.Max(a => a.Order))) + 1;
					template.Groups.Add(newTemplateGroup);
				}

				templateGroup = newTemplateGroup;
			}

			IItemDefinitionValue<string> groupNameValue = templateGroup.DisplayValues.Find(i => i.LanguageId == LanguageId);
			if (groupNameValue == null)
			{
				groupNameValue = Module.I<IItemDefinitionValue<string>>();
				groupNameValue.LanguageId = LanguageId;
				templateGroup.DisplayValues.Add(groupNameValue);
			}

			groupNameValue.Value = GroupName;

			context.SaveChanges();
			ICache cache = Module.I<ICache>(CacheNames.AdminCache);
			cache.AppendClearBuffer(typeof(IAttributeTemplate));
			cache.Clear();
		}
	}
}