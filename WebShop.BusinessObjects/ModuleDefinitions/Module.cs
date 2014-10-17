using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjects.Item;
using Syrilium.Interfaces.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.BusinessObjects.ModuleDefinitions
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

            container.RegisterInstance(CacheNames.AdminCache, Module.I<ICache>());
            container.RegisterInstance(CacheNames.MainCache, Module.I<ICache>());
            container.RegisterType<IAttributeLocator, AttributeLocator>();
            container.RegisterType<IItem, Item.Item>();
            container.RegisterType<IItemAttribute, ItemAttribute>();
            container.RegisterType<IAttributeDefinition, AttributeDefinition>();
            container.RegisterType<IItemValue<int>, ItemValue<int>>();
            container.RegisterType<IItemValue<string>, ItemValue<string>>();
            container.RegisterType<IItemValue<decimal>, ItemValue<decimal>>();
            container.RegisterType<IItemValue<DateTime>, ItemValue<DateTime>>();
            container.RegisterType<IItemDefinitionValue<int>, ItemDefinitionValue<int>>();
            container.RegisterType<IItemDefinitionValue<string>, ItemDefinitionValue<string>>();
            container.RegisterType<IItemDefinitionValue<decimal>, ItemDefinitionValue<decimal>>();
            container.RegisterType<IItemDefinitionValue<DateTime>, ItemDefinitionValue<DateTime>>();
            container.RegisterType<IAttributeTemplate, AttributeTemplate>();
            container.RegisterType<IAttributeTemplateAttribute, AttributeTemplateAttribute>();
            container.RegisterType<IAttributeTemplateConstraint, AttributeTemplateConstraint>();
            container.RegisterType<IAttributeTemplateAttributeGroup, AttributeTemplateAttributeGroup>();
            container.RegisterType<IPredefinedList, PredefinedList>();
            container.RegisterType<IPredefinedListValue, PredefinedListValue>();
            container.RegisterType<IReference, Reference>();
            container.RegisterType<IValueConverter, ValueConverter>();
            container.RegisterType<IGroup, Group>();
            container.RegisterType<IConfig, Config>();
            container.RegisterType<IArchiveItemChange, ArchiveItemChange>();
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
