using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Interfaces.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface;

namespace Syrilium.Modules.BusinessObjects.Item
{
	public class ItemDefinition : IItem
	{
		public int Id { get; set; }
		public int? TypeId { get; set; }
		public int? ParentId { get; set; }

		public IItem Parent
		{
			get
			{
				return (IItem)EFParent;
			}
			set
			{
				EFParent = (ItemDefinition)value;
			}
		}

		public ObservableCollection<IItem> Children
		{
			get
			{
				return childrenReplication.Collection1;
			}
			set
			{
				childrenReplication.Collection1 = value;
			}
		}
		public ObservableCollection<IItemData<int>> IntData
		{
			get
			{
				return intDataReplication.Collection1;
			}
			set
			{
				intDataReplication.Collection1 = value;
			}
		}
		public ObservableCollection<IItemData<string>> StringData
		{
			get
			{
				return stringDataReplication.Collection1;
			}
			set
			{
				stringDataReplication.Collection1 = value;
			}
		}
		public ObservableCollection<IItemData<decimal>> DecimalData
		{
			get
			{
				return decimalDataReplication.Collection1;
			}
			set
			{
				decimalDataReplication.Collection1 = value;
			}
		}
		public ObservableCollection<IItemData<DateTime>> DateTimeData
		{
			get
			{
				return dateTimeDataReplication.Collection1;
			}
			set
			{
				dateTimeDataReplication.Collection1 = value;
			}
		}

		public virtual ItemDefinition EFParent { get; set; }
		public virtual ObservableCollection<ItemDefinition> EFChildren
		{
			get
			{
				return childrenReplication.Collection2;
			}
			set
			{
				childrenReplication.Collection2 = value;
			}
		}
		public virtual ObservableCollection<ItemDefinitionDataInt> EFIntData
		{
			get
			{
				return intDataReplication.Collection2;
			}
			set
			{
				intDataReplication.Collection2 = value;
			}
		}
		public virtual ObservableCollection<ItemDefinitionDataString> EFStringData
		{
			get
			{
				return stringDataReplication.Collection2;
			}
			set
			{
				stringDataReplication.Collection2 = value;
			}
		}
		public virtual ObservableCollection<ItemDefinitionDataDecimal> EFDecimalData
		{
			get
			{
				return decimalDataReplication.Collection2;
			}
			set
			{
				decimalDataReplication.Collection2 = value;
			}
		}
		public virtual ObservableCollection<ItemDefinitionDataDateTime> EFDateTimeData
		{
			get
			{
				return dateTimeDataReplication.Collection2;
			}
			set
			{
				dateTimeDataReplication.Collection2 = value;
			}
		}

		private ObservableCollectionReplication<IItem, ItemDefinition> childrenReplication;
		private ObservableCollectionReplication<IItemData<int>, ItemDefinitionDataInt> intDataReplication;
		private ObservableCollectionReplication<IItemData<string>, ItemDefinitionDataString> stringDataReplication;
		private ObservableCollectionReplication<IItemData<decimal>, ItemDefinitionDataDecimal> decimalDataReplication;
		private ObservableCollectionReplication<IItemData<DateTime>, ItemDefinitionDataDateTime> dateTimeDataReplication;

		public ItemDefinition()
		{
			childrenReplication = new ObservableCollectionReplication<IItem, ItemDefinition>();
			intDataReplication = new ObservableCollectionReplication<IItemData<int>, ItemDefinitionDataInt>();
			stringDataReplication = new ObservableCollectionReplication<IItemData<string>, ItemDefinitionDataString>();
			decimalDataReplication = new ObservableCollectionReplication<IItemData<decimal>, ItemDefinitionDataDecimal>();
			dateTimeDataReplication = new ObservableCollectionReplication<IItemData<DateTime>, ItemDefinitionDataDateTime>();

			Children = new ObservableCollection<IItem>();
			IntData = new ObservableCollection<IItemData<int>>();
			StringData = new ObservableCollection<IItemData<string>>();
			DecimalData = new ObservableCollection<IItemData<decimal>>();
			DateTimeData = new ObservableCollection<IItemData<DateTime>>();

			EFChildren = new ObservableCollection<ItemDefinition>();
			EFIntData = new ObservableCollection<ItemDefinitionDataInt>();
			EFStringData = new ObservableCollection<ItemDefinitionDataString>();
			EFDecimalData = new ObservableCollection<ItemDefinitionDataDecimal>();
			EFDateTimeData = new ObservableCollection<ItemDefinitionDataDateTime>();
		}
	}
}
