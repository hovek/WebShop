using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
    public class PageLocation : TranslatableEntity<PageLocationTranslation>
    {
        public string Name { get { return GetTranslation(p => p.Name); } }
        public string Description { get { return GetTranslation(p => p.Description); } }
        public virtual List<Page> Page { get; set; }
    }

    public class PageLocationTranslation : EntityTranslation
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}