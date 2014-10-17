using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface.Item
{
	public enum ItemDefinitionDataTypeEnum
	{
		AttributeDataType = 51,
		AttributeKey = 65,

		AttributeReferencePredefinedListId = 52,
		AttributeReferenceSystemListId = 61,

		AttributeTemplateOrder = 64,
		AttributeTemplateAttributeId = 57,
		AttributeTemplateConstraintMinEntry = 59,
		AttributeTemplateConstraintMaxEntry = 60,
		AttributeTemplateConstraintForeignKeyAttribute = 63,

		AttributeSettingHidden = 58,
		AttributeSettingDeleteNotAllowed = 62,
		AttributeSettingDataTypeChangeNotAllowed = 66
	}
}
