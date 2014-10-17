using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class Login
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public DateTime DateTimeOfCreation { get; set; }
		public string MailConfirmationCode { get; set; }
		public virtual List<User> Users { get; set; }
		public virtual List<Partner> Partners { get; set; }
		public virtual List<PermissionGroup> PermissionGroups { get; set; }

		public Login()
		{
			Users = new List<User>();
			Partners = new List<Partner>();
			PermissionGroups = new List<PermissionGroup>();
		}
		public static Login GetUser(string username, string password)
		{
			return (from login in WebShopDb.I.Login
					where login.UserName == username && login.Password == password
					select login).FirstOrDefault();
		}
		public static Login GetUserById(int loginId)
		{
			return (from login in WebShopDb.I.Login
					where login.Id == loginId
					select login).FirstOrDefault();
		}
		public static Login GetLoginByMCC(string mcc, WebShopDb context = null)
		{
			WebShopDb ctx = context ?? WebShopDb.I;
			return (from login in ctx.Login
					where login.MailConfirmationCode == mcc
					select login).FirstOrDefault();
		}

		public virtual Login Get(string userName)
		{
			return (from l in WebShopDb.I.Login
					where l.UserName == userName
					select l).FirstOrDefault();
		}

		public bool HasPermission(string permissionName)
		{
			return PermissionGroups.Exists(g => g.Permissions.Exists(p => p.Name == permissionName));
		}

		public bool AnyHasPermission(string permissionName)
		{
			return PermissionGroups.Exists(g => g.Permissions.Exists(p => p.Name == permissionName))
				|| Users.Exists(u => u.HasPermission(permissionName))
				|| Partners.Exists(p => p.HasPermission(permissionName));
		}
	}
}
