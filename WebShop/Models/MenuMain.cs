using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
	public class MenuMain
	{
		public Login Login { get; set; }

		public MenuMain()
		{
			Login = new Login();
		}
	}
}