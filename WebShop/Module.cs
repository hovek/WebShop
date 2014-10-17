using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Configuration;
using Syrilium.Interfaces.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;
using WebShop.Infrastructure.Jobs;

namespace WebShop
{
    public class Module : UnityBootstrapper
    {
        private static Module instance { get; set; }
        private static IServiceLocator serviceLocator { get; set; }

        protected override System.Windows.DependencyObject CreateShell()
        {
            return null;
        }

        protected override Microsoft.Practices.Prism.Modularity.IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        protected override IUnityContainer CreateContainer()
        {
            IUnityContainer container = base.CreateContainer();

            container.RegisterInstance<string>("ConnectionString", ConfigurationManager.ConnectionStrings["WebShopDb"].ConnectionString);
            container.RegisterType<ILanguageProvider, SessionState>();

            return container;
        }

        public static void Init()
        {
            instance = new Module();
            instance.Run();
            serviceLocator = instance.Container.Resolve<IServiceLocator>();

            startJobs();
        }

        public static TService I<TService>()
        {
            return serviceLocator.GetInstance<TService>();
        }

        public static TService I<TService>(string key)
        {
            return serviceLocator.GetInstance<TService>(key);
        }

        private static void startJobs()
        {
            TimeSpan startOnTime = new TimeSpan(8, 0, 0);
            //TimeSpan startOnTime = DateTime.Now.AddSeconds(1).TimeOfDay;
            new NewsletterItemChangedNotifyJob(startOnTime).Start();
        }
    }
}