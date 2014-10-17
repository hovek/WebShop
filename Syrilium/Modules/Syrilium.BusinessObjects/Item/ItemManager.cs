using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.DataAccessInterface;
using System.Data;
using System.Reflection;
using System.Data.Entity;
using Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.DataAccessInterface.SQL;

namespace Syrilium.Modules.BusinessObjects.Item
{
	public class ItemManager : IItemManager
	{
		private static Dictionary<Type, List<PropertyInfo>> itemTypesWithProperties = null;
		private static Dictionary<Type, List<PropertyInfo>> itemDefinitionTypesWithProperties = null;
		private static Type itemType = typeof(Item);
		private static Type itemDataStringType = typeof(ItemDataString);
		private static Type itemDataIntType = typeof(ItemDataInt);
		private static Type itemDataDecimalType = typeof(ItemDataDecimal);
		private static Type itemDataDateTimeType = typeof(ItemDataDateTime);
		private static Type itemDefinitionType = typeof(ItemDefinition);
		private static Type itemDefinitionDataStringType = typeof(ItemDefinitionDataString);
		private static Type itemDefinitionDataIntType = typeof(ItemDefinitionDataInt);
		private static Type itemDefinitionDataDecimalType = typeof(ItemDefinitionDataDecimal);
		private static Type itemDefinitionDataDateTimeType = typeof(ItemDefinitionDataDateTime);
		private static Type intType = typeof(int);
		private static Type strignType = typeof(string);
		private static Type decimalType = typeof(decimal);
		private static Type dateTimeType = typeof(DateTime);
		private static Type objectType = typeof(object);

        public List<IItem> GetItems(string storedProcedure, List<SySqlParameter> parameters = null)
		{
			DataSet ds = ModuleDefinitions.Module.I<IQuery>().GetDataSetWithProcedure(storedProcedure, parameters);
			return GetItems(ds);
		}

		public List<IItem> GetItems(DataSet ds)
		{
			Dictionary<DataTable, KeyValuePair<Type, List<PropertyInfo>>> mapping = getDataTableItemMapping(ds, getItemTypesWithProperties());
			return convertToItemHierarchy(getItems(mapping));
		}

        public List<IItem> GetItemDefinitions(string storedProcedure, List<SySqlParameter> parameters = null)
		{
			DataSet ds = ModuleDefinitions.Module.I<IQuery>().GetDataSetWithProcedure(storedProcedure, parameters);
			Dictionary<DataTable, KeyValuePair<Type, List<PropertyInfo>>> mapping = getDataTableItemMapping(ds, getItemDefinitionTypesWithProperties());
			return convertToItemDefinitionHierarchy(getItems(mapping));
		}

		#region GetNew
		public IItem GetNewItem()
		{
			return new Item();
		}

		public IItem GetNewItemDefinition()
		{
			return new ItemDefinition();
		}

		public IItemData<T> GetNewItemData<T>()
		{
			Type typeOfT = typeof(T);
			if (typeOfT == intType)
			{
				return (IItemData<T>)new ItemDataInt();
			}
			else if (typeOfT == strignType)
			{
				return (IItemData<T>)new ItemDataString();
			}
			else if (typeOfT == decimalType)
			{
				return (IItemData<T>)new ItemDataDecimal();
			}
			else if (typeOfT == dateTimeType)
			{
				return (IItemData<T>)new ItemDataDateTime();
			}
			else if (typeOfT == objectType)
			{
				return (IItemData<T>)new ItemData<dynamic>();
			}

			throw new NotImplementedException();
		}

		public IItemData<T> GetNewItemDefinitionData<T>()
		{
			Type typeOfT = typeof(T);
			if (typeOfT == intType)
			{
				return (IItemData<T>)new ItemDefinitionDataInt();
			}
			else if (typeOfT == strignType)
			{
				return (IItemData<T>)new ItemDefinitionDataString();
			}
			else if (typeOfT == decimalType)
			{
				return (IItemData<T>)new ItemDefinitionDataDecimal();
			}
			else if (typeOfT == dateTimeType)
			{
				return (IItemData<T>)new ItemDefinitionDataDateTime();
			}
			else if (typeOfT == objectType)
			{
				return (IItemData<T>)new ItemDefinitionData<dynamic>();
			}

			throw new NotImplementedException();
		}
		#endregion

