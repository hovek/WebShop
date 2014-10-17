using Syrilium.DataAccessInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using System.Web.Mvc;

namespace WebShop.Models
{
    public class Group
    {
        public static List<SelectListItem> GetGroupItems(int? groupId)
        {
            List<IItem> iitems = Module.I<IGroup>().Get(groupId);
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (IItem iitem in iitems)
            {
                SelectListItem item = new SelectListItem();
                item.Text = iitem.Attributes[AttributeKeyEnum.Name].GetRawValue();
                item.Value = iitem.Id.ToString();
                if (groupId == iitem.Id)
                {
                    item.Selected = true;
                }
                items.Add(item);
            }
            return items;
        }
    }
}