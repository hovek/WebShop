using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebShop.BusinessObjectsInterface;
using S = Syrilium.Modules.BusinessObjects;

namespace WebShop.Models
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Persistent { get; set; }
        public string ErrorMessage { get; set; }
        public bool LoginSucceeded { get; set; }
        public bool LoginAttempt { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsAjaxRequest { get; set; }

        public void Enter(IEnumerable<string> requiredPermissions = null)
        {
            S.Login login = Username == null ? SessionState.I.Login : S.Login.GetUser(Username, Password);
            if (login != null)
            {
                if (requiredPermissions == null || requiredPermissions.Count() == 0 || requiredPermissions.All(i => login.AnyHasPermission(i)))
                {
                    if (!string.IsNullOrEmpty(login.MailConfirmationCode))
                    {
                        ErrorMessage = Translation.Get("Niste potvrdili e-mail adresu.");
                    }
                    else
                    {
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, Username, DateTime.Now, DateTime.Now.AddYears(100), Persistent, "");
                        string encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(authTicket);
                        HttpCookie authCookie = new HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, encryptedTicket);
                        if (Persistent)
                            authCookie.Expires = authTicket.Expiration;
                        else
                            authCookie.Expires = DateTime.Now.AddMinutes(30);
                        HttpContext.Current.Response.Cookies.Add(authCookie);
                        LoginSucceeded = true;
                    }
                }
                else
                    ErrorMessage = Translation.Get("Korisnik nema ovlasti.");
            }
            else if (Username != null)
            {
                ErrorMessage = Translation.Get("Unesite točno korisničko ime i lozinku.");
            }
        }
    }
}