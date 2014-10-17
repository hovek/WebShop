using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class GenderDev
	{
		public static void FillData(DbSet<Gender> dbSet)
		{
			dbSet.Add(new Gender
			{
				Translation = new List<GenderTranslation> { 
					new GenderTranslation { LanguageId = 1, Name = "Muško" },
					new GenderTranslation { LanguageId = 2, Name = "Male" }}
			});
			dbSet.Add(new Gender
			{
				Translation = new List<GenderTranslation> { 
					new GenderTranslation { LanguageId = 1, Name = "Žensko" },
					new GenderTranslation { LanguageId = 2, Name = "Female" }}
			});
		}
	}
}
