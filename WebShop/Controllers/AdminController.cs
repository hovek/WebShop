using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;
using System.IO;
using System.Web.Helpers;
using System.Data;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface;
using Syrilium.Modules.BusinessObjects.Item;
using System.Data.Entity;
using Models = WebShop.Models;
using System.Text;
using WebShop.Infrastructure;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Security;
using Syrilium.CommonInterface;
using Syrilium.CommonInterface.Caching;
using System.Drawing;
using D = System.Drawing.Imaging;

namespace WebShop.Controllers
{
    public class AdminController : Controller
    {
        [AdminAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAttempt(string username, string password, string returnUrl)
        {
            Models.Login model = new Models.Login
            {
                Username = username,
                Password = password,
                LoginAttempt = true,
                ReturnUrl = returnUrl
            };

            model.Enter(new string[] { PermissionNames.AdminAccess });
            if (model.LoginSucceeded)
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Login", new { returnUrl = model.ReturnUrl, errorMessage = model.ErrorMessage });
        }

        public ActionResult Login(string returnUrl, string errorMessage)
        {
            if (returnUrl == null) returnUrl = Request.UrlReferrer.AbsoluteUri;
            return View("Login", new Models.Login
            {
                ReturnUrl = returnUrl,
                ErrorMessage = errorMessage
            });
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect(Request.UrlReferrer.ToString());
        }

        [AdminAuthorize]
        [ValidateInput(false)]
        public ActionResult News(int? newsList, string title, string editorText, string introductionText, string showNews, string autor, string autorPicture, DateTime? dateTime, HttpPostedFileBase file, int? idNews, string button)
        {
            WebShopDb context = WebShopDb.I;
            News news;

            if (newsList != null)
            {
                news = Syrilium.Modules.BusinessObjects.News.GetNews(newsList.Value, context);
            }
            else
            {
                news = new News();
                context.News.Add(news);
            }

            if (button == "Delete")
            {
                if (news.Id > 0)
                {
                    context.News.Remove(news);
                    context.SaveChanges();
                    news.Id = 0;
                    ViewData["Message"] = Models.Translation.Get("Vijest je uspješno obrisana");
                }
                else
                {
                    ViewData["Message"] = Models.Translation.Get("Niste odabrali vijest.");
                }
            }
            else if (button == "Save")
            {
                if (idNews.HasValue)
                {

                    //ažuriranje vijesti
                    news.Title = title;
                    news.IntroductionText = introductionText;
                    news.Text = editorText;
                    news.Visible = Convert.ToBoolean(showNews);
                    news.Autor = autor;
                    news.AutorPicture = autorPicture;
                    news.Date = dateTime;


                    if (file != null && file.ContentLength > 0)
                    {
                        var path = Server.MapPath("/Resources/Upload/Images/News/Original/");
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                        Image originalImage = Image.FromStream(file.InputStream);
                        string fileName = CommonHelpers.SaveImage(originalImage, path, file.FileName);

                        news.ImageUrl = fileName;
                    }

                    string message;
                    if (!newsCheckSave(news, out message))
                    {
                        ViewData["Message"] = message;
                    }
                    else
                    {
                        context.SaveChanges();
                        ViewData["Message"] = Models.Translation.Get("Promjene su spremljene!!!");
                    }
                }
                else
                {
                    ViewData["Message"] = Models.Translation.Get("Molimo Vas odaberite vijest");
                }
            }
            else if (button == "New")
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            ViewData["Title"] = news.Title;
            ViewData["IntroductionText"] = news.IntroductionText;
            ViewData["EditorText"] = news.Text;
            ViewData["Autor"] = news.Autor;
            ViewData["AutorPicture"] = news.AutorPicture;
            ViewData["IdNews"] = news.Id;
            ViewData["ImageUrl"] = news.ImageUrl;
            ViewData["ShowNews"] = news.Visible;
            ViewData["DateTime"] = news.Date;


            ModelState.Remove("Day");
            ModelState.Remove("Month");
            ModelState.Remove("Year");
            ModelState.Remove("Title");
            ModelState.Remove("IntroductionText");
            ModelState.Remove("EditorText");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ShowNews");
            ModelState.Remove("DateTime");
            ModelState.Remove("Autor");
            ModelState.Remove("AutorPicture");

            return View();
        }

        private bool newsCheckSave(News news, out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(news.Title))
            {
                message = Models.Translation.Get("Unesite ime vijesti!!!");
                return false;
            }

            return true;
        }

        [AdminAuthorize]
        [ValidateInput(false)]
        public ActionResult Banner(string bannerHtml, string description, string title, int? bannerLocationId, int? bannerId, HttpPostedFileBase file, string button, int? partnerId)
        {

            WebShopDb context = WebShopDb.I;
            Banner banner;
            if (bannerId != null)
            {
                banner = Syrilium.Modules.BusinessObjects.Banner.GetBanner(bannerId, context);
            }
            else
            {
                banner = new Banner();
                context.Banner.Add(banner);
            }

            if (button == "Delete")
            {
                if (banner.Id > 0)
                {
                    context.Banner.Remove(banner);
                    context.SaveChanges();
                    banner.Id = 0;
                    ViewData["Message"] = "Banner je uspješno obrisan";
                }
                else
                {
                    ViewData["Message"] = "Niste odabrali banner.";
                }
            }
            else if (button == "Save")
            {
                if (bannerLocationId.HasValue)
                {

                    //ažuriranje banera
                    banner.BannerLocationId = bannerLocationId.Value;
                    banner.Title = title;
                    banner.Html = bannerHtml;
                    banner.Description = description;
                    if (partnerId != null)
                    {
                        banner.PartnerId = partnerId.Value;
                    }

                    if (file != null && file.ContentLength > 0)
                    {
                        banner.BannerPath = Server.MapPath("/Resources/Upload/Images/Banners/") + Path.GetFileName(file.FileName);
                    }

                    string message;
                    if (!bannerCheckSave(banner, out message))
                    {
                        ViewData["Message"] = message;
                    }
                    else
                    {
                        if (banner.BannerPath != null)
                        {
                            file.SaveAs(banner.BannerPath);
                        }
                        context.SaveChanges();
                        ViewData["Message"] = "Promjene su spremljene!!!";
                    }
                }
                else
                {
                    ViewData["Message"] = "Molimo Vas odaberite grupu bannera";
                }
            }

            //punjenje drop down kontrola
            ViewData["BannerLocationList"] = WebShop.Models.Banner.GetBannerLocationList();
            if (bannerLocationId != null)
            {
                Dictionary<int, string> bannerList = new Dictionary<int, string>();
                foreach (Syrilium.Modules.BusinessObjects.Banner bnr in Syrilium.Modules.BusinessObjects.Banner.GetAllChild(bannerLocationId.Value))
                {
                    bannerList.Add(bnr.Id, bnr.Title);
                }
                string selectedValue = banner.Id == 0 ? "-- popis bannera -- " : banner.Title;
                ViewData["BannerList"] = new SelectList(bannerList, "Key", "Value", selectedValue);
            }

            //popunjavanje ViewData sa banerom

            ViewData["idBanner"] = banner.Id;
            ViewData["Title"] = banner.Title;
            ViewData["Html"] = banner.Html;
            ViewData["Description"] = banner.Description;
            ViewData["PartnerList"] = WebShop.Models.Partner.GetPartnerList();

            if (banner.PartnerId != 0)
            {
                Dictionary<int, string> partnerList = new Dictionary<int, string>();
                Partner partner = new Partner().GetPartner(banner.PartnerId, context);
                partnerList.Add(partner.Id, partner.Name);
                ViewData["PartnerList"] = new SelectList(partnerList, "Key", "Value", partner.Name);
            }


            ModelState.Remove("title");
            ModelState.Remove("bannerHtml");
            ModelState.Remove("description");
            return View("Banner");
        }

