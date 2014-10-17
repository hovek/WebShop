using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using Syrilium.Modules.BusinessObjects.ModuleDefinitions;
using Syrilium.Interfaces.BusinessObjectsInterface;

namespace Syrilium.Modules.BusinessObjects
{
	public class TranslatableEntity<TTranslation> where TTranslation : EntityTranslation
	{
		private bool allTranslationsLoaded = false;
		private List<TTranslation> _translation = new List<TTranslation>();

		public int Id { get; set; }
		public List<TTranslation> Translation
		{
			get
			{
				if (!allTranslationsLoaded)
				{
					allTranslationsLoaded = true;
					string langIds = "";
					_translation.ForEach(p => langIds += p.LanguageId.ToString() + ",");
					List<TTranslation> newTrans = WebShopDb.I.Database.SqlQuery<TTranslation>(
						"select * from " + typeof(TTranslation).Name + " where ParentId=@Id" + (langIds.Length > 0 ? @" and LanguageId not in(" + langIds.Trim(',') + ")" : ""),
															new SqlParameter("@Id", Id)).ToList();

					_translation.AddRange(newTrans);
				}

				return _translation;
			}
			set
			{
				_translation = value;
			}
		}

		public TTranslation CurrentTranslation
		{
			get
			{
				return GetTranslation(Module.LanguageId);
			}
		}

		public TResult GetTranslation<TResult>(Func<TTranslation, TResult> property, int? languageId = null)
		{
			TTranslation tran = languageId == null ? CurrentTranslation : GetTranslation(languageId.Value);
			if (tran != null)
			{
				return property(tran);
			}
			return default(TResult);
		}

		public TTranslation GetTranslation(int languageId)
		{
			foreach (TTranslation t in _translation)
			{
				if (t.LanguageId == languageId)
				{
					return t;
				}
			}

			TTranslation tran = WebShopDb.I.Set<TTranslation>().Where(p => p.ParentId == Id && p.LanguageId == languageId).FirstOrDefault();
			if (tran != null)
			{
				_translation.Add(tran);
			}

			return tran;
		}
	}

	public class EntityTranslation
	{
		public int Id { get; set; }
		public int ParentId { get; set; }
		public int LanguageId { get; set; }
	}
}
