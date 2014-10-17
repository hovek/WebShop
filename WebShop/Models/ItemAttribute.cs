using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using Syrilium.Modules.BusinessObjects;
using System.Data.SqlClient;
using Syrilium.DataAccessInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class ItemAttribute
    {
        public void Delete(int attributeDefinitionId, out string error)
        {
            IAttributeLocator al = Module.I<IAttributeLocator>();
            al.Cache = Module.I<ICache>(CacheNames.AdminCache);
            IAttributeDefinition attrDef = al.Find(a => a.Id == attributeDefinitionId);
            if (attrDef.DeleteNotAllowed)
            {
                error = string.Concat(@"<div id=""error"">", Models.Translation.Get("Brisanje atributa"), " '", attrDef.GetNameValue(), "' ", Models.Translation.Get("nije dozvoljeno."), "</div>");
                return;
            }
            else if (WebShopDb.I.Database.SqlQuery<bool>(string.Concat(SPNames.IsAttributeInUse, " @AttributeDefinitionId"), new SqlParameter("@AttributeDefinitionId", attrDef.Id)).First())
            {
                error = string.Concat(@"<div id=""error"">", Models.Translation.Get("Brisanje atributa"), " '", attrDef.GetNameValue(), "' ", Models.Translation.Get("nije dozvoljeno jer se koristi."), "</div>");
                return;
            }

            error = null;
            if (WebShopDb.I.Database.ExecuteSqlCommand("delete from ItemDefinition where id={0}", attributeDefinitionId) > 0)
            {
                ICache cache = Module.I<ICache>(CacheNames.AdminCache);
                cache.AppendClearBuffer(typeof(IAttributeDefinition));
                cache.Clear();
            }
        }

        public static List<IAttributeDefinition> GetAttributeDefinitions()
        {
            List<IAttributeDefinition> atts = new List<IAttributeDefinition>(Module.I<ICache>(CacheNames.AdminCache).I<IAttributeDefinition>(Module.I<IAttributeDefinition>().GetType()).Get(SPNames.GetAttributeDefinition)
                                                                                .Where(a => !a.Hidden));
            atts.Sort((a1, a2) => { return a1.GetNameValue().CompareTo(a2.GetNameValue()); });
            return atts;
        }
    }
}