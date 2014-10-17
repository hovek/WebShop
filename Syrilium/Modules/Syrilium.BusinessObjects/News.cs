using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IntroductionText { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string Autor { get; set; }
        public string AutorPicture { get; set; }
        public bool Visible { get; set; }
        public DateTime? Date { get; set; }

        public static News GetNews(int newsId, WebShopDb context = null)
        {
            return (from news in (context ?? WebShopDb.I).News
                    where news.Id == newsId
                    select news).First();
        }
        public static List<News> GetAllVisibleNews()
        {
            var newsList = from news in WebShopDb.I.News
                           where news.Visible == true
                           orderby news.Date descending
                           select news;
            return (newsList).ToList();
        }
        public static List<News> GetTop5VisibleNews()
        {
            var newsList = from news in WebShopDb.I.News
                           where news.Visible == true
                           orderby news.Date descending
                           select news;
            return (newsList).Take(5).ToList();
        }
    }
}

