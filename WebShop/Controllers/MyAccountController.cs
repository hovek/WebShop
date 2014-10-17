using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using Syrilium.Modules.BusinessObjects;
using System.Collections;
using WebShop.Infrastructure;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using B = Syrilium.Modules.BusinessObjects;
using M = WebShop.Models;
using System.Configuration;
using System.Web.Security;
using WebShop.BusinessObjectsInterface;
using Syrilium.CommonInterface;
using Syrilium.CommonInterface.Caching;


namespace WebShop.Controllers
{
    public class MyAccountController : Controller
    {

        [MyAccountAuthorize]
        public ActionResult Index()
        {
            ViewBag.MyAccountMenu = true;
            WebShop.Models.MyAccount myAccount = new Models.MyAccount();
            myAccount.user = new WebShop.Models.User();
            myAccount.partner = new WebShop.Models.Partner();



            if (SessionState.I.Login == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Syrilium.Modules.BusinessObjects.User user = SessionState.I.User;
                Syrilium.Modules.BusinessObjects.Partner partner = SessionState.I.Partner;
                if (user != null)
                {
                    myAccount.user.Name = user.Name;
                    myAccount.user.Surname = user.Surname;
                    myAccount.user.Address = user.Address;
                    myAccount.user.City = user.City;
                    myAccount.user.DateOfBirth = user.DateOfBirth;
                    myAccount.user.Email = user.Email;
                    myAccount.user.Phone = user.Phone;
                    myAccount.user.GenderId = user.GenderId;
                }
                else if (partner != null)
                {
                    myAccount.partner.Name = partner.Name;
                    myAccount.partner.Address = partner.Address;
                    myAccount.partner.City = partner.City;
                    myAccount.partner.Email = partner.Email;
                    myAccount.partner.Phone = partner.Phone;
                    myAccount.partner.Fax = partner.Fax;
                    myAccount.partner.PostalCode = partner.PostalCode;
                    myAccount.partner.Services = partner.Services;
                    myAccount.partner.URL = partner.URL;
                    myAccount.partner.WorkDescription = partner.WorkDescription;
                }
            }

            return View(myAccount);
        }

