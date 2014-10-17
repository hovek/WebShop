using S = Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using WebShop.Infrastructure;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
	public class Background
	{
		public string HtmlBackground { get; set; }

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
				int? partnerId = product.GetRawValue(AttributeKeyEnum.Partner);
				if (partnerId != null)
				{
					S.Partner partner = Module.I<ICache>(CacheNames.MainCache).I<S.Partner>().GetPartner(partnerId.Value);
					if (partner != null)
					{
                        HtmlBackground = string.IsNullOrWhiteSpace(partner.HtmlBackground) ? Module.I<IConfig>().GetValue(ConfigNames.DefaultBackground) : partner.HtmlBackground;
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
                        HtmlBackground = partner == null || string.IsNullOrWhiteSpace(partner.HtmlBackground) ? Module.I<IConfig>().GetValue(ConfigNames.DefaultBackground) : partner.HtmlBackground;
					}
				}
			}
		}

		public Background()
		{
            HtmlBackground = Module.I<IConfig>().GetValue(ConfigNames.DefaultBackground);
		}
	}
}