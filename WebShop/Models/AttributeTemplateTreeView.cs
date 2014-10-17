using Syrilium.CommonInterface.Caching;
using Syrilium.DataAccessInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class AttributeTemplateTreeView
    {
        public int GroupId { get; set; }

        public IAttributeTemplate GetAttributeTemplate()
        {
            return GetAttributeTemplate(GroupId);
        }

        public static IAttributeTemplate GetAttributeTemplate(int groupId)
        {
            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            IAttributeTemplate template = cache.I<IAttributeTemplate>(Module.I<IAttributeTemplate>().GetType());
            template.Cache = cache;
            return template.Get(itemId: groupId, allowGetFromNearestParent: false).FirstOrDefault();
        }
    }
}