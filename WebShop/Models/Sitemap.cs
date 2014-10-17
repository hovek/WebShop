using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using M = WebShop.Models;

namespace WebShop.Models
{
    public class Sitemap 
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<Sitemap> SitemapList { get; set; }

        public virtual List<Sitemap> Get(string path)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);

            List<M.Sitemap> list = new List<M.Sitemap>();
            
            foreach (XmlElement elem in xDoc.GetElementsByTagName("loc"))
            {
                M.Sitemap sitemap = new M.Sitemap();
                sitemap.Url = elem.InnerText;
                list.Add(sitemap);
            }
            return list;

        }

    
    }
}