		public void AssociateWithDbContext(DbContext context, IItem item, EntityState entityState = EntityState.Unchanged)
		{
			DbSet dbSet = context.Set(item.GetType());
			dbSet.Add(item);

			List<dynamic> locals = new List<dynamic>();
			foreach (var i in dbSet.Local) locals.Add(i);
			foreach (var i in locals)
			{
				context.Entry(i).State = entityState;
			}

			if (item is Item)
			{
				locals.Clear();
				foreach (var i in context.Set<ItemDataInt>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}

				locals.Clear();
				foreach (var i in context.Set<ItemDataDecimal>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}

				locals.Clear();
				foreach (var i in context.Set<ItemDataString>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}

				locals.Clear();
				foreach (var i in context.Set<ItemDataDateTime>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}
			}
			else if (item is ItemDefinition)
			{
				locals.Clear();
				foreach (var i in context.Set<ItemDefinitionDataInt>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}

				locals.Clear();
				foreach (var i in context.Set<ItemDefinitionDataDecimal>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}

				locals.Clear();
				foreach (var i in context.Set<ItemDefinitionDataString>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}

				locals.Clear();
				foreach (var i in context.Set<ItemDefinitionDataDateTime>().Local) locals.Add(i);
				foreach (var i in locals)
				{
					context.Entry(i).State = entityState;
				}
			}
		}

		private static List<IItem> convertToItemHierarchy(Dictionary<Type, List<dynamic>> items)
		{
			List<IItem> itemsConv = items[itemType].ConvertAll<IItem>(c => (IItem)c);
			List<IItem> itemsForDelete = new List<IItem>();

			foreach (IItem item in itemsConv)
			{
				if (items.ContainsKey(itemDataStringType))
				{
					foreach (ItemDataString id in items[itemDataStringType])
					{
						if (item.Id == id.ItemId)
						{
							item.StringData.Add(id);
						}
					}
				}
				if (items.ContainsKey(itemDataIntType))
				{
					foreach (ItemDataInt id in items[itemDataIntType])
					{
						if (item.Id == id.ItemId)
						{
							item.IntData.Add(id);
						}
					}
				}
				if (items.ContainsKey(itemDataDecimalType))
				{
					foreach (ItemDataDecimal id in items[itemDataDecimalType])
					{
						if (item.Id == id.ItemId)
						{
							item.DecimalData.Add(id);
						}
					}
				}
				if (items.ContainsKey(itemDataDateTimeType))
				{
					foreach (ItemDataDateTime id in items[itemDataDateTimeType])
					{
						if (item.Id == id.ItemId)
						{
							item.DateTimeData.Add(id);
						}
					}
				}

				foreach (IItem itemC in itemsConv)
				{
					if (item.Id == itemC.ParentId)
					{
						item.Children.Add(itemC);
						itemC.Parent = item;
						itemsForDelete.Add(itemC);
					}
				}
			}

			itemsForDelete.ForEach(i => itemsConv.Remove(i));

			return itemsConv;
		}

