using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebShop.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult DefaultError()
        {
            return View("Error");
        }
    }
}
