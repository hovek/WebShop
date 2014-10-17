using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.BusinessObjectsInterface;

namespace WebShop.Infrastructure
{
	public class MyAccountAuthorize : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);
			if (filterContext.Result is HttpUnauthorizedResult
				|| !SessionState.I.HasPermission(PermissionNames.User))
			{
				filterContext.Result = new RedirectToRouteResult(
				 new System.Web.Routing.RouteValueDictionary 
				 { 
				  { "site", filterContext.RouteData.Values["site"] }, 
				  { "controller", "MyAccount" },
				  { "action", "Login" },
				  { "returnUrl", filterContext.HttpContext.Request.RawUrl }  
				 });
			}
		}
	}

	public class AdminAuthorize : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);
			if (filterContext.Result is HttpUnauthorizedResult
				|| !SessionState.I.HasPermission(PermissionNames.AdminAccess))
			{
				filterContext.Result = new RedirectToRouteResult(
				 new System.Web.Routing.RouteValueDictionary 
				 { 
				  { "site", filterContext.RouteData.Values["site"] }, 
				  { "controller", "Admin" },
				  { "action", "Login" },
				  { "returnUrl", filterContext.HttpContext.Request.RawUrl }  
				 });
			}
		}
	}
}