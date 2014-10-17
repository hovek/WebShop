using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
    public class Banner
    {
        public int Id { get; set; }
        public int BannerLocationId { get; set; }
        public string Title { get; set; }    
        public string Html { get; set; }
        public string BannerPath { get; set; }
        public string Description { get; set; }
		public virtual BannerLocation BannerLocation { get; set; }

        public int PartnerId { get; set; }

   
        public static Banner GetBanner(int? LocationId, WebShopDb context = null)
        {
            return (from banner in (context ?? WebShopDb.I).Banner
                    where banner.Id == LocationId
                    select banner).First();
        }
        public static List<Banner> GetAllChild(int idLocation)
        {
            return (from banner in WebShopDb.I.Banner
                    where banner.BannerLocationId == idLocation
                    select banner).ToList();
        }
    }
}
