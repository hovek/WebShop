using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Home
    {
        public LeftMenu LeftMenu { get; set; }
        public SearchBox SearchBox { get; set; }
        public SliderPremium SliderPremium { get; set; }

        public Home()
        {
            LeftMenu = new LeftMenu();
            SearchBox = new SearchBox();
            SliderPremium = new SliderPremium();
        }
    }
}