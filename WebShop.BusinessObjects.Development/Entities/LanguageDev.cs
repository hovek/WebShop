using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects;
using System.Data.Entity;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class LanguageDev
	{
		public static void ExecuteDDL(WebShopDb context)
		{
			context.Database.ExecuteSqlCommand(
				@"ALTER TABLE [dbo].[Language]  WITH CHECK ADD  CONSTRAINT [CK_Language] CHECK  (([Id]>=(1) AND [Id]<=(50)))"
			);
		}

		public static void FillData(DbSet<Language> dbSet)
		{
            dbSet.Add(new Language { Id = 1, Name = "Hrvatski", ImageUrl = "../../Resources/Images/Language/Croatian.png" });
            dbSet.Add(new Language { Id = 2, Name = "English", ImageUrl = "../../Resources/Images/Language/England.png" });
            dbSet.Add(new Language { Id = 3, Name = "Deutsch", ImageUrl = "../../Resources/Images/Language/German.png" });
            dbSet.Add(new Language { Id = 4, Name = "Italiano", ImageUrl = "../../Resources/Images/Language/Italy.png" });
            dbSet.Add(new Language { Id = 5, Name = "Español", ImageUrl = "../../Resources/Images/Language/Spain.png" });
		}
	}
}
