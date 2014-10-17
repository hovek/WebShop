using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjects.ModuleDefinitions;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using Syrilium.DataAccessInterface;
using Syrilium.CommonInterface.Caching;
using Syrilium.DataAccessInterface.SQL;

namespace WebShop.BusinessObjects.Item
{
    public class Group : IGroup
    {
        public int GetDepartmentId(int itemId, ICache cache = null)
        {
            return GetClosestParentId(itemId, ((int)ItemTypeEnum.Department).ToString(), cache);
        }

        public int GetClosestParentId(int itemId, string parentTypeIds, ICache cache = null)
        {
            if (cache == null) cache = Module.I<ICache>(CacheNames.MainCache);
            return (int)cache.I<Group>().GetClosestParentId(itemId, parentTypeIds);
        }

        //mora vračati object jer kod primarnih tipova kao što je int cache ne radi dobro, vraća krivi rezultat
        public virtual object GetClosestParentId(int itemId, string parentTypeIds)
        {
            SySqlParameter itemIdParam = new SySqlParameter();
            itemIdParam.ParameterName = "@childId";
            itemIdParam.Value = itemId;
            SySqlParameter typeIdParam = new SySqlParameter();
            typeIdParam.ParameterName = "@typeId";
            typeIdParam.Value = parentTypeIds;
            return Module.I<IQuery>().GetScalarValueWithProcedure<int>(SPNames.GetClosestParentId, new List<SySqlParameter> { itemIdParam, typeIdParam });
        }

        public List<IItem> Get(int? getGroupsByItemParentId = null, bool includeShowAttribute = true, ICache cache = null)
        {
            if (cache == null) cache = Module.I<ICache>(CacheNames.MainCache);

            Group group = cache.I<Group>();
            return group.Get(Module.LanguageId, getGroupsByItemParentId, includeShowAttribute);
        }

        public virtual List<IItem> Get(int languageId, int? getGroupsByItemParentId = null, bool includeShowAttribute = true)
        {
            List<SySqlParameter> parameters = new List<SySqlParameter>();

            if (getGroupsByItemParentId != null)
            {
                SySqlParameter paramGroupId = new SySqlParameter();
                paramGroupId.ParameterName = "@getGroupsByItemParentId";
                paramGroupId.Value = getGroupsByItemParentId;
                parameters.Add(paramGroupId);
            }

            SySqlParameter param = new SySqlParameter();
            param.ParameterName = "@includeShowAttribute";
            param.Value = includeShowAttribute;
            parameters.Add(param);

            param = new SySqlParameter();
            param.ParameterName = "@languageId";
            param.Value = languageId;
            parameters.Add(param);

            List<IItem> groups = new Item().Get(SPNames.GetGroups, parameters);
            OrderGroups(groups);
            return groups;
        }

        public void OrderGroups(IList<IItem> groups)
        {
            Dictionary<int, int> orderedGroups = new Dictionary<int, int>();
            List<IItem> ordered = new List<IItem>();
            int index = 1;
            foreach (IItem group in groups)
            {
                S.IItemData<int> order = group.RawItem.IntData.Find(i => i.TypeId == (int)ItemDataTypeEnum.GroupOrder);
                int orderNum;
                if (order == null) orderNum = index;
                else orderNum = order.Value;
                orderedGroups.Add(group.Id, orderNum);
                ordered.Add(group);
                index++;
            }

            groups.Clear();
            foreach (IItem i in ordered.OrderBy(g => orderedGroups[g.Id]))
                groups.Add(i);

            foreach (IItem group in groups)
                OrderGroups(group);
        }
    }
}
