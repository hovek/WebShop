using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface.Item
{
    public static class SystemListForeignKeyFilter
    {
        public delegate dynamic SystemListFilter(IEnumerable<int> foreignKeys, dynamic list);
        public static Dictionary<KeyValuePair<AttributeDataSystemListReferenceEnum, AttributeDataSystemListReferenceEnum>, SystemListFilter> Filters { get; private set; }

        static SystemListForeignKeyFilter()
        {
            Filters = new Dictionary<KeyValuePair<AttributeDataSystemListReferenceEnum, AttributeDataSystemListReferenceEnum>, SystemListFilter>();
            Filters.Add(new KeyValuePair<AttributeDataSystemListReferenceEnum, AttributeDataSystemListReferenceEnum>(AttributeDataSystemListReferenceEnum.County, AttributeDataSystemListReferenceEnum.DistrictCity),
                getDistrictCityByCounty);
        }

        public static dynamic Filter(AttributeDataSystemListReferenceEnum primaryListType, AttributeDataSystemListReferenceEnum foreignListType, IEnumerable<int> foreignKeys, dynamic list)
        {
            return Filters[new KeyValuePair<AttributeDataSystemListReferenceEnum, AttributeDataSystemListReferenceEnum>(primaryListType, foreignListType)](foreignKeys, list);
        }

        private static dynamic getDistrictCityByCounty(IEnumerable<int> foreignKeys, dynamic list)
        {
            List<DistrictCity> districtCityList = new List<DistrictCity>();
            foreach (DistrictCity dc in list)
            {
                if (foreignKeys.Contains(dc.CountyId))
                {
                    districtCityList.Add(dc);
                }
            }

            return districtCityList;
        }
    }

}
