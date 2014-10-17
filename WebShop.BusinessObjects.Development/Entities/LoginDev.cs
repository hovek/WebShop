using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class LoginDev
	{
		public static void FillData(DbContext context)
		{
			List<PermissionGroup> pg = (from g in context.Set<PermissionGroup>()
										select g).ToList();

			context.Set<Login>().Add(new Login
			{
				UserName = "admin",
				Password = "lozinka",
				DateTimeOfCreation = DateTime.Now,
				PermissionGroups = pg
			});
		}

	}
}
