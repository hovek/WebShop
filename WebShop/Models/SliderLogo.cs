using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using S = Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class SliderLogo
    {
        private IItem product;
        public IItem Product
        {
            get
            {
                return product;
            }
            set
            {
                product = value;
                PartnerId = product.GetRawValue(AttributeKeyEnum.Partner);
            }
        }

        public HttpRequestBase Request
        {
            set
            {
                NameValueCollection queryString = value.Form.Count > 0 ? value.Form : value.QueryString;
                int? attributeId = Module.I<IAttributeLocator>()[AttributeKeyEnum.Partner];
                if (attributeId.HasValue)
                {
                    int partnerId;
                    if (int.TryParse(CommonHelpers.GetQueryStringParamValue(queryString, attributeId.ToString()), out partnerId))
                    {
                        PartnerId = partnerId;
                    }
                }
            }
        }

        public int? PartnerId { get; set; }

        public List<S.Partner> GetPartnerList()
        {
            return Module.I<ICache>(CacheNames.MainCache).I<S.Partner>().GetPartners();
        }
    }
}