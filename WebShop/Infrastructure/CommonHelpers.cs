using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.Mvc;
using Syrilium.Modules.BusinessObjects;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Web.Mvc.Html;
using WebShop.BusinessObjectsInterface;
using S = Syrilium.Modules.BusinessObjects;
using Syrilium.CommonInterface.Caching;
using System.IO;
using System.Drawing;
using Syrilium.CommonInterface;
using D = System.Drawing.Imaging;

namespace WebShop.Infrastructure
{
    public static class CommonHelpers
    {
        #region HtmlHelpers
        public static MvcHtmlString Image(this HtmlHelper helper, string id, string src, string alt, string cssClass, string onclick, string width, string height)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("id", id);
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", alt);
            builder.MergeAttribute("class", cssClass);
            builder.MergeAttribute("onclick", onclick);
            builder.MergeAttribute("width", width);
            builder.MergeAttribute("height", height);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString ImageHider(this HtmlHelper helper, string id, string hidingControlId, string src, string alt, string cssClass, string onclick, string width, string height)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("id", id);
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", alt);
            builder.MergeAttribute("class", cssClass);
            builder.MergeAttribute("onclick", "CustomScripts_HideControl('" + hidingControlId + "');" + onclick);
            builder.MergeAttribute("width", width);
            builder.MergeAttribute("height", height);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static IHtmlString TextWritter(this HtmlHelper html, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new HtmlString("&nbsp;");
            }
            return new HtmlString(html.Encode(value));
        }

