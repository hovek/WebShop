using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class County
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string NameShort { get; set; }

		public virtual List<County> Get()
		{
			return WebShopDb.I.County.ToList();
		}

		public static County GetCounty(int? countyId, WebShopDb context = null)
		{
			return (from county in (context ?? WebShopDb.I).County
					where county.Id == countyId
					select county).First();
		}
	}
}
