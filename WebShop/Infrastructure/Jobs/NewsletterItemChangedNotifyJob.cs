using Syrilium.CommonInterface;
using Syrilium.DataAccessInterface.SQL;
using Syrilium.JobsInterface;
using S = Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Models;

namespace WebShop.Infrastructure.Jobs
{
    public class NewsletterItemChangedNotifyJob : Job
    {
        public NewsletterItemChangedNotifyJob(TimeSpan startOnTime)
        {
            StartOnTime = startOnTime;
        }

        protected override dynamic DoWork(dynamic parameter)
        {
            DateTime start = DateTime.Now;
            DataSet ds = Module.I<IQuery>().GetDataSetWithProcedure(SPNames.GetChangedItemsAndSubscribers);

            Dictionary<string, List<ItemChangeHistory>> items = new Dictionary<string, List<ItemChangeHistory>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string email = dr["Email"].ToString();
                if (!items.ContainsKey(email))
                    items[email] = new List<ItemChangeHistory>();
                items[email].Add(new ItemChangeHistory { ItemId = (int)dr["ItemId"], LanguageId = (int)dr["LanguageId"], FirstChange = (DateTime)dr["FirstChange"] });
            }

            string serverUrl = (ConfigurationManager.AppSettings["ServerUrl"] ?? "").TrimEnd('/');
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            string newsletterMailFrom = ConfigurationManager.AppSettings["NewsletterMailFrom"];
            string newsletterMailUserName = ConfigurationManager.AppSettings["NewsletterMailUserName"];
            string newsletterMailPassword = ConfigurationManager.AppSettings["NewsletterMailPassword"];

            List<IItem> loadedItems = new List<IItem>();

            foreach (string email in items.Keys)
            {
                StringBuilder sb = new StringBuilder();
                int index = 1;
                foreach (ItemChangeHistory ich in items[email])
                {
                    IItem item = loadedItems.Find(i => i.Id == ich.ItemId);
                    if (item == null)
                    {
                        if ((item = Module.I<IItem>().Get(ich.ItemId, AttributeKeyEnum.Name, AttributeKeyEnum.AuctionDate, AttributeKeyEnum.Price)) == null) continue;
                        loadedItems.Add(item);
                    }

                    string path = string.Concat(serverUrl, VirtualPathUtility.ToAbsolute(string.Concat("~/Subject/Index?pid=", ich.ItemId)));
                    sb.AppendLine(string.Concat("<div>", index, ". ", "<a href=\"", path, "\">", item.Attributes[AttributeKeyEnum.Name].GetValueFormated(ich.LanguageId), "</a> - ", item.Attributes[AttributeKeyEnum.Price].GetValueFormated(ich.LanguageId), "</div>"));
                    string datumDrazbe = item.Attributes[AttributeKeyEnum.AuctionDate].GetValueFormated(ich.LanguageId);
                    if (!string.IsNullOrEmpty(datumDrazbe))
                        datumDrazbe = string.Concat("&nbsp;&nbsp;&nbsp;&nbsp;", Translation.Get("Datum dražbe", ich.LanguageId), ": ", datumDrazbe);
                    sb.AppendLine(string.Concat("<div>", Translation.Get("Prva promjena", ich.LanguageId), ": ", ich.FirstChange.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                                                , datumDrazbe, "</div><br/>"));
                    index++;
                }

                Module.I<IMail>().SendMail(
                    from: newsletterMailFrom,
                    to: email,
                    //to: "hove1982@hotmail.com",
                    subject: "Promjene",
                    body: sb.ToString(),
                    smtpHost: smtpHost,
                    enableSsl: false,
                    useCredentials: !string.IsNullOrEmpty(newsletterMailUserName),
                    userName: newsletterMailUserName,
                    password: newsletterMailPassword);
            }

            S.WebShopDb.I.Database.ExecuteSqlCommand("delete from ItemChangeHistory where DateOfEntry<={0}", start);

            return null;
        }
    }

    public class ItemChangeHistory
    {
        public int ItemId { get; set; }
        public int LanguageId { get; set; }
        public DateTime FirstChange { get; set; }
    }
}