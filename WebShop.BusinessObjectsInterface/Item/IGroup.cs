using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface.Item
{
    public interface IGroup
    {
        int GetDepartmentId(int itemId, ICache cache = null);
        int GetClosestParentId(int itemId, string parentTypeIds, ICache cache = null);
        List<IItem> Get(int? getGroupsByItemParentId = null, bool includeShowAttribute = true, ICache cache = null);
        void OrderGroups(IList<IItem> groups);
    }
}
