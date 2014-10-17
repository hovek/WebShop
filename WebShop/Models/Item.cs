using Syrilium.DataAccessInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class Item
    {
        public int? GroupId { get; set; }
        public GroupTreeView GroupTreeViewModel { get; set; }
        public AttributeTemplateTreeView AttributeTemplateTreeViewModel { get; set; }
        public ItemAttribute ItemAttributeModel { get; set; }
		public ProductAdminListView ProductAdminListViewModel { get; set; }

        public Item()
        {
            ItemAttributeModel = new ItemAttribute();
        }
	}
}