using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjects.ModuleDefinitions;
using WebShop.BusinessObjectsInterface;
using S = Syrilium.Modules.BusinessObjects;

namespace WebShop.BusinessObjects
{
    public class Config : IConfig
    {
        public dynamic GetValue(string name, ICache cache = null)
        {
            List<S.Config> configs = (cache ?? (ICache)Module.I<ICache>(CacheNames.MainCache)).I<S.Config>().Get();
            S.Config config = configs.Where(p => p.Name == name).FirstOrDefault();
            return config == null ? null : config.Value;
        }
    }
}
