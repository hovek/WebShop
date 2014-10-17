using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;

namespace WebShop.Models
{
    public class Cache
    {
        public string ClearType { get; set; }

        public List<ClearCacheItem> GetItems()
        {
            List<ClearCacheItem> cacheItems = new List<ClearCacheItem>()
            {
               // new ClearCacheItem{ ClearType="top", Name=Translation.Get("Top ponuda")}
            };
            cacheItems.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
            cacheItems.Add(new ClearCacheItem { ClearType = "all", Name = Translation.Get("Osvježi sve") });
            return cacheItems;
        }

        public void Clear()
        {
            if (ClearType == "all")
            {
                Module.I<ICache>(CacheNames.MainCache).ClearAll();
            }
        }
    }

    public class ClearCacheItem
    {
        public string Name { get; set; }
        public string ClearType { get; set; }
    }
}