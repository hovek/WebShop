using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using WebShop.BusinessObjectsInterface;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class PermissionDev
	{
		public static void FillData(DbContext context)
		{
			context.Set<PermissionGroup>().Add(
				new PermissionGroup
				{
					Name = PermissionNames.AdminAccess,
					Permissions = new List<Permission>(){
						  new Permission{
							   Name = PermissionNames.AdminAccess
						  }
					  }
				});

			context.Set<PermissionGroup>().Add(
			new PermissionGroup
			{
				Name = PermissionNames.User,
				Permissions = new List<Permission>(){
						  new Permission{
							   Name = PermissionNames.User
						  }
					  }
			});

			context.SaveChanges();
		}
	}
}
