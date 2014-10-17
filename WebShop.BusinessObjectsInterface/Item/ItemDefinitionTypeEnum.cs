using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface.Item
{
    public enum ItemDefinitionTypeEnum
    {
        AttributeDefinition = 1,
        AttributeDefinitionDisplayValue = 2,
        AttributeDefinitionValueFormat = 3,

        AttributeTemplate = 4,
        AttributeTemplateAttribute = 5,
        AttributeTemplateConstraint = 10,
        AttributeTemplateAttributeGroup = 11,

        PredefinedList = 9
    }
}
