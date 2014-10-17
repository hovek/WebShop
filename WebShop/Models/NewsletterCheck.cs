using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class NewsletterCheck
    {
        public int ItemId { get; set; }

        private bool? isChecked = null;
        public bool IsChecked
        {
            get
            {
                if (!isChecked.HasValue)
                {
                    SessionState ss = SessionState.I;
                    if (ss.Login == null)
                    {
                        isChecked = false;
                    }
                    else
                    {
                        WebShopDb context = WebShopDb.I;
                        int? userId = ss.User == null ? null : (int?)ss.User.Id;
                        int? partnerId = ss.Partner == null ? null : (int?)ss.Partner.Id;

                        isChecked = (from sb in context.NewsletterSubscriber
                                     join sp in context.NewsletterSubscription on sb.Id equals sp.NewsletterSubscriberId
                                     where sp.ItemId == ItemId && (sb.PartnerId == partnerId || sb.UserId == userId)
                                            && (sp.LanguageId == 0 || sp.LanguageId == ss.LanguageId)
                                     select true).FirstOrDefault();
                    }
                }
                return isChecked.Value;
            }
        }

        public void Check(bool isChecked)
        {
            SessionState ss = SessionState.I;
            int? userId = ss.User == null ? null : (int?)ss.User.Id;
            int? partnerId = ss.Partner == null ? null : (int?)ss.Partner.Id;

            if ((userId ?? partnerId) == null) return;

            WebShopDb context = WebShopDb.I;
            NewsletterSubscriber sb = context.NewsletterSubscriber.Where(s => s.PartnerId == partnerId || s.UserId == userId).FirstOrDefault();
            NewsletterSubscription sp = null;
            if (sb == null)
            {
                sb = new NewsletterSubscriber
                {
                    PartnerId = partnerId,
                    UserId = userId
                };
                context.NewsletterSubscriber.Add(sb);
            }
            else
            {
                sp = context.NewsletterSubscription.Where(s => s.ItemId == ItemId && s.NewsletterSubscriberId == sb.Id
                                                                && (s.LanguageId == 0 || s.LanguageId == SessionState.I.LanguageId)).FirstOrDefault();
            }

            if (isChecked)
            {
                if (sp == null)
                {
                    sp = new NewsletterSubscription
                    {
                        ItemId = ItemId,
                        LanguageId = SessionState.I.LanguageId
                    };
                    sb.Subscriptions.Add(sp);
                }
            }
            else
            {
                context.NewsletterSubscription.Remove(sp);
            }

            context.SaveChanges();

            this.isChecked = isChecked;
        }
    }
}