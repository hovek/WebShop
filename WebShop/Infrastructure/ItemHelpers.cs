using Syrilium.CommonInterface;
using Syrilium.Modules.BusinessObjects.ModuleDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Infrastructure
{
    public static class ItemHelpers
    {
        public static IAttributeDefinition GetAttributeDefinition(AttributeKeyEnum attributeKey)
        {
            return Module.I<IAttributeLocator>().Find(a => a.Key == attributeKey);
        }

        public static int? GetAttributeDefinitionId(AttributeKeyEnum attributeKey)
        {
            return Module.I<IAttributeLocator>().FindId(a => a.Key == attributeKey);
        }

        public static MvcHtmlString GetDropDownListReferenceUpdateForms(RequestContext requestContext, IAttributeTemplate attributeTemplate)
        {
            if (attributeTemplate == null)
            {
                return new MvcHtmlString("");
            }

            StringBuilder sb = new StringBuilder();
            string formAction = new UrlHelper(requestContext).Action("GetConstrainedList", "Home");
            int languageId = WebShop.SessionState.I.LanguageId;
            foreach (IAttributeTemplateAttribute attr in attributeTemplate.GetAllAttributes())
            {
                List<int> constrainAttributes = attr.GetForeignKeyConstrainAttributes();
                if (constrainAttributes.Count > 0)
                {
                    string getConstrainedListFormId = string.Concat(attr.Attribute.Id, "_Form");
                    sb.AppendLine(string.Concat(@"<form action=""", formAction, @""" method=""post"" id=""", getConstrainedListFormId, @""">"));
                    sb.AppendLine(string.Concat(@"<input type=""hidden"" name=""templateId"" value=""", attributeTemplate.Id, @"""/>"));
                    sb.AppendLine(string.Concat(@"<input type=""hidden"" name=""constrainedAttributeId"" value=""", attr.Attribute.Id, @"""/>"));
                    sb.AppendLine(string.Concat(@"<input type=""hidden"" name=""languageId"" value=""", languageId, @"""/>"));
                    foreach (int constrainAttributeId in constrainAttributes)
                    {
                        sb.AppendLine(string.Concat(@"<input type=""hidden"" name=""", constrainAttributeId, @""" id=""", string.Concat(attr.Attribute.Id, "_", constrainAttributeId), @"""/>"));
                    }
                    sb.AppendLine("</form>");
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString ItemDropDownListReference(this HtmlHelper helper, IAttributeDefinition attribute, IAttributeTemplate attributeTemplate = null,
            IItem product = null, object htmlAttributes = null, string optionLabel = null, int? defaultValue = null, bool loadSelectedValue = true)
        {
            List<ICommonReferenceListItem> crl = new List<ICommonReferenceListItem>();
            string onChangeScript = "";

            HttpRequestBase request = helper.ViewContext.RequestContext.HttpContext.Request;
            NameValueCollection queryString = !loadSelectedValue ? new NameValueCollection() : (request.Form.AllKeys.Count() > 0 ? request.Form : request.QueryString);

            if (attributeTemplate != null)
            {
                IAttributeTemplateAttribute templateAttribute = attributeTemplate.GetAttribute(attribute.Id);
                if (templateAttribute != null)
                {
                    if (product != null)
                        crl = templateAttribute.GetConstrainedReferenceList(product.Attributes);
                    else
                        crl = templateAttribute.GetConstrainedReferenceList(GetAttributeValueDictionary(queryString));
                }
                onChangeScript = getReferenceListControlOnChangeHtmlAttribute(attribute.Id, attributeTemplate.GetForeignKeyConstrainedAttributes(attribute.Id));
            }
            else
                crl = Module.I<IReference>().ConvertToCommonReferenceList(attribute.GetReferenceObject(), SessionState.I.LanguageId);

            //postavljanje defaultne vrijednosti 
            if (defaultValue == null)
            {
                if (product != null)
                {
                    IItemAttribute itemAttribute = product.GetItemAttribute(attribute.Id);
                    if (itemAttribute != null)
                        defaultValue = itemAttribute.GetRawValue();
                }
                if (defaultValue == null)
                {
                    string paramValue = CommonHelpers.GetQueryStringParamValue(queryString, attribute.Id.ToString());
                    if (paramValue != null)
                        defaultValue = Module.I<IValueConverter>().Convert(attribute.DataType, paramValue);
                }
            }

            Dictionary<string, dynamic> htmlAttributesDictionary = CommonHelpers.GetHtmlAttributes(htmlAttributes);
            if (!htmlAttributesDictionary.ContainsKey("id"))
                htmlAttributesDictionary["id"] = attribute.Id.ToString();
            if (!htmlAttributesDictionary.ContainsKey("name"))
                htmlAttributesDictionary["name"] = attribute.Id.ToString();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Concat(@"<select ", CommonHelpers.GetHtmlAttributesString(htmlAttributesDictionary), onChangeScript.ToString(), ">"));
            if (optionLabel != null)
                sb.AppendLine(string.Concat(@"<option value", defaultValue == null ? @" selected=""selected""" : "", ">", optionLabel, "</option>"));
            foreach (var item in GetSelectList(crl, defaultValue))
            {
                sb.AppendLine(string.Concat(@"<option value=""", item.Value, @""" ", (item.Selected ? @"selected=""selected""" : ""), ">", item.Text, "</option>"));
            }
            sb.AppendLine("</select>");
            sb.AppendLine(string.Concat(@"<img src=""../../Resources/Images/ajax-loader16.gif"" id=""", attribute.Id, @"_ajaxLoader"" style=""display:none;float:right;"" />"));

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString ItemDropDown(this HtmlHelper helper, IAttributeDefinition attribute = null, List<SelectListItem> items = null, string optionLabel = null,
            object htmlAttributes = null, bool loadSelectedValue = true, bool enabled = true)
        {
            string id = attribute == null ? "" : attribute.Id.ToString();

            Dictionary<string, dynamic> htmlAttributesDictionary = CommonHelpers.GetHtmlAttributes(htmlAttributes);
            if (!htmlAttributesDictionary.ContainsKey("id"))
                htmlAttributesDictionary["id"] = id;
            if (!htmlAttributesDictionary.ContainsKey("name"))
                htmlAttributesDictionary["name"] = id;

            string stringValue = "";
            if (loadSelectedValue)
            {
                HttpRequestBase request = helper.ViewContext.RequestContext.HttpContext.Request;
                NameValueCollection queryString = request.Form.AllKeys.Count() > 0 ? request.Form : request.QueryString;
                stringValue = CommonHelpers.GetQueryStringParamValue(queryString, id);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Concat(@"<select ", CommonHelpers.GetHtmlAttributesString(htmlAttributesDictionary), enabled ? "" : @"disabled=""disabled"" ", ">"));
            if (optionLabel != null)
                sb.AppendLine(string.Concat(@"<option value>", optionLabel, "</option>"));
            if (items != null)
            {
                foreach (var item in items)
                {
                    sb.AppendLine(string.Concat(@"<option value=""", item.Value, @""" ", (item.Selected || item.Value == stringValue ? @"selected=""selected""" : ""), ">", item.Text, "</option>"));
                }
            }
            sb.AppendLine("</select>");

            return new MvcHtmlString(sb.ToString());
        }

        public static Dictionary<int, List<int>> GetAttributeValueDictionary(NameValueCollection queryString)
        {
            Dictionary<int, List<int>> attributesValues = new Dictionary<int, List<int>>();
            foreach (var key in queryString.AllKeys)
            {
                int attributeId;
                if (int.TryParse(key, out attributeId))
                {
                    string[] values = queryString[key].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var value in values)
                    {
                        int intValue;
                        if (int.TryParse(queryString[key], out intValue))
                        {
                            if (!attributesValues.ContainsKey(attributeId)) attributesValues[attributeId] = new List<int>();
                            attributesValues[attributeId].Add(intValue);
                        }
                    }
                }
            }
            return attributesValues;
        }

        private static string getReferenceListControlOnChangeHtmlAttribute(int constrainAttributeId, List<int> constrainedAttributes)
        {
            System.Text.StringBuilder onChangeScript = new System.Text.StringBuilder();
            if (constrainedAttributes.Count > 0)
            {
                onChangeScript.Append(@" onChange=""javascript:");
                foreach (int constrainedAttributeId in constrainedAttributes)
                {
                    string inputId = string.Concat(constrainedAttributeId, "_", constrainAttributeId);
                    string formId = string.Concat(constrainedAttributeId, "_Form");
                    string listDDLId = constrainedAttributeId.ToString();
                    onChangeScript.Append(string.Concat("$('#", inputId, "').attr('value',this.value); RefreshDropDownListReference('", formId, "', '", listDDLId, "');"));
                }
                onChangeScript.Append(@"""");
            }

            return onChangeScript.ToString();
        }

        public static List<SelectListItem> GetSelectList(List<ICommonReferenceListItem> items, int? defaultValue = null)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (ICommonReferenceListItem item in items)
            {
                selectListItems.Add(new SelectListItem
                {
                    Selected = defaultValue == item.Id,
                    Text = item.Value.ToString(),
                    Value = item.Id.ToString()
                });
            }

            return selectListItems;
        }

        public static MvcHtmlString GetRefreshDropDownListReferenceJSF()
        {
            return new MvcHtmlString(@"
				function RefreshDropDownListReference(formId, listId){
					var ajaxLoader = $('#' + listId + '_ajaxLoader');
					var list = $('#' + listId);

					list.hide();
					ajaxLoader.show();

					AjaxSubmitForm(
						$('#'+formId),
						function (data) {
							$('#'+listId).html(data);
						},
						function () {
							ajaxLoader.hide();
							list.show();
						}
					);
				}
			");
        }

        public static MvcHtmlString ItemCheckBox(this HtmlHelper helper, IAttributeDefinition attribute = null, ComparisonOperator? comparisonOperator = null, object htmlAttributes = null,
            bool sendFalseValue = true, bool isChecked = false, bool loadSelectedValue = true)
        {
            string id = attribute == null ? null : string.Concat(attribute.Id, comparisonOperator != null ? "_" + EnumStringValue.GetStringValue(comparisonOperator.Value) : "");
            string name = id;

            Dictionary<string, dynamic> htmlAttributesDictionary = CommonHelpers.GetHtmlAttributes(htmlAttributes);
            if (!htmlAttributesDictionary.ContainsKey("id") && id != null)
                htmlAttributesDictionary["id"] = id;
            if (!htmlAttributesDictionary.ContainsKey("name") && name != null)
                htmlAttributesDictionary["name"] = name;
            else
                name = htmlAttributesDictionary["name"];

            if (!isChecked && loadSelectedValue)
            {
                HttpRequestBase request = helper.ViewContext.RequestContext.HttpContext.Request;
                NameValueCollection queryString = request.Form.AllKeys.Count() > 0 ? request.Form : request.QueryString;
                string selectedValueString = (CommonHelpers.GetQueryStringParamValue(queryString, id ?? "") ?? "").ToLower().Trim();
                isChecked = selectedValueString == "true" || selectedValueString == "1";
            }

            string falseInput = "";
            if (sendFalseValue)
            {
                string idOfFalseInput = string.Concat(id, "_false");
                string onChange = string.Concat("this.checked?$('#", idOfFalseInput, "').attr('name',''):$('#", idOfFalseInput, "').attr('name','", name, "');");
                if (htmlAttributesDictionary.ContainsKey("onchange"))
                    htmlAttributesDictionary["onchange"] += string.Concat(" ", onChange);
                else
                    htmlAttributesDictionary["onchange"] = string.Concat("javascript:", onChange);
                falseInput = string.Concat(@"<input type=""hidden"" id=""", idOfFalseInput, @""" name=""", isChecked ? "" : name, @""" value=""false""/>");
            }

            string checkBox = string.Concat(@"<input type=""checkbox"" ", isChecked ? @"checked=""checked"" " : "", CommonHelpers.GetHtmlAttributesString(htmlAttributesDictionary), @"value=""true"" />",
                                            falseInput);

            return new MvcHtmlString(checkBox);
        }

        public static MvcHtmlString ItemNumberInput(this HtmlHelper helper, IAttributeDefinition attribute, ComparisonOperator? comparisonOperator = null, object htmlAttributes = null,
            dynamic value = null, bool loadSelectedValue = true)
        {
            string id = string.Concat(attribute.Id, comparisonOperator != null ? "_" + EnumStringValue.GetStringValue(comparisonOperator.Value) : "");

            Dictionary<string, dynamic> htmlAttributesDictionary = CommonHelpers.GetHtmlAttributes(htmlAttributes);
            if (!htmlAttributesDictionary.ContainsKey("id"))
                htmlAttributesDictionary["id"] = id;
            if (!htmlAttributesDictionary.ContainsKey("name"))
                htmlAttributesDictionary["name"] = id;

            string stringValue;
            if (value == null && loadSelectedValue)
            {
                HttpRequestBase request = helper.ViewContext.RequestContext.HttpContext.Request;
                NameValueCollection queryString = request.Form.AllKeys.Count() > 0 ? request.Form : request.QueryString;
                stringValue = CommonHelpers.GetQueryStringParamValue(queryString, id);
            }
            else
                stringValue = value.ToString();

            string input = string.Concat(@"<input type=""text"" ", CommonHelpers.GetHtmlAttributesString(htmlAttributesDictionary), @"value=""", stringValue, @""" />");

            return new MvcHtmlString(input);
        }

        public static MvcHtmlString ItemCustomInput(this HtmlHelper helper, IAttributeDefinition attribute, ComparisonOperator? comparisonOperator = null, object htmlAttributes = null,
            dynamic value = null, bool loadSelectedValue = true)
        {
            string id = string.Concat(attribute.Id, comparisonOperator != null ? "_" + EnumStringValue.GetStringValue(comparisonOperator.Value) : "");

            Dictionary<string, dynamic> htmlAttributesDictionary = CommonHelpers.GetHtmlAttributes(htmlAttributes);
            if (!htmlAttributesDictionary.ContainsKey("id"))
                htmlAttributesDictionary["id"] = id;
            if (!htmlAttributesDictionary.ContainsKey("name"))
                htmlAttributesDictionary["name"] = id;
            if (!htmlAttributesDictionary.ContainsKey("type"))
                htmlAttributesDictionary["type"] = "text";

            string stringValue;
            if (value == null && loadSelectedValue)
            {
                HttpRequestBase request = helper.ViewContext.RequestContext.HttpContext.Request;
                NameValueCollection queryString = request.Form.AllKeys.Count() > 0 ? request.Form : request.QueryString;
                stringValue = CommonHelpers.GetQueryStringParamValue(queryString, id);
            }
            else
                stringValue = value.ToString();

            string input = string.Concat(@"<input ", CommonHelpers.GetHtmlAttributesString(htmlAttributesDictionary), @"value=""", stringValue, @""" />");

            return new MvcHtmlString(input);
        }
    }
}