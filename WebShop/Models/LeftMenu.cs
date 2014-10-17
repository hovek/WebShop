using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;
using itm = Syrilium.Modules.BusinessObjects.Item;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using Syrilium.DataAccessInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class LeftMenu
    {
        public bool IsLeftMenu { get; set; }
        public int DepartmentId
        {
            get
            {
                return Module.I<IGroup>().GetDepartmentId(GroupId);
            }
        }
        public int GroupId { get; set; }

        public LeftMenu()
        {
            IsLeftMenu = true;
        }

        public static List<IItem> GetMenuCached(ICache cache = null, bool includeShowAttribute = true)
        {
            return Module.I<IGroup>().Get(null, includeShowAttribute, cache);
        }
    }
}