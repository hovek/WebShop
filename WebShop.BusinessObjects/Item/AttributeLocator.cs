using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WebShop.BusinessObjects.ModuleDefinitions;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjects.Item
{
    public class AttributeLocator : IAttributeLocator
    {
        private static Type attributeDefinitionType;

        static AttributeLocator()
        {
            attributeDefinitionType = Module.I<IAttributeDefinition>().GetType();
        }

        private ICache cache = null;
        public ICache Cache
        {
            get
            {
                if (cache == null)
                {
                    cache = Module.I<ICache>(CacheNames.MainCache);
                }
                return cache;
            }
            set
            {
                cache = value;
            }
        }

        private List<IAttributeDefinition> attributeDefinitions;
        public List<IAttributeDefinition> AttributeDefinitions
        {
            get
            {
                if (attributeDefinitions == null)
                {
                    attributeDefinitions = Cache.I<IAttributeDefinition>(attributeDefinitionType).Get();
                }
                return attributeDefinitions;
            }
            set
            {
                attributeDefinitions = value;
            }
        }

        public int? this[AttributeKeyEnum key]
        {
            get
            {
                return FindId(a => a.Key == key);
            }
        }

        public IAttributeLocator Set(Action<IAttributeLocator> action)
        {
            action((IAttributeLocator)this);
            return (IAttributeLocator)this;
        }

        public int? FindId(Predicate<IAttributeDefinition> match, ICache cache = null)
        {
            IAttributeDefinition attrDef = Find(match, cache);
            return attrDef == null ? null : (int?)attrDef.Id;
        }

        public IAttributeDefinition Find(Predicate<IAttributeDefinition> match, ICache cache = null)
        {
            if (cache != null) Cache = cache;
            return Find(AttributeDefinitions, match);
        }

        public IAttributeDefinition Find(IEnumerable<IAttributeDefinition> attrDefs, Predicate<IAttributeDefinition> match, ICache cache = null)
        {
            if (cache != null) Cache = cache;
            foreach (var attr in attrDefs)
            {
                if (match(attr))
                    return attr;
                else
                {
                    IAttributeDefinition attrDef = Find(attr.Attributes, match);
                    if (attrDef != null) return attrDef;
                }
            }
            return null;
        }
    }
}
