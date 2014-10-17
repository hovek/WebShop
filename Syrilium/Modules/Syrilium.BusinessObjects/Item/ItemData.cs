using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace Syrilium.Modules.BusinessObjects.Item
{
	public class ItemData<T> : IItemData<T>
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
		public int TypeId { get; set; }
		public T Value { get; set; }
    }

    public class ItemDataInt : ItemData<int>
	{
	}

    public class ItemDataString : ItemData<string>
	{
	}

    public class ItemDataDecimal : ItemData<decimal>
	{
	}

    public class ItemDataDateTime : ItemData<DateTime>
	{
	}
}
