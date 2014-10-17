using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects;
using System.Data.Entity;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class ConfigDev
	{
		public static void ExecuteDDL(WebShopDb context)
		{
			context.Database.ExecuteSqlCommand(
				@"CREATE UNIQUE NONCLUSTERED INDEX [IX_Name] ON [dbo].[Config] 
				(
					[Name] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
		}

		public static void FillData(DbSet<Config> dbSet)
		{
			dbSet.Add(new Config { Name = ConfigNames.DefaultLanguageId, IntValue = 1 });

			WebShopDb context = new WebShopDb();
			int groupId = (from i in context.Item
						   where i.TypeId == (int)ItemTypeEnum.Department
						   select i.Id).First();

			dbSet.Add(new Config { Name = ConfigNames.KeyTranslationLanguageId, IntValue = 1 });
            dbSet.Add(new Config { Name = ConfigNames.PreferedLanguageIdIfNoTranslation, IntValue = 2 });
            dbSet.Add(new Config { Name = ConfigNames.DefaultLogoPath, StringValue = "../../Resources/Images/Logo.png" });
            dbSet.Add(new Config { Name = ConfigNames.DefaultBackground, StringValue = @"background: url('/Resources/Images/DefaultBackground.png') top center no-repeat; position:absolute; height:763px; width:100%;" });
            dbSet.Add(new Config { Name = ConfigNames.DefaultProductImage, StringValue = "LogoSearch.png" });

//            dbSet.Add(new Config { Name = ConfigNames.DefaultBackground, StringValue = @"    <div class=""HeaderBackground"">
//                        </div>
//                        <div class=""MenuBackground"">
//                            <div class=""MenuBackgroundLeftSide"">
//                            </div>
//                            <div class=""MenuBackgroundRightSide"">
//                            </div>
//                        </div>
//                        <div class=""ContentBackground"">
//                        </div>" });
		}
	}
}
