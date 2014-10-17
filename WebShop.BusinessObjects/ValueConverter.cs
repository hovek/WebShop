using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjects
{
	public class ValueConverter : IValueConverter
	{
		public dynamic Convert(AttributeDataTypeEnum attributeDataTypeEnum, string value, bool toItemTableData = false)
		{
			switch (attributeDataTypeEnum)
			{
				case AttributeDataTypeEnum.Bool:
					value = value.Trim();
					if (toItemTableData)
					{
						if (value.Equals("false", StringComparison.InvariantCultureIgnoreCase) || value == "0")
							return 0;
						else
							return 1;
					}
					else
					{
						bool boolValue;
						if (!bool.TryParse(value, out boolValue))
						{
							if (value == "0") return false;
							else if (value.Length > 0) return true;
							return null;
						}
						return boolValue;
					}
				case AttributeDataTypeEnum.DateTime:
					DateTime dateValue;
					if (!DateTime.TryParse(value.Trim(), out dateValue))
						return null;
					return dateValue;
				case AttributeDataTypeEnum.Decimal:
					decimal decimalValue;
					value = value.Trim().Replace(',', '.');
					if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
						return null;
					return decimalValue;
				case AttributeDataTypeEnum.Int:
				case AttributeDataTypeEnum.Reference:
					int intValue;
					if (!int.TryParse(value.Trim(), out intValue))
						return null;
					return intValue;
				default:
					return value;
			}
		}
	}
}
