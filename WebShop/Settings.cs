using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace WebShop
{
    public class Settings
    {
        public static ExcludeExceptions ExcludeExceptions { get; private set; }

        static Settings()
        {
            ExcludeExceptions = Json.Decode<ExcludeExceptions>(ConfigurationManager.AppSettings["ExcludeExceptions"]);
        }
    }

    public class ExcludeExceptions
    {
        public List<string> HeaderFrom { get; set; }
        public List<string> URL { get; set; }
    }
}