using Syrilium.CommonInterface.Caching;
using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class Stars
    {
        public IItem Product { get; set; }

        public int Grade
        {
            get
            {
                IItemAttribute attr = Product.Attributes[AttributeKeyEnum.Grade];
                return attr == null ? 0 : (int)attr.GetRawValue();
            }
        }

        public int UserGrade
        {
            get
            {
                ProductGrade pg = GetProductGrade(Product.Id);
                return pg == null ? 0 : pg.Grade;
            }
        }

        public static ProductGrade GetProductGrade(int productId, ICache cache = null)
        {
            SessionState ss = SessionState.I;
            int? parentId = ss.Partner == null ? null : (int?)ss.Partner.Id;
            int? userId = ss.User == null ? null : (int?)ss.User.Id;
            ProductGrade pg = cache == null ? new ProductGrade() : cache.I<ProductGrade>();
            return pg.Get(productId, parentId, userId);
        }
    }
}