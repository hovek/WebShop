using Syrilium.CommonInterface.Caching;
using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjects.ModuleDefinitions;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjects.Item
{
    public class Reference : IReference
    {
        public static Reference I
        {
            get
            {
                return new Reference();
            }
        }

        public dynamic GetObject(ItemDefinitionDataTypeEnum referenceType, int referenceId)
        {
            if (referenceType == ItemDefinitionDataTypeEnum.AttributeReferencePredefinedListId)
            {
                return Module.I<ICache>(CacheNames.MainCache).I<PredefinedList>().Get(referenceId);
            }
            else if (referenceType == ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId)
            {
                if (referenceId == (int)AttributeDataSystemListReferenceEnum.Partner)
                {
                    return Module.I<ICache>(CacheNames.MainCache).I<Partner>().GetPartners(null);
                }
                else if (referenceId == (int)AttributeDataSystemListReferenceEnum.County)
                {
                    return Module.I<ICache>(CacheNames.MainCache).I<County>().Get();
                }
                else if (referenceId == (int)AttributeDataSystemListReferenceEnum.DistrictCity)
                {
                    return Module.I<ICache>(CacheNames.MainCache).I<DistrictCity>().Get();
                }
                else
                {
                    throw new NotImplementedException("Attribute.SystemListReferenceId not implemented.");
                }
            }
            else
            {
                throw new NotImplementedException("Attribute.ReferenceType not implemented.");
            }
        }

        public List<ICommonReferenceListItem> ConvertToCommonReferenceList(dynamic referenceObject, int languageId)
        {
            List<ICommonReferenceListItem> referenceList = new List<ICommonReferenceListItem>();
            if (referenceObject is PredefinedList)
            {
                foreach (IPredefinedListValue pl in referenceObject.Values)
                {
                    referenceList.Add(new CommonReferenceListItem
                    {
                        Id = pl.Id,
                        Value = pl.GetValue(languageId)
                    });
                }
            }
            else if (referenceObject is List<Partner>)
            {
                foreach (Partner partner in referenceObject)
                {
                    referenceList.Add(new CommonReferenceListItem
                    {
                        Id = partner.Id,
                        Value = partner.Name
                    });
                }
            }
            else if (referenceObject is List<County>)
            {
                foreach (County county in referenceObject)
                {
                    referenceList.Add(new CommonReferenceListItem
                    {
                        Id = county.Id,
                        Value = county.Name
                    });
                }
            }
            else if (referenceObject is List<DistrictCity>)
            {
                foreach (DistrictCity districtCity in referenceObject)
                {
                    referenceList.Add(new CommonReferenceListItem
                    {
                        Id = districtCity.Id,
                        Value = districtCity.Name
                    });
                }
            }

            referenceList.Sort((i1, i2) => i1.Value.CompareTo(i2.Value));

            return referenceList;
        }

        public List<dynamic> GetListValues(List<ICommonReferenceListItem> referenceList, List<int> listValuesId)
        {
            List<dynamic> values = new List<dynamic>();
            foreach (ICommonReferenceListItem crlItem in referenceList)
            {
                foreach (int valueID in listValuesId)
                {
                    if (crlItem.Id == valueID)
                    {
                        values.Add(crlItem.Value);
                        break;
                    }
                }
            }

            return values;
        }
    }
}
