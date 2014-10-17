using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Syrilium.DataAccessInterface;
using System.Data;
using Syrilium.DataAccessInterface.SQL;

namespace Syrilium.Interfaces.BusinessObjectsInterface.Item
{
	public interface IItemManager
	{
		void AssociateWithDbContext(DbContext context, IItem item, EntityState entityState = EntityState.Unchanged);
		List<IItem> GetItemDefinitions(string storedProcedure, List<SySqlParameter> parameters = null);
        List<IItem> GetItems(string storedProcedure, List<SySqlParameter> parameters = null);
		List<IItem> GetItems(DataSet ds);
		IItem GetNewItem();
		IItem GetNewItemDefinition();
		IItemData<T> GetNewItemData<T>();
		IItemData<T> GetNewItemDefinitionData<T>();
	}
}
