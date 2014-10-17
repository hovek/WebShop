using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public int? GenderId { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int? LoginId { get; set; }
        public virtual Login Login { get; set; }
        public virtual List<PermissionGroup> PermissionGroups { get; set; }

        public User()
        {
            PermissionGroups = new List<PermissionGroup>();
        }
        public static User GetUserByEmail(string email)
        {
            return (from user in WebShopDb.I.User
                    where user.Email == email
                    select user).FirstOrDefault();
        }
        public static User GetUserByEmail(string email, WebShopDb context = null)
        {
            WebShopDb ctx = context ?? WebShopDb.I;
            return (from user in ctx.User
                    join login in ctx.Login on user.LoginId equals login.Id
                    where user.Email == email
                    select user).FirstOrDefault();

        }

        public virtual User Get(int userId)
        {
            return (from u in WebShopDb.I.User
                    where u.Id == userId
                    select u).First();
        }

		public bool HasPermission(string permissionName)
		{
			return PermissionGroups.Exists(g => g.Permissions.Exists(p => p.Name == permissionName));
		}
	}
}
