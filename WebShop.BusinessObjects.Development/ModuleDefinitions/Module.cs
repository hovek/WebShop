using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using System.Data.Entity;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.BusinessObjects.Development.ModuleDefinitions
{
	public class Module : IModule
	{
		public Module()
		{
		
		}

		public void Initialize()
		{
			WebShopDbInitializer webShopDbInitializer = new WebShopDbInitializer();
			Database.SetInitializer<WebShopDb>(webShopDbInitializer);
			webShopDbInitializer.Load();
		}
	}
}
