using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;
using S = Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class County
    {
        //public static SelectList DistrictList()
        //{
        //    Dictionary<int, string> districtList = new Dictionary<int, string>();
        //    foreach (Syrilium.Modules.BusinessObjects.District district in WebShopDb.I.District)
        //    {
        //        districtList.Add(district.Id, district.Name);
        //    }
        //    SelectList selectList = new SelectList(districtList, "Key", "Value");
        //    return selectList;

        //}
        public static List<SelectListItem> CountyList(int? selectedCounty = null)
        {
			List<Syrilium.Modules.BusinessObjects.County> counties = Module.I<ICache>(CacheNames.MainCache).I<S.County>().Get();
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (Syrilium.Modules.BusinessObjects.County county in counties)
            {
                SelectListItem item = new SelectListItem();
                item.Text = county.Name;
                item.Value = county.Id.ToString();
                if (selectedCounty == county.Id)
                {
                    item.Selected = true;
                }
                items.Add(item);
            }
            return items;
        }

        public static List<SelectListItem> DistrictCityList(int? countyId = null, int? districtCityId = null)
        {
			List<Syrilium.Modules.BusinessObjects.DistrictCity> districtCities = Module.I<ICache>(CacheNames.MainCache).I<DistrictCity>().GetDistrictCities(countyId);
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (Syrilium.Modules.BusinessObjects.DistrictCity districtCity in districtCities)
            {
                SelectListItem item = new SelectListItem();
                item.Text = districtCity.Name;
                item.Value = districtCity.Id.ToString();
                if (districtCityId == districtCity.Id)
                {
                    item.Selected = true;
                }
                items.Add(item);
            }
            return items;
        }
    }
}