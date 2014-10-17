using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
    public class SearchInquiry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? UserId { get; set; }
        public int? PartnerId { get; set; }
        public DateTime DateTime { get; set; }

        public static List<SearchInquiry> SearchInquiryList(int? userId)
        {
            return (from searchInquiry in WebShopDb.I.SearchInquiry
                    where searchInquiry.UserId == userId
                    select searchInquiry).ToList();

        }
    }
}
