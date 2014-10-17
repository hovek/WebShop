using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface
{
	public static class SPNames
	{
		public const string GetGroups = "spGetGroups";
		public const string GetAttributeDefinition = "spGetAttributeDefinition";
		public const string GetItem = "spGetItem";
		public const string GetAttributeTemplate = "spGetAttributeTemplate";
		public const string GetPredefinedList = "spGetPredefinedList";
		public const string GetProduct = "spGetProduct";
        public const string GetClosestParentId = "spGetClosestParentId";
		public const string ExecuteSql = "sp_executesql";
        public const string IsAttributeInUse = "spIsAttributeInUse";
        public const string IsItemInNewsletterSubscription = "spIsItemInNewsletterSubscription";
        public const string GetChangedItemsAndSubscribers = "spGetChangedItemsAndSubscribers";
	}
}
