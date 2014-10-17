using S = Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
	public class Header
	{

		public string Logo { get; set; }
        public string Link { get; set; }
        private string DefaultLink = "/";
		public Banner Banner { get; set; }

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
				Banner.Product = value;
				int? partnerId = product.GetRawValue(AttributeKeyEnum.Partner);
				if (partnerId != null)
				{
					S.Partner partner = Module.I<ICache>(CacheNames.MainCache).I<S.Partner>().GetPartner(partnerId.Value);
					if (partner != null)
					{
                        Logo = string.IsNullOrWhiteSpace(partner.Logo) ? Module.I<IConfig>().GetValue(ConfigNames.DefaultLogoPath) : partner.Logo;
                        Link = string.IsNullOrWhiteSpace(partner.URL) ? DefaultLink : partner.URL;
					}
				}
			}
		}

		public HttpRequestBase Request
		{
			set
			{
				NameValueCollection queryString = value.Form.Count > 0 ? value.Form : value.QueryString;
				IAttributeDefinition attribute = Module.I<IAttributeLocator>().Find(a => a.Key == AttributeKeyEnum.Partner);
				if (attribute != null)
				{
					int partnerId;
					if (int.TryParse(CommonHelpers.GetQueryStringParamValue(queryString, attribute.Id.ToString()), out partnerId))
					{
						S.Partner partner = Module.I<ICache>(CacheNames.MainCache).I<S.Partner>().GetPartner(partnerId);
                        Logo = partner == null || string.IsNullOrWhiteSpace(partner.Logo) ? Module.I<IConfig>().GetValue(ConfigNames.DefaultLogoPath) : partner.Logo;
                        Link = partner == null || string.IsNullOrWhiteSpace(partner.URL) ? DefaultLink : partner.URL;
					}
				}
			}
		}

		public Header()
		{
            Logo = Module.I<IConfig>().GetValue(ConfigNames.DefaultLogoPath);
            Link = DefaultLink;
			this.Banner = new Banner();
		}
	}
}