using Syrilium.Modules.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class ImageFormatDev
    {
        public static void FillData(DbSet<ImageFormat> dbSet)
        {
            dbSet.Add(new ImageFormat
            {
                Group = "Product",
                Name = "W500H300",
                Width = 500,
                Height = 300
            });
            dbSet.Add(new ImageFormat
            {
                Group = "Product",
                Name = "W150H120",
                Width = 150,
                Height = 120
            });
            dbSet.Add(new ImageFormat
            {
                Group = "Product",
                Name = "W85H65",
                Width = 85,
                Height = 65
            });
            dbSet.Add(new ImageFormat
            {
                Group = "Product",
                Name = "W278H230",
                Width = 278,
                Height = 230
            });
            dbSet.Add(new ImageFormat
            {
                Group = "Product",
                Name = "W90H60",
                Width = 90,
                Height = 60
            });
            dbSet.Add(new ImageFormat
            {
                Group = "News",
                Name = "W250H200",
                Width = 250,
                Height = 200
            });
        }
    }
}
