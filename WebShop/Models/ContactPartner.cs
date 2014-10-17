using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface.Item;
using S = Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface.Caching;

namespace WebShop.Models
{
    public class ContactPartner
    {
        private IItem product;
        public IItem Product
        {
            get
            {
                return product;
            }
            set
            {
                product = value;
                ContactForm.Product = value;
                int? partnerId = product.GetRawValue(AttributeKeyEnum.Partner);
                if (partnerId.HasValue)
                {
                    ContactForm.PartnerId = partnerId.Value;
					Partner = Module.I<ICache>(CacheNames.MainCache).I<S.Partner>().GetPartner(partnerId.Value);
                }
            }
        }

        public S.Partner Partner { get; set; }

        public ContactForm ContactForm { get; set; }

        public ContactPartner()
        {
            ContactForm = new ContactForm();
        }
    }
}
