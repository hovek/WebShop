using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using Syrilium.Modules.BusinessObjects;
using WebShop.Infrastructure;
using M = WebShop.Models;
using System.Configuration;
using S = Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface;
using Syrilium.CommonInterface.Caching;
using WebShop.BusinessObjectsInterface;

namespace WebShop.Controllers
{
    public class SubjectController : Controller
    {
        //
        // GET: /Subject/

        public ActionResult Index(int pid)
        {
            ViewData["LoginPopUp"] = false;
            if (SessionState.I.Login == null)
            {
                ViewData["LoginPopUp"] = true;
            }

            Subject sb = new Subject();
            sb.ProductId = pid;
            sb.NewsletterCheck.ItemId = pid;
            sb.Descriptions.Product = sb.Product;
            sb.PriceBox.Product = sb.Product;
            sb.Slider.Product = sb.Product;
            sb.CountDown.Product = sb.Product;
            sb.GoogleMap.Product = sb.Product;
            sb.ContactPartner.Product = sb.Product;
            sb.SliderLogo.Product = sb.Product;

            Header header = new Header
            {
                Product = sb.Product
            };
            ViewData["HeaderModel"] = header;
            ViewData["BackgroundModel"] = new Background
            {
                Product = sb.Product
            };
            ViewData["SliderLogo"] = new SliderLogo
            {
                Product = sb.Product
            };
            ViewData["BannerModel"] = header.Banner;

            ViewData["RepeaterItemsModel"] = new RepeaterItems
            {
                PartnerId = sb.Product.GetRawValue(AttributeKeyEnum.Partner)
            };

            return View(sb);
        }

        [HttpPost]
        [MyAccountAuthorize]
        public ActionResult Grade(int pid, int? score)
        {
            lock (Module.I<ICache>(CacheNames.MainCache).I<ThreadLocker>().Get("product", pid))
            {
                ProductGrade pg = Stars.GetProductGrade(pid);
                int addGradeCount = 0;
                int addGradeSum = 0;

                //brisanje ocjene
                if (score == null)
                {
                    if (pg != null)
                        if (WebShopDb.I.Database.ExecuteSqlCommand("delete from ProductGrade where Id={0}", pg.Id) > 0)
                        {
                            addGradeCount = -1;
                            addGradeSum = -pg.Grade;
                        }
                }
                //dodavanje ocjene
                else if (pg == null)
                {
                    SessionState ss = SessionState.I;
                    int? partnerId = ss.Partner == null ? null : (int?)ss.Partner.Id;
                    int? userId = ss.User == null ? null : (int?)ss.User.Id; ;
                    pg = new ProductGrade
                    {
                        DateOfEntry = DateTime.Now,
                        Grade = score.Value,
                        ItemId = pid,
                        PartnerId = userId.HasValue ? null : partnerId,
                        UserId = userId
                    };
                    WebShopDb context = WebShopDb.I;
                    context.ProductGrade.Add(pg);
                    context.SaveChanges();
                    addGradeCount = 1;
                    addGradeSum = pg.Grade;
                }
                //mjenjanje ocjene
                else
                {
                    pg = new ProductGrade
                    {
                        Id = pg.Id,
                        DateOfEntry = pg.DateOfEntry,
                        Grade = pg.Grade,
                        ItemId = pg.ItemId,
                        PartnerId = pg.PartnerId,
                        UserId = pg.UserId
                    };
                    WebShopDb context = WebShopDb.I;
                    context.ProductGrade.Add(pg);
                    context.Entry(pg).State = System.Data.EntityState.Unchanged;
                    int prevGrade = pg.Grade;
                    pg.Grade = score.Value;
                    pg.DateOfEntry = DateTime.Now;
                    context.SaveChanges();
                    addGradeSum = pg.Grade - prevGrade;
                }

                IItem product = Module.I<IItem>().Get(pid, AttributeKeyEnum.GradeCount, AttributeKeyEnum.GradeSum, AttributeKeyEnum.Grade);
                WebShopDb ctx = WebShopDb.I;
                new S.Item.ItemManager().AssociateWithDbContext(ctx, product.RawItem);

                IAttributeLocator attributeLocator = Module.I<IAttributeLocator>();
                IItemAttribute attr = product.Attributes[AttributeKeyEnum.GradeCount];
                IItemValue<int> itemValueInt;
                if (attr == null)
                {
                    attr = Module.I<IItemAttribute>();
                    product.Attributes.Add(attr);
                    attr.Attribute = attributeLocator.Find(a => a.Key == AttributeKeyEnum.GradeCount);
                    attr.AttributeId = attr.Attribute.Id;
                    itemValueInt = Module.I<IItemValue<int>>();
                    attr.IntValues.Add(itemValueInt);
                }
                else
                {
                    itemValueInt = attr.IntValues[0];
                }
                itemValueInt.Value += addGradeCount;
                int gradeCount = itemValueInt.Value;

                attr = product.Attributes[AttributeKeyEnum.GradeSum];
                if (attr == null)
                {
                    attr = Module.I<IItemAttribute>();
                    product.Attributes.Add(attr);
                    attr.Attribute = attributeLocator.Find(a => a.Key == AttributeKeyEnum.GradeSum);
                    attr.AttributeId = attr.Attribute.Id;
                    itemValueInt = Module.I<IItemValue<int>>();
                    attr.IntValues.Add(itemValueInt);
                }
                else
                {
                    itemValueInt = attr.IntValues[0];
                }
                itemValueInt.Value += addGradeSum;
                int gradeSum = itemValueInt.Value;

                attr = product.Attributes[AttributeKeyEnum.Grade];
                IItemValue<decimal> itemValueDecimal;
                if (attr == null)
                {
                    attr = Module.I<IItemAttribute>();
                    product.Attributes.Add(attr);
                    attr.Attribute = attributeLocator.Find(a => a.Key == AttributeKeyEnum.Grade);
                    attr.AttributeId = attr.Attribute.Id;
                    itemValueDecimal = Module.I<IItemValue<decimal>>();
                    attr.DecimalValues.Add(itemValueDecimal);
                }
                else
                {
                    itemValueDecimal = attr.DecimalValues[0];
                }
                itemValueDecimal.Value = gradeCount == 0 ? 0 : (decimal)gradeSum / gradeCount;
                score = (int)itemValueDecimal.Value;

                ctx.SaveChanges();
            }

            return Content(score.ToString());
        }

