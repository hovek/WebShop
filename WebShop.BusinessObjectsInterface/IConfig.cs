using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface
{
    public interface IConfig
    {
        dynamic GetValue(string name, ICache cache = null);
    }
}
