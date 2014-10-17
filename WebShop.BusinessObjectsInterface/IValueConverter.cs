using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjectsInterface
{
	public interface IValueConverter
	{
		dynamic Convert(AttributeDataTypeEnum attributeDataTypeEnum, string value, bool toItemTableData = false);
	}
}
