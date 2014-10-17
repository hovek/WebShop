using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;

namespace WebShop.Models
{
    public class RepeaterItems
    {
        private int? departmentId = null;
        public int DepartmentId
        {
            get
            {
                if (departmentId == null)
                {
                    departmentId = LeftMenu.GetMenuCached().First().Id;
                }
                return departmentId.Value;
            }
            set
            {
                departmentId = value;
            }
        }

        public HttpRequestBase Request
        {
            set
            {
                NameValueCollection queryString = value.Form.Count > 0 ? value.Form : value.QueryString;
                int id;
                if (int.TryParse(CommonHelpers.GetQueryStringParamValue(queryString, "did"), out id)
                    || int.TryParse(CommonHelpers.GetQueryStringParamValue(queryString, "gid"), out id)
                    || int.TryParse(CommonHelpers.GetQueryStringParamValue(queryString, "pid"), out id))
                    DepartmentId = Module.I<IGroup>().GetDepartmentId(id);

                int partnerId;
                if (int.TryParse(CommonHelpers.GetQueryStringParamValue(queryString, AttributeLocator.FindId(a => a.Key == AttributeKeyEnum.Partner).ToString()), out partnerId))
                    PartnerId = partnerId;
            }
        }

        private IAttributeLocator attributeLocator;
        public IAttributeLocator AttributeLocator
        {
            get
            {
                if (attributeLocator == null)
                {
                    attributeLocator = Module.I<IAttributeLocator>();
                }
                return attributeLocator;
            }
        }

        public int? PartnerId { get; set; }

        public RepeaterItems()
        {
            PartnerId = null;
        }

        public List<RepeaterItem> GetLastItems()
        {
            ICache cache = Module.I<ICache>(CacheNames.MainCache);
            ItemSearch search = cache.I<ItemSearch>();
            List<SearchParameter> searchParameters = new List<SearchParameter>();
            searchParameters.Add(new SearchParameter
            {
                Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.Show),
                ComparisonOperator = ComparisonOperator.NotExistsOrEquals,
                Value = true
            });
            if (PartnerId.HasValue)
            {
                searchParameters.Add(new SearchParameter
                {
                    Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.Partner),
                    ComparisonOperator = ComparisonOperator.Equals,
                    Value = PartnerId
                });
            }

            List<SortParameter> sortParameters = new List<SortParameter>();
            sortParameters.Add(new SortParameter
            {
                Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.DateTimeEntry),
                SortDirection = System.Web.Helpers.SortDirection.Descending
            });

            List<IItem> products = search.Find(searchParameters, sortParameters: sortParameters, parentId: DepartmentId, recordsPerPage: 5);

            int? imageAttributeId = AttributeLocator[AttributeKeyEnum.Image];
            int? countyAttributeId = AttributeLocator[AttributeKeyEnum.County].Value;
            int? auctionDateAttributeId = AttributeLocator[AttributeKeyEnum.AuctionDate];
            int? priceAttributeId = AttributeLocator[AttributeKeyEnum.Price];

            List<RepeaterItem> items = new List<RepeaterItem>();
            foreach (var product in products)
            {
                var imageName = imageAttributeId.HasValue ? product.GetValueFormated(imageAttributeId.Value) : "";
                if (string.IsNullOrEmpty(imageName))
                    imageName = WebShop.Module.I<IConfig>().GetValue(ConfigNames.DefaultProductImage);
                items.Add(new RepeaterItem
                {
                    Id = product.Id,
                    ImageUrl = CommonHelpers.GetProductImagePath("W85H65", imageName),
                    Title = countyAttributeId.HasValue ? product.GetValueFormated(countyAttributeId.Value) : "",
                    Date = auctionDateAttributeId.HasValue ? product.GetValueFormated(auctionDateAttributeId.Value) : "",
                    Price = priceAttributeId.HasValue ? product.GetValueFormated(priceAttributeId.Value) : ""
                });
            }

            return items;
        }

        public List<RepeaterItem> GetTopItems()
        {
            ICache cache = Module.I<ICache>(CacheNames.MainCache);

            ItemSearch search = cache.I<ItemSearch>();
            List<SearchParameter> searchParameters = new List<SearchParameter>();
            searchParameters.Add(new SearchParameter
            {
                Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.Show),
                ComparisonOperator = ComparisonOperator.NotExistsOrEquals,
                Value = true
            });
            searchParameters.Add(new SearchParameter
            {
                Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.Top),
                ComparisonOperator = ComparisonOperator.Equals,
                Value = 1
            });
            if (PartnerId.HasValue)
            {
                searchParameters.Add(new SearchParameter
                {
                    Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.Partner),
                    ComparisonOperator = ComparisonOperator.Equals,
                    Value = PartnerId
                });
            }

            List<SortParameter> sortParameters = new List<SortParameter>();
            sortParameters.Add(new SortParameter
            {
                Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.DateTimeEntry),
                SortDirection = System.Web.Helpers.SortDirection.Descending
            });

            List<IItem> products = search.Find(searchParameters, sortParameters, parentId: DepartmentId, recordsPerPage: 5);

            int? imageAttributeId = AttributeLocator[AttributeKeyEnum.Image];
            int? countyAttributeId = AttributeLocator[AttributeKeyEnum.County];
            int? auctionDateAttributeId = AttributeLocator[AttributeKeyEnum.AuctionDate];
            int? priceAttributeId = AttributeLocator[AttributeKeyEnum.Price];

            List<RepeaterItem> items = new List<RepeaterItem>();
            foreach (var product in products)
            {
                var imageName = imageAttributeId.HasValue ? product.GetValueFormated(imageAttributeId.Value) : "";
                if (string.IsNullOrEmpty(imageName))
                    imageName = WebShop.Module.I<IConfig>().GetValue(ConfigNames.DefaultProductImage);
                items.Add(new RepeaterItem
                {
                    Id = product.Id,
                    ImageUrl = CommonHelpers.GetProductImagePath("W85H65", imageName),
                    Title = countyAttributeId.HasValue ? product.GetValueFormated(countyAttributeId.Value) : "",
                    Date = auctionDateAttributeId.HasValue ? product.GetValueFormated(auctionDateAttributeId.Value) : "",
                    Price = priceAttributeId.HasValue ? product.GetValueFormated(priceAttributeId.Value) : ""
                });
            }

            return items;
        }
    }

    public class RepeaterItem
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Price { get; set; }
        public string Date { get; set; }
    }
}