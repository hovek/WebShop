using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjects.Development.Entities
{
   public class TranslationDev
    {
       public static void FillData(DbSet<Translation> dbSet)
       {

           Translation tran = new Translation
           {
               Key = "OpciUvjeti",
               Value = @"Trenutno nemam zadnju verziju",
               LanguageId = 1
           };
               dbSet.Add(tran);    
       }
    }
}
