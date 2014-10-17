using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.Models
{
    public class User 
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string Address { get; set; }  

        //za prikaz slika na view-u
        public bool DisplayNameAlert = false;
        public bool DisplaySurnameAlert = false;
        public bool DisplayUserNameAlert = false;
        public bool DisplayEmailAlert = false;
        public bool DisplayPasswordAlert = false;
        public bool DisplayGender = false;
        public bool DisplayDateOfBirth = false;
        public bool DisplayConditionsOfUse = false;
   


  
        public static List<SelectListItem> GenderList()
        {
			List<Gender> genders = WebShopDb.I.Gender.ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (Gender gender in genders)
            {
                SelectListItem item = new SelectListItem();
                item.Text = gender.Name;
                item.Value = gender.Id.ToString();
          
                items.Add(item);
            }
            return items;
        }
      


    }
}
