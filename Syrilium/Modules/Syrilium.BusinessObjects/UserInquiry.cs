using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class UserInquiry
	{
		public int Id { get; set; }
		public int? ItemId { get; set; }
		public int? UserId { get; set; }
		public int? PartnerId { get; set; }
		public virtual User User { get; set; }
		public virtual Partner Partner { get; set; }
		public string Name { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Message { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }

        public static List<UserInquiry> UserInquiryList(int? userId)
        { 
             return (from userInquiry in WebShopDb.I.UserInquiry
                    where userInquiry.UserId == userId 
                    select userInquiry).ToList();
        
        }
	}
}