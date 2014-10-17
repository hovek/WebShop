using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using SB = Syrilium.Modules.BusinessObjects.Item;
using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class OrderGroup
    {
        public string OrderData { get; set; }

        public void Save()
        {
            string[] dataParts = OrderData.Split(';');
            int parentId = int.Parse(dataParts[0]);
            int positionIndex = int.Parse(dataParts[1]);
            List<int> elements = dataParts[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(e => int.Parse(e));

            IItem item = Module.I<IItem>();
            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            List<IItem> groups = Module.I<IGroup>().Get(null, false, cache);

            WebShopDb context = new WebShopDb();
            S.IItemManager itemManager = Module.I<S.IItemManager>();
            foreach (IItem group in groups)
                itemManager.AssociateWithDbContext(context, group.RawItem);

            List<IItem> parentElements;

            if (parentId < 1)
                parentElements = groups;
            else
                parentElements = item.Find(groups, g => g.Id == parentId).ToList();

            int? newParentId = parentId < 1 ? null : (int?)parentId;
            List<IItem> orderedElements = new List<IItem>();
            foreach (var elementId in elements)
            {
                IItem itm = item.Find(groups, i => i.Id == elementId);
                itm.ParentId = newParentId;
                itm.TypeId = newParentId.HasValue ? ItemTypeEnum.Group : ItemTypeEnum.Department;
                orderedElements.Add(itm);
            }

            List<IItem> finalElements = new List<IItem>();
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
                S.IItemData<int> data = element.RawItem.IntData.Find(i => i.TypeId == (int)ItemDataTypeEnum.GroupOrder);
                if (data == null)
                {
                    data = itemManager.GetNewItemData<int>();
                    data.TypeId = (int)ItemDataTypeEnum.GroupOrder;
                    element.RawItem.IntData.Add(data);
                }

                data.Value = index++;
            }

            context.SaveChanges();

            cache.AppendClearBuffer(Module.I<IGroup>().Get(null, false, cache));
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
    }
}