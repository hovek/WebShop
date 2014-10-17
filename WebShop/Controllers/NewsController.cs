using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /News/

        public ActionResult Index(int NewsId)
        {
            News News = News.GetNews(NewsId);
            return View(News);
        }

        public ActionResult NewsItems()
        {
            return View("NewsItems");
        }

    }
}
