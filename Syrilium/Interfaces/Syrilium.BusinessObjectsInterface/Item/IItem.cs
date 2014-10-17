using Syrilium.CommonInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Interfaces.BusinessObjectsInterface.Item
{
	public interface IItem
	{
		int Id { get; set; }
		int? ParentId { get; set; }
		int? TypeId { get; set; }
		IItem Parent { get; set; }
		ObservableCollection<IItem> Children { get; set; }
		ObservableCollection<IItemData<DateTime>> DateTimeData { get; set; }
		ObservableCollection<IItemData<decimal>> DecimalData { get; set; }
		ObservableCollection<IItemData<int>> IntData { get; set; }
		ObservableCollection<IItemData<string>> StringData { get; set; }
	}
}
