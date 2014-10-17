using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
	public class PriceBox
	{
		public IItem Product { get; set; }

		public string Price
		{
			get
			{
                return Product.GetValueFormated(AttributeKeyEnum.Price);
			}
		}
	}
}