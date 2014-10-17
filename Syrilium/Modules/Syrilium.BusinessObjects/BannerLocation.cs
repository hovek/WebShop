using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class BannerLocation : TranslatableEntity<BannerLocationTranslation>
	{
		public string Name { get { return GetTranslation(p => p.Name); } }
		public string Description { get { return GetTranslation(p => p.Description); } }
		public virtual List<Banner> Banners { get; set; }
	}

	public class BannerLocationTranslation : EntityTranslation
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
