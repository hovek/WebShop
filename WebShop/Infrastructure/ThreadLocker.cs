using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Infrastructure
{
    public class ThreadLocker
    {
        public virtual ThreadLocker Get(params object[] parameters)
        {
            return new ThreadLocker();
        }
    }
}