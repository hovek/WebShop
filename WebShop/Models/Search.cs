using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class Search
    {
        public SearchBox SearchBox { get; set; }
        public WebShop.Infrastructure.Paging Paging { get; set; }
        public Login Login { get; set; }

        private IAttributeLocator attributeLocator;
        public IAttributeLocator AttributeLocator
        {
            get
            {
                if (attributeLocator == null)
                {
                    attributeLocator = Module.I<IAttributeLocator>();
                }
                return attributeLocator;
            }
        }

        private ItemSort itemSort;
        public ItemSort ItemSort
        {
            get
            {
                if (itemSort == null)
                {
                    itemSort = new ItemSort
                     {
                         FormId = "SearchForm"
                     };
                }
                return itemSort;
            }
        }

        public Search()
        {
            Login = new Login();
        }
    }
}