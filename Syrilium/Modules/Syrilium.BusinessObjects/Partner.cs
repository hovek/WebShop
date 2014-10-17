using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Syrilium.Modules.BusinessObjects
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
        public bool Visible { get; set; }
        public bool Advertiser { get; set; }
        public string Background { get; set; }
        public string HtmlBackground { get; set; }
        public virtual Login Login { get; set; }
        public virtual List<PermissionGroup> PermissionGroups { get; set; }

        public Partner()
        {
            PermissionGroups = new List<PermissionGroup>();
        }

        public virtual List<Partner> GetPartners(bool? premium = true)
        {
            if (premium.HasValue)
                return WebShopDb.I.Partner.Where(p => p.Premium == premium && p.Visible == true).ToList();
            else
                return (from partner in WebShopDb.I.Partner
                    where partner.Visible == true
                    select partner).ToList();
        }

        public virtual Partner GetPartner(int partnerId, WebShopDb context = null)
        {
            return (from partner in (context ?? WebShopDb.I).Partner
                    where partner.Id == partnerId
                    select partner).First();
        }

		public bool HasPermission(string permissionName)
		{
			return PermissionGroups.Exists(g => g.Permissions.Exists(p => p.Name == permissionName));
		}
	}
}
