using Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects.Item;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class AddTemplate
    {
        public int GroupId { get; set; }

        public IItem Group
        {
            get
            {
                return Module.I<IItem>().GetChild(GroupId, Module.I<IGroup>().Get(null, false, Module.I<ICache>(CacheNames.AdminCache)));
            }
        }

        public void Save()
        {
            IItem group = Group;
            group.RawItem.Parent = null;

            IItemAttribute attribute = group.Attributes[AttributeKeyEnum.Template];
            IItemValue<int> attributeValue = attribute.IntValues.FirstOrDefault();

            WebShopDb context = WebShopDb.I;
            Module.I<S.IItemManager>().AssociateWithDbContext(context, group.RawItem);
            if (attributeValue == null)
            {
                attributeValue = Module.I<IItemValue<int>>();
                attribute.IntValues.Add(attributeValue);
            }
            else if (attributeValue.Value > 0)
            {
                return;
            }

            IAttributeTemplate template = Module.I<IAttributeTemplate>();
            context.ItemDefinition.Add((ItemDefinition)template.RawItem);
            context.SaveChanges();

            attributeValue.Value = template.Id;
            context.SaveChanges();

            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            cache.AppendClearBuffer(typeof(IAttributeTemplate));
            cache.AppendClearBuffer(Module.I<IGroup>().Get(null, false, cache));
            cache.Clear();
        }
    }
}