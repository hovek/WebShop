using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Infrastructure;

namespace WebShop.Models
{
    public class Slider
    {
        public IItem Product { get; set; }
        public int PicturesPerSlide { get; set; }

        private List<string> pictures;
        public List<string> Pictures
        {
            get
            {
                if (pictures == null)
                {
                    IItemAttribute attr = Product.Attributes[AttributeKeyEnum.Image];
                    if (attr != null)
                    {
                        pictures = attr.GetRawValues().ConvertAll(v => (string)v);
                    }
                    else
                        pictures = new List<string>();
                }
                return pictures;
            }
        }

        public Slider()
        {
            PicturesPerSlide = 6;
        }
    }
}