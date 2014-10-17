using Syrilium.Modules.BusinessObjects.ModuleDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class Config
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? IntValue { get; set; }
		public string StringValue { get; set; }
		public decimal? DecimalValue { get; set; }
		public bool? BoolValue { get; set; }
		public DateTime? DateTimeValue { get; set; }


		public dynamic Value
		{
			get
			{
				if (IntValue != null)
				{
					return IntValue;
				}
				else if (BoolValue != null)
				{
					return BoolValue;
				}
				else if (DecimalValue != null)
				{
					return DecimalValue;
				}
				else if (DateTimeValue != null)
				{
					return DateTimeValue;
				}
				else if (StringValue != null)
				{
					return StringValue;
				}

				return null;
			}
		}

		public virtual List<Config> Get()
		{
			return WebShopDb.I.Config.ToList();
		}
	}
}