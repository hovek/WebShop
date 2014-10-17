using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class DistrictCity
	{
		public int Id { get; set; }
		public int CountyId { get; set; }
		public string Name { get; set; }
		public bool IsCity { get; set; }
		public bool IsDistrict { get; set; }
		public virtual County County { get; set; }

		public virtual List<DistrictCity> Get()
		{
			return WebShopDb.I.DistrictCity.ToList();
		}

		public virtual List<DistrictCity> GetDistrictCities(int? countyId, WebShopDb context = null)
		{
			return (from districtCity in (context ?? WebShopDb.I).DistrictCity
					where districtCity.CountyId == countyId
					select districtCity).ToList();
		}
	}
}
