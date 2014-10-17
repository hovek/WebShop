using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects;
using System.Data.Entity;
using WebShop.BusinessObjectsInterface;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class BannerLocationDev
	{
		public static void FillData(DbSet<BannerLocation> bannerLocations)
		{
			bannerLocations.Add(new BannerLocation
			{
				Id = (int)BannerLocationEnum.LeaderBord,
				Translation = new List<BannerLocationTranslation> { 
					new BannerLocationTranslation { LanguageId = 1, Name = "LeaderBord (728x90) Header" } 
				}
			});
			bannerLocations.Add(new BannerLocation
			{
				Id = (int)BannerLocationEnum.RectangleLeftSide,
				Translation = new List<BannerLocationTranslation> { 
					new BannerLocationTranslation {LanguageId = 1, Name = "Rectangle (180x150) Left Side" } 
				}
			});
			bannerLocations.Add(new BannerLocation
			{
				Id = (int)BannerLocationEnum.RectangleRightSide,
				Translation = new List<BannerLocationTranslation> { 
					new BannerLocationTranslation {LanguageId = 1, Name = "Rectangle (180x150) Right Side" } 
				}
			});
		}
	}
}
