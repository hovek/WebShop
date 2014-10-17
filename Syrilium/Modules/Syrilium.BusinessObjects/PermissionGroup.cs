using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class PermissionGroup
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public virtual List<Permission> Permissions { get; set; }
		public virtual List<Login> Logins { get; set; }
		public virtual List<User> Users { get; set; }
		public virtual List<Partner> Partners { get; set; }

		public PermissionGroup()
		{
			Permissions = new List<Permission>();
			Logins = new List<Login>();
			Users = new List<User>();
			Partners = new List<Partner>();
		}
	}
}