        private bool bannerCheckSave(Banner banner, out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(banner.Title))
            {
                message = "Unesite ime bannera!!!";
                return false;
            }

            return true;
        }

        [AdminAuthorize]
        [ValidateInput(false)]
        public ActionResult Partner(string button, int? partnerList, int? idPartner, HttpPostedFileBase fileLogo, HttpPostedFileBase fileBackground, FormCollection form, string name, string city,
            string address, string postalCode, string phone, string email, string Url, string fax, string about, string workDescription, string services, string premium, string advertiser, string htmlBackground, string showPartner)
        {
            WebShopDb context = WebShopDb.I;
            Partner partner;
            if (partnerList != null)
            {
                partner = new Partner().GetPartner(partnerList.Value, context);
            }
            else
            {
                partner = new Partner();
                context.Partner.Add(partner);
            }

            if (button == "Delete")
            {
                if (partner.Id > 0)
                {
                    context.Partner.Remove(partner);
                    context.SaveChanges();
                    partner.Id = 0;
                    ViewData["Message"] = "Partner je uspješno obrisan";
                }
                else
                {
                    ViewData["Message"] = "Niste odabrali partnera.";
                }
            }
            else if (button == "Save")
            {
                if (idPartner.HasValue)
                {

                    //ažuriranje partnera
                    partner.About = about;
                    partner.Address = address;
                    partner.City = city;
                    partner.Email = email;
                    partner.Fax = fax;
                    partner.Name = name;
                    partner.Phone = phone;
                    partner.PostalCode = postalCode;
                    partner.Services = services;
                    partner.URL = Url;
                    partner.WorkDescription = workDescription;
                    partner.Premium = Convert.ToBoolean(premium);
                    partner.Advertiser = Convert.ToBoolean(advertiser);
                    partner.HtmlBackground = htmlBackground;
                    partner.Visible = Convert.ToBoolean(showPartner);

                    string pathLogo = "/Resources/Upload/Images/Partner/Logo/";

                    if (fileLogo != null && fileLogo.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(fileLogo.FileName);
                        var path = Server.MapPath(pathLogo) + fileName;
                        fileLogo.SaveAs(path);

                        partner.Logo = pathLogo + fileLogo.FileName;
                    }

                    string pathBackground = "/Resources/Upload/Images/Partner/Background/";

                    if (fileBackground != null && fileBackground.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(fileBackground.FileName);
                        var path = Server.MapPath(pathBackground) + fileName;
                        fileBackground.SaveAs(path);

                        partner.Background = pathBackground + fileBackground.FileName;
                    }


                    string message;
                    if (!partnerCheckSave(partner, out message))
                    {
                        ViewData["Message"] = message;
                    }
                    else
                    {
                        context.SaveChanges();
                        ViewData["Message"] = "Promjene su spremljene!!!";
                    }
                }
                else
                {
                    ViewData["Message"] = "Molimo Vas odaberite partnera";
                }
            }


            ViewData["idPartner"] = partner.Id;
            ViewData["Name"] = partner.Name;
            ViewData["City"] = partner.City;
            ViewData["Address"] = partner.Address;
            ViewData["PostalCode"] = partner.PostalCode;
            ViewData["Phone"] = partner.Phone;
            ViewData["Logo"] = partner.Logo;
            ViewData["Email"] = partner.Email;
            ViewData["Url"] = partner.URL;
            ViewData["Fax"] = partner.Fax;
            ViewData["About"] = partner.About;
            ViewData["WorkDescription"] = partner.WorkDescription;
            ViewData["Services"] = partner.Services;
            ViewData["Premium"] = partner.Premium;
            ViewData["Advertiser"] = partner.Advertiser;
            ViewData["Background"] = partner.Background;
            ViewData["HtmlBackground"] = partner.HtmlBackground;
            ViewData["showPartner"] = partner.Visible;


            ModelState.Remove("idPartner");
            ModelState.Remove("Name");
            ModelState.Remove("City");
            ModelState.Remove("Address");
            ModelState.Remove("PostalCode");
            ModelState.Remove("Phone");
            ModelState.Remove("Logo");
            ModelState.Remove("Email");
            ModelState.Remove("Url");
            ModelState.Remove("Fax");
            ModelState.Remove("About");
            ModelState.Remove("WorkDescription");
            ModelState.Remove("Services");
            ModelState.Remove("Premium");
            ModelState.Remove("Advertiser");
            ModelState.Remove("Background");
            ModelState.Remove("HtmlBackground");
            ModelState.Remove("showPartner");

            return View();
        }

        private bool partnerCheckSave(Partner partner, out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(partner.Name))
            {
                message = "Unesite ime partnera!!!";
                return false;
            }

