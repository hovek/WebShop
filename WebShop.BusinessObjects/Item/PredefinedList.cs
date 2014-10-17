using Syrilium.CommonInterface;
using Syrilium.DataAccessInterface;
using Syrilium.DataAccessInterface.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjects.ModuleDefinitions;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjects.Item
{
	public class PredefinedList : IPredefinedList
	{
		private bool inLoadProcess = false;

		public int Id
		{
			get { return RawItem.Id; }
			set { RawItem.Id = value; }
		}

		public ObservableCollection<IItemDefinitionValue<string>> DisplayValues { get; private set; }
		public ObservableCollection<IPredefinedListValue> Values { get; private set; }

		private S.IItem rawItem;
		public S.IItem RawItem
		{
			get
			{
				if (rawItem == null)
				{
					rawItem = Module.I<S.IItemManager>().GetNewItem();
					rawItem.TypeId = (int)ItemDefinitionTypeEnum.PredefinedList;
				}
				return rawItem;
			}
			set
			{
				rawItem = value;
			}
		}

		public PredefinedList()
		{
			DisplayValues = new ObservableCollection<IItemDefinitionValue<string>>();
			DisplayValues.CollectionChanged += displayValues_CollectionChanged;
			Values = new ObservableCollection<IPredefinedListValue>();
			Values.CollectionChanged += values_CollectionChanged;
		}

		public IPredefinedList Get(int listId)
		{
            List<SySqlParameter> parameters = new List<SySqlParameter>();
            SySqlParameter param = new SySqlParameter();
			param.ParameterName = "@listId";
			param.Value = listId;
			parameters.Add(param);

			return Get(SPNames.GetPredefinedList, parameters).FirstOrDefault();
		}

        public virtual List<IPredefinedList> Get(string storedProcedure, List<SySqlParameter> parameters = null)
		{
			List<S.IItem> rawItems = Module.I<S.IItemManager>().GetItemDefinitions(storedProcedure, parameters);
			List<IPredefinedList> items = new List<IPredefinedList>();
			foreach (S.IItem item in rawItems)
			{
				items.Add(get(item));
			}

			return items;
		}

		private PredefinedList get(S.IItem rawItem)
		{
			PredefinedList item = new PredefinedList();
			item.RawItem = rawItem;
			item.inLoadProcess = true;
			foreach (S.IItemData<string> itemDisplayValue in rawItem.StringData)
			{
				IItemDefinitionValue<string> itemValue = ModuleDefinitions.Module.I<IItemDefinitionValue<string>>();
				itemValue.RawItem = itemDisplayValue;
				item.DisplayValues.Add(itemValue);
			}
			foreach (S.IItem itemC in rawItem.Children)
			{
				item.Values.Add(PredefinedListValue.Get(itemC));
			}
			item.inLoadProcess = false;

			return item;
		}

		private void values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (inLoadProcess) return;
			if (e.OldItems != null)
			{
				foreach (IPredefinedListValue oldItem in e.OldItems)
				{
					RawItem.Children.Remove(oldItem.RawItem);
				}
			}
			if (e.NewItems != null)
			{
				foreach (IPredefinedListValue newItem in e.NewItems)
				{
					RawItem.Children.Add(newItem.RawItem);
				}
			}
		}

		private void displayValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (inLoadProcess) return;
			if (e.OldItems != null)
			{
				foreach (IItemValue<string> oldItem in e.OldItems)
				{
					RawItem.StringData.Remove(oldItem.RawItem);
				}
			}
			if (e.NewItems != null)
			{
				foreach (IItemValue<string> newItem in e.NewItems)
				{
					RawItem.StringData.Add(newItem.RawItem);
				}
			}
		}
	}

	public class PredefinedListValue : IPredefinedListValue
	{
		private bool inLoadProcess = false;

		public int Id
		{
			get { return RawItem.Id; }
			set { RawItem.Id = value; }
		}

		public ObservableCollection<IItemDefinitionValue<string>> StringValues { get; private set; }
		public ObservableCollection<IItemDefinitionValue<int>> IntValues { get; private set; }
		public ObservableCollection<IItemDefinitionValue<decimal>> DecimalValues { get; private set; }
		public ObservableCollection<IItemDefinitionValue<DateTime>> DateTimeValues { get; private set; }

		private S.IItem rawItem;
		public S.IItem RawItem
		{
			get
			{
				if (rawItem == null)
				{
					rawItem = Module.I<S.IItemManager>().GetNewItem();
				}
				return rawItem;
			}
			set
			{
				rawItem = value;
			}
		}

		public dynamic Value
		{
			get
			{
				return GetValue(Module.LanguageId);
			}
		}

		public dynamic GetValue(int languageId)
		{
			foreach (var i in IntValues)
			{
				if (i.LanguageId == languageId || i.LanguageId == 0)
				{
					return i.Value;
				}
			}
			foreach (var i in DecimalValues)
			{
				if (i.LanguageId == languageId || i.LanguageId == 0)
				{
					return i.Value;
				}
			}
			foreach (var i in StringValues)
			{
				if (i.LanguageId == languageId || i.LanguageId == 0)
				{
					return i.Value;
				}
			}
			foreach (var i in DateTimeValues)
			{
				if (i.LanguageId == languageId || i.LanguageId == 0)
				{
					return i.Value;
				}
			}
			return "";
		}

		public PredefinedListValue()
		{
			StringValues = new ObservableCollection<IItemDefinitionValue<string>>();
			StringValues.CollectionChanged += displayValues_CollectionChanged;
			IntValues = new ObservableCollection<IItemDefinitionValue<int>>();
			IntValues.CollectionChanged += intValues_CollectionChanged;
			DecimalValues = new ObservableCollection<IItemDefinitionValue<decimal>>();
			DecimalValues.CollectionChanged += decimalValues_CollectionChanged;
			DateTimeValues = new ObservableCollection<IItemDefinitionValue<DateTime>>();
			DateTimeValues.CollectionChanged += dateTimeValues_CollectionChanged;
		}

		public static PredefinedListValue Get(S.IItem rawItem)
		{
			PredefinedListValue predefinedListValue = new PredefinedListValue();
			predefinedListValue.inLoadProcess = true;
			predefinedListValue.RawItem = rawItem;
			foreach (S.IItemData<string> item in rawItem.StringData)
			{
				IItemDefinitionValue<string> itemValue = ModuleDefinitions.Module.I<IItemDefinitionValue<string>>();
				itemValue.RawItem = item;
				predefinedListValue.StringValues.Add(itemValue);
			}
			foreach (S.IItemData<int> item in rawItem.IntData)
			{
				IItemDefinitionValue<int> itemValue = ModuleDefinitions.Module.I<IItemDefinitionValue<int>>();
				itemValue.RawItem = item;
				predefinedListValue.IntValues.Add(itemValue);
			}
			foreach (S.IItemData<decimal> item in rawItem.DecimalData)
			{
				IItemDefinitionValue<decimal> itemValue = ModuleDefinitions.Module.I<IItemDefinitionValue<decimal>>();
				itemValue.RawItem = item;
				predefinedListValue.DecimalValues.Add(itemValue);
			}
			foreach (S.IItemData<DateTime> item in rawItem.DateTimeData)
			{
				IItemDefinitionValue<DateTime> itemValue = ModuleDefinitions.Module.I<IItemDefinitionValue<DateTime>>();
				itemValue.RawItem = item;
				predefinedListValue.DateTimeValues.Add(itemValue);
			}
			predefinedListValue.inLoadProcess = false;

			return predefinedListValue;
		}

		private void displayValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (inLoadProcess) return;
			if (e.OldItems != null)
			{
				foreach (IItemValue<string> oldItem in e.OldItems)
				{
					RawItem.StringData.Remove(oldItem.RawItem);
				}
			}
			if (e.NewItems != null)
			{
				foreach (IItemValue<string> newItem in e.NewItems)
				{
					RawItem.StringData.Add(newItem.RawItem);
				}
			}
		}

		void dateTimeValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (inLoadProcess) return;
			if (e.OldItems != null)
			{
				foreach (IItemValue<DateTime> oldItem in e.OldItems)
				{
					RawItem.DateTimeData.Remove(oldItem.RawItem);
				}
			}
			if (e.NewItems != null)
			{
				foreach (IItemValue<DateTime> newItem in e.NewItems)
				{
					RawItem.DateTimeData.Add(newItem.RawItem);
				}
			}
		}

		void decimalValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (inLoadProcess) return;
			if (e.OldItems != null)
			{
				foreach (IItemValue<decimal> oldItem in e.OldItems)
				{
					RawItem.DecimalData.Remove(oldItem.RawItem);
				}
			}
			if (e.NewItems != null)
			{
				foreach (IItemValue<decimal> newItem in e.NewItems)
				{
					RawItem.DecimalData.Add(newItem.RawItem);
				}
			}
		}

		void intValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (inLoadProcess) return;
			if (e.OldItems != null)
			{
				foreach (IItemValue<int> oldItem in e.OldItems)
				{
					RawItem.IntData.Remove(oldItem.RawItem);
				}
			}
			if (e.NewItems != null)
			{
				foreach (IItemValue<int> newItem in e.NewItems)
				{
					RawItem.IntData.Add(newItem.RawItem);
				}
			}
		}
	}
}
