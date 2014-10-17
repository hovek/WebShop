using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syrilium.Modules.BusinessObjects
{
    public class ImageFormat
    {
        public int Id { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual List<ImageFormat> Get(string group = null)
        {
            return WebShopDb.I.ImageFormat.Where(f => group == null || f.Group == group).ToList();
        }
    }
}
