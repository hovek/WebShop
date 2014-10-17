using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using Syrilium.Modules.BusinessObjects;
using System.Web.Routing;
using System.IO;
using System.Web.Hosting;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;

namespace WebShop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int? gid)
        {
            gid = gid ?? LeftMenu.GetMenuCached().First().Id;
            WebShop.Models.Home home = new WebShop.Models.Home();
            home.LeftMenu.GroupId = gid.Value;
            home.SearchBox.SetDepartmentAndGroupId(gid.Value);

            return View(home);
        }

        public ActionResult SetLanguage(string LanguagePicker, string controller, string action)
        {
            SessionState.I.LanguageId = Convert.ToInt32(LanguagePicker);
            Syrilium.Modules.BusinessObjects.Language language = new Syrilium.Modules.BusinessObjects.Language();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult RepeaterTopItems(int departmentId, int? partnerId)
        {
            return PartialView("RepeaterTopItems", new RepeaterItems { DepartmentId = departmentId, PartnerId = partnerId });
        }

        public ActionResult RepeaterLastItems(int departmentId, int? partnerId)
        {
            return PartialView("RepeaterLastItems", new RepeaterItems { DepartmentId = departmentId, PartnerId = partnerId });
        }

        [HttpPost]
        public ActionResult GetConstrainedList(int templateId, int constrainedAttributeId, int languageId, FormCollection fc)
        {
            return PartialView("ConstrainedList", new ConstrainedList()
            {
                TemplateId = templateId,
                ConstrainedAttributeId = constrainedAttributeId,
                LanguageId = languageId,
                FormCollection = fc
            });
        }
        public ActionResult Test()
        {
            string host = Request.Url.Host.ToLower();
            if (host == "localhost")
            {
                return RedirectToAction("Index");
            }
            return View("TestPage");
        }
    }
}
