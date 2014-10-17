using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class Page : TranslatableEntity<PageTranslation>
	{
		public int PagesLocationId { get; set; }
        public bool Static { get; set; }
        public bool Visible { get; set; }
		public virtual PageLocation PageLocation { get; set; }


		public string Name
		{
			get
			{
				return GetTranslation(p => p.Name);
			}
		}
		public string Content
		{
			get
			{
				return GetTranslation(p => p.Content);
			}
		}

        public virtual List<Page> GetPages(int pageLocationId)
		{
            WebShopDb ctx = WebShopDb.I;
            return (from p in ctx.Page
                    where p.PagesLocationId == pageLocationId && p.Static == false && p.Visible == true
                    select p).ToList();
		}
        public virtual Page GetStaticPage(int pageLocationId)
        {
            WebShopDb ctx = WebShopDb.I;
            return (from p in ctx.Page
                    where p.PagesLocationId == pageLocationId && p.Static == true && p.Visible == true
                    select p).FirstOrDefault();

        }

        public static Page GetPage(int pageId, WebShopDb context = null)
        {
            WebShopDb ctx = context ?? WebShopDb.I;
            return (from p in ctx.Page
                    where p.Id == pageId
                    select p).First();
        }
        public static List<Page> GetAllPages(int pageLocationId)
        {
            WebShopDb ctx = WebShopDb.I;
            return (from p in ctx.Page
                    where p.PagesLocationId == pageLocationId
                    select p).ToList();
        }

		public static List<PageTranslation> GetTranslations(int pageLocationId, int languageId)
		{
			WebShopDb ctx = WebShopDb.I;
			return (from p in ctx.Page
					join pt in ctx.PageTranslation on p.Id equals pt.ParentId
					where p.PagesLocationId == pageLocationId && pt.LanguageId == languageId
					select pt).ToList();
		}
        public static PageTranslation GetPageTranslationByName(string pageName)
        {
            return (from pageTranslation in WebShopDb.I.PageTranslation
                    where pageTranslation.Name == pageName
                    select pageTranslation).First();
        }
  
	}
	public class PageTranslation : EntityTranslation
	{
		public string Name { get; set; }
		public string Content { get; set; }

	}
}
