using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class GroupTreeView
    {
        public int? SelectedGroupId { get; set; }

        public List<IItem> Groups
        {
            get
            {
                return Module.I<IGroup>().Get(null, false, Module.I<ICache>(CacheNames.AdminCache));
            }
        }
    }
}