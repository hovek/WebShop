using Syrilium.CommonInterface.Caching;
using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;

namespace WebShop.Infrastructure
{
	public class TranslationHelper
	{
		public static IEnumerable<Translation> GetCached(ICache cache = null)
		{
			return (cache ?? (ICache)Module.I<ICache>(CacheNames.MainCache)).I<Translation>().Get();
		}
	}
}