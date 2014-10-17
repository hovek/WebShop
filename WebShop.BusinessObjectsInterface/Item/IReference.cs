using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface.Item
{
	public interface IReference
	{
		dynamic GetObject(ItemDefinitionDataTypeEnum referenceType, int referenceId);
		List<ICommonReferenceListItem> ConvertToCommonReferenceList(dynamic referenceObject, int languageId);
		List<dynamic> GetListValues(List<ICommonReferenceListItem> referenceList, List<int> listValuesId);
	}
}