        [MyAccountAuthorize]
        public ActionResult NewsletterCheck(int itemId, bool isChecked)
        {
            NewsletterCheck nc = new NewsletterCheck
            {
                ItemId = itemId
            };
            nc.Check(isChecked);
            return PartialView("NewsletterCheck", nc);
        }

        //Metoda koja šalje mail partneru

        public ActionResult ContactForm(int partnerId, string name, string lastName, string email, string phone, string title, string messageText)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(messageText) && !string.IsNullOrWhiteSpace(title))
            {
                if (Module.I<IMail>().IsEmailValid(email))
                {
                    //Ovdje mora ic jos metoda za slanje maila

                    WebShopDb context = WebShopDb.I;
                    Syrilium.Modules.BusinessObjects.UserInquiry userInquiry = new Syrilium.Modules.BusinessObjects.UserInquiry();

                    userInquiry.PartnerId = partnerId;
                    userInquiry.Name = name;
                    userInquiry.LastName = lastName;
                    userInquiry.Message = HttpUtility.HtmlEncode(messageText).Replace(Environment.NewLine, "<br />");
                    userInquiry.Phone = phone;
                    userInquiry.Email = email;
                    userInquiry.DateTime = DateTime.Now;
                    if (SessionState.I.User != null)
                    {
                        userInquiry.UserId = SessionState.I.User.Id;
                    }

                    userInquiry.Title = title;

                    context.UserInquiry.Add(userInquiry);
                    context.SaveChanges();

                    S.Partner partner = new S.Partner().GetPartner(partnerId);

                    if (partner != null && !string.IsNullOrWhiteSpace(partner.Email))
                    {
                        string siteMailUserName = ConfigurationManager.AppSettings["SiteMailUserName"];

                        Module.I<IMail>().SendMail(
                                smtpHost: ConfigurationManager.AppSettings["SmtpHost"],
                                enableSsl: false,
                                from: email,
                            // Za sada komentiramo radi testa da ne bi slali mailove bzvze
                            //Odkomentirao 17.2.2013 pušta se stranica u pogon
                               to: partner.Email,
                            //   to: ConfigurationManager.AppSettings["SiteMailFrom"],
                                subject: title,
                                isBodyHtml: true,
                                body: userInquiry.Message,
                                useCredentials: !string.IsNullOrEmpty(siteMailUserName),
                                userName: siteMailUserName,
                                password: ConfigurationManager.AppSettings["SiteMailPassword"]);


                        ViewData["MessageSuccessTitle"] = M.Translation.Get("Uspješno ste poslali poruku");
                        ViewData["MessageSuccess"] = M.Translation.Get("Vaša poruka je poslana! Odgovor ćete dobiti na e-mail u najkraćem mogućem roku.");
                    }
                    else
                    {
                        ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška!");
                        ViewData["MessageBoxError"] = M.Translation.Get("Dogodila se greška pri slanju e-maila. Molimo Vas pokušajte povno kasnije.");
                    }

                }
                else
                {
                    ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška!");
                    ViewData["MessageBoxError"] = M.Translation.Get("Unesite ispravan e-mail");
                }

            }
            else
            {
                ViewData["MessageBoxErrorTitle"] = M.Translation.Get("Greška!");
                ViewData["MessageBoxError"] = M.Translation.Get("Unesite sve potrebne podatke označene zvijezdicom (*)");
            }
            return PartialView("MessageBox");

        }
    }
}
