using S = Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using WebShop.Infrastructure;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.Modules.BusinessObjects;
using System.Web.Mvc;

namespace WebShop.Models
{
	public class Banner
	{
		private string bannerLedearBoard = null;
		public string BannerLedearBoard
		{
			get
			{
				if (bannerLedearBoard == null)
				{
					bannerLedearBoard = GetBanner(PartnerId, BannerLocationEnum.LeaderBord);
				}
				return bannerLedearBoard;
			}
			set
			{
				bannerLedearBoard = value;
			}
		}
		private string bannerLeftSide = null;
		public string BannerLeftSide
		{
			get
			{
				if (bannerLeftSide == null)
				{
					bannerLeftSide = GetBanner(PartnerId, BannerLocationEnum.RectangleLeftSide);
				}
				return bannerLeftSide;
			}
			set
			{
				bannerLeftSide = value;
			}
		}
		private string bannerRightSide = null;
		public string BannerRightSide
		{
			get
			{
				if (bannerRightSide == null)
				{
					bannerRightSide = GetBanner(PartnerId, BannerLocationEnum.RectangleRightSide);
				}
				return bannerRightSide;
			}
			set
			{
				bannerRightSide = value;
			}
		}

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

		public Banner()
		{
			PartnerId = null;
		}

		public virtual string GetBanner(int? partnerId, BannerLocationEnum bannerLocation)
		{
			WebShopDb context = WebShopDb.I;
			return (from b in context.Banner
					where (partnerId == null || b.PartnerId == partnerId) && b.BannerLocationId == (int)bannerLocation
					select b.Html).FirstOrDefault();
		}

		public static SelectList GetBannerLocationList()
		{
			Dictionary<int, string> bannerLocationList = new Dictionary<int, string>();
			foreach (Syrilium.Modules.BusinessObjects.BannerLocation bannerLocation in WebShopDb.I.BannerLocation)
			{
				bannerLocationList.Add(bannerLocation.Id, bannerLocation.Name);
			}
			SelectList selectList = new SelectList(bannerLocationList, "Key", "Value");
			return selectList;

		}
		public static SelectList GetBannerList(int idLocation)
		{
			Dictionary<int, string> bannerList = new Dictionary<int, string>();
			foreach (Syrilium.Modules.BusinessObjects.Banner banner in Syrilium.Modules.BusinessObjects.Banner.GetAllChild(idLocation))
			{
				bannerList.Add(banner.Id, banner.Title);
			}
			SelectList selectList = new SelectList(bannerList, "Key", "Value");
			return selectList;

		}

	}
}