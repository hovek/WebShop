using Syrilium.CommonInterface.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using WebShop.BusinessObjectsInterface;
using M = WebShop.Models;

namespace WebShop.Controllers
{
    public class SitemapController : Controller
    {
        //
        // GET: /Sitemap/
        public ActionResult Index()
        {
           
            string sitemapXmlPath = Server.MapPath("/sitemap.xml");
            List<M.Sitemap> sitemapList = Module.I<ICache>(CacheNames.MainCache).I<M.Sitemap>().Get(sitemapXmlPath);
            M.Sitemap sitemap = new M.Sitemap();
            sitemap.SitemapList = sitemapList;

            return View(sitemap);


        }
        

    }
}