        public static MvcHtmlString MenuLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string cssClass, string pageId)
        {
            string currentAction = helper.ViewContext.RouteData.GetRequiredString("action");
            string currentController = helper.ViewContext.RouteData.GetRequiredString("controller");


            if (controllerName == currentController && actionName == currentAction)
            {
                return helper.ActionLink(
                    linkText,
                    actionName,
                    controllerName,
                    new
                    {
                        pageId = pageId
                    },
                    new
                    {
                        @class = cssClass
                    });
            }
            return helper.ActionLink(linkText, actionName, controllerName);
        }
        #endregion

        public static SelectList DaysOfMonth(string day)
        {
            Dictionary<int, string> days = new Dictionary<int, string>();

            for (int i = 1; i <= 31; i++)
            {
                days.Add(i, i.ToString());
            }
            SelectList selectList = new SelectList(days, "Key", "Value", day);
            return selectList;
        }

        public static SelectList Months(string month)
        {
            Dictionary<int, string> months = new Dictionary<int, string>();

            for (int i = 1; i <= 12; i++)
            {
                months.Add(i, i.ToString());
            }
            SelectList selectList = new SelectList(months, "Key", "Value", month);
            return selectList;
        }

        public static SelectList Year(string year)
        {
            Dictionary<int, string> years = new Dictionary<int, string>();

            for (int i = DateTime.Now.Year; i >= 1900; i--)
            {
                years.Add(i, i.ToString());
            }
            SelectList selectList = new SelectList(years, "Key", "Value", year);
            return selectList;
        }

        public static List<Page> GetCurrentPage()
        {
            string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            int pageId = 0;

            if (controllerName == "Info")
            {
                pageId = 1;
            }
            if (controllerName == "AboutUs")
            {
                pageId = 2;
            }
            if (controllerName == "Partner")
            {
                pageId = 3;
            }
            if (controllerName == "Services")
            {
                pageId = 4;
            }
            if (controllerName == "Marketing")
            {
                pageId = 5;
            }

            List<Page> page = Module.I<ICache>(CacheNames.MainCache).I<S.Page>().GetPages(pageId);

            return page;
        }

        public static Dictionary<string, dynamic> GetHtmlAttributes(object htmlAttributes)
        {
            Dictionary<string, dynamic> coll = new Dictionary<string, dynamic>();

            if (htmlAttributes == null) return coll;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(htmlAttributes);
            foreach (PropertyDescriptor prop in props)
            {
                coll[prop.Name.Replace("@", "").ToLower()] = prop.GetValue(htmlAttributes);
            }
            return coll;
        }

        public static string GetHtmlAttributesString(object htmlAttributes)
        {
            return GetHtmlAttributesString(GetHtmlAttributes(htmlAttributes));
        }

        public static string GetHtmlAttributesString(Dictionary<string, dynamic> htmlAttributesDictionary)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, dynamic> attr in htmlAttributesDictionary)
            {
                sb.Append(string.Concat(attr.Key, @"=""", attr.Value, @""" "));
            }
            return sb.ToString();
        }

        public static MvcHtmlString GetAjaxSubmitFormJSF()
        {
            return new MvcHtmlString(@"
				function AjaxSubmitForm(form, onSuccess, onComplete, onError) {
                    $(""body"").css(""cursor"", ""progress"");
					$.ajax({
						url: form.attr(""action""),
						type: ""POST"",
						data: form.serialize(),
						success: onSuccess,
                        error: function () {
                            $(""body"").css(""cursor"", ""default"");
                            if (onError) onError();
                        },
                        complete: function () {
                            $(""body"").css(""cursor"", ""default"");
                            if (onComplete) onComplete();
                        }
					});
				}
			");
        }

        public static string GetQueryStringParamValue(NameValueCollection queryString, string paramName, bool nameStartsWith = false)
        {
            foreach (var key in queryString.AllKeys)
            {
                if (key == null) continue;

                if (nameStartsWith && key.StartsWith(paramName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return queryString[key];
                }
                if (paramName.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                {
                    return queryString[key];
                }
            }

            return null;
        }

        public static string GetProductImagePath(string imageFormatName, string imageName)
        {
            return GetProductImagePath("/Resources/Upload/Images/Product/", imageFormatName, imageName);
        }

        public static string GetProductImagePath(string path, string imageFormatName, string imageName)
        {
            return GetImagePath("Product", path, imageFormatName, imageName);
        }

        public static string GetNewsImagePath(string imageFormatName, string imageName)
        {
            return GetImagePath("News", "/Resources/Upload/Images/News/", imageFormatName, imageName);
        }

        public static string GetImagePath(string group, string path, string imageFormatName, string imageName)
        {
            string pathToOriginal = string.Concat(path, "Original/");
            string pathToFormated = string.Concat(path, imageFormatName, "/");
            IImageHelper imageHelper = Module.I<IImageHelper>();
            bool createImageFormat;
            string returnPath = imageHelper.CheckAndGetImagePath(pathToOriginal, pathToFormated, imageName, out createImageFormat);
            if (!createImageFormat) return returnPath;

            ImageFormat format = Module.I<ICache>(CacheNames.MainCache).I<ImageFormat>().Get(group).Find(f => f.Name == imageFormatName);
            if (format == null) return returnPath;

            imageHelper.CreateImageFormat(
                pathToOriginal,
                pathToFormated,
                imageName,
                format.Height,
                format.Width
            );

            return returnPath;
        }

        public static string SaveImage(Image image, string path, string fileName)
        {
            string fullPath = "";
            try
            {
                fileName = GetFileName(path, ChangeFileExtension(fileName, "jpeg"));
                fullPath = Path.Combine(path, fileName);
                image.Save(fullPath, D.ImageFormat.Jpeg);
            }
            catch
            {
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                fileName = GetFileName(path, ChangeFileExtension(fileName, "png"));
                image.Save(Path.Combine(path, fileName), D.ImageFormat.Png);
            }
            return fileName;
        }

        public static string ChangeFileExtension(string fileName, string extension)
        {
            int lastDotIndex = fileName.LastIndexOf('.');
            if (lastDotIndex == -1) lastDotIndex = fileName.Length - 1;
            return string.Concat(fileName.Substring(0, lastDotIndex), ".", extension);
        }

        public static string GetFileName(string path, string fileName)
        {
            int lastDotIndex = fileName.LastIndexOf('.');
            if (lastDotIndex == -1) lastDotIndex = fileName.Length - 1;

            var name = fileName.Substring(0, lastDotIndex);
            string extension = fileName.Substring(lastDotIndex + 1, fileName.Length - lastDotIndex - 1);

            string fullPath = Path.Combine(path, fileName);
            int i = 1;
            while (System.IO.File.Exists(fullPath))
            {
                fileName = string.Concat(name, i, ".", extension);
                fullPath = Path.Combine(path, fileName);
                i++;
            }

            return fileName;
        }
    }
}