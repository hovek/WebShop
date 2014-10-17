using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using WebShop.Models;
using S = Syrilium.Modules.BusinessObjects;
using WebShop.Infrastructure;
using M = WebShop.Models;

namespace WebShop.Controllers
{
	public class SearchController : Controller
	{
		public ActionResult Index(int did, string action, int? gid)
		{     
			SearchBox searchBox = new SearchBox
				{
					DepartmentId = did,
					GroupId = gid,
					QueryString = Request.QueryString.AllKeys.Count() > 0 ? Request.QueryString : Request.Form
				};
         
			if (action == "searchBoxRefresh")
				return PartialView("SearchBox", searchBox);
         
			Search index = new Search()
			{
				SearchBox = searchBox
			};

            searchBox.SearchObject.RecordsPerPage = 10;
			searchBox.Search();

            index.Paging = Paging.Items(searchBox.SearchObject.NumberOfRecords).PerPage(searchBox.SearchObject.RecordsPerPage).Move(searchBox.SearchObject.Page).Segment(3).Center();
    
			return View(index);
		}
        public ActionResult SearchInquiry(string name)
        {

            S.User user = SessionState.I.User;
            S.Partner partner = SessionState.I.Partner;

            if (user != null || partner != null)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    WebShopDb context = WebShopDb.I;
                    S.SearchInquiry searchInquiry = new SearchInquiry();

                    context.SearchInquiry.Add(searchInquiry);

                    searchInquiry.Name = name;
                    searchInquiry.Url = Request.UrlReferrer.OriginalString;
                    searchInquiry.DateTime = DateTime.Now;

                    if (user != null)
                    {
                        searchInquiry.UserId = user.Id;
                    }
                    else if (partner != null)
                    {
                        searchInquiry.PartnerId = partner.Id;
                    }
                    context.SaveChanges();
                    ViewData["Validation"] = M.Translation.Get("Pretraživanje je spremljeno");
                }
                else
                {
                    ViewData["Validation"] = M.Translation.Get("Unesite ime pretraživanja");
                }
            }
            else
            {
                ViewData["Validation"] = M.Translation.Get("Kako bi snimili pretraživanje morate se prijaviti.");
            }
            return PartialView("SearchInquiry");
        }
	}
}
