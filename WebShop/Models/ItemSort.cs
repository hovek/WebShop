using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;

namespace WebShop.Models
{
    public class ItemSort
    {
        public List<SelectListItem> Items { get; set; }
        public string FormId { get; set; }
        public List<SortParameter> SortParameters { get; set; }

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

        public void Init(HttpRequestBase request)
        {
            NameValueCollection queryString = request.Form.AllKeys.Count() > 0 ? request.Form : request.QueryString;
            SortParameters = SortParameter.Get(queryString);

            if (SortParameters.Count == 0)
            {
                SortParameters.Add(new SortParameter
                {
                    Attribute = AttributeLocator.Find(a => a.Key == AttributeKeyEnum.DateTimeEntry),
                    SortDirection = SortDirection.Descending
                });
            }

            int? priceAttrId = AttributeLocator[AttributeKeyEnum.Price];
            int? auctionDateAttrId = AttributeLocator[AttributeKeyEnum.AuctionDate];
            int? dateTimeEntryAttrId = AttributeLocator[AttributeKeyEnum.DateTimeEntry];

            Items = new List<SelectListItem>();
            Items.Add(new SelectListItem
            {
                Selected = SortParameters.Find(p => p.Attribute.Id == dateTimeEntryAttrId && p.SortDirection == SortDirection.Descending) != null,
                Text = Translation.Get("Najnovije"),
                Value = string.Concat(dateTimeEntryAttrId, "_d")
            });
            Items.Add(new SelectListItem
            {
                Selected = SortParameters.Find(p => p.Attribute.Id == dateTimeEntryAttrId && p.SortDirection == SortDirection.Ascending) != null,
                Text = Translation.Get("Najstarije"),
                Value = string.Concat(dateTimeEntryAttrId, "_a")
            });
            Items.Add(new SelectListItem
            {
                Selected = SortParameters.Find(p => p.Attribute.Id == priceAttrId && p.SortDirection == SortDirection.Ascending) != null,
                Text = Translation.Get("Cijena: manja"),
                Value = string.Concat(priceAttrId, "_a")
            });
            Items.Add(new SelectListItem
            {
                Selected = SortParameters.Find(p => p.Attribute.Id == priceAttrId && p.SortDirection == SortDirection.Descending) != null,
                Text = Translation.Get("Cijena: veća"),
                Value = string.Concat(priceAttrId, "_d")
            });
            Items.Add(new SelectListItem
            {
                Selected = SortParameters.Find(p => p.Attribute.Id == auctionDateAttrId && p.SortDirection == SortDirection.Ascending) != null,
                Text = Translation.Get("Datum dražbe: najprije"),
                Value = string.Concat(auctionDateAttrId, "_a")
            });
            Items.Add(new SelectListItem
            {
                Selected = SortParameters.Find(p => p.Attribute.Id == auctionDateAttrId && p.SortDirection == SortDirection.Descending) != null,
                Text = Translation.Get("Datum dražbe: najkasnije"),
                Value = string.Concat(auctionDateAttrId, "_d")
            });
        }
    }
}