using System.Collections.Generic;
using Syrilium.Interfaces.BusinessObjectsInterface.Item;
using System;
using System.Linq;
using System.Data.Entity.ModelConfiguration;
using Syrilium.CommonInterface;

namespace Syrilium.Modules.BusinessObjects.Item
{
    public class Item : IItem
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
                EFParent = (Item)value;
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

        private ObservableCollection<IItemData<dynamic>> data;
        public ObservableCollection<IItemData<dynamic>> Data
        {
            get
            {
                data.CollectionChanged -= data_CollectionChanged;
                data.Clear();
                data.AddRange(IntData.ConvertAll<IItemData<dynamic>>(i =>
                {
                    return new ItemData<dynamic>
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        TypeId = i.TypeId,
                        Value = i.Value
                    };
                }));
                data.AddRange(StringData.ConvertAll<IItemData<dynamic>>(i =>
                {
                    return new ItemData<dynamic>
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        TypeId = i.TypeId,
                        Value = i.Value
                    };
                }));
                data.AddRange(DecimalData.ConvertAll<IItemData<dynamic>>(i =>
                {
                    return new ItemData<dynamic>
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        TypeId = i.TypeId,
                        Value = i.Value
                    };
                }));
                data.AddRange(DateTimeData.ConvertAll<IItemData<dynamic>>(i =>
                {
                    return new ItemData<dynamic>
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        TypeId = i.TypeId,
                        Value = i.Value
                    };
                }));
                data.CollectionChanged += data_CollectionChanged;
                return data;
            }
            private set
            {
                data = value;
            }
        }

        public virtual Item EFParent { get; set; }
        public virtual ObservableCollection<Item> EFChildren
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
        public virtual ObservableCollection<ItemDataInt> EFIntData
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
        public virtual ObservableCollection<ItemDataString> EFStringData
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
        public virtual ObservableCollection<ItemDataDecimal> EFDecimalData
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
        public virtual ObservableCollection<ItemDataDateTime> EFDateTimeData
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

        private ObservableCollectionReplication<IItem, Item> childrenReplication;
		private ObservableCollectionReplication<IItemData<int>, ItemDataInt> intDataReplication;
		private ObservableCollectionReplication<IItemData<string>, ItemDataString> stringDataReplication;
		private ObservableCollectionReplication<IItemData<decimal>, ItemDataDecimal> decimalDataReplication;
		private ObservableCollectionReplication<IItemData<DateTime>, ItemDataDateTime> dateTimeDataReplication;

        public Item()
        {
            childrenReplication = new ObservableCollectionReplication<IItem, Item>();
			intDataReplication = new ObservableCollectionReplication<IItemData<int>, ItemDataInt>();
			stringDataReplication = new ObservableCollectionReplication<IItemData<string>, ItemDataString>();
			decimalDataReplication = new ObservableCollectionReplication<IItemData<decimal>, ItemDataDecimal>();
			dateTimeDataReplication = new ObservableCollectionReplication<IItemData<DateTime>, ItemDataDateTime>();

            Children = new ObservableCollection<IItem>();
            IntData = new ObservableCollection<IItemData<int>>();
            StringData = new ObservableCollection<IItemData<string>>();
            DecimalData = new ObservableCollection<IItemData<decimal>>();
            DateTimeData = new ObservableCollection<IItemData<DateTime>>();
            Data = new ObservableCollection<IItemData<dynamic>>();

            EFChildren = new ObservableCollection<Item>();
            EFIntData = new ObservableCollection<ItemDataInt>();
            EFStringData = new ObservableCollection<ItemDataString>();
            EFDecimalData = new ObservableCollection<ItemDataDecimal>();
            EFDateTimeData = new ObservableCollection<ItemDataDateTime>();
        }

        private void data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }
    }
}