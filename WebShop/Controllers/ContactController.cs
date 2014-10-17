using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Infrastructure;
using Syrilium.Modules.BusinessObjects;
using M = WebShop.Models;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using Syrilium.CommonInterface;

namespace WebShop.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Contact/

        public ActionResult Index()
        {
            return View();
        }
        //Metoda koja šalje mail nama
        public ActionResult ContactForm(string name, string lastName, string email, string phone, string title, string messageText, string button, string captchaValue)
        {

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(messageText)
                && !string.IsNullOrWhiteSpace(title) && (Session["Captcha"] != null && Session["Captcha"].ToString() == captchaValue))
            {
                if (Module.I<IMail>().IsEmailValid(email))
                {
                    //Ovdje mora ic jos metoda za slanje maila

                    WebShopDb context = WebShopDb.I;
                    Syrilium.Modules.BusinessObjects.UserInquiry userInquiry = new Syrilium.Modules.BusinessObjects.UserInquiry();

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


                    string siteMailUserName = ConfigurationManager.AppSettings["SiteMailUserName"];

                    Module.I<IMail>().SendMail(
                            smtpHost: ConfigurationManager.AppSettings["SmtpHost"],
                            enableSsl: false,
                            from: email,
                            to: ConfigurationManager.AppSettings["SiteMailFrom"],
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

        public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);

            //generate new question
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("{0} + {1} = ?", a, b);

            //store answer
            Session["Captcha" + prefix] = a + b;

            //image stream
            FileContentResult img = null;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        int xa = x - r;
                        int ya = y - r;

                        gfx.DrawEllipse(pen, xa, ya, r, r);
                    }
                }

                //add question
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                //render as Jpeg
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
        }
    }
}
