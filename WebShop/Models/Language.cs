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
	public class Language
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Country { get; set; }
		public string ImageUrl { get; set; }
		public string Selected { get; set; }

		public static SelectList GetLanguageList()
		{
			Dictionary<int, string> languageList = new Dictionary<int, string>();
			foreach (Syrilium.Modules.BusinessObjects.Language language in Get())
			{
				languageList.Add(language.Id, language.Name);
			}
			SelectList selectList = new SelectList(languageList, "Key", "Value");
			return selectList;

		}

		public static List<Syrilium.Modules.BusinessObjects.Language> Get()
		{
			return Module.I<ICache>(CacheNames.MainCache).I<Syrilium.Modules.BusinessObjects.Language>().Get();
		}
	}
}