using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
	public class Descriptions
	{
		public IItem Product { get; set; }

		public IAttributeTemplate Template
		{
			get
			{
				return Module.I<ICache>(CacheNames.MainCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(itemId: Product.Id).FirstOrDefault();
			}
		}

		private Dictionary<IAttributeDefinition, string> basicInfo;
		public Dictionary<IAttributeDefinition, string> BasicInfo
		{
			get
			{
				if (basicInfo == null)
				{
					basicInfo = new Dictionary<IAttributeDefinition, string>();
					if (Template != null)
					{
						IEnumerable<dynamic> attrs = Template.GetSortedAttributesAndGroups().Where(a => !(a is IAttributeTemplateAttributeGroup) && !attributesToExclude.Contains(((IAttributeTemplateAttribute)a).Attribute.Id));
						foreach (IAttributeTemplateAttribute attr in attrs)
						{
							IAttributeDefinition key = attr.Attribute;
							string value = Product.GetValueFormated(attr.AttributeId);
							basicInfo[key] = value;
						}
					}
				}
				return basicInfo;
			}
		}

		public bool IsAttributeDescription(int attrDefId)
		{
			int? descriptionAttributeId = Module.I<IAttributeLocator>()[AttributeKeyEnum.Description];
			return attrDefId == descriptionAttributeId;
		}

		private List<int> attributesToExclude;

		public KeyValuePair<string, string> GetAttributeNameValue(IAttributeTemplateAttribute attr)
		{
			return new KeyValuePair<string, string>(
				attr.Attribute.GetNameValue(),
				Product.GetValueFormated(attr.AttributeId));
		}

		public List<dynamic> GetElements()
		{
			if (Template == null) return new List<dynamic>();
			return GetElements(Template.GetSortedAttributesAndGroups().Where(g => (g is IAttributeTemplateAttributeGroup)));
		}

		public List<dynamic> GetElements(IAttributeTemplateAttributeGroup group)
		{
			if (Template == null) return new List<dynamic>();
			return GetElements(group.GetSortedAttributesAndGroups());
		}

		public List<dynamic> GetElements(IEnumerable<dynamic> elements)
		{
			List<dynamic> retElements = new List<dynamic>();
			foreach (var element in elements)
			{
				if (element is IAttributeTemplateAttributeGroup)
				{
					if (GroupHasElements(element))
						retElements.Add(element);
				}
				else
				{
					IAttributeTemplateAttribute attr = element;
					if (!attributesToExclude.Contains(attr.AttributeId) && AttributeHasValue(attr.Attribute))
						retElements.Add(element);
				}
			}
			return retElements;
		}

		public bool GroupHasElements(IAttributeTemplateAttributeGroup templateGroup)
		{
			if (templateGroup.Attributes.Exists(a => !attributesToExclude.Contains(a.AttributeId) && AttributeHasValue(a.Attribute))) return true;

			foreach (var group in templateGroup)
			{
				if (GroupHasElements(group)) return true;
			}
			return false;
		}

		public bool AttributeHasValue(IAttributeDefinition attr)
		{
			return !string.IsNullOrWhiteSpace(Product.GetValueFormated(attr.Id));
		}

		public Descriptions()
		{
			IAttributeLocator attributeLocator = Module.I<IAttributeLocator>();
			attributesToExclude = new List<int> { 
				attributeLocator[AttributeKeyEnum.AuctionDate]??0, 
				attributeLocator[AttributeKeyEnum.Image]??0,
				attributeLocator[AttributeKeyEnum.Location]??0,
				attributeLocator[AttributeKeyEnum.Name]??0,
				attributeLocator[AttributeKeyEnum.Partner]??0,
				attributeLocator[AttributeKeyEnum.Premium]??0,
				attributeLocator[AttributeKeyEnum.Top]??0,
				attributeLocator[AttributeKeyEnum.Show]??0,
				attributeLocator[AttributeKeyEnum.Price]??0};
			attributesToExclude.RemoveAll(a => a == 0);
		}
	}
}