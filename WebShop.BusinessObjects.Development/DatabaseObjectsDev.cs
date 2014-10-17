using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects;
using System.IO;
using System.Reflection;

namespace WebShop.BusinessObjects.Development
{
	public class DatabaseObjectsDev
	{
		public static void ExecuteDDL(WebShopDb context)
		{
			Assembly ass = Assembly.GetExecutingAssembly();
			List<string> resources = new List<string>(ass.GetManifestResourceNames());
			List<string> sps = resources.Where(r => r.Contains("WebShop.BusinessObjects.Development.DatabaseObjects.") && r.Contains(".sql")).ToList();

			foreach (string res in sps)
			{
				context.Database.ExecuteSqlCommand(new StreamReader(ass.GetManifestResourceStream(res)).ReadToEnd());
			}
		}
	}
}
