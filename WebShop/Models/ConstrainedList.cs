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
	public class ConstrainedList
	{
		private Dictionary<AttributeDataSystemListReferenceEnum, List<int>> constrainAttributesValues = new Dictionary<AttributeDataSystemListReferenceEnum, List<int>>();

		public int TemplateId { get; set; }
		public int ConstrainedAttributeId { get; set; }
		public int LanguageId { get; set; }

		public Dictionary<AttributeDataSystemListReferenceEnum, List<int>> ConstrainAttributesValues
		{
			get
			{
				return constrainAttributesValues;
			}
		}

		public IAttributeTemplate AttributeTemplate
		{
			get
			{
				return Module.I<ICache>(CacheNames.MainCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(TemplateId).First();
			}
		}

		public FormCollection FormCollection
		{
			set
			{
				List<IAttributeDefinition> attributeDefinitions = Module.I<ICache>(CacheNames.MainCache).I<IAttributeDefinition>(Module.I<IAttributeDefinition>().GetType()).Get(SPNames.GetAttributeDefinition);
				foreach (string key in value.AllKeys)
				{
					int attributeId;
					if (int.TryParse(key, out attributeId))
					{
						IAttributeDefinition attrDefinition = attributeDefinitions.Find(a => a.Id == attributeId);
						if (attrDefinition != null && attrDefinition.SystemListReferenceId != null)
						{
							if (!constrainAttributesValues.ContainsKey(attrDefinition.SystemListReferenceId.Value))
								constrainAttributesValues[attrDefinition.SystemListReferenceId.Value] = new List<int>();

							constrainAttributesValues[attrDefinition.SystemListReferenceId.Value].AddRange(
								value.GetValues(key).Where(a => !string.IsNullOrWhiteSpace(a)).ToList().ConvertAll(a => int.Parse(a))
							);
						}
					}
				}
			}
		}

		public List<ICommonReferenceListItem> GetList()
		{
			IAttributeTemplateAttribute constrainedAttribute = AttributeTemplate.GetAllAttributes().Find(a => a.Attribute.Id == ConstrainedAttributeId);
			if (constrainedAttribute != null)
			{
				return constrainedAttribute.GetConstrainedReferenceList(ConstrainAttributesValues, LanguageId);
			}

			return new List<ICommonReferenceListItem>();
		}
	}
}