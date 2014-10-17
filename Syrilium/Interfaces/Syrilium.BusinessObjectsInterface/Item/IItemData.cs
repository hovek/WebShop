using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Interfaces.BusinessObjectsInterface.Item
{
	public interface IItemData<T>
	{
		int Id { get; set; }
		int ItemId { get; set; }
		int TypeId { get; set; }
		T Value { get; set; }
	}
}
