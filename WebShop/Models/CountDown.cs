using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class CountDown
    {
        public IItem Product { get; set; }

        public DateTime? AuctionDate
        {
            get
            {
                IItemAttribute attr = Product.Attributes[AttributeKeyEnum.AuctionDate];
                if (attr != null)
                    return attr.GetRawValue();
                else
                    return null;
            }
        }
    }
}