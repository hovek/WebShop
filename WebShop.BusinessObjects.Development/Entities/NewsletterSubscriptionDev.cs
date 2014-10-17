using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class NewsletterSubscriptionDev
    {
        public static void ExecuteDDL(WebShopDb context)
        {
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE NewsletterSubscription ADD CONSTRAINT DF_NewsletterSubscriptionDefaultDate DEFAULT GETDATE() FOR DateOfEntry"
            );
        }
    }
}
