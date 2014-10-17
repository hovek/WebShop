using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjects.Development.Entities;

namespace WebShop.BusinessObjects.Development
{
    public class WebShopDbInitializer : DropCreateDatabaseIfModelChanges<WebShopDb>
    {
        public void Load()
        {
            WebShopDb.I.Login.FirstOrDefault();
        }

        protected override void Seed(WebShopDb context)
        {
            executeDDL(context);
            base.Seed(context);
            fillData(context);
        }

        private void executeDDL(WebShopDb context)
        {
            ConfigDev.ExecuteDDL(context);
            ItemDefinitionDev.ExecuteDDL(context);
            ItemDev.ExecuteDDL(context);
            LanguageDev.ExecuteDDL(context);
            ItemChangeHistoryDev.ExecuteDDL(context);
            NewsletterSubscriptionDev.ExecuteDDL(context);

            DatabaseObjectsDev.ExecuteDDL(context);
        }

        private void fillData(WebShopDb context)
        {
            PartnerDev.FillData(context.Partner);
            ImageFormatDev.FillData(context.ImageFormat);
            PageDev.FillData(context.Page);
            PageLocationDev.FillData(context.PageLocation);
            NewsDev.FillData(context.News);
            BannerLocationDev.FillData(context.BannerLocation);
            BannerDev.FillData(context.Banner);
            GenderDev.FillData(context.Gender);
            LanguageDev.FillData(context.Language);
            PermissionDev.FillData(context);
            LoginDev.FillData(context);
            UserDev.FillData(context.User);
            CountyDev.FillData(context.County);
            DistrictCityDev.FillData(context.DistrictCity);
            ItemDefinitionDev.FillData(context);
            ItemDev.FillData(context);
            ConfigDev.FillData(context.Config);
            TranslationDev.FillData(context.Translation);
        }
    }
}