            return true;
        }


        [AdminAuthorize]
        public ActionResult LeftMenu()
        {
            return View();
        }

        [AdminAuthorize]
        [ValidateInput(false)]
        public ActionResult Newsletter(string content, string subject, string button, string newslleterMailId, int? newsletterMailingListId)
        {
            WebShopDb context = WebShopDb.I;
            Syrilium.Modules.BusinessObjects.NewsletterMail newsletterMail = new Syrilium.Modules.BusinessObjects.NewsletterMail();
            WebShop.Models.Newsletter newslletterModel = new Models.Newsletter();

            if (newslleterMailId != null)
            {
                newsletterMail = Syrilium.Modules.BusinessObjects.NewsletterMail.Get(Convert.ToInt32(newslleterMailId), context);
                if (newsletterMail != null)
                {
                    newslletterModel.NewsletterMailId = newsletterMail.Id;
                    newslletterModel.Subject = newsletterMail.Subject;
                    newslletterModel.Content = newsletterMail.Content;
                    if (button == "Delete")
                    {
                        context.NewsletterMail.Remove(newsletterMail);
                        context.SaveChanges();
                        ViewData["Message"] = "Newslleter je uspješno obrisan";

                    }
                    if (button == "Send")
                    {
                        return PartialView("NewsletterSend", newslletterModel);
                    }
                    if (button == "SendMail")
                    {
                        //Slanje newslletera
                        if (newsletterMailingListId != null)
                        {
                            List<NewsletterMailingListSubscriber> nmlsl = Syrilium.Modules.BusinessObjects.NewsletterMailingListSubscriber.GetUsersByListId(newsletterMailingListId);

                            subject = newsletterMail.Subject;
                            content = newsletterMail.Content;
                            foreach (NewsletterMailingListSubscriber nmls in nmlsl)
                            {
                                string newsletterMailUserName = ConfigurationManager.AppSettings["NewsletterMailUserName"];

                                Module.I<IMail>().SendMail(
                                     smtpHost: ConfigurationManager.AppSettings["SmtpHost"],
                                     enableSsl: false,
                                     from: ConfigurationManager.AppSettings["NewsletterMailFrom"],
                                     to: nmls.Email,
                                     subject: subject,
                                     isBodyHtml: true,
                                     body: content,
                                     useCredentials: !string.IsNullOrEmpty(newsletterMailUserName),
                                     userName: newsletterMailUserName,
                                     password: ConfigurationManager.AppSettings["NewsletterMailPassword"]);
                            }
                            ViewData["Message"] = "Newsletter je uspješno poslan";
                        }
                        else
                        {
                            ViewData["Message"] = "Odberite listu korisnika kojoj želite poslati mail";
                        }
                    }
                    if (button == "View")
                    {
                        return PartialView("NewsletterView", newslletterModel);
                    }
                }
            }
            if (button == "Save")
            {
                if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(content))
                {
                    //Spremanje poslanog newslletra u bazu 

                    if (Convert.ToInt32(newslleterMailId) == 0)
                    {
                        newsletterMail = new NewsletterMail();
                        context.NewsletterMail.Add(newsletterMail);
                    }
                    else
                    {
                        context.Entry(newsletterMail).State = EntityState.Modified;
                    }
                    newsletterMail.Content = content;
                    newsletterMail.Subject = subject;
                    newsletterMail.DateCreated = DateTime.Now.Date;
                    context.SaveChanges();
                }
                else
                {
                    ViewData["Message"] = "Unesite subject i content kako biste mogli spremiti newslettere";
                }
            }


            newslletterModel.NewsletterMail = WebShopDb.I.NewsletterMail.ToList();

            ModelState.Remove("newslleterMailId");
            ModelState.Remove("subject");
            ModelState.Remove("content");
            return View("Newsletter", newslletterModel);
        }

        [AdminAuthorize]
        public ActionResult NewsletterList(string button, string nameNewsletterList, int? newsletterMailingListId)
        {
            WebShopDb context = WebShopDb.I;
            Syrilium.Modules.BusinessObjects.NewsletterMailingList newsletterMailingList = new Syrilium.Modules.BusinessObjects.NewsletterMailingList();
            WebShop.Models.Newsletter newslletterModel = new Models.Newsletter();

            newslletterModel.NewsletterMailingListSubscriber = WebShopDb.I.NewsletterMailingListSubscriber.ToList();

            if (newsletterMailingListId != null)
            {
                newsletterMailingList = Syrilium.Modules.BusinessObjects.NewsletterMailingList.GetByID(newsletterMailingListId.Value, context);
                List<NewsletterMailingListSubscriber> nmlsl = Syrilium.Modules.BusinessObjects.NewsletterMailingListSubscriber.GetUsersByListId(newsletterMailingListId, context);

                if (!string.IsNullOrEmpty(newsletterMailingList.Name))
                {
                    newslletterModel.NewsletterMailingListName = newsletterMailingList.Name;
                }
                if (button == "Delete")
                {

                    foreach (NewsletterMailingListSubscriber nmls in nmlsl)
                    {
                        context.NewsletterMailingListSubscriber.Remove(nmls);
                    }



                    context.NewsletterMailingList.Remove(newsletterMailingList);
                    context.SaveChanges();
                    ViewData["Message"] = "Lista je uspješno obrisana";

                    return Redirect(Request.UrlReferrer.ToString());

                }
            }

            if (button == "Save")
            {
                if (Convert.ToInt32(newsletterMailingListId) == 0)
                {
                    //Spremanje imena liste u bazu
                    newsletterMailingList = new NewsletterMailingList();
                    context.NewsletterMailingList.Add(newsletterMailingList);
                }
                else
                {
                    context.Entry(newsletterMailingList).State = EntityState.Modified;
                }
                newsletterMailingList.Name = nameNewsletterList;
                context.SaveChanges();
            }

            ModelState.Remove("nameNewsletterList");
            return View(newslletterModel);
        }

        [AdminAuthorize]
        public ActionResult NewsletterMailingListSubscriber(string button, string emailSubscriber, int? newsletterListId)
        {
            WebShopDb context = WebShopDb.I;
            Syrilium.Modules.BusinessObjects.NewsletterMailingListSubscriber nmls = new Syrilium.Modules.BusinessObjects.NewsletterMailingListSubscriber();
            WebShop.Models.Newsletter newslletterModel = new Models.Newsletter();



            if (button == "Save")
            {
                if (String.IsNullOrEmpty(emailSubscriber))
                {
                    ViewData["Message"] = "Unesite Email";

                }
                bool IsValidEmail = Module.I<IMail>().IsEmailValid(emailSubscriber);
                if (IsValidEmail == false && !String.IsNullOrEmpty(emailSubscriber))
                {
                    ViewData["Message"] = "Unesite ispravan Email";
                }
                if (IsValidEmail == true)
                {

                    if (!string.IsNullOrEmpty(emailSubscriber) && newsletterListId.HasValue)
                    {
                        //Spremanje emaila u određenu listu
                        nmls = new NewsletterMailingListSubscriber();
                        context.NewsletterMailingListSubscriber.Add(nmls);
                        nmls.Email = emailSubscriber;
                        nmls.NewsletterMailingListId = newsletterListId;
                        context.SaveChanges();
                    }
                    else
                    {
                        ViewData["Message"] = "Odaberite ime liste u koju želite spremiti mail";
                    }
                }
            }

            newslletterModel.NewsletterMailingListSubscriber = WebShopDb.I.NewsletterMailingListSubscriber.ToList();
            return View("NewsletterList", newslletterModel);
        }

        #region ITEM
        [AdminAuthorize]
        public ActionResult Item(int? id = null)
        {
            Models.Item itemModel = new Models.Item
            {
                GroupId = id
            };

            itemModel.GroupTreeViewModel = new Models.GroupTreeView()
            {
                SelectedGroupId = id,
            };

            itemModel.ProductAdminListViewModel = new Models.ProductAdminListView()
            {
                GroupId = id,
            };

            if (id != null)
            {
                itemModel.AttributeTemplateTreeViewModel = new Models.AttributeTemplateTreeView()
                {
                    GroupId = id.Value
                };
            }

            return View("Item", itemModel);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult DeleteGroup(int groupId, Models.GroupTreeView model)
        {
            WebShopDb.I.Database.ExecuteSqlCommand("delete from Item where Id={0}", groupId);
            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            cache.AppendClearBuffer(Module.I<IGroup>().Get(null, false, cache));
            cache.Clear();

            if (model.SelectedGroupId == groupId)
            {
                return RedirectToAction("Item");
            }

            return PartialView("GroupTreeView", model);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult EditGroup(string action, int? groupId, Models.GroupTreeView model, string name, bool show)
        {
            WebShopDb context = new WebShopDb();

            IItem group;
            if (action == "add")
            {
                group = Module.I<IItem>();
                context.Item.Add((Item)group.RawItem);
                group.ParentId = groupId;
                group.TypeId = groupId.HasValue ? ItemTypeEnum.Group : ItemTypeEnum.Department;
            }
            else
            {
                group = Module.I<IItem>().Get(groupId.Value);
                Module.I<S.IItemManager>().AssociateWithDbContext((DbContext)context, group.RawItem);
            }

            IAttributeLocator al = Module.I<IAttributeLocator>();
            al.Cache = Module.I<ICache>(CacheNames.AdminCache);
            int languageId = SessionState.I.LanguageId;

            IItemValue<string> stringValue;
            IItemAttribute attNaziv = group.Attributes[AttributeKeyEnum.Name];
            if (attNaziv == null)
            {
                attNaziv = Module.I<IItemAttribute>();
                attNaziv.Attribute = al.Find(a => a.Key == AttributeKeyEnum.Name);
                attNaziv.AttributeId = attNaziv.Attribute.Id;
                stringValue = Module.I<IItemValue<string>>();
                stringValue.LanguageId = languageId;
                attNaziv.StringValues.Add(stringValue);
                group.Attributes.Add(attNaziv);
            }
            else
            {
                stringValue = attNaziv.StringValues.Find(v => v.LanguageId == 0 || v.LanguageId == languageId);
            }
            stringValue.Value = name;

            IItemValue<int> intValue;
            IItemAttribute attShow = group.Attributes[AttributeKeyEnum.Show];
            if (attShow == null)
            {
                attShow = Module.I<IItemAttribute>();
                attShow.Attribute = al.Find(a => a.Key == AttributeKeyEnum.Show);
                attShow.AttributeId = attShow.Attribute.Id;
                intValue = Module.I<IItemValue<int>>();
                intValue.LanguageId = languageId;
                attShow.IntValues.Add(intValue);
                group.Attributes.Add(attShow);
            }
            else
            {
                intValue = attShow.IntValues.Find(v => v.LanguageId == 0 || v.LanguageId == languageId);
            }
            intValue.Value = Convert.ToInt32(show);

            IItemAttribute attTemplate = group.Attributes[AttributeKeyEnum.Template];
            if (attTemplate == null)
            {
                attTemplate = Module.I<IItemAttribute>();
                attTemplate.Attribute = al.Find(a => a.Key == AttributeKeyEnum.Template);
                attTemplate.AttributeId = attTemplate.Attribute.Id;
                group.Attributes.Add(attTemplate);
            }

            context.SaveChanges();

            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            cache.AppendClearBuffer(Module.I<IGroup>().Get(null, false, cache));
            cache.Clear();

            return PartialView("GroupTreeView", model);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult OrderGroup(string orderData)
        {
            Models.OrderGroup model = new Models.OrderGroup
            {
                OrderData = orderData
            };
            model.Save();

            return new TextActionResult();
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult GetGroupEditView(Models.GroupEdit model)
        {
            return PartialView("GroupEdit", model);
        }

        #region NEWITEM
        [AdminAuthorize]
        [HttpGet]
        public ActionResult Product(int? gid, int? pid, string command = null)
        {
            if (command == "cancel")
            {
                return RedirectToAction("Item", "Admin", new { id = gid });
            }

            Models.Product productModel = new Models.Product
            {
                GroupId = gid,
                ProductId = pid
            };

            return View(productModel);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult SaveProduct(int gid, int? pid)
        {
            Models.Product productModel = new Models.Product
            {
                GroupId = gid,
                ProductId = pid
            };

            Dictionary<int, List<dynamic>> newValues = convertProductFormCollectionToDictionary(Request.Form.Count > 0 ? Request.Form : Request.QueryString, productModel.AttributeTemplate.GetAllAttributes());
            saveProduct(productModel, newValues, SessionState.I.LanguageId);
            productModel.ProductId = null;
            productModel.ProductObject = null;
            return RedirectToAction("Product", "Admin", new { gid = gid });
        }

        [AdminAuthorize]
        public ActionResult DeleteProduct(int groupId, int productId)
        {
            IItem product = Module.I<IItem>().Get(productId);

            if (product != null)
            {
                WebShopDb.I.Database.ExecuteSqlCommand("delete from Item where Id={0}", product.Id);
                foreach (IItemAttribute attr in product.Attributes)
                {
                    if (attr.Attribute.DataType == AttributeDataTypeEnum.Image)
                    {
                        foreach (IItemValue<string> itemValue in attr.StringValues)
                        {
                            deleteImage(itemValue.Value);
                        }
                    }
                }
            }

            return PartialView("ProductAdminListView", new Models.ProductAdminListView
            {
                GroupId = groupId
            });
        }

        private void deleteImage(string imageName)
        {
            string serverProductPath = Server.MapPath("~/Resources/Upload/Images/Product");
            string filePath = Path.Combine(serverProductPath, "Original", imageName);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            foreach (ImageFormat f in Module.I<ICache>(CacheNames.AdminCache).I<ImageFormat>().Get("Product"))
            {
                filePath = Path.Combine(serverProductPath, f.Name, imageName);
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }
        }

        private void saveProduct(Models.Product productModel, Dictionary<int, List<dynamic>> newValues, int languageId)
        {
            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            WebShopDb context = new WebShopDb();
            IAttributeLocator attributeLocator = Module.I<IAttributeLocator>();
            attributeLocator.Cache = cache;

            IArchiveItemChange archiveItemChange = Module.I<IArchiveItemChange>();
            IItem product;
            bool newProduct;

            if (productModel.ProductObject == null)
            {
                newProduct = true;
                product = Module.I<IItem>();
                product.Cache = cache;
                product.TypeId = ItemTypeEnum.Product;
                product.ParentId = productModel.GroupId;
                createAndFillStaticAttributes(product, attributeLocator);
                context.Item.Add((Item)product.RawItem);
            }
            else
            {
                newProduct = false;
                product = productModel.ProductObject;

                archiveItemChange.SetPreviousItem(product.Id);
                archiveItemChange.CurrentItem = product;

                Module.I<S.IItemManager>().AssociateWithDbContext(context, product.RawItem);
                updateStaticAttributes(context, product, attributeLocator);
            }

            List<int> allTemplateAttributeDefinitions = new List<int>();
            foreach (var attrTemplate in productModel.AttributeTemplate.GetAllAttributes())
            {
                allTemplateAttributeDefinitions.AddRange(
                    attrTemplate.Attribute.GetAllAttributes()
                        .Where(a => a.DataType != AttributeDataTypeEnum.Coordinates)
                        .Select(a => a.Id));
            }
            List<int> allTemplateAndStaticAttributes = new List<int>(allTemplateAttributeDefinitions);
            List<int> staticAttributes = getProductStaticAttributes();
            allTemplateAndStaticAttributes.AddRange(staticAttributes);

            List<int> deletedAttributes = new List<int>();

            //brisanje atributa koji su na produktu a nisu na templateu
            for (int i = 0; i < product.Attributes.Count; i++)
            {
                int attributeId = product.Attributes[i].Attribute.Id;
                if (allTemplateAndStaticAttributes.Find(a => a == attributeId) == 0)
                {
                    context.Database.ExecuteSqlCommand("delete from Item where Id={0}", product.Attributes[i].RawItem.Id);
                    deletedAttributes.Add(product.Attributes[i].AttributeId);
                }
            }

            List<string> imagesForDelete = new List<string>();
            //brišemo stare vrijednosti atributa za trenutni i nepostavljeni jezik
            foreach (IItemAttribute itemAttr in product.Attributes)
            {
                if (staticAttributes.Contains(itemAttr.Attribute.Id) || deletedAttributes.Contains(itemAttr.AttributeId)) continue;

                deleteItemvalues<IItemValue<decimal>, decimal>(itemAttr.DecimalValues, context, 0, languageId);
                deleteItemvalues<IItemValue<int>, int>(itemAttr.IntValues, context, 0, languageId);
                deleteItemvalues<IItemValue<DateTime>, DateTime>(itemAttr.DateTimeValues, context, 0, languageId);
                //prvo punimo listu slika za brisanje sa svim slikama
                if (itemAttr.Attribute.DataType == AttributeDataTypeEnum.Image)
                {
                    itemAttr.StringValues.ForEach(v =>
                    {
                        if (v.LanguageId == languageId || v.LanguageId == 0)
                        {
                            imagesForDelete.Add(v.Value);
                        }
                    });
                }
                deleteItemvalues<IItemValue<string>, string>(itemAttr.StringValues, context, 0, languageId);
            }

            //dodamo atribute koji ne postoje na produktu a postoje u templateu
            foreach (int attributeId in allTemplateAttributeDefinitions)
            {
                IItemAttribute attribute = product.GetItemAttribute(attributeId);
                if (attribute == null)
                {
                    attribute = Module.I<IItemAttribute>();
                    attribute.Attribute = attributeLocator.Find(a => a.Id == attributeId);
                    attribute.AttributeId = attributeId;
                    product.Attributes.Add(attribute);
                }
            }

            //jezici za koje treba unijeti vrijednosti
            List<int> languages = !newProduct ? new List<int>() { languageId } : cache.I<Language>().Get().Select(l => l.Id).ToList();

            //dodajemo nove vrijednosti atributa
            foreach (KeyValuePair<int, List<dynamic>> fcAttr in newValues)
            {
                IItemAttribute itemAttribute = product.GetItemAttribute(fcAttr.Key);

                foreach (dynamic value in fcAttr.Value)
                {
                    if (itemAttribute.Attribute.DataType == AttributeDataTypeEnum.Image)
                    {
                        IItemValue<string> itemValue = Module.I<IItemValue<string>>();
                        itemAttribute.StringValues.Add(itemValue);
                        itemValue.Value = moveImageToPermanentFolder(value);
                        //zatim izbacujemo slike iz liste za brisanje koje su odabrane
                        imagesForDelete.Remove(value);
                        itemValue.LanguageId = 0;
                    }
                    else
                    {
                        addValuesToItemAttribute(itemAttribute, value, languages);
                    }
                }
            }

            imagesForDelete.ForEach(i => deleteImage(i));
            context.SaveChanges();

            if (archiveItemChange.IsItemInNewsletterSubscription)
            {
                archiveItemChange.DeletedAttributes.AddRange(deletedAttributes);
                archiveItemChange.CompareAndSave();
            }
        }

        private void addValuesToItemAttribute(IItemAttribute itemAttribute, dynamic value, IEnumerable<int> languages)
        {
            if (value is string)
            {
                foreach (var langId in languages)
                {
                    IItemValue<string> itemValue = Module.I<IItemValue<string>>();
                    itemValue.LanguageId = langId;
                    itemValue.Value = value;
                    itemAttribute.StringValues.Add(itemValue);
                }
            }
            else if (value is decimal)
            {
                foreach (var langId in languages)
                {
                    IItemValue<decimal> itemValue = Module.I<IItemValue<decimal>>();
                    itemValue.LanguageId = langId;
                    itemValue.Value = value;
                    itemAttribute.DecimalValues.Add(itemValue);
                }
            }
            else if (value is int)
            {
                foreach (var langId in languages)
                {
                    IItemValue<int> itemValue = Module.I<IItemValue<int>>();
                    itemValue.LanguageId = langId;
                    itemValue.Value = value;
                    itemAttribute.IntValues.Add(itemValue);
                }
            }
            else if (value is DateTime)
            {
                foreach (var langId in languages)
                {
                    IItemValue<DateTime> itemValue = Module.I<IItemValue<DateTime>>();
                    itemValue.LanguageId = langId;
                    itemValue.Value = value;
                    itemAttribute.DateTimeValues.Add(itemValue);
                }
            }
            else
            {
                throw new NotImplementedException("Item data type not implemented.");
            }
        }

        private List<int> getProductStaticAttributes()
        {
            IAttributeLocator al = Module.I<IAttributeLocator>();
            al.Cache = Module.I<ICache>(CacheNames.AdminCache);
            List<int> attrs = new List<int>();
            attrs.Add(al[AttributeKeyEnum.DateTimeChange].Value);
            attrs.Add(al[AttributeKeyEnum.DateTimeEntry].Value);
            attrs.Add(al[AttributeKeyEnum.PartnerChangedBy].Value);
            attrs.Add(al[AttributeKeyEnum.UserChangedBy].Value);
            return attrs;
        }

        private void createAndFillStaticAttributes(IItem product, IAttributeLocator attributeLocator)
        {
            product.Attributes.Add(getNewItemAttribute(attributeLocator, AttributeKeyEnum.DateTimeEntry, DateTime.Now));
            product.Attributes.Add(getNewItemAttribute(attributeLocator, AttributeKeyEnum.DateTimeChange, DateTime.Now));

            if (SessionState.I.Partner != null)
                product.Attributes.Add(getNewItemAttribute(attributeLocator, AttributeKeyEnum.PartnerChangedBy, SessionState.I.Partner.Id));
            if (SessionState.I.User != null)
                product.Attributes.Add(getNewItemAttribute(attributeLocator, AttributeKeyEnum.UserChangedBy, SessionState.I.User.Id));
        }

        private IItemAttribute getNewItemAttribute(IAttributeLocator attributeLocator, AttributeKeyEnum attributeKey, dynamic value)
        {
            int attributeId = attributeLocator[attributeKey].Value;
            IItemAttribute attribute = Module.I<IItemAttribute>();
            attribute.Attribute = attributeLocator.Find(a => a.Id == attributeId);
            attribute.AttributeId = attributeId;
            if (value is int)
            {
                IItemValue<int> itemValue = Module.I<IItemValue<int>>();
                attribute.IntValues.Add(itemValue);
                itemValue.Value = value;
            }
            else if (value is decimal)
            {
                IItemValue<decimal> itemValue = Module.I<IItemValue<decimal>>();
                attribute.DecimalValues.Add(itemValue);
                itemValue.Value = value;
            }
            else if (value is DateTime)
            {
                IItemValue<DateTime> itemValue = Module.I<IItemValue<DateTime>>();
                attribute.DateTimeValues.Add(itemValue);
                itemValue.Value = value;
            }
            else if (value is string)
            {
                IItemValue<string> itemValue = Module.I<IItemValue<string>>();
                attribute.StringValues.Add(itemValue);
                itemValue.Value = value;
            }
            return attribute;
        }

        private void updateStaticAttributes(DbContext context, IItem product, IAttributeLocator attributeLocator)
        {
            //DateTimeChange
            int dateTimeChangeAttributeId = attributeLocator[AttributeKeyEnum.DateTimeChange].Value;
            IItemAttribute itemAttribute = product.GetItemAttribute(dateTimeChangeAttributeId);
            if (itemAttribute == null)
                product.Attributes.Add(getNewItemAttribute(attributeLocator, AttributeKeyEnum.DateTimeChange, DateTime.Now));
            else
            {
                itemAttribute.DateTimeValues[0].Value = DateTime.Now;
            }

            Type itemType = typeof(Item);

            //PartnerChangedBy
            int partnerChangedByAttributeId = attributeLocator[AttributeKeyEnum.PartnerChangedBy].Value;
            itemAttribute = product.GetItemAttribute(partnerChangedByAttributeId);
            if (itemAttribute == null)
            {
                if (SessionState.I.Partner != null)
                    product.Attributes.Add(getNewItemAttribute(attributeLocator, AttributeKeyEnum.PartnerChangedBy, SessionState.I.Partner.Id));
            }
            else
            {
                if (SessionState.I.Partner != null)
                    itemAttribute.IntValues[0].Value = SessionState.I.Partner.Id;
                else
                    context.Set(itemType).Remove(itemAttribute.RawItem);
            }

            //PartnerChangedBy
            int userChangedByAttributeId = attributeLocator[AttributeKeyEnum.UserChangedBy].Value;
            itemAttribute = product.GetItemAttribute(userChangedByAttributeId);
            if (itemAttribute == null)
            {
                if (SessionState.I.User != null)
                    product.Attributes.Add(getNewItemAttribute(attributeLocator, AttributeKeyEnum.UserChangedBy, SessionState.I.User.Id));
            }
            else
            {
                if (SessionState.I.User != null)
                    itemAttribute.IntValues[0].Value = SessionState.I.User.Id;
                else
                    context.Set(itemType).Remove(itemAttribute.RawItem);
            }
        }

        private string moveImageToPermanentFolder(string originalFileName)
        {
            string serverTempPath = Server.MapPath("~/Resources/Upload/Temp/Product");
            if (!System.IO.File.Exists(Path.Combine(serverTempPath, "Original", originalFileName)))
                return originalFileName;

            string newFileName = originalFileName;
            int lastDotIndex = newFileName.LastIndexOf('.');
            if (lastDotIndex == -1) lastDotIndex = newFileName.Length - 1;
            var fileName = newFileName.Substring(0, lastDotIndex);
            var extension = newFileName.Substring(lastDotIndex + 1, newFileName.Length - lastDotIndex - 1);
            string serverOriginalPath = Server.MapPath("~/Resources/Upload/Images/Product/Original");
            var path = Path.Combine(serverOriginalPath, newFileName);
            int i = 1;
            while (System.IO.File.Exists(path))
            {
                newFileName = string.Concat(fileName, i, ".", extension);
                path = Path.Combine(serverOriginalPath, newFileName);
                i++;
            }

            string serverProductPath = Server.MapPath("~/Resources/Upload/Images/Product");
            string pathTo = Path.Combine(serverProductPath, "Original");
            if (!Directory.Exists(pathTo))
                Directory.CreateDirectory(pathTo);
            System.IO.File.Move(Path.Combine(serverTempPath, "Original", originalFileName), Path.Combine(pathTo, newFileName));

            foreach (ImageFormat f in Module.I<ICache>(CacheNames.AdminCache).I<ImageFormat>().Get("Product"))
            {
                pathTo = Path.Combine(serverProductPath, f.Name);
                if (!Directory.Exists(pathTo))
                    Directory.CreateDirectory(pathTo);
                System.IO.File.Move(Path.Combine(serverTempPath, f.Name, originalFileName), Path.Combine(pathTo, newFileName));
            }

            return newFileName;
        }

        private void deleteItemvalues<TItemValue, TValue>(IList<TItemValue> itemValues, DbContext context, params int[] languageIds) where TItemValue : IItemValue<TValue>
        {
            itemValues.ToList().ForEach(i =>
                            {
                                if (languageIds.Contains(i.LanguageId))
                                {
                                    itemValues.Remove(i);
                                    context.Entry(i.RawItem).State = EntityState.Deleted;
                                }
                            });
        }

        /// <summary>
        /// Dictionary<AttributeId, List<Value>>
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        private Dictionary<int, List<dynamic>> convertProductFormCollectionToDictionary(NameValueCollection fc, List<IAttributeTemplateAttribute> allTemplateAttributes)
        {
            Dictionary<int, List<dynamic>> fcDict = new Dictionary<int, List<dynamic>>();
            IValueConverter valueConverter = Module.I<IValueConverter>();

            foreach (string key in fc.AllKeys)
            {
                int attributeId;
                if (int.TryParse(key, out attributeId))
                {
                    if (!fcDict.ContainsKey(attributeId)) fcDict[attributeId] = new List<dynamic>();

                    IAttributeDefinition attrDefinition = null;
                    foreach (var attr in allTemplateAttributes)
                    {
                        attrDefinition = attr.Attribute.Find(a => a.Id == attributeId);
                        if (attrDefinition != null) break;
                    }

                    List<dynamic> values = fcDict[attributeId];
                    foreach (string value in fc.GetValues(key))
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            values.Add(valueConverter.Convert(attrDefinition.DataType, value, true));
                        }
                    }
                }
            }

            return fcDict;
        }
        #endregion

        #region TEMPLATE
        [AdminAuthorize]
        [HttpPost]
        public ActionResult AddTemplate(Models.AddTemplate model)
        {
            model.Save();
            return PartialView("AttributeTemplateTreeView", new Models.AttributeTemplateTreeView { GroupId = model.GroupId });
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult DeleteTemplate(int groupId, int templateId)
        {
            IItem group = Module.I<IItem>().GetChild(groupId, Module.I<IGroup>().Get(null, false, Module.I<ICache>(CacheNames.AdminCache)));
            //postavljamo na null jer kod associacije geta sve pa i parente
            group.RawItem.Parent = null;
            WebShopDb context = WebShopDb.I;
            Module.I<S.IItemManager>().AssociateWithDbContext(context, group.RawItem);
            IItemAttribute attribute = group.Attributes[AttributeKeyEnum.Template];

            List<IItemValue<int>> valuesForDelete = attribute.IntValues.ToList();
            attribute.IntValues.RemoveAll();
            foreach (var i in valuesForDelete)
            {
                context.Entry(i.RawItem).State = EntityState.Deleted;
            }

            context.SaveChanges();
            context.Database.ExecuteSqlCommand("delete from ItemDefinition where Id={0}", templateId);

            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            cache.AppendClearBuffer(typeof(IAttributeTemplate));
            cache.AppendClearBuffer(Module.I<IGroup>().Get(null, false, cache));
            cache.Clear();

            return PartialView("AttributeTemplateTreeView", new Models.AttributeTemplateTreeView { GroupId = groupId });
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult DeleteTemplateElement(int groupId, int templateElementId)
        {
            WebShopDb.I.Database.ExecuteSqlCommand("delete from ItemDefinition where Id={0}", templateElementId);
            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            cache.AppendClearBuffer(typeof(IAttributeTemplate));
            cache.Clear();

            return PartialView("AttributeTemplateTreeView", new Models.AttributeTemplateTreeView { GroupId = groupId });
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult AttributeTemplateConstraintEdit(int templateId, int templateElementId, int attributeDefinitionId, int languageId)
        {
            Models.AttributeTemplateConstraintEdit model = new Models.AttributeTemplateConstraintEdit
                {
                    LanguageId = languageId,
                    TemplateId = templateId,
                    TemplateElementId = templateElementId,
                    AttributeDefinitionId = attributeDefinitionId
                };
            return PartialView("AttributeTemplateConstraintEdit", model);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult AttributeTemplateEdit(Models.AttributeTemplateEdit model)
        {
            return PartialView(model);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult EditAttributeTemplateGroup(Models.EditAttributeTemplateGroup model)
        {
            model.Save();
            return PartialView("AttributeTemplateTreeView", new Models.AttributeTemplateTreeView { GroupId = model.GroupId });
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult EditAttributeTemplateAttribute(Models.EditAttributeTemplateAttribute model, FormCollection fc)
        {
            model.QueryString = fc;
            model.Save();
            return PartialView("AttributeTemplateTreeView", new Models.AttributeTemplateTreeView { GroupId = model.GroupId });
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult OrderAttributeTemplate(string orderData, int templateId)
        {
            Models.OrderAttributeTemplate model = new Models.OrderAttributeTemplate
            {
                OrderData = orderData,
                TemplateId = templateId
            };
            model.Save();
            return new TextActionResult();
        }
        #endregion

        #region ATTRIBUTE
        [AdminAuthorize]
        [HttpPost]
        public ActionResult DeleteItemAttribute(int attributeDefinitionId)
        {
            Models.ItemAttribute model = new Models.ItemAttribute();
            string error;
            model.Delete(attributeDefinitionId, out error);

            if (error != null)
            {
                return new TextActionResult(error);
            }

            return PartialView("ItemAttribute", model);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult ItemAttributeEditView(int? attributeDefinitionId)
        {
            Models.ItemAttributeEditView model = new Models.ItemAttributeEditView
            {
                AttributeDefinitionId = attributeDefinitionId
            };
            return PartialView("ItemAttributeEditView", model);
        }

        [AdminAuthorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditItemAttribute(int? attributeDefinitionId, string name, string display, string format, AttributeDataTypeEnum dataType,
            AttributeDataSystemListReferenceEnum? systemListReferenceType, string latDisplay, string lngDisplay)
        {
            Models.EditItemAttribute editItemAttribute = new Models.EditItemAttribute
            {
                AttributeDefinitionId = attributeDefinitionId,
                DataType = dataType,
                Display = display,
                Format = format,
                LatDisplay = latDisplay,
                LngDisplay = lngDisplay,
                Name = name,
                SystemListReferenceType = systemListReferenceType
            };
            editItemAttribute.Save();
            return PartialView("ItemAttribute", new Models.ItemAttribute());
        }
        #endregion
        #endregion

        [AdminAuthorize]
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                string serverPath = Server.MapPath("~/Resources/Upload/Temp/Product/Original");
                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                string fileName = saveProductTempImages(file.InputStream, file.FileName);

                return new TextActionResult
                {
                    Text = fileName
                };
            }

            return new TextActionResult
            {
                Text = ""
            };
        }

        private string saveProductTempImages(Stream imageStream, string fileName)
        {
            IImageHelper imageHelper = Module.I<IImageHelper>();

            string serverPath = Server.MapPath("~/Resources/Upload/Temp/Product/");

            Image originalImage = Image.FromStream(imageStream);
            fileName = CommonHelpers.SaveImage(originalImage, Path.Combine(serverPath, "Original"), fileName);

            D.ImageFormat imageFormat = Module.I<IImageHelper>().GetImageFormatByFileExtension(fileName);
            foreach (ImageFormat i in Module.I<ICache>(CacheNames.AdminCache).I<ImageFormat>().Get("Product"))
            {
                Image resizedImage = imageHelper.Resize(imageStream, i.Width, i.Height);
                string path = string.Concat(serverPath, i.Name, "\\");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                resizedImage.Save(string.Concat(path, fileName), imageFormat);
            }

            return fileName;
        }

        [AdminAuthorize]
        [ValidateInput(false)]
        public ActionResult Page(string button, string name, string content, int? languageId, int? pageId, int? pageLocationId, string staticPage, string showPage)
        {
            WebShopDb context = WebShopDb.I;
            Page page;
            PageTranslation pageTranslation = null;
            if (pageId != null)
            {
                page = Syrilium.Modules.BusinessObjects.Page.GetPage(pageId.Value, context);
                if (languageId != null)
                {
                    pageTranslation = page.GetTranslation(languageId.Value);
                }
            }
            else
            {
                page = new Page() { PagesLocationId = pageLocationId ?? 0 };
                context.Page.Add(page);
            }
            if (pageTranslation == null)
            {
                pageTranslation = new PageTranslation() { LanguageId = languageId ?? 0 };
                page.Translation.Add(pageTranslation);
            }
            else
            {
                context.Entry(pageTranslation).State = EntityState.Modified;
            }

            if (button != null)
            {
                if (languageId == null)
                {
                    ViewData["Message"] = "Potrebno je odabrati jezik.";
                }
                else if (pageLocationId == null)
                {
                    ViewData["Message"] = "Potrebno je odabrati grupu.";
                }
                else if (button == "Delete")
                {
                    ViewData["Message"] = "Stranica je uspješno obrisana";
                    if (page.Id > 0)
                    {
                        context.Page.Remove(page);
                        context.SaveChanges();
                        page.Id = 0;
                    }
                    else
                    {
                        ViewData["Message"] = "Niste odabrali stranicu.";
                    }
                }
                else if (button == "Save")
                {
                    pageTranslation.Name = name;
                    pageTranslation.Content = content;
                    page.Static = Convert.ToBoolean(staticPage);
                    page.Visible = Convert.ToBoolean(showPage);
                    context.SaveChanges();
                    ViewData["Message"] = "Promjene su spremljene!!!";
                }
            }

            ViewData["languageList"] = WebShop.Models.Language.GetLanguageList();
            ViewData["pageLocationList"] = WebShop.Models.Pages.GetPageLocationList();
            if (languageId != null && pageLocationId != null)
            {
                Dictionary<int, string> listPages = new Dictionary<int, string>();
                foreach (Syrilium.Modules.BusinessObjects.Page pg in Syrilium.Modules.BusinessObjects.Page.GetAllPages(pageLocationId.Value))
                {
                    string nameTraslated = pg.GetTranslation(p => p.Name, languageId);
                    if (string.IsNullOrWhiteSpace(nameTraslated))
                    {
                        foreach (PageTranslation pt in pg.Translation)
                        {
                            if (!string.IsNullOrWhiteSpace(pt.Name))
                            {
                                nameTraslated = pt.Name;
                                break;
                            }
                        }
                    }
                    listPages.Add(pg.Id, nameTraslated);
                }

                string selectedValue = page.Id == 0 ? "-- popis stranica -- " : pageTranslation.Name;
                ViewData["PageList"] = new SelectList(listPages, "Key", "Value", selectedValue);
            }

            ViewData["languageId"] = languageId;
            ViewData["pageId"] = page.Id;
            ViewData["name"] = pageTranslation.Name;
            ViewData["content"] = pageTranslation.Content;
            ViewData["staticPage"] = page.Static;
            ViewData["showPage"] = page.Visible;

            ModelState.Remove("pageId");
            ModelState.Remove("name");
            ModelState.Remove("content");
            ModelState.Remove("staticPage");
            ModelState.Remove("showPage");

            return View("Page");
        }

        private bool pagesCheckSave(PageTranslation pageTranslation, out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(pageTranslation.Name))
            {
                message = "Unesite ime stranice!!!";
                return false;
            }

            return true;
        }

        [AdminAuthorize]
        public ActionResult Translation(int? languageId, string valueSearch)
        {
            Models.Translation model = new Models.Translation();
            model.Translations = Syrilium.Modules.BusinessObjects.Translation.Search(valueSearch, languageId);

            return View("Translation", model);
        }

        [AdminAuthorize]
        public ActionResult EditTranslation(string keyLanguage, string text)
        {
            string[] parts = keyLanguage.Split(new char[] { '_' });
            string key = parts[0];
            int languageId = int.Parse(parts[1]);

            WebShopDb context = new WebShopDb();
            Translation translation = context.Translation.Where(t => t.Key == key && t.LanguageId == languageId).FirstOrDefault();
            if (translation == null)
            {
                translation = new Translation();
                translation.Key = key;
                translation.LanguageId = languageId;
                context.Translation.Add(translation);
            }

            translation.Value = text.Replace("\\r\\n", "\r\n");
            context.SaveChanges();

            ICache cache = Module.I<ICache>(CacheNames.AdminCache);
            cache.AppendClearBuffer(typeof(Translation));
            cache.Clear();

            return new TextActionResult();
        }

        [AdminAuthorize]
        public ActionResult Cache(string clearType)
        {
            Models.Cache cache = new Models.Cache
            {
                ClearType = clearType
            };
            cache.Clear();
            return View(cache);
        }

        [HttpGet]
        [AdminAuthorize]
        public ActionResult Instruction()
        {
            return View(new WebShop.Models.Instruction());
        }

        [HttpPost]
        [AdminAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Instruction(WebShop.Models.Instruction model)
        {
            model.Execute();
            return View(model);
        }
    }

    public class TextActionResult : ActionResult
    {
        private string text = "";
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        public TextActionResult()
        {
        }

        public TextActionResult(string text)
        {
            Text = text;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.Write(Text);
        }


    }
}
