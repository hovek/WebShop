using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Infrastructure;
using Syrilium.Modules.BusinessObjects;
using M = WebShop.Models;
using Syrilium.CommonInterface;

namespace WebShop.Controllers
{
    public class NewsletterController : Controller
    {
        //
        // GET: /Newsletter/

        public ActionResult Index()
        {


            return View();
        }

        //Metoda koja prima mail iz newsletterbox-a i sprema ga u bazu
        public ActionResult Newsletter(string newsletterEmail)
        {

            if (String.IsNullOrEmpty(newsletterEmail))
            {
                TempData["ValidationEmailFalse"] = M.Translation.Get("Unesite e-mail");

            }
            bool IsValidEmail = Module.I<IMail>().IsEmailValid(newsletterEmail);
            if (IsValidEmail == false && !String.IsNullOrEmpty(newsletterEmail))
            {
                TempData["ValidationEmailFalse"] = M.Translation.Get("Unesite ispravan e-mail");
            }
            if (IsValidEmail == true)
            {
                WebShopDb context = WebShopDb.I;
                NewsletterSubscriber newsletter = new NewsletterSubscriber();

                newsletter.Email = newsletterEmail;
                context.NewsletterSubscriber.Add(newsletter);
                context.SaveChanges();
                TempData["ValidationEmailTrue"] = M.Translation.Get("E-mail je unesen. Novosti ćete primat na uneseni e-mail");

            }

            return PartialView("NewsletterBox");
        }


    }
}