        [MyAccountAuthorize]
        public ActionResult MySearch()
        {
            ViewBag.MyAccountMenu = true;
            if (SessionState.I.Login == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("MySearch");
        }

        [HttpPost]
        public ActionResult LoginAttempt(string username, string password, bool? persistent, string returnUrl, bool isAjaxRequest)
        {
            M.Login model = new M.Login
            {
                Username = username,
                Password = password,
                Persistent = persistent == null ? false : persistent.Value,
                LoginAttempt = true,
                ReturnUrl = returnUrl,
                IsAjaxRequest = isAjaxRequest
            };

            model.Enter(new string[] { PermissionNames.User });
            return PartialView("Login", model);
        }

        public ActionResult Login(string returnUrl)
        {
            if (returnUrl == null) returnUrl = Request.UrlReferrer.AbsoluteUri;
            return PartialView("Login", new M.Login
            {
                ReturnUrl = returnUrl,
                IsAjaxRequest = Request.IsAjaxRequest()
            });
        }

        [MyAccountAuthorize]
        public ActionResult CheckLogin()
        {
            return new TextActionResult("");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            if (Request.UrlReferrer.ToString().Contains("MyAccount"))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Registration(WebShop.Models.User userModel, DateTime? dateTime, bool? cbConditionsOfUse, string button)
        {
            ViewBag.MyAccountMenu = false;
            Syrilium.Modules.BusinessObjects.User user = new Syrilium.Modules.BusinessObjects.User();
            Syrilium.Modules.BusinessObjects.Login login = new Syrilium.Modules.BusinessObjects.Login();

            user.Login = login;
            WebShopDb context = WebShopDb.I;

            PermissionGroup userPermissionGroup = context.PermissionGroup.Where(p => p.Name == PermissionNames.User).FirstOrDefault();
            if (userPermissionGroup != null)
                login.PermissionGroups.Add(userPermissionGroup);

            bool Error = false;

            if (button == "submit")
            {
                string userName = userModel.UserName == null ? "" : userModel.UserName.ToUpper();
                if (WebShopDb.I.User.Where(u => u.Name.ToUpper().Equals(userName)).Count() > 0)
                {
                    ViewData["DisplayUserName"] = M.Translation.Get("Korisničko ime je zauzeto");
                    userModel.DisplayUserNameAlert = true;
                    Error = true;
                }
                string email = userModel.Email == null ? "" : userModel.Email.ToUpper();
                if (WebShopDb.I.User.Where(u => u.Email.ToUpper().Equals(email)).Count() > 0)
                {
                    ViewData["DisplayEmail"] = M.Translation.Get("E-mail je zauzet");
                    userModel.DisplayEmailAlert = true;
                    Error = true;
                }
                if (string.IsNullOrEmpty(userModel.Name))
                {
                    ViewData["DisplayName"] = M.Translation.Get("Unesite ime");
                    userModel.DisplayNameAlert = true;
                    Error = true;
                }
                if (string.IsNullOrEmpty(userModel.Surname))
                {
                    ViewData["DisplaySurname"] = M.Translation.Get("Unesite prezime");
                    userModel.DisplaySurnameAlert = true;
                    Error = true;
                }

                if (string.IsNullOrEmpty(userModel.UserName))
                {
                    ViewData["DisplayUserName"] = M.Translation.Get("Unesite korisničko ime");
                    userModel.DisplayUserNameAlert = true;
                    Error = true;
                }

                if (string.IsNullOrEmpty(userModel.Email) || !Module.I<IMail>().IsEmailValid(userModel.Email))
                {
                    ViewData["DisplayEmail"] = M.Translation.Get("Unesite ispravan e-mail");
                    userModel.DisplayEmailAlert = true;
                    Error = true;
                }

                if (string.IsNullOrEmpty(userModel.Password) || string.IsNullOrEmpty(userModel.PasswordConfirm) || userModel.Password != userModel.PasswordConfirm || userModel.Password.Length <= 6)
                {
                    ViewData["DisplayPassword"] = M.Translation.Get("Lozinka i ponovljena lozinka moraju biti unešene i iste, te se moraju sastojati od minimalno 7 znakova.");
                    userModel.DisplayPasswordAlert = true;
                    Error = true;
                }
                if (userModel.GenderId == null)
                {
                    ViewData["DisplayGender"] = M.Translation.Get("Unesite spol");
                    userModel.DisplayGender = true;
                    Error = true;
                }
                if (cbConditionsOfUse == false)
                {
                    ViewData["DisplayConditionsOfUse"] = M.Translation.Get("Odaberite da prihvačate uvjete korištenja");
                    userModel.DisplayConditionsOfUse = true;
                    Error = true;
                }

                if (Error == false)
                {
                    user.Name = userModel.Name;
                    user.Surname = userModel.Surname;
                    user.Email = userModel.Email;

                    user.Login.UserName = userModel.UserName;
                    user.Login.Password = userModel.Password;

                    if (userModel.GenderId != null)
                    {
                        user.GenderId = userModel.GenderId;
                    }
                    if (!string.IsNullOrEmpty(userModel.Phone))
                    {
                        user.Phone = userModel.Phone;
                    }
                    if (!string.IsNullOrEmpty(userModel.City))
                    {
                        user.City = userModel.City;
                    }
                    if (!string.IsNullOrEmpty(userModel.Address))
                    {
                        user.Address = userModel.Address;
                    }

                    user.DateOfBirth = dateTime;

                    Guid mailConfirmationCode = Guid.NewGuid();

                    user.Login.MailConfirmationCode = mailConfirmationCode.ToString();
                    user.Login.DateTimeOfCreation = DateTime.Now;

                    string domainName = Request.Url.Authority.ToString();

                    string content = M.Translation.Get("Poštovani/a") + "," + "<br style=\"clear:left\" />" +
                                     M.Translation.Get("Za autorizaciju e-mail adrese i aktivaciju korisničkog računa na stranicama www.ovrhe.hr kliknite na dolje navedeni link:") + "<br style=\"clear:left\" />" +
                                     "https://" + domainName + "/MyAccount/ConfirmRegistration?mcc=" + mailConfirmationCode.ToString() + "<br style=\"clear:left\" />" +
                                     "--------------" + "<br style=\"clear:left\" />" +
                                     "www.ovrhe.hr";
                    string siteMailUserName = ConfigurationManager.AppSettings["SiteMailUserName"];

                    Module.I<IMail>().SendMail(
                           smtpHost: ConfigurationManager.AppSettings["SmtpHost"],
                           enableSsl: false,
                           from: ConfigurationManager.AppSettings["SiteMailFrom"],
                           to: user.Email,
                           subject: M.Translation.Get("Autorizacija e-mail adrese"),
                           isBodyHtml: true,
                           body: content,
                           useCredentials: !string.IsNullOrEmpty(siteMailUserName),
                           userName: siteMailUserName,
                           password: ConfigurationManager.AppSettings["SiteMailPassword"]);

                    context.User.Add(user);
                    context.SaveChanges();
                    ViewData["MessageSuccessTitle"] = M.Translation.Get("Završetak registracije!");
                    ViewData["MessageSuccess"] = M.Translation.Get("Za završetak registracije potvrdite link koji ste dobili putem e-maila!");
                    ModelState.Clear();
                }
            }
            string termsOfUse = Module.I<ICache>(CacheNames.MainCache).I<B.Translation>().Get("OpciUvjeti", SessionState.I.LanguageId);
            ViewData["TermsOfUse"] = string.IsNullOrEmpty(termsOfUse) ? "" : termsOfUse;

            return View("Registration", userModel);
        }

        public ActionResult ForgottenPassword(string email, string button)
        {
            if (button == "submit")
            {
                if (String.IsNullOrEmpty(email))
                {
                    ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška!");
                    ViewData["MessageBoxError"] = M.Translation.Get("Unesite e-mail");

                }
                bool IsValidEmail = Module.I<IMail>().IsEmailValid(email);
                if (IsValidEmail == false && !String.IsNullOrEmpty(email))
                {
                    ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška!");
                    ViewData["MessageBoxError"] = M.Translation.Get("Unesite ispravan e-mail");
                }

                WebShopDb context = WebShopDb.I;
                Syrilium.Modules.BusinessObjects.User user = Syrilium.Modules.BusinessObjects.User.GetUserByEmail(email, context);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Email) && IsValidEmail == true)
                    {
                        //stvaranje random stringa.. updejtanje userovog passworda i slanje na mail
                        string randomString = Guid.NewGuid().ToString("N").Substring(0, 12);
                        user.Login.Password = randomString;

                        context.User.Add(user);
                        context.Entry(user).State = System.Data.EntityState.Modified;
                        context.SaveChanges();
                        string content = M.Translation.Get("Korisničko ime: ") + user.Login.UserName + "<br style=\"clear:left\" />"
                                        + M.Translation.Get("Lozinka: ") + user.Login.Password;

                        string siteMailUserName = ConfigurationManager.AppSettings["SiteMailUserName"];
                        Module.I<IMail>().SendMail(
                                          smtpHost: ConfigurationManager.AppSettings["SmtpHost"],
                                          enableSsl: false,
                                          from: ConfigurationManager.AppSettings["SiteMailFrom"],
                                          to: email,
                                          subject: M.Translation.Get("Zaboravljena lozinka"),
                                          isBodyHtml: true,
                                          body: content,
                                          useCredentials: !string.IsNullOrEmpty(siteMailUserName),
                                          userName: siteMailUserName,
                                          password: ConfigurationManager.AppSettings["SiteMailPassword"]);

                        ViewData["MessageSuccessTitle"] = M.Translation.Get("Uspješno slanje lozinke");
                        ViewData["MessageSuccess"] = M.Translation.Get("Nova lozinka je poslana na vašu e-mail adresu");
                    }
                }
                else if (user == null && IsValidEmail == true)
                {
                    ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška!");
                    ViewData["MessageBoxError"] = M.Translation.Get("Ne postoji korisnik sa tim e-mailom");
                }
            }
            return View();
        }

