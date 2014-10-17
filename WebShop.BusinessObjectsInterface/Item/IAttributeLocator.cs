using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjectsInterface.Item
{
    public interface IAttributeLocator
    {
        ICache Cache { get; set; }
        List<IAttributeDefinition> AttributeDefinitions { get; set; }
        int? this[AttributeKeyEnum key] { get; }
        int? FindId(Predicate<IAttributeDefinition> match, ICache cache = null);
        IAttributeDefinition Find(Predicate<IAttributeDefinition> match, ICache cache = null);
        IAttributeDefinition Find(IEnumerable<IAttributeDefinition> attrDefs, Predicate<IAttributeDefinition> match, ICache cache = null);
        IAttributeLocator Set(Action<IAttributeLocator> action);
    }
}
