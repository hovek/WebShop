using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects;
using System.Data.Entity;
using System.Data.SqlTypes;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class UserDev
    {
        public static void FillData(DbSet<User> dbSet)
        {
            dbSet.Add(new User
            {
                LoginId = 1 , Address="Ulica", City="Grad", Email="admin@email.com", Name="Ime", Surname="Prezime" , DateOfBirth = (DateTime)SqlDateTime.MinValue
            });
        }
    }
}
