using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects.ModuleDefinitions;
using System.Collections.Concurrent;
using Syrilium.CommonInterface;

namespace Syrilium.Modules.BusinessObjects
{
    public class Translation
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }

        public virtual BlockingCollection<Translation> Get()
        {
            BlockingCollection<Translation> bc = new BlockingCollection<Translation>();
            WebShopDb.I.Translation.ToList().ForEach(t => bc.Add(t));
            return bc;
        }

        public virtual string Get(string key, int languageId)
        {
            return (from t in WebShopDb.I.Translation
                    where t.Key == key && t.LanguageId == languageId
                    select t.Value).FirstOrDefault();
        }

        public virtual string Get(string keyTranslation, int keyLanguageId, int languageId, int preferedLanguageId)
        {
            string key = Module.I<ICryptography>().GetMD5Hash(keyTranslation);
            BlockingCollection<Translation> translations = Get();

            Translation translationObj = null;
            foreach (Translation t in translations)
            {
                if (t.Key == key && t.LanguageId == keyLanguageId)
                {
                    translationObj = t;
                    break;
                }
            }

            if (translationObj == null)
            {
                WebShopDb context = new WebShopDb();
                translationObj = new Translation
                {
                    Key = key,
                    LanguageId = keyLanguageId,
                    Value = keyTranslation
                };
                context.Translation.Add(translationObj);
                context.SaveChanges();
                translations.Add(translationObj);
            }

            string translationString = "";
            foreach (Translation t in translations)
            {
                if (t.Key == key
                    && !string.IsNullOrEmpty(t.Value))
                {
                    if (t.LanguageId == languageId)
                    {
                        translationString = t.Value;
                        break;
                    }
                    else if (t.LanguageId == preferedLanguageId || string.IsNullOrEmpty(translationString))
                        translationString = t.Value;
                }
            }

            return translationString;
        }

        public static List<Translation> Search(string value, int? languageId)
        {
            WebShopDb ctx = WebShopDb.I;
            return (from st in ctx.Translation
                    join tran in ctx.Translation on st.Key equals tran.Key
                    where string.IsNullOrEmpty(value)
                            || (st.Value.Contains(value)
                                && (languageId == null || st.LanguageId == languageId))
                    select tran).ToList();
        }

        public static Translation GetById(int transaltionId, WebShopDb context = null)
        {
            WebShopDb ctx = context ?? WebShopDb.I;
            return (from t in ctx.Translation
                    where t.Id == transaltionId
                    select t).FirstOrDefault();
        }
    }
}
