using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class ItemChangeHistoryDev
    {
        public static void ExecuteDDL(WebShopDb context)
        {
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemChangeHistory ADD CONSTRAINT DF_ItemChangeHistoryDefaultDate DEFAULT GETDATE() FOR DateOfEntry"
            );
        }
    }
}
