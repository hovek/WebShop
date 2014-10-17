using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Common;
using Syrilium.CommonInterface;

namespace WebShop
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Module.Init();
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RequireHttpsAttribute());
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
        }

        void Application_Error(object sender, EventArgs e)
        {
            OnError();
        }

        private void OnError()
        {
            var excludeExceptions = Settings.ExcludeExceptions;
            string absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
            if (excludeExceptions.URL.Any(h => absoluteUri.Contains(h)))
                return;

            Exception ex = HttpContext.Current.Server.GetLastError();

            string errorDescription = "";

            errorDescription += "Request URL:\n" + HttpContext.Current.Request.Url.AbsoluteUri + "\n\n";

            errorDescription += "Request time:\n" + HttpContext.Current.Timestamp.ToString() + "\n\n";

            errorDescription += "Exception time:\n" + DateTime.Now.ToString() + "\n\n";

            //exception info
            while (ex != null)
            {
                // ovo je za ignore webresource errora kada google prikazuje iz cachea stranice
                if (ex is System.Security.Cryptography.CryptographicException && HttpContext.Current.Request.Url.LocalPath.ToLower().IndexOf("resource.axd") > -1 && HttpContext.Current.Request.UrlReferrer.Host.ToLower().IndexOf(ConfigurationManager.AppSettings["HostName"].ToLower()) == -1)
                {
                    HttpContext.Current.Server.ClearError();
                    return;
                }

                errorDescription += "\n\nException source: " + ex.GetType().ToString() + "\n\nError message: " + ex.Message.Replace("\n", "").Replace("\r", "") + "\n\n";

                errorDescription += "Stack trace: \n" + ex.StackTrace + "\n\n";
                ex = ex.InnerException;
            }

            //session data
            SessionState sessionState = SessionState.I;
            errorDescription += "Session data: \n";
            if (sessionState.Login != null)
                errorDescription += string.Concat("LoginId: ", sessionState.Login.Id, "\n");
            if (sessionState.User != null)
                errorDescription += string.Concat("User (", sessionState.User.Id, "): ", sessionState.User.Name, " ", sessionState.User.Surname, "\n");
            if (sessionState.Partner != null)
                errorDescription += string.Concat("Partner (", sessionState.Partner.Id, "): ", sessionState.Partner.Name, "\n");
            errorDescription += "\n";

            errorDescription += "HTTP headers: \n";

            foreach (string header in HttpContext.Current.Request.Headers.AllKeys)
            {
                string[] values = HttpContext.Current.Request.Headers.GetValues(header);
                if (header == "From")
                {
                    if (excludeExceptions.HeaderFrom.Any(f => values.Any(v => v.Contains(f))))
                        return;
                }

                errorDescription += header + ": ";
                foreach (string val in values)
                    errorDescription += val + "\n";
            }
            errorDescription += "\n";
            errorDescription += "IP: " + HttpContext.Current.Request.UserHostAddress + "\n";

            string errorMailUserName = ConfigurationManager.AppSettings["ErrorMailUserName"];

            Module.I<IMail>().SendMail(
                smtpHost: ConfigurationManager.AppSettings["SmtpHost"],
                enableSsl: false,
                from: ConfigurationManager.AppSettings["ErrorMailFrom"],
                to: ConfigurationManager.AppSettings["AdminMail"],
                subject: "error@" + ConfigurationManager.AppSettings["HostName"],
                isBodyHtml: false,
                body: errorDescription,
                useCredentials: !string.IsNullOrEmpty(errorMailUserName),
                userName: errorMailUserName,
                password: ConfigurationManager.AppSettings["ErrorMailPassword"]);
        }
    }
}