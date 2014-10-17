using Syrilium.CommonInterface;
using Syrilium.DataAccessInterface;
using Syrilium.DataAccessInterface.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjectsInterface.Item
{
	public interface IPredefinedList
	{
		int Id { get; set; }
		ObservableCollection<IItemDefinitionValue<string>> DisplayValues { get; }
		S.IItem RawItem { get; set; }
		ObservableCollection<IPredefinedListValue> Values { get; }
		IPredefinedList Get(int listId);
        List<IPredefinedList> Get(string storedProcedure, List<SySqlParameter> parameters = null);
}

	public interface IPredefinedListValue
	{
		int Id { get; set; }
		ObservableCollection<IItemDefinitionValue<string>> StringValues { get; }
		ObservableCollection<IItemDefinitionValue<int>> IntValues { get; }
		ObservableCollection<IItemDefinitionValue<decimal>> DecimalValues { get; }
		ObservableCollection<IItemDefinitionValue<DateTime>> DateTimeValues { get; }
		dynamic Value { get; }
		S.IItem RawItem { get; set; }
		dynamic GetValue(int languageId);
}
}
