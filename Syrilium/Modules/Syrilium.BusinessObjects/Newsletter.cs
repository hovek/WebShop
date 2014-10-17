using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class NewsletterSubscriber
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public int? UserId { get; set; }
		public virtual User User { get; set; }
		public int? PartnerId { get; set; }
		public virtual Partner Partner { get; set; }
		public virtual List<NewsletterSubscription> Subscriptions { get; set; }

		public static List<NewsletterSubscriber> GetNewsletter()
		{
			return (from newsletter in WebShopDb.I.NewsletterSubscriber
					where newsletter.Email != null && newsletter.PartnerId == null && newsletter.UserId == null
					select newsletter).ToList();
		}

		public NewsletterSubscriber()
		{
			Subscriptions = new List<NewsletterSubscription>();
		}
	}

	public class NewsletterSubscription
	{
		public int Id { get; set; }
        public DateTime DateOfEntry { get; set; }
        public int NewsletterSubscriberId { get; set; }
		public virtual NewsletterSubscriber NewsletterSubscriber { get; set; }
		public int ItemId { get; set; }
        public int LanguageId { get; set; }
	}

	public class NewsletterMail
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Subject { get; set; }
		public string Content { get; set; }
		public DateTime DateCreated { get; set; }

        public static NewsletterMail Get(int newsletterMailId, WebShopDb context = null)
        {
            WebShopDb ctx = context ?? WebShopDb.I;
            return (from na in ctx.NewsletterMail
                    where na.Id == newsletterMailId
                    select na).FirstOrDefault();
        }
	}

	public class NewsletterMailingList
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual List<NewsletterMailingListSubscriber> Subscribers { get; set; }

        public static NewsletterMailingList GetByID(int NewsletterMailingListId, WebShopDb context = null)
        {
            WebShopDb ctx = context ?? WebShopDb.I;
            return (from nml in ctx.NewsletterMailingList
                    where nml.Id == NewsletterMailingListId
                    select nml).FirstOrDefault();
        }
	}

	public class NewsletterMailingListSubscriber
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public int? NewsletterMailingListId { get; set; }
		public virtual NewsletterMailingList NewsletterMailingList { get; set; }

        public static List<NewsletterMailingListSubscriber> GetUsersByListId(int? NewsletterMailingListId, WebShopDb context = null)
        {
            WebShopDb ctx = context ?? WebShopDb.I;
            return (from nmls in ctx.NewsletterMailingListSubscriber
                    where nmls.NewsletterMailingListId == NewsletterMailingListId
                    select nmls).ToList();
        }
	}
}