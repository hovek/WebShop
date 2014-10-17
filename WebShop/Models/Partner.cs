using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class Partner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string About { get; set; }
        public string WorkDescription { get; set; }
        public string Services { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string URL { get; set; }
        public string Fax { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int? LoginId { get; set; }
        public bool Premium { get; set; }
        public string Background { get; set; }
        public string HtmlBackground { get; set; }
        public virtual Login Login { get; set; }
        public virtual List<PermissionGroup> PermissionGroups { get; set; }


        //Samo za partnere bez oglasivaca
        public static SelectList GetPartnerList()
        {
            Dictionary<int, string> partnerList = new Dictionary<int, string>();
            ICache cache = Module.I<ICache>(CacheNames.MainCache);
            foreach (Syrilium.Modules.BusinessObjects.Partner partner in cache.I<Syrilium.Modules.BusinessObjects.Partner>().GetPartners())
            {
                partnerList.Add(partner.Id, partner.Name);
            }
            SelectList selectList = new SelectList(partnerList, "Key", "Value");
            return selectList;

        }

        //I oglasivaci i partneri za admin
        public static SelectList GetPartnerAdvertiserList()
        {
            Dictionary<int, string> partnerList = new Dictionary<int, string>();
            ICache cache = Module.I<ICache>(CacheNames.MainCache);
            foreach (Syrilium.Modules.BusinessObjects.Partner partner in cache.I<Syrilium.Modules.BusinessObjects.Partner>().GetPartners(null))
            {
                partnerList.Add(partner.Id, partner.Name);
            }
            SelectList selectList = new SelectList(partnerList, "Key", "Value");
            return selectList;

        }
    }
}