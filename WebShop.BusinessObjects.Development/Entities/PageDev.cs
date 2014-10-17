using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syrilium.Modules.BusinessObjects;
using System.Data.Entity;
using WebShop.BusinessObjectsInterface;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class PageDev
	{
		public static void FillData(DbSet<Page> dbSet)
		{
			Page page1 = new Page
			{
				Translation = new List<PageTranslation>() { new PageTranslation { LanguageId = 1, Name = "STRANICA 1", Content = "Sadržaj 1" } },
				PagesLocationId = (int)PageLocationEnum.Info
			};
			dbSet.Add(page1);
			Page page2 = new Page
			{
				Translation = new List<PageTranslation>() { new PageTranslation { LanguageId = 1, Name = "STRANICA 2", Content = "Sadržaj 2" } },
				PagesLocationId = (int)PageLocationEnum.AboutUs
			};
			dbSet.Add(page2);
			Page page3 = new Page
			{
				Translation = new List<PageTranslation>() { new PageTranslation { LanguageId = 1, Name = "STRANICA 3", Content = "Sadržaj 3" } },
				PagesLocationId = (int)PageLocationEnum.Marketing
			};
			dbSet.Add(page3);
			Page page4 = new Page
			{
				Translation = new List<PageTranslation>() { new PageTranslation { LanguageId = 1, Name = "STRANICA 4", Content = "Sadržaj 4" } },
				PagesLocationId = (int)PageLocationEnum.Partner
			};
			dbSet.Add(page4);
			Page page5 = new Page
			{
				Translation = new List<PageTranslation>() { new PageTranslation { LanguageId = 1, Name = "STRANICA 5", Content = "Sadržaj 5" } },
				PagesLocationId = (int)PageLocationEnum.Services
			};
			dbSet.Add(page5);

		}
	}
}
