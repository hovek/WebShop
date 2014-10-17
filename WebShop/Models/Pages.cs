using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
	public class Pages
	{
		public static SelectList GetPageLocationList()
		{
			Dictionary<int, string> pageLocationList = new Dictionary<int, string>();
			foreach (Syrilium.Modules.BusinessObjects.PageLocation page in WebShopDb.I.PageLocation)
			{
				pageLocationList.Add(page.Id, page.Name);
			}
			SelectList selectList = new SelectList(pageLocationList, "Key", "Value");
			return selectList;

		}
		public static SelectList GetPageList(int pageLocationId, int languageId)
		{
			Dictionary<int, string> pageList = new Dictionary<int, string>();

			foreach (Syrilium.Modules.BusinessObjects.PageTranslation page in Syrilium.Modules.BusinessObjects.Page.GetTranslations(pageLocationId, languageId))
			{
				pageList.Add(page.ParentId, page.Name);
			}
			SelectList selectList = new SelectList(pageList, "Key", "Value");
			return selectList;

		}
        public static Page GetStaticPage(int pageLocationId)
        {
            return Module.I<ICache>(CacheNames.MainCache).I<Page>().GetStaticPage(pageLocationId);
        }
	}
}