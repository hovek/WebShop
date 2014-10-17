using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class CountyDev
    {
        public static void FillData(DbSet<County> dbSet)
        {

            County county1 = new County { Name = "Zagrebačka", NameShort="ZGZ" };
            dbSet.Add(county1);

            County county2 = new County { Name = "Krapinsko-zagorska", NameShort = "KZZ" };
            dbSet.Add(county2);

            County county3 = new County { Name = "Sisačko-moslavačka", NameShort = "SMZ" };
            dbSet.Add(county3);

            County county4 = new County { Name = "Karlovačka", NameShort = "KZ" };
            dbSet.Add(county4);

            County county5 = new County { Name = "Varaždinska", NameShort = "VZ" };
            dbSet.Add(county5);

            County county6 = new County { Name = "Koprivničko-križevačka", NameShort = "KKZ" };
            dbSet.Add(county6);

            County county7 = new County { Name = "Bjelovarsko-bilogorska", NameShort = "BBZ" };
            dbSet.Add(county7);

            County county8 = new County { Name = "Primorsko-goranska", NameShort = "PGZ" };
            dbSet.Add(county8);

            County county9 = new County { Name = "Ličko-senjska", NameShort = "LSZ" };
            dbSet.Add(county9);

            County county10 = new County { Name = "Virovitičko-podravska", NameShort = "VPZ" };
            dbSet.Add(county10);

            County county11 = new County { Name = "Požeško-slavonska", NameShort = "PSZ" };
            dbSet.Add(county11);

            County county12 = new County { Name = "Brodsko-posavska", NameShort = "BPZ" };
            dbSet.Add(county12);

            County county13 = new County { Name = "Vukovarsko-srijemska", NameShort = "VSZ" };
            dbSet.Add(county13);

            County county14 = new County { Name = "Splitsko-dalmatinska", NameShort = "SDZ" };
            dbSet.Add(county14);

            County county15 = new County { Name = "Istarska", NameShort = "IZ" };
            dbSet.Add(county15);

            County county16 = new County { Name = "Šibensko-kninska", NameShort = "SKZ" };
            dbSet.Add(county16);

            County county17 = new County { Name = "Dubrovačko-neretvanska", NameShort = "DNZ" };
            dbSet.Add(county17);

            County county18 = new County { Name = "Osječko-baranjska", NameShort = "OBZ" };
            dbSet.Add(county18);

            County county19 = new County { Name = "Zadarska", NameShort = "ZZ" };
            dbSet.Add(county19);

            County county20 = new County { Name = "Međimurska", NameShort = "MZ" };
            dbSet.Add(county20);

            County county21 = new County { Name = "Grad Zagreb", NameShort = "GZG" };
            dbSet.Add(county21);

        }
    }
}
