using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new { controller = "Home", action = "Index" } // Parameter defaults
            );
   
            /*Rute koje traba ignorirat*/
            routes.IgnoreRoute("robots.txt");
            routes.IgnoreRoute("sitemap.xml");
            routes.IgnoreRoute("favicon.ico");

        }
    }
}