		private static List<IItem> convertToItemDefinitionHierarchy(Dictionary<Type, List<dynamic>> items)
		{
			List<IItem> itemsConv = items[itemDefinitionType].ConvertAll<IItem>(c => (IItem)c);
			List<IItem> itemsForDelete = new List<IItem>();

			foreach (IItem item in itemsConv)
			{
				if (items.ContainsKey(itemDefinitionDataStringType))
				{
					foreach (ItemDefinitionDataString id in items[itemDefinitionDataStringType])
					{
						if (item.Id == id.ItemId)
						{
							item.StringData.Add(id);
						}
					}
				}
				if (items.ContainsKey(itemDefinitionDataIntType))
				{
					foreach (ItemDefinitionDataInt id in items[itemDefinitionDataIntType])
					{
						if (item.Id == id.ItemId)
						{
							item.IntData.Add(id);
						}
					}
				}
				if (items.ContainsKey(itemDefinitionDataDecimalType))
				{
					foreach (ItemDefinitionDataDecimal id in items[itemDefinitionDataDecimalType])
					{
						if (item.Id == id.ItemId)
						{
							item.DecimalData.Add(id);
						}
					}
				}
				if (items.ContainsKey(itemDefinitionDataDateTimeType))
				{
					foreach (ItemDefinitionDataDateTime id in items[itemDefinitionDataDateTimeType])
					{
						if (item.Id == id.ItemId)
						{
							item.DateTimeData.Add(id);
						}
					}
				}
				foreach (IItem itemC in itemsConv)
				{
					if (item.Id == itemC.ParentId)
					{
						item.Children.Add(itemC);
						itemC.Parent = item;
						itemsForDelete.Add(itemC);
					}
				}
			}

			itemsForDelete.ForEach(i => itemsConv.Remove(i));

			return itemsConv;
		}

		private static Dictionary<Type, List<dynamic>> getItems(Dictionary<DataTable, KeyValuePair<Type, List<PropertyInfo>>> mapping)
		{
			Dictionary<Type, List<dynamic>> items = new Dictionary<Type, List<dynamic>>();

			foreach (KeyValuePair<DataTable, KeyValuePair<Type, List<PropertyInfo>>> mapp in mapping)
			{
				List<dynamic> dynItems = new List<dynamic>();
				foreach (DataRow dr in mapp.Key.Rows)
				{
					object item = mapp.Value.Key.GetConstructor(new Type[0]).Invoke(null);
					dynItems.Add(item);
					foreach (PropertyInfo pi in mapp.Value.Value)
					{
						pi.SetValue(item, dr[pi.Name] == DBNull.Value ? null : dr[pi.Name], null);
					}
				}

				items[mapp.Value.Key] = dynItems;
			}

			return items;
		}

		private static Dictionary<DataTable, KeyValuePair<Type, List<PropertyInfo>>> getDataTableItemMapping(DataSet dataSet, Dictionary<Type, List<PropertyInfo>> typePIs)
		{
			Dictionary<DataTable, KeyValuePair<Type, List<PropertyInfo>>> mapping = new Dictionary<DataTable, KeyValuePair<Type, List<PropertyInfo>>>();

			foreach (DataTable tbl in dataSet.Tables)
			{
				Type bestMatchedType = null;
				List<PropertyInfo> bestMatchedPI = null;
				int bestNumOfDataTypeMatch = 0;
				foreach (KeyValuePair<Type, List<PropertyInfo>> type in typePIs)
				{
					int numOfColNameMatch = 0;
					int numOfDataTypeMatch = 0;
					List<PropertyInfo> matchedPI = new List<PropertyInfo>();
					foreach (PropertyInfo pi in type.Value)
					{
						if (tbl.Columns.Contains(pi.Name))
						{
							numOfColNameMatch++;
							matchedPI.Add(pi);
							if (tbl.Columns[pi.Name].DataType == pi.PropertyType)
							{
								numOfDataTypeMatch++;
							}
						}
					}
					if (bestMatchedType == null
						|| bestMatchedPI.Count < numOfColNameMatch
						|| (bestMatchedPI.Count == numOfColNameMatch && bestNumOfDataTypeMatch < numOfDataTypeMatch))
					{
						bestMatchedType = type.Key;
						bestMatchedPI = matchedPI;
						bestNumOfDataTypeMatch = numOfDataTypeMatch;
					}
				}
				mapping[tbl] = new KeyValuePair<Type, List<PropertyInfo>>(bestMatchedType, bestMatchedPI);
			}

			return mapping;
		}

