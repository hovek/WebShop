using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Syrilium.Modules.BusinessObjects
{
	public class Permission
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public virtual List<PermissionGroup> Groups { get; set; }

		public Permission()
		{
			Groups = new List<PermissionGroup>();
		}
	}
}
