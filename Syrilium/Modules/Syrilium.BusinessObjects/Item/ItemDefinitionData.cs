using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace Syrilium.Modules.BusinessObjects.Item
{
    public class ItemDefinitionData<T> : IItemData<T>
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int ItemId
        {
            get
            {
                return ItemDefinitionId;
            }
            set
            {
                ItemDefinitionId = value;
            }
        }
        public int ItemDefinitionId { get; set; }
        public T Value { get; set; }
    }

    public class ItemDefinitionDataInt : ItemDefinitionData<int>
	{
	}

    public class ItemDefinitionDataString : ItemDefinitionData<string>
	{
	}

    public class ItemDefinitionDataDecimal : ItemDefinitionData<decimal>
	{
	}

    public class ItemDefinitionDataDateTime : ItemDefinitionData<DateTime>
	{
	}
}
