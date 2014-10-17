using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Syrilium.Interfaces.BusinessObjectsInterface;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.Modules.BusinessObjects.Item;

namespace Syrilium.Modules.BusinessObjects.ModuleDefinitions
{
	public class Module : IModule
	{
		private static IServiceLocator serviceLocator { get; set; }
		private static ILanguageProvider languageProvider { get; set; }
		IUnityContainer container;

		public static int LanguageId
		{
			get
			{
				return languageProvider.LanguageId;
			}
		}

		public Module(IUnityContainer container, IServiceLocator serviceLocator)
		{
			Module.serviceLocator = serviceLocator;
			this.container = container;
		}

		public void Initialize()
		{
			languageProvider = serviceLocator.GetInstance<ILanguageProvider>();

			container.RegisterType<IItemManager, ItemManager>();
		}

		public static TService I<TService>()
		{
			return serviceLocator.GetInstance<TService>();
		}

        public static TService I<TService>(string key)
        {
            return serviceLocator.GetInstance<TService>(key);
        }
    }
}
