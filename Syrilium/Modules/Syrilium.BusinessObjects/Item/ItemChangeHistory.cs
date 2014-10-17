using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects.Item
{
    public class ItemChangeHistory
    {
        public int Id { get; set; }
        public DateTime DateOfEntry { get; private set; }
		public int LanguageId { get; set; }
		public int ItemId { get; set; }
        public string XmlChangeData { get; set; } 
    }
}
