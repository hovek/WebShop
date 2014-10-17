using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
	public class Gender : TranslatableEntity<GenderTranslation>
	{
		public string Name
		{
			get
			{
				return GetTranslation(p => p.Name);
			}
		}
	}

	public class GenderTranslation : EntityTranslation
	{
		public string Name { get; set; }
	}
}