using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using S = Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects;
using WebShop.BusinessObjectsInterface;
using System.Web.Helpers;
using System.Data;
using System.Dynamic;
using System.Web.Mvc;
using WebShop.Infrastructure;
using Syrilium.CommonInterface.Caching;



namespace WebShop.Models
{
    public class Translation
    {
        private List<S.Translation> translations;
        public List<S.Translation> Translations
        {
            get
            {
                if (translations == null) translations = TranslationHelper.GetCached().ToList();
                return translations;
            }
            set
            {
                translations = value;
            }
        }

        public int? TranslationId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public virtual Language Language { get; set; }

        public static string Get(string keyTranslation, int? languageId = null)
        {
            return Module.I<ICache>(CacheNames.AdminCache).I<S.Translation>().Get(
                keyTranslation,
                Module.I<IConfig>().GetValue(ConfigNames.KeyTranslationLanguageId),
                languageId ?? SessionState.I.LanguageId,
                Module.I<IConfig>().GetValue(ConfigNames.PreferedLanguageIdIfNoTranslation));
        }

        public List<dynamic> GetSourceAndColumns(out List<WebGridColumn> columns)
        {
            Dictionary<string, IDictionary<string, object>> rowsByKey = new Dictionary<string, IDictionary<string, object>>();
            List<dynamic> rows = new List<dynamic>();
            List<S.Language> languages = Language.Get();
            foreach (S.Translation tran in Translations)
            {
                S.Language languageForTran = languages.Find(l => l.Id == tran.LanguageId);

                IDictionary<string, object> row;
                if (!rowsByKey.ContainsKey(tran.Key))
                {
                    row = new ExpandoObject();
                    rowsByKey[tran.Key] = row;
                    rows.Add(row);
                    foreach (S.Language lang in languages)
                    {
                        if (lang == languageForTran) continue;
                        row[lang.Id.ToString()] = new KeyValuePair(tran.Key, "");
                    }
                }
                else
                    row = rowsByKey[tran.Key];

                row[languageForTran.Id.ToString()] = new KeyValuePair(tran.Key, tran.Value);
            }

            List<WebGridColumn> webGridColumns = new List<WebGridColumn>();
            foreach (S.Language lang in languages)
            {
                webGridColumns.Add(
                    new WebGridColumn
                    {
                        Header = lang.Name,
                        ColumnName = lang.Id.ToString(),
                        CanSort = true,
                        Format = i =>
                        {
                            KeyValuePair keyValue = (KeyValuePair)i[lang.Id.ToString()];
                            string id = string.Concat(keyValue.Key, "_", lang.Id);
                            return new HtmlString(string.Concat("<textarea id=\"", id, "\" oninput=\"javascript:OnTranslationChange('", id, "')\">", keyValue.Value, "</textarea>"));
                        }
                    }
                );
            }

            columns = webGridColumns;

            return rows;
        }

        public List<SelectListItem> GetLanguages()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var lang in Module.I<ICache>(CacheNames.MainCache).I<S.Language>().Get())
            {
                selectList.Add(new SelectListItem
                {
                    Text = lang.Name,
                    Value = lang.Id.ToString()
                });
            }
            return selectList;
        }
    }

    public class KeyValuePair : IComparable
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public int CompareTo(object obj)
        {
            return this.Value.CompareTo(((KeyValuePair)obj).Value);
        }
    }
}