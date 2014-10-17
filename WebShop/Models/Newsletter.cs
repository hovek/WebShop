using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.Models
{
    public class Newsletter
    {
        public LeftMenu LeftMenu { get; set; }
        public string Email { get; set; }
        public List<int> chekedItems { get; set; }


        public List<Syrilium.Modules.BusinessObjects.NewsletterSubscriber> NewsletterSubscriber { get; set; }
        public List<Syrilium.Modules.BusinessObjects.NewsletterMail> NewsletterMail { get; set; }
        public List<Syrilium.Modules.BusinessObjects.NewsletterMailingListSubscriber> NewsletterMailingListSubscriber { get; set; }

        public int NewsletterMailId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public string NewsletterMailingListName { get; set; }

        public static SelectList NewsletterMailingList()
        {
            Dictionary<int, string> newsletterMailingList = new Dictionary<int, string>();
            foreach (Syrilium.Modules.BusinessObjects.NewsletterMailingList nml in WebShopDb.I.NewsletterMailingList.ToList())
            {
                newsletterMailingList.Add(nml.Id, nml.Name);
            }
            SelectList selectList = new SelectList(newsletterMailingList, "Key", "Value");
            return selectList;

        }
    }
}