using Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface.Caching;
using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
	public class OrderAttributeTemplate
	{
		public string OrderData { get; set; }
		public int TemplateId { get; set; }

		private IAttributeTemplate template;
		public IAttributeTemplate Template
		{
			get
			{
				if (template == null)
				{
					template = Module.I<ICache>(CacheNames.AdminCache).I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType()).Get(TemplateId).FirstOrDefault();
				}
				return template;
			}
		}

		public void Save()
		{
			string[] dataParts = OrderData.Split(';');
			int parentId = int.Parse(dataParts[0]);
			int positionIndex = int.Parse(dataParts[1]);
			List<int> elements = dataParts[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(e => int.Parse(e));

			WebShopDb context = new WebShopDb();
			Module.I<IItemManager>().AssociateWithDbContext(context, Template.RawItem);

			IAttributeTemplateAttributeGroup parentGroup = null;
			List<dynamic> parentElements;
			if (parentId < 1)
			{
				parentElements = Template.GetSortedAttributesAndGroups();
			}
			else
			{
				parentGroup = getTemplateGroup(parentId, Template.Groups);
				parentElements = parentGroup.GetSortedAttributesAndGroups();
			}

			List<dynamic> allElements = Template.GetAllElements();
			List<dynamic> orderedElements = new List<dynamic>();
			foreach (var elementId in elements)
			{
				orderedElements.Add(allElements.Find(i => i.Id == elementId));
			}
			List<dynamic> finalElements = new List<dynamic>();

			//prvo popunimo listu finalElements sa elementima koji se ne nalaze u elementima koje premještamo i podešavamo positionIndex
			int index = 0;
			foreach (var parentElement in parentElements)
			{
				if (!orderedElements.Exists(e => e.Id == parentElement.Id))
				{
					finalElements.Add(parentElement);
				}
				else if (index <= positionIndex)
				{
					positionIndex--;
				}
				index++;
			}
			//dodajemo elemente koji se premještaju na odgovarajući index
			foreach (var orderedElement in orderedElements)
			{
				finalElements.Insert(positionIndex, orderedElement);
				positionIndex++;
			}
			//postavljamo novi order broj
			index = 1;
			foreach (var element in finalElements)
			{
				element.Order = index++;
			}

			//sa templatea brišemo sve elemente koji se premještaju
			if (parentGroup != null)
			{
				parentGroup.Attributes.ToList().ForEach(a => parentGroup.Attributes.Remove(a));
				parentGroup.ToList().ForEach(g => parentGroup.Remove(g));
				Template.Attributes.ToList().ForEach(a =>
				{
					if (orderedElements.Contains(a)) Template.Attributes.Remove(a);
				});
				Template.Groups.ToList().ForEach(g =>
				{
					if (orderedElements.Contains(g)) Template.Groups.Remove(g);
				});
			}
			else
			{
				Template.Attributes.ToList().ForEach(a => Template.Attributes.Remove(a));
				Template.Groups.ToList().ForEach(g => Template.Groups.Remove(g));
			}

			List<IAttributeTemplateAttributeGroup> allGroups = allElements.Where(e => e is IAttributeTemplateAttributeGroup).ToList().ConvertAll(g => (IAttributeTemplateAttributeGroup)g);
			deleteAllElements(orderedElements, allGroups);

			//dodajemo sve elemente koji se premještaju u parent element
			foreach (var el in finalElements)
			{
				if (parentGroup != null)
				{
					if (el is IAttributeTemplateAttributeGroup)
						parentGroup.Add((IAttributeTemplateAttributeGroup)el);
					else
						parentGroup.Attributes.Add(el);
				}
				else
				{
					if (el is IAttributeTemplateAttributeGroup)
						Template.Groups.Add(el);
					else
						Template.Attributes.Add(el);
				}
			}

			context.SaveChanges();

			ICache cache = Module.I<ICache>(CacheNames.AdminCache);
			cache.AppendClearBuffer(typeof(IAttributeTemplate));
			cache.Clear();
		}

		private void deleteAllElements(List<dynamic> elements, List<IAttributeTemplateAttributeGroup> groups)
		{
			foreach (var group in groups)
			{
				group.Attributes.ToList().ForEach(i =>
				{
					if (elements.Contains(i))
					{
						group.Attributes.Remove(i);
						elements.Remove(i);
					}
				});
				group.ToList().ForEach(i =>
				{
					if (elements.Contains(i))
					{
						group.Remove(i);
						elements.Remove(i);
					}
				});
			}
		}

		private IAttributeTemplateAttributeGroup getTemplateGroup(int groupId, IEnumerable<IAttributeTemplateAttributeGroup> groups)
		{
			foreach (var group in groups)
			{
				if (group.Id == groupId) return group;
				var retGroup = getTemplateGroup(groupId, group);
				if (retGroup != null) return retGroup;
			}
			return null;
		}
	}
}