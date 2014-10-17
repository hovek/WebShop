using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.Controllers
{
    public class ServicesController : Controller
    {
        //
        // GET: /Services/

        public ActionResult Index(string page)
        {
            ViewBag.CustomPage = true;

            PageTranslation currentPage = null;
            if (page != null)
            {
                currentPage = Page.GetPageTranslationByName(page);
                ViewData["TitlePage"] = currentPage.Name;
                ViewData["Content"] = currentPage.Content;
            }
            return View();
        }

    }
}