        [MyAccountAuthorize]
        public ActionResult EditMyProfile(WebShop.Models.User userModel, string button, DateTime? dateTime)
        {
            ViewBag.MyAccountMenu = true;

            WebShopDb context = WebShopDb.I;
            Syrilium.Modules.BusinessObjects.User user = context.User.Where(n => n.LoginId == SessionState.I.User.Id).FirstOrDefault();


            if (button == "submit")
            {
                bool Error = false;

                if (string.IsNullOrEmpty(userModel.Name))
                {
                    ViewData["DisplayName"] = M.Translation.Get("Unesite ime");
                    userModel.DisplayNameAlert = true;
                    Error = true;
                }
                if (string.IsNullOrEmpty(userModel.Surname))
                {
                    ViewData["DisplaySurname"] = M.Translation.Get("Unesite prezime");
                    userModel.DisplaySurnameAlert = true;
                    Error = true;
                }
                if (string.IsNullOrEmpty(userModel.Password) || string.IsNullOrEmpty(userModel.PasswordConfirm) || userModel.Password != userModel.PasswordConfirm || userModel.Password.Length <= 6)
                {
                    ViewData["DisplayPassword"] = M.Translation.Get("Lozinka i ponovljena lozinka moraju biti unešene i iste, te se moraju sastojati od minimalno 7 znakova.");
                    userModel.DisplayPasswordAlert = true;
                    Error = true;
                }

                if (string.IsNullOrEmpty(userModel.Email) || !Module.I<IMail>().IsEmailValid(userModel.Email))
                {
                    ViewData["DisplayEmail"] = M.Translation.Get("Unesite ispravan e-mail");
                    userModel.DisplayEmailAlert = true;
                    Error = true;
                }

                if (userModel.GenderId == null)
                {
                    ViewData["DisplayGender"] = M.Translation.Get("Unesite spol");
                    userModel.DisplayGender = true;
                    Error = true;
                }
                if (Error == false)
                {
                    user.Name = userModel.Name;
                    user.Surname = userModel.Surname;
                    user.Email = userModel.Email;


                    user.Login.Password = userModel.Password;

                    user.Phone = "";
                    user.City = "";
                    user.Address = "";
                    if (userModel.GenderId != null)
                    {
                        user.GenderId = userModel.GenderId;
                    }
                    if (!string.IsNullOrEmpty(userModel.Phone))
                    {
                        user.Phone = userModel.Phone;
                    }
                    if (!string.IsNullOrEmpty(userModel.City))
                    {
                        user.City = userModel.City;
                    }
                    if (!string.IsNullOrEmpty(userModel.Address))
                    {
                        user.Address = userModel.Address;
                    }
                    user.DateOfBirth = dateTime;


                    user.Login.DateTimeOfCreation = user.Login.DateTimeOfCreation;

                    context.User.Add(user);
                    context.Entry(user).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                    ViewData["MessageSuccessTitle"] = M.Translation.Get("Uspješna promjena profila!");
                    ViewData["MessageSuccess"] = M.Translation.Get("Promjene su spremljene!!");
                }
            }
            userModel.Address = user.Address;
            userModel.City = user.City;
            userModel.Email = user.Email;
            userModel.GenderId = user.GenderId;
            userModel.Mobile = user.Mobile;
            userModel.Name = user.Name;
            userModel.Phone = user.Phone;
            userModel.Surname = user.Surname;
            userModel.DateOfBirth = user.DateOfBirth;


            return View("EditMyProfile", userModel);
        }

