using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
    public class ProductGrade
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int? PartnerId { get; set; }
        public virtual Partner Partner { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public int Grade { get; set; }
        public DateTime DateOfEntry { get; set; }

        public virtual int GetAverageGrade(int itemId)
        {
            return (int)(from g in WebShopDb.I.ProductGrade
                         where g.ItemId == ItemId
                         group g by g.ItemId into result
                         select result.Average(i => i.Grade)).SingleOrDefault();
        }

        public virtual ProductGrade Get(int itemId, int? partnerId = null, int? userId = null)
        {
            return (from g in WebShopDb.I.ProductGrade
                    where g.ItemId == itemId && (g.UserId == userId || g.PartnerId == partnerId)
                    select g).FirstOrDefault();
        }
    }
}