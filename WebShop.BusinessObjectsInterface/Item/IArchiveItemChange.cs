using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface.Item
{
    public interface IArchiveItemChange
    {
        IItem PreviousItem { get; set; }
        IItem CurrentItem { get; set; }
        bool IsItemInNewsletterSubscription { get; }
        void SetPreviousItem(int itemId);
        void CompareAndSave();
        List<int> DeletedAttributes { get; set; }
    }
}
