using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class ContactForm
    {
        public int PartnerId { get; set; }
        public IItem Product { get; set; }

        public string SubjectName
        {
            get
            {
                return Product.GetValueFormated(AttributeKeyEnum.Name) + " ( " + Translation.Get("poslano sa ovrhe.hr") + " ) ";
            }
        }
    }
}