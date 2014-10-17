using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class Subject
    {
        private int productId;
        public int ProductId
        {
            get
            {
                return productId;
            }
            set
            {
                productId = value;
                Stars.Product = Product;
            }
        }
        private IItem product;
        public IItem Product
        {
            get
            {
                if (product == null)
                {
                    product = Module.I<IItem>().Get(ProductId);
                }
                return product;
            }
        }

        public string Name
        {
            get
            {
                return Product.GetValueFormated(AttributeKeyEnum.Name);
            }
        }

        public GoogleMap GoogleMap { get; set; }

        public NewsletterCheck NewsletterCheck { get; set; }
        public Descriptions Descriptions { get; set; }
        public PriceBox PriceBox { get; set; }
        public Slider Slider { get; set; }
        public CountDown CountDown { get; set; }
        public ContactPartner ContactPartner { get; set; }
        public SliderLogo SliderLogo { get; set; }
        public Stars Stars { get; set; }

        public Subject()
        {
            this.GoogleMap = new GoogleMap();
            this.NewsletterCheck = new NewsletterCheck();
            this.Descriptions = new Descriptions();
            this.PriceBox = new PriceBox();
            this.Slider = new Slider();
            this.CountDown = new CountDown();
            this.ContactPartner = new ContactPartner();
            this.SliderLogo = new SliderLogo();
            this.Stars = new Stars();
        }
    }
}