		private static Dictionary<Type, List<PropertyInfo>> getItemDefinitionTypesWithProperties()
		{
			if (itemDefinitionTypesWithProperties != null)
			{
				return itemDefinitionTypesWithProperties;
			}

			itemDefinitionTypesWithProperties = new Dictionary<Type, List<PropertyInfo>>();
			List<PropertyInfo> properties = new List<PropertyInfo>();

			properties.Add(itemDefinitionType.GetProperty("Id"));
			properties.Add(itemDefinitionType.GetProperty("TypeId"));
			properties.Add(itemDefinitionType.GetProperty("ParentId"));
			itemDefinitionTypesWithProperties[itemDefinitionType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDefinitionDataStringType.GetProperty("Id"));
			properties.Add(itemDefinitionDataStringType.GetProperty("TypeId"));
			properties.Add(itemDefinitionDataStringType.GetProperty("ItemDefinitionId"));
			properties.Add(itemDefinitionDataStringType.GetProperty("Value"));
			itemDefinitionTypesWithProperties[itemDefinitionDataStringType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDefinitionDataIntType.GetProperty("Id"));
			properties.Add(itemDefinitionDataIntType.GetProperty("TypeId"));
			properties.Add(itemDefinitionDataIntType.GetProperty("ItemDefinitionId"));
			properties.Add(itemDefinitionDataIntType.GetProperty("Value"));
			itemDefinitionTypesWithProperties[itemDefinitionDataIntType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDefinitionDataDecimalType.GetProperty("Id"));
			properties.Add(itemDefinitionDataDecimalType.GetProperty("TypeId"));
			properties.Add(itemDefinitionDataDecimalType.GetProperty("ItemDefinitionId"));
			properties.Add(itemDefinitionDataDecimalType.GetProperty("Value"));
			itemDefinitionTypesWithProperties[itemDefinitionDataDecimalType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDefinitionDataDateTimeType.GetProperty("Id"));
			properties.Add(itemDefinitionDataDateTimeType.GetProperty("TypeId"));
			properties.Add(itemDefinitionDataDateTimeType.GetProperty("ItemDefinitionId"));
			properties.Add(itemDefinitionDataDateTimeType.GetProperty("Value"));
			itemDefinitionTypesWithProperties[itemDefinitionDataDateTimeType] = properties;

			return itemDefinitionTypesWithProperties;
		}

		private static Dictionary<Type, List<PropertyInfo>> getItemTypesWithProperties()
		{
			if (itemTypesWithProperties != null)
			{
				return itemTypesWithProperties;
			}

			itemTypesWithProperties = new Dictionary<Type, List<PropertyInfo>>();
			List<PropertyInfo> properties = new List<PropertyInfo>();

			properties.Add(itemType.GetProperty("Id"));
			properties.Add(itemType.GetProperty("TypeId"));
			properties.Add(itemType.GetProperty("ParentId"));
			itemTypesWithProperties[itemType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDataStringType.GetProperty("Id"));
			properties.Add(itemDataStringType.GetProperty("TypeId"));
			properties.Add(itemDataStringType.GetProperty("ItemId"));
			properties.Add(itemDataStringType.GetProperty("Value"));
			itemTypesWithProperties[itemDataStringType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDataIntType.GetProperty("Id"));
			properties.Add(itemDataIntType.GetProperty("TypeId"));
			properties.Add(itemDataIntType.GetProperty("ItemId"));
			properties.Add(itemDataIntType.GetProperty("Value"));
			itemTypesWithProperties[itemDataIntType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDataDecimalType.GetProperty("Id"));
			properties.Add(itemDataDecimalType.GetProperty("TypeId"));
			properties.Add(itemDataDecimalType.GetProperty("ItemId"));
			properties.Add(itemDataDecimalType.GetProperty("Value"));
			itemTypesWithProperties[itemDataDecimalType] = properties;

			properties = new List<PropertyInfo>();
			properties.Add(itemDataDateTimeType.GetProperty("Id"));
			properties.Add(itemDataDateTimeType.GetProperty("TypeId"));
			properties.Add(itemDataDateTimeType.GetProperty("ItemId"));
			properties.Add(itemDataDateTimeType.GetProperty("Value"));
			itemTypesWithProperties[itemDataDateTimeType] = properties;

			return itemTypesWithProperties;
		}
	}
}