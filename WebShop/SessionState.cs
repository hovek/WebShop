using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface;
using Syrilium.Interfaces.BusinessObjectsInterface;
using WebShop.Infrastructure;

namespace WebShop
{
	public class SessionState : ILanguageProvider
	{
		public static SessionState I
		{
			get
			{
				return new SessionState();
			}
		}

		public int LanguageId
		{
			get
			{
				int? langId = (int?)System.Web.HttpContext.Current.Session["CurrentLanguageId"];
				if (langId == null)
				{
					langId = Module.I<IConfig>().GetValue(ConfigNames.DefaultLanguageId);
					System.Web.HttpContext.Current.Session["CurrentLanguageId"] = langId.Value;
				}
				return langId.Value;
			}
			set
			{
				System.Web.HttpContext.Current.Session["CurrentLanguageId"] = value;
			}
		}

		public Login Login
		{
			get
			{
				Login login = (Login)System.Web.HttpContext.Current.Session["Login"];
				if (login == null || login.UserName != HttpContext.Current.User.Identity.Name)
				{
					login = new Login().Get(HttpContext.Current.User.Identity.Name);
					HttpContext.Current.Session["Login"] = login;
				}

				return login;
			}
		}

		public User User
		{
			get
			{
				return Login == null ? null : Login.Users.FirstOrDefault();
			}
		}

		public Partner Partner
		{
			get
			{
				return Login == null ? null : Login.Partners.FirstOrDefault();
			}
		}

		public bool HasPermission(string permissionName)
		{
			if (Login == null) return false;

			return Login.AnyHasPermission(permissionName);
		}
	}
}