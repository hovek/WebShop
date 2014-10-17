using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class Language
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ImageUrl { get; set; }

		public virtual List<Language> Get()
		{
			return WebShopDb.I.Language.ToList();
		}
	}
}
