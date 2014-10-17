using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects;
using System.Data.Entity;
using WebShop.BusinessObjectsInterface;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class PageLocationDev
	{
		public static void FillData(DbSet<PageLocation> pageLocations)
		{
			pageLocations.Add(new PageLocation
			{
				Id = (int)PageLocationEnum.Info,
				Translation = new List<PageLocationTranslation> { 
					new PageLocationTranslation { LanguageId=1,  Name =  "Info" } 
				}
			});
			pageLocations.Add(new PageLocation
			{
				Id = (int)PageLocationEnum.AboutUs,
				Translation = new List<PageLocationTranslation> { 
					new PageLocationTranslation {LanguageId=1, Name =  "O nama"} 
				}
			});
			pageLocations.Add(new PageLocation
			{
				Id = (int)PageLocationEnum.Partner,
				Translation = new List<PageLocationTranslation> { 
					new PageLocationTranslation { LanguageId=1,Name =  "Partner"} 
				}
			});
			pageLocations.Add(new PageLocation
			{
				Id = (int)PageLocationEnum.Services,
				Translation = new List<PageLocationTranslation> { 
					new PageLocationTranslation {LanguageId=1, Name =  "Usluge"} 
				}
			});
			pageLocations.Add(new PageLocation
			{
				Id = (int)PageLocationEnum.Marketing,
				Translation = new List<PageLocationTranslation> { 
					new PageLocationTranslation { LanguageId=1, Name =  "Marketing"} 
				}
			});
		}
	}
}
