using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.Models
{
    public class News
    {
        public static SelectList GetNews()
        {
            Dictionary<int, string> newsList = new Dictionary<int, string>();
            foreach(Syrilium.Modules.BusinessObjects.News news in WebShopDb.I.News)
            {
                newsList.Add(news.Id, news.Title);
            }
            SelectList selectList = new SelectList(newsList, "Key", "Value");

            return selectList;
         
        }
    }
}