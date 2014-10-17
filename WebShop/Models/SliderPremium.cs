using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;

namespace WebShop.Models
{
    public class SliderPremium
    {
        public virtual List<IItem> GetProducts()
        {
            IAttributeLocator attributeLocator = Module.I<IAttributeLocator>();

            ItemSearch search = new ItemSearch();
            List<SearchParameter> searchParameters = new List<SearchParameter>();
            searchParameters.Add(new SearchParameter
            {
                Attribute = attributeLocator.Find(a => a.Key == AttributeKeyEnum.Show),
                ComparisonOperator = ComparisonOperator.NotExistsOrEquals,
                Value = true
            });
            searchParameters.Add(new SearchParameter
            {
                Attribute = attributeLocator.Find(a => a.Key == AttributeKeyEnum.Premium),
                ComparisonOperator = ComparisonOperator.Equals,
                Value = 1
            });
            searchParameters.Add(new SearchParameter
            {
                Attribute = attributeLocator.Find(a => a.Key == AttributeKeyEnum.Image),
                ComparisonOperator = ComparisonOperator.Exists,
                Value = true
            });

            List<SortParameter> sortParameters = new List<SortParameter>();
            sortParameters.Add(new SortParameter
            {
                Attribute = attributeLocator.Find(a => a.Key == AttributeKeyEnum.DateTimeEntry),
                SortDirection = System.Web.Helpers.SortDirection.Descending
            });

            return search.Find(searchParameters, sortParameters, recordsPerPage: 10);
        }

        public List<SliderItem> GetSliderItems()
        {
            List<SliderItem> items = new List<SliderItem>();
            List<IItem> products = Module.I<ICache>(CacheNames.MainCache).I<SliderPremium>().GetProducts();

            List<IAttributeDefinition> attributeDefinitions = Module.I<ICache>(CacheNames.MainCache).I<IAttributeDefinition>(Module.I<IAttributeDefinition>().GetType()).Get();
            int imageAttrId = attributeDefinitions.Find(a => a.Key == AttributeKeyEnum.Image).Id;
            int nameAttrId = attributeDefinitions.Find(a => a.Key == AttributeKeyEnum.Name).Id;

            foreach (var product in products)
            {
                items.Add(new SliderItem
                {
                    Id = product.Id,
                    ImageUrl = CommonHelpers.GetProductImagePath("W500H300", product.GetRawValue(imageAttrId)),
                    Title = product.GetRawValue(nameAttrId)
                });
            }
            return items;
        }
    }

    public class SliderItem
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
    }
}