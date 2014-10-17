using Syrilium.CommonInterface.Caching;
using Syrilium.DataAccessInterface;
using Syrilium.DataAccessInterface.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using S = Syrilium.Modules.BusinessObjects;

namespace WebShop.Models
{
    public class ProductAdminListView
    {
        public int? GroupId { get; set; }

        public List<ProductViewItem> ProductViewItemList
        {
            get
            {
                List<ProductViewItem> productViewItems = new List<ProductViewItem>();
                if (GroupId != null)
                {
                    foreach (IItem product in GetProductsForGroup())
                    {
                        IItemAttribute attribute = product.Attributes[AttributeKeyEnum.Name];
                        productViewItems.Add(
                            new ProductViewItem
                            {
                                ItemId = product.Id,
                                ItemName = attribute == null ? "" : attribute.GetRawValue(),
                                CreateDateTime = product.GetValueFormated(AttributeKeyEnum.DateTimeEntry),
                                EditDateTime = product.GetValueFormated(AttributeKeyEnum.DateTimeChange),
                                PersonCreated = GetUserChangedBy(product)
                            }
                        );
                    }
                }
                return productViewItems;
            }
        }

        public string GetUserChangedBy(IItem product)
        {
            ICache cache = Module.I<ICache>(CacheNames.AdminCache);

            int? partnerId = product.GetRawValue(AttributeKeyEnum.PartnerChangedBy);
            if (partnerId != null)
            {
                S.Partner partner = cache.I<S.Partner>().GetPartner(partnerId.Value);
                return partner.Name;
            }
            else
            {
                int? userId = product.GetRawValue(AttributeKeyEnum.UserChangedBy);
                if (userId != null)
                {
                    S.User user = cache.I<S.User>().Get(userId.Value);
                    return string.Concat(user.Name, " ", user.Surname);
                }
                else return "";
            }
        }

        public List<IItem> GetProductsForGroup()
        {
            List<SySqlParameter> parameters = new List<SySqlParameter>();
            SySqlParameter paramGroupId = new SySqlParameter();
            paramGroupId.ParameterName = "@groupID";
            paramGroupId.Value = GroupId;
            parameters.Add(paramGroupId);
            IItem item = Module.I<IItem>();
            item.Cache = Module.I<ICache>(CacheNames.AdminCache);
            return item.Get(SPNames.GetProduct, parameters);
        }

    }

    public class ProductViewItem
    {
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public string CreateDateTime { get; set; }
        public string EditDateTime { get; set; }
        public string PersonCreated { get; set; }
    }
}