        [MyAccountAuthorize]
        public ActionResult MyMessage()
        {
            ViewBag.MyAccountMenu = true;

            return View();
        }

        //Punjenje newslettera za pojedinog usera
        [MyAccountAuthorize]
        public ActionResult Newsletter(FormCollection collection, string button)
        {
            ViewBag.MyAccountMenu = true;
            WebShopDb context = WebShopDb.I;
            Syrilium.Modules.BusinessObjects.NewsletterSubscriber newsletter = context.NewsletterSubscriber.Where(n => n.UserId == SessionState.I.User.Id).FirstOrDefault();
            List<int> checkedItems = new List<int>();
            int languageId = SessionState.I.LanguageId;

            if (button == "submit")
            {
                if (newsletter == null)
                {
                    newsletter = new Syrilium.Modules.BusinessObjects.NewsletterSubscriber();
                    newsletter.UserId = SessionState.I.User.Id;
                    newsletter.Email = SessionState.I.User.Email;
                    context.NewsletterSubscriber.Add(newsletter);
                }

                foreach (var key in collection.Keys)
                {
                    if (key.ToString().StartsWith("ItemDepartment"))
                    {
                        int ItemDepartmentId = int.Parse(key.ToString().Replace("ItemDepartment", ""));
                        if (collection[key.ToString()].Contains("true"))
                        {
                            checkedItems.Add(ItemDepartmentId);
                        }
                    }
                    if (key.ToString().StartsWith("ItemGroup"))
                    {
                        int ItemGroupId = int.Parse(key.ToString().Replace("ItemGroup", ""));
                        if (collection[key.ToString()].Contains("true"))
                        {
                            checkedItems.Add(ItemGroupId);
                        }
                    }
                }

                foreach (NewsletterSubscription ns in newsletter.Subscriptions.Where(s => s.LanguageId == 0 || s.LanguageId == languageId))
                {
                    if (checkedItems.Contains(ns.Id))
                    {
                        checkedItems.Remove(ns.Id);
                    }
                    else
                    {
                        newsletter.Subscriptions.Remove(ns);
                        context.Entry(ns).State = System.Data.EntityState.Deleted;
                    }
                }

                foreach (int itemId in checkedItems)
                {
                    newsletter.Subscriptions.Add(new NewsletterSubscription()
                    {
                        ItemId = itemId,
                        LanguageId = languageId
                    });
                }
                context.SaveChanges();
                ViewData["MessageSuccessTitle"] = "Uspješna promjena newsletter-a!";
                ViewData["MessageSuccess"] = "Promjene su spremljene!";
            }

            if (newsletter != null)
            {
                foreach (NewsletterSubscription ns in newsletter.Subscriptions.Where(s => s.LanguageId == 0 || s.LanguageId == languageId))
                {
                    checkedItems.Add(ns.ItemId);
                }
            }


            WebShop.Models.Newsletter newsletterModel = new Models.Newsletter();
            newsletterModel.chekedItems = checkedItems;

            return View("Newsletter", newsletterModel);

        }

        [RequireHttps]
        public ActionResult ConfirmRegistration(string mcc, string button, string username, string password, bool? cbRememberPassword)
        {
            B.Login login = new B.Login();

            if (!string.IsNullOrEmpty(mcc))
            {
                WebShopDb context = WebShopDb.I;
                login = B.Login.GetLoginByMCC(mcc, context);

                if (login != null)
                {
                    login.MailConfirmationCode = null;
                    context.Login.Add(login);
                    context.Entry(login).State = System.Data.EntityState.Modified;
                    context.SaveChanges();

                    ViewData["MessageSuccessTitle"] = M.Translation.Get("Prijavite se");
                    ViewData["MessageSuccess"] = M.Translation.Get("Registracija je uspješno završena.");
                }
                else
                {
                    ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška pri autorizaciji e-mail adrese");
                    ViewData["MessageBoxError"] = M.Translation.Get("Ukoliko smatrate da ste pravilno pratili upute registracije, molimo Vas da nas kontaktirate putem kontakt obrasca.");
                }

            }
            if (button == "submit")
            {
                M.Login loginModel = new M.Login
                {
                    Username = username,
                    Password = password
                };
                loginModel.Enter(new string[] { PermissionNames.User });

                if (loginModel.LoginSucceeded)
                {
                    ViewData["MessageSuccessTitle"] = M.Translation.Get("Uspješna prijava");
                    ViewData["MessageSuccess"] = M.Translation.Get("Uspješno ste prijavljeni. Možete nastaviti sa radom.");
                }
                else
                {
                    ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška");
                    ViewData["MessageBoxError"] = M.Translation.Get("Unesite ispravno korisničko ime i lozinku!");
                }
            }
            return View();
        }
    }
}
