using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class DistrictCityDev
    {
        public static void FillData(DbSet<DistrictCity> dbSet)
        {
            /*Grad Zagreb*/

            DistrictCity DistrictCity1 = new DistrictCity { CountyId = 21, Name = "Donji grad", IsDistrict = true };
            dbSet.Add(DistrictCity1);

            DistrictCity DistrictCity2 = new DistrictCity { CountyId = 21, Name = "Gornji grad - Medveščak", IsDistrict = true };
            dbSet.Add(DistrictCity2);

            DistrictCity DistrictCity3 = new DistrictCity { CountyId = 21, Name = "Trnje", IsDistrict = true };
            dbSet.Add(DistrictCity3);

            DistrictCity DistrictCity4 = new DistrictCity { CountyId = 21, Name = "Maksimir", IsDistrict = true };
            dbSet.Add(DistrictCity4);

            DistrictCity DistrictCity5 = new DistrictCity { CountyId = 21, Name = "Peščenica - Žitnjak", IsDistrict = true };
            dbSet.Add(DistrictCity5);

            DistrictCity DistrictCity6 = new DistrictCity { CountyId = 21, Name = "Novi Zagreb - istok", IsDistrict = true };
            dbSet.Add(DistrictCity6);

            DistrictCity DistrictCity7 = new DistrictCity { CountyId = 21, Name = "Novi Zagreb - zapad", IsDistrict = true };
            dbSet.Add(DistrictCity7);

            DistrictCity DistrictCity8 = new DistrictCity { CountyId = 21, Name = "Trešnjevka - sjever", IsDistrict = true };
            dbSet.Add(DistrictCity8);

            DistrictCity DistrictCity9 = new DistrictCity { CountyId = 21, Name = "Trešnjevka - jug", IsDistrict = true };
            dbSet.Add(DistrictCity9);

            DistrictCity DistrictCity10 = new DistrictCity { CountyId = 21, Name = "Črnomerec", IsDistrict = true };
            dbSet.Add(DistrictCity10);

            DistrictCity DistrictCity11 = new DistrictCity { CountyId = 21, Name = "Gornja Dubrava", IsDistrict = true };
            dbSet.Add(DistrictCity11);

            DistrictCity DistrictCity12 = new DistrictCity { CountyId = 21, Name = "Donja Dubrava", IsDistrict = true };
            dbSet.Add(DistrictCity12);

            DistrictCity DistrictCity13 = new DistrictCity { CountyId = 21, Name = "Stenjevec", IsDistrict = true };
            dbSet.Add(DistrictCity13);

            DistrictCity DistrictCity14 = new DistrictCity { CountyId = 21, Name = "Podsused - Vrapče", IsDistrict = true };
            dbSet.Add(DistrictCity14);

            DistrictCity DistrictCity15 = new DistrictCity { CountyId = 21, Name = "Podsljeme", IsDistrict = true };
            dbSet.Add(DistrictCity15);

            DistrictCity DistrictCity16 = new DistrictCity { CountyId = 21, Name = "Sesvete", IsDistrict = true };
            dbSet.Add(DistrictCity16);

            DistrictCity DistrictCity17 = new DistrictCity { CountyId = 21, Name = "Brezovica", IsDistrict = true };
            dbSet.Add(DistrictCity17);

            /*Zagrebačka županija*/

            DistrictCity DistrictCity1a = new DistrictCity { CountyId = 1, Name = "Dugo Selo", IsCity = true };
            dbSet.Add(DistrictCity1a);

            DistrictCity DistrictCity2a = new DistrictCity { CountyId = 1, Name = "Ivanić Grad", IsCity = true };
            dbSet.Add(DistrictCity2a);

            DistrictCity DistrictCity3a = new DistrictCity { CountyId = 1, Name = "Jastrebarsko", IsCity = true };
            dbSet.Add(DistrictCity3a);

            DistrictCity DistrictCity4a = new DistrictCity { CountyId = 1, Name = "Samobor", IsCity = true };
            dbSet.Add(DistrictCity4a);

            DistrictCity DistrictCity5a = new DistrictCity { CountyId = 1, Name = "Sveta Nedelja", IsCity = true };
            dbSet.Add(DistrictCity5a);

            DistrictCity DistrictCity6a = new DistrictCity { CountyId = 1, Name = "Sveti Ivan Zelina", IsCity = true };
            dbSet.Add(DistrictCity6a);

            DistrictCity DistrictCity7a = new DistrictCity { CountyId = 1, Name = "Velika Gorica", IsCity = true };
            dbSet.Add(DistrictCity7a);

            DistrictCity DistrictCity8a = new DistrictCity { CountyId = 1, Name = "Vrbovec", IsCity = true };
            dbSet.Add(DistrictCity8a);

            DistrictCity DistrictCity9a = new DistrictCity { CountyId = 1, Name = "Zaprešić", IsCity = true };
            dbSet.Add(DistrictCity9a);

            DistrictCity DistrictCity10a = new DistrictCity { CountyId = 1, Name = "Bedenica", IsDistrict = true };
            dbSet.Add(DistrictCity10a);

            DistrictCity DistrictCity11a = new DistrictCity { CountyId = 1, Name = "Bistra", IsDistrict = true };
            dbSet.Add(DistrictCity11a);

            DistrictCity DistrictCity12a = new DistrictCity { CountyId = 1, Name = "Brckovljani", IsDistrict = true };
            dbSet.Add(DistrictCity12a);

            DistrictCity DistrictCity13a = new DistrictCity { CountyId = 1, Name = "Brdovec", IsDistrict = true };
            dbSet.Add(DistrictCity13a);

            DistrictCity DistrictCity14a = new DistrictCity { CountyId = 1, Name = "Dubrava", IsDistrict = true };
            dbSet.Add(DistrictCity14a);

            DistrictCity DistrictCity15a = new DistrictCity { CountyId = 1, Name = "Dubravica", IsDistrict = true };
            dbSet.Add(DistrictCity15a);

            DistrictCity DistrictCity16a = new DistrictCity { CountyId = 1, Name = "Farkaševac", IsDistrict = true };
            dbSet.Add(DistrictCity16a);

            DistrictCity DistrictCity17a = new DistrictCity { CountyId = 1, Name = "Gradec", IsDistrict = true };
            dbSet.Add(DistrictCity17a);

            DistrictCity DistrictCity18a = new DistrictCity { CountyId = 1, Name = "Jakovlje", IsDistrict = true };
            dbSet.Add(DistrictCity18a);

            DistrictCity DistrictCity19a = new DistrictCity { CountyId = 1, Name = "Klinča Sela", IsDistrict = true };
            dbSet.Add(DistrictCity19a);

            DistrictCity DistrictCity20a = new DistrictCity { CountyId = 1, Name = "Kloštar Ivanić", IsDistrict = true };
            dbSet.Add(DistrictCity20a);

            DistrictCity DistrictCity21a = new DistrictCity { CountyId = 1, Name = "Krašić", IsDistrict = true };
            dbSet.Add(DistrictCity21a);

            DistrictCity DistrictCity22a = new DistrictCity { CountyId = 1, Name = "Kravarsko", IsDistrict = true };
            dbSet.Add(DistrictCity22a);

            DistrictCity DistrictCity23a = new DistrictCity { CountyId = 1, Name = "Križ", IsDistrict = true };
            dbSet.Add(DistrictCity23a);

            DistrictCity DistrictCity24a = new DistrictCity { CountyId = 1, Name = "Luka", IsDistrict = true };
            dbSet.Add(DistrictCity24a);

            DistrictCity DistrictCity25a = new DistrictCity { CountyId = 1, Name = "Marija Gorica", IsDistrict = true };
            dbSet.Add(DistrictCity25a);

            DistrictCity DistrictCity26a = new DistrictCity { CountyId = 1, Name = "Orle", IsDistrict = true };
            dbSet.Add(DistrictCity26a);

            DistrictCity DistrictCity27a = new DistrictCity { CountyId = 1, Name = "Pisarovina", IsDistrict = true };
            dbSet.Add(DistrictCity27a);

            DistrictCity DistrictCity28a = new DistrictCity { CountyId = 1, Name = "Pokupsko", IsDistrict = true };
            dbSet.Add(DistrictCity28a);

            DistrictCity DistrictCity29a = new DistrictCity { CountyId = 1, Name = "Preseka", IsDistrict = true };
            dbSet.Add(DistrictCity29a);

            DistrictCity DistrictCity30a = new DistrictCity { CountyId = 1, Name = "Pušća", IsDistrict = true };
            dbSet.Add(DistrictCity30a);

            DistrictCity DistrictCity31a = new DistrictCity { CountyId = 1, Name = "Rakovec", IsDistrict = true };
            dbSet.Add(DistrictCity31a);

            DistrictCity DistrictCity32a = new DistrictCity { CountyId = 1, Name = "Rugvica", IsDistrict = true };
            dbSet.Add(DistrictCity32a);

            DistrictCity DistrictCity33a = new DistrictCity { CountyId = 1, Name = "Stupnik", IsDistrict = true };
            dbSet.Add(DistrictCity33a);

            DistrictCity DistrictCity34a = new DistrictCity { CountyId = 1, Name = "Žumberak", IsDistrict = true };
            dbSet.Add(DistrictCity34a);

            /*Bjelovarsko-Bilogorska županija*/

            DistrictCity DistrictCity1b = new DistrictCity { CountyId = 7, Name = "Bjelovar", IsCity = true };
            dbSet.Add(DistrictCity1b);

            DistrictCity DistrictCity2b = new DistrictCity { CountyId = 7, Name = "Čazma", IsCity = true };
            dbSet.Add(DistrictCity2b);

            DistrictCity DistrictCity3b = new DistrictCity { CountyId = 7, Name = "Daruvar", IsCity = true };
            dbSet.Add(DistrictCity3b);

            DistrictCity DistrictCity4b = new DistrictCity { CountyId = 7, Name = "Garešnica", IsCity = true };
            dbSet.Add(DistrictCity4b);

            DistrictCity DistrictCity5b = new DistrictCity { CountyId = 7, Name = "Grubišno Polje", IsCity = true };
            dbSet.Add(DistrictCity5b);

            DistrictCity DistrictCity6b = new DistrictCity { CountyId = 7, Name = "Berek", IsCity = true };
            dbSet.Add(DistrictCity6b);

            DistrictCity DistrictCity7b = new DistrictCity { CountyId = 7, Name = "Dežanovac", IsCity = true };
            dbSet.Add(DistrictCity7b);

            DistrictCity DistrictCity8b = new DistrictCity { CountyId = 7, Name = " Đulovac", IsCity = true };
            dbSet.Add(DistrictCity8b);

            DistrictCity DistrictCity9b = new DistrictCity { CountyId = 7, Name = "Hercegovac", IsCity = true };
            dbSet.Add(DistrictCity9b);

            DistrictCity DistrictCity10b = new DistrictCity { CountyId = 7, Name = "Ivanska", IsDistrict = true };
            dbSet.Add(DistrictCity10b);

            DistrictCity DistrictCity11b = new DistrictCity { CountyId = 7, Name = "Kapela", IsDistrict = true };
            dbSet.Add(DistrictCity11b);

            DistrictCity DistrictCity12b = new DistrictCity { CountyId = 7, Name = "Končanica", IsDistrict = true };
            dbSet.Add(DistrictCity12b);

            DistrictCity DistrictCity13b = new DistrictCity { CountyId = 7, Name = "Nova Rača", IsDistrict = true };
            dbSet.Add(DistrictCity13b);

            DistrictCity DistrictCity14b = new DistrictCity { CountyId = 7, Name = "Rovišće", IsDistrict = true };
            dbSet.Add(DistrictCity14b);

            DistrictCity DistrictCity15b = new DistrictCity { CountyId = 7, Name = "Severin", IsDistrict = true };
            dbSet.Add(DistrictCity15b);

            DistrictCity DistrictCity16b = new DistrictCity { CountyId = 7, Name = "Sirač", IsDistrict = true };
            dbSet.Add(DistrictCity16b);

            DistrictCity DistrictCity17b = new DistrictCity { CountyId = 7, Name = "Šandrovac", IsDistrict = true };
            dbSet.Add(DistrictCity17b);

            DistrictCity DistrictCity18b = new DistrictCity { CountyId = 7, Name = "Štefanje", IsDistrict = true };
            dbSet.Add(DistrictCity18b);

            DistrictCity DistrictCity19b = new DistrictCity { CountyId = 7, Name = "Velika Pisanica", IsDistrict = true };
            dbSet.Add(DistrictCity19b);

            DistrictCity DistrictCity20b = new DistrictCity { CountyId = 7, Name = "Veliki Grđevac", IsDistrict = true };
            dbSet.Add(DistrictCity20b);

            DistrictCity DistrictCity21b = new DistrictCity { CountyId = 7, Name = "Veliko Trojstvo", IsDistrict = true };
            dbSet.Add(DistrictCity21b);

            DistrictCity DistrictCity22b = new DistrictCity { CountyId = 7, Name = "Velika Trnovitica", IsDistrict = true };
            dbSet.Add(DistrictCity22b);

            DistrictCity DistrictCity23b = new DistrictCity { CountyId = 7, Name = "Zrinski Topolovac", IsDistrict = true };
            dbSet.Add(DistrictCity23b);

            /*Brodsko posavska*/

            DistrictCity DistrictCity1c = new DistrictCity { CountyId = 12, Name = "Nova Gradiška", IsCity = true };
            dbSet.Add(DistrictCity1c);

            DistrictCity DistrictCity2c = new DistrictCity { CountyId = 12, Name = "Slavonski Brod", IsCity = true };
            dbSet.Add(DistrictCity2c);

            DistrictCity DistrictCity3c = new DistrictCity { CountyId = 12, Name = "Bebrina", IsDistrict = true };
            dbSet.Add(DistrictCity3c);

            DistrictCity DistrictCity4c = new DistrictCity { CountyId = 12, Name = "Brodski Stupnik", IsDistrict = true };
            dbSet.Add(DistrictCity4c);

            DistrictCity DistrictCity5c = new DistrictCity { CountyId = 12, Name = "Bukovlje", IsDistrict = true };
            dbSet.Add(DistrictCity5c);

            DistrictCity DistrictCity6c = new DistrictCity { CountyId = 12, Name = "Cernik", IsDistrict = true };
            dbSet.Add(DistrictCity6c);

            DistrictCity DistrictCity7c = new DistrictCity { CountyId = 12, Name = "Davor", IsDistrict = true };
            dbSet.Add(DistrictCity7c);

            DistrictCity DistrictCity8c = new DistrictCity { CountyId = 12, Name = "Donji Andrijevci", IsDistrict = true };
            dbSet.Add(DistrictCity8c);

            DistrictCity DistrictCity9c = new DistrictCity { CountyId = 12, Name = "Dragalić", IsDistrict = true };
            dbSet.Add(DistrictCity9c);

            DistrictCity DistrictCity10c = new DistrictCity { CountyId = 12, Name = "Garčin", IsDistrict = true };
            dbSet.Add(DistrictCity10c);

            DistrictCity DistrictCity11c = new DistrictCity { CountyId = 12, Name = "Gornja Vrba", IsDistrict = true };
            dbSet.Add(DistrictCity11c);

            DistrictCity DistrictCity12c = new DistrictCity { CountyId = 12, Name = "Gornji Bogićevci", IsDistrict = true };
            dbSet.Add(DistrictCity12c);

            DistrictCity DistrictCity13c = new DistrictCity { CountyId = 12, Name = "Gundinci", IsDistrict = true };
            dbSet.Add(DistrictCity13c);

            DistrictCity DistrictCity14c = new DistrictCity { CountyId = 12, Name = "Klakar", IsDistrict = true };
            dbSet.Add(DistrictCity14c);

            DistrictCity DistrictCity15c = new DistrictCity { CountyId = 12, Name = "Nova Kapela", IsDistrict = true };
            dbSet.Add(DistrictCity15c);

            DistrictCity DistrictCity16c = new DistrictCity { CountyId = 12, Name = "Okučani", IsDistrict = true };
            dbSet.Add(DistrictCity16c);

            DistrictCity DistrictCity17c = new DistrictCity { CountyId = 12, Name = "Oprisavci", IsDistrict = true };
            dbSet.Add(DistrictCity17c);

            DistrictCity DistrictCity18c = new DistrictCity { CountyId = 12, Name = "Oriovac", IsDistrict = true };
            dbSet.Add(DistrictCity18c);

            DistrictCity DistrictCity19c = new DistrictCity { CountyId = 12, Name = "Podcrkavlje", IsDistrict = true };
            dbSet.Add(DistrictCity19c);

            DistrictCity DistrictCity20c = new DistrictCity { CountyId = 12, Name = "Rešetari", IsDistrict = true };
            dbSet.Add(DistrictCity20c);

            DistrictCity DistrictCity21c = new DistrictCity { CountyId = 12, Name = "Sibinj", IsDistrict = true };
            dbSet.Add(DistrictCity21c);

            DistrictCity DistrictCity22c = new DistrictCity { CountyId = 12, Name = "Sikirevci", IsDistrict = true };
            dbSet.Add(DistrictCity22c);

            DistrictCity DistrictCity23c = new DistrictCity { CountyId = 12, Name = "Slavonski Šamac", IsDistrict = true };
            dbSet.Add(DistrictCity23c);

            DistrictCity DistrictCity24c = new DistrictCity { CountyId = 12, Name = "Stara Gradiška", IsDistrict = true };
            dbSet.Add(DistrictCity24c);

            DistrictCity DistrictCity25c = new DistrictCity { CountyId = 12, Name = "Staro Petrovo Selo", IsDistrict = true };
            dbSet.Add(DistrictCity25c);

            DistrictCity DistrictCity26c = new DistrictCity { CountyId = 12, Name = "Velika Kopanica", IsDistrict = true };
            dbSet.Add(DistrictCity26c);

            DistrictCity DistrictCity27c = new DistrictCity { CountyId = 12, Name = "Vrbje", IsDistrict = true };
            dbSet.Add(DistrictCity27c);

            DistrictCity DistrictCity28c = new DistrictCity { CountyId = 12, Name = "Vrpolje", IsDistrict = true };
            dbSet.Add(DistrictCity28c);

            /*Dubrovačko-neretvanska županija*/

            DistrictCity DistrictCity1d = new DistrictCity { CountyId = 17, Name = "Dubrovnik", IsCity = true };
            dbSet.Add(DistrictCity1d);

            DistrictCity DistrictCity2d = new DistrictCity { CountyId = 17, Name = "Korčula", IsCity = true };
            dbSet.Add(DistrictCity2d);

            DistrictCity DistrictCity3d = new DistrictCity { CountyId = 17, Name = "Metković", IsCity = true };
            dbSet.Add(DistrictCity3d);

            DistrictCity DistrictCity4d = new DistrictCity { CountyId = 17, Name = "Opuzen", IsCity = true };
            dbSet.Add(DistrictCity4d);

            DistrictCity DistrictCity5d = new DistrictCity { CountyId = 17, Name = "Ploče", IsCity = true };
            dbSet.Add(DistrictCity5d);

            DistrictCity DistrictCity6d = new DistrictCity { CountyId = 17, Name = "Blato", IsDistrict = true };
            dbSet.Add(DistrictCity6d);

            DistrictCity DistrictCity7d = new DistrictCity { CountyId = 17, Name = "Dubrovačko primorje", IsDistrict = true };
            dbSet.Add(DistrictCity7d);

            DistrictCity DistrictCity8d = new DistrictCity { CountyId = 17, Name = "Janjina", IsDistrict = true };
            dbSet.Add(DistrictCity8d);

            DistrictCity DistrictCity9d = new DistrictCity { CountyId = 17, Name = "Konavle", IsDistrict = true };
            dbSet.Add(DistrictCity9d);

            DistrictCity DistrictCity10d = new DistrictCity { CountyId = 17, Name = "Kula Norinska", IsDistrict = true };
            dbSet.Add(DistrictCity10d);

            DistrictCity DistrictCity11d = new DistrictCity { CountyId = 17, Name = "Lastovo", IsDistrict = true };
            dbSet.Add(DistrictCity11d);

            DistrictCity DistrictCity12d = new DistrictCity { CountyId = 17, Name = "Lumbarda", IsDistrict = true };
            dbSet.Add(DistrictCity12d);

            DistrictCity DistrictCity13d = new DistrictCity { CountyId = 17, Name = "Mljet", IsDistrict = true };
            dbSet.Add(DistrictCity13d);

            DistrictCity DistrictCity14d = new DistrictCity { CountyId = 17, Name = "Orebić", IsDistrict = true };
            dbSet.Add(DistrictCity14d);

            DistrictCity DistrictCity15d = new DistrictCity { CountyId = 17, Name = "Pojezerje", IsDistrict = true };
            dbSet.Add(DistrictCity15d);

            DistrictCity DistrictCity16d = new DistrictCity { CountyId = 17, Name = "Slivno ", IsDistrict = true };
            dbSet.Add(DistrictCity16d);

            DistrictCity DistrictCity17d = new DistrictCity { CountyId = 17, Name = "Smokvica", IsDistrict = true };
            dbSet.Add(DistrictCity17d);

            DistrictCity DistrictCity18d = new DistrictCity { CountyId = 17, Name = "Ston", IsDistrict = true };
            dbSet.Add(DistrictCity18d);

            DistrictCity DistrictCity19d = new DistrictCity { CountyId = 17, Name = "Trpanj", IsDistrict = true };
            dbSet.Add(DistrictCity19d);

            DistrictCity DistrictCity20d = new DistrictCity { CountyId = 17, Name = "Vela Luka", IsDistrict = true };
            dbSet.Add(DistrictCity20d);

            DistrictCity DistrictCity21d = new DistrictCity { CountyId = 17, Name = "Zažablje", IsDistrict = true };
            dbSet.Add(DistrictCity21d);

            DistrictCity DistrictCity22d = new DistrictCity { CountyId = 17, Name = "Župa dubrovačka ", IsDistrict = true };
            dbSet.Add(DistrictCity22d);

            /*Istarska županija*/

            DistrictCity DistrictCity1e = new DistrictCity { CountyId = 15, Name = "Pula", IsCity = true };
            dbSet.Add(DistrictCity1e);

            DistrictCity DistrictCity2e = new DistrictCity { CountyId = 15, Name = "Pazin", IsCity = true };
            dbSet.Add(DistrictCity2e);

            DistrictCity DistrictCity3e = new DistrictCity { CountyId = 15, Name = "Poreč", IsCity = true };
            dbSet.Add(DistrictCity3e);

            DistrictCity DistrictCity4e = new DistrictCity { CountyId = 15, Name = "Buje", IsCity = true };
            dbSet.Add(DistrictCity4e);

            DistrictCity DistrictCity5e = new DistrictCity { CountyId = 15, Name = "Buzet", IsCity = true };
            dbSet.Add(DistrictCity5e);

            DistrictCity DistrictCity6e = new DistrictCity { CountyId = 15, Name = "Labin", IsCity = true };
            dbSet.Add(DistrictCity6e);

            DistrictCity DistrictCity7e = new DistrictCity { CountyId = 15, Name = "Novigrad", IsCity = true };
            dbSet.Add(DistrictCity7e);

            DistrictCity DistrictCity8e = new DistrictCity { CountyId = 15, Name = "Rovinj", IsCity = true };
            dbSet.Add(DistrictCity8e);

            DistrictCity DistrictCity9e = new DistrictCity { CountyId = 15, Name = "Umag", IsCity = true };
            dbSet.Add(DistrictCity9e);

            DistrictCity DistrictCity10e = new DistrictCity { CountyId = 15, Name = "Vodnjan", IsCity = true };
            dbSet.Add(DistrictCity10e);

            DistrictCity DistrictCity11e = new DistrictCity { CountyId = 15, Name = "Bale", IsDistrict = true };
            dbSet.Add(DistrictCity11e);

            DistrictCity DistrictCity12e = new DistrictCity { CountyId = 15, Name = "Barban", IsDistrict = true };
            dbSet.Add(DistrictCity12e);

            DistrictCity DistrictCity13e = new DistrictCity { CountyId = 15, Name = "Brtonigla", IsDistrict = true };
            dbSet.Add(DistrictCity13e);

            DistrictCity DistrictCity14e = new DistrictCity { CountyId = 15, Name = "Cerovlje", IsDistrict = true };
            dbSet.Add(DistrictCity14e);

            DistrictCity DistrictCity15e = new DistrictCity { CountyId = 15, Name = "Fažana", IsDistrict = true };
            dbSet.Add(DistrictCity15e);

            DistrictCity DistrictCity16e = new DistrictCity { CountyId = 15, Name = "Funtana", IsDistrict = true };
            dbSet.Add(DistrictCity16e);

            DistrictCity DistrictCity17e = new DistrictCity { CountyId = 15, Name = "Gračišće", IsDistrict = true };
            dbSet.Add(DistrictCity17e);

            DistrictCity DistrictCity18e = new DistrictCity { CountyId = 15, Name = "Grožnjan", IsDistrict = true };
            dbSet.Add(DistrictCity18e);

            DistrictCity DistrictCity19e = new DistrictCity { CountyId = 15, Name = "Kanfanar", IsDistrict = true };
            dbSet.Add(DistrictCity19e);

            DistrictCity DistrictCity20e = new DistrictCity { CountyId = 15, Name = "Karojba", IsDistrict = true };
            dbSet.Add(DistrictCity20e);

            DistrictCity DistrictCity21e = new DistrictCity { CountyId = 15, Name = "Kaštelir-Labinci", IsDistrict = true };
            dbSet.Add(DistrictCity21e);

            DistrictCity DistrictCity22e = new DistrictCity { CountyId = 15, Name = "Kršan", IsDistrict = true };
            dbSet.Add(DistrictCity22e);

            DistrictCity DistrictCity23e = new DistrictCity { CountyId = 15, Name = "Lanišće", IsDistrict = true };
            dbSet.Add(DistrictCity23e);

            DistrictCity DistrictCity24e = new DistrictCity { CountyId = 15, Name = "Ližnjan", IsDistrict = true };
            dbSet.Add(DistrictCity24e);

            DistrictCity DistrictCity25e = new DistrictCity { CountyId = 15, Name = "Lupoglav", IsDistrict = true };
            dbSet.Add(DistrictCity25e);

            DistrictCity DistrictCity26e = new DistrictCity { CountyId = 15, Name = "Marčana", IsDistrict = true };
            dbSet.Add(DistrictCity26e);

            DistrictCity DistrictCity27e = new DistrictCity { CountyId = 15, Name = "Medulin", IsDistrict = true };
            dbSet.Add(DistrictCity27e);

            DistrictCity DistrictCity28e = new DistrictCity { CountyId = 15, Name = "Motovun", IsDistrict = true };
            dbSet.Add(DistrictCity28e);

            DistrictCity DistrictCity29e = new DistrictCity { CountyId = 15, Name = "Oprtalj", IsDistrict = true };
            dbSet.Add(DistrictCity29e);

            DistrictCity DistrictCity30e = new DistrictCity { CountyId = 15, Name = "Pićan", IsDistrict = true };
            dbSet.Add(DistrictCity30e);

            DistrictCity DistrictCity31e = new DistrictCity { CountyId = 15, Name = "Raša", IsDistrict = true };
            dbSet.Add(DistrictCity31e);

            DistrictCity DistrictCity32e = new DistrictCity { CountyId = 15, Name = "Sveta Nedelja", IsDistrict = true };
            dbSet.Add(DistrictCity32e);

            DistrictCity DistrictCity33e = new DistrictCity { CountyId = 15, Name = "Sveti Lovreč", IsDistrict = true };
            dbSet.Add(DistrictCity33e);

            DistrictCity DistrictCity34e = new DistrictCity { CountyId = 15, Name = "Sveti Petar u Šumi", IsDistrict = true };
            dbSet.Add(DistrictCity34e);

            DistrictCity DistrictCity35e = new DistrictCity { CountyId = 15, Name = "Svetvinčenat", IsDistrict = true };
            dbSet.Add(DistrictCity35e);

            DistrictCity DistrictCity36e = new DistrictCity { CountyId = 15, Name = "Tar-Vabriga", IsDistrict = true };
            dbSet.Add(DistrictCity36e);

            DistrictCity DistrictCity37e = new DistrictCity { CountyId = 15, Name = "Tinjan", IsDistrict = true };
            dbSet.Add(DistrictCity37e);

            DistrictCity DistrictCity38e = new DistrictCity { CountyId = 15, Name = "Višnjan", IsDistrict = true };
            dbSet.Add(DistrictCity38e);

            DistrictCity DistrictCity39e = new DistrictCity { CountyId = 15, Name = "Vižinada", IsDistrict = true };
            dbSet.Add(DistrictCity39e);

            DistrictCity DistrictCity40e = new DistrictCity { CountyId = 15, Name = "Vrsar", IsDistrict = true };
            dbSet.Add(DistrictCity40e);

            DistrictCity DistrictCity41e = new DistrictCity { CountyId = 15, Name = "Žminj", IsDistrict = true };
            dbSet.Add(DistrictCity41e);

            /*Karlovačka županija*/

            DistrictCity DistrictCity1f = new DistrictCity { CountyId = 4, Name = "Duga Resa", IsCity = true };
            dbSet.Add(DistrictCity1f);

            DistrictCity DistrictCity2f = new DistrictCity { CountyId = 4, Name = "Karlovac", IsCity = true };
            dbSet.Add(DistrictCity2f);

            DistrictCity DistrictCity3f = new DistrictCity { CountyId = 4, Name = "Ogulin", IsCity = true };
            dbSet.Add(DistrictCity3f);

            DistrictCity DistrictCity4f = new DistrictCity { CountyId = 4, Name = "Slunj", IsCity = true };
            dbSet.Add(DistrictCity4f);

            DistrictCity DistrictCity5f = new DistrictCity { CountyId = 4, Name = "Ozalj", IsCity = true };
            dbSet.Add(DistrictCity5f);

            DistrictCity DistrictCity6f = new DistrictCity { CountyId = 4, Name = "Barilović", IsDistrict = true };
            dbSet.Add(DistrictCity6f);

            DistrictCity DistrictCity7f = new DistrictCity { CountyId = 4, Name = "Bosiljevo", IsDistrict = true };
            dbSet.Add(DistrictCity7f);

            DistrictCity DistrictCity8f = new DistrictCity { CountyId = 4, Name = "Cetingrad", IsDistrict = true };
            dbSet.Add(DistrictCity8f);

            DistrictCity DistrictCity9f = new DistrictCity { CountyId = 4, Name = "Draganić", IsDistrict = true };
            dbSet.Add(DistrictCity9f);

            DistrictCity DistrictCity10f = new DistrictCity { CountyId = 4, Name = "Generalski Stol", IsDistrict = true };
            dbSet.Add(DistrictCity10f);

            DistrictCity DistrictCity11f = new DistrictCity { CountyId = 4, Name = "Josipdol", IsDistrict = true };
            dbSet.Add(DistrictCity11f);

            DistrictCity DistrictCity12f = new DistrictCity { CountyId = 4, Name = "Kamanje", IsDistrict = true };
            dbSet.Add(DistrictCity12f);

            DistrictCity DistrictCity13f = new DistrictCity { CountyId = 4, Name = "Krnjak", IsDistrict = true };
            dbSet.Add(DistrictCity13f);

            DistrictCity DistrictCity14f = new DistrictCity { CountyId = 4, Name = "Lasinja", IsDistrict = true };
            dbSet.Add(DistrictCity14f);

            DistrictCity DistrictCity15f = new DistrictCity { CountyId = 4, Name = "Netretić", IsDistrict = true };
            dbSet.Add(DistrictCity15f);

            DistrictCity DistrictCity16f = new DistrictCity { CountyId = 4, Name = "Plaški", IsDistrict = true };
            dbSet.Add(DistrictCity16f);

            DistrictCity DistrictCity17f = new DistrictCity { CountyId = 4, Name = "Rakovica", IsDistrict = true };
            dbSet.Add(DistrictCity17f);

            DistrictCity DistrictCity18f = new DistrictCity { CountyId = 4, Name = "Ribnik", IsDistrict = true };
            dbSet.Add(DistrictCity18f);

            DistrictCity DistrictCity19f = new DistrictCity { CountyId = 4, Name = "Saborsko", IsDistrict = true };
            dbSet.Add(DistrictCity19f);

            DistrictCity DistrictCity20f = new DistrictCity { CountyId = 4, Name = "Tounj", IsDistrict = true };
            dbSet.Add(DistrictCity20f);

            DistrictCity DistrictCity21f = new DistrictCity { CountyId = 4, Name = "Vojnić", IsDistrict = true };
            dbSet.Add(DistrictCity21f);

            DistrictCity DistrictCity22f = new DistrictCity { CountyId = 4, Name = "Žakanje", IsDistrict = true };
            dbSet.Add(DistrictCity22f);

            /*Koprivničko-križevačka županija*/

            DistrictCity DistrictCity1g = new DistrictCity { CountyId = 6, Name = "Đurđevac", IsCity = true };
            dbSet.Add(DistrictCity1g);

            DistrictCity DistrictCity2g = new DistrictCity { CountyId = 6, Name = "Koprivnica", IsCity = true };
            dbSet.Add(DistrictCity2g);

            DistrictCity DistrictCity3g = new DistrictCity { CountyId = 6, Name = "Križevci", IsCity = true };
            dbSet.Add(DistrictCity3g);

            DistrictCity DistrictCity4g = new DistrictCity { CountyId = 6, Name = "Drnje", IsDistrict = true };
            dbSet.Add(DistrictCity4g);

            DistrictCity DistrictCity5g = new DistrictCity { CountyId = 6, Name = "Đelekovec", IsDistrict = true };
            dbSet.Add(DistrictCity5g);

            DistrictCity DistrictCity6g = new DistrictCity { CountyId = 6, Name = "Ferdinandovac", IsDistrict = true };
            dbSet.Add(DistrictCity6g);

            DistrictCity DistrictCity7g = new DistrictCity { CountyId = 6, Name = "Gola", IsDistrict = true };
            dbSet.Add(DistrictCity7g);

            DistrictCity DistrictCity8g = new DistrictCity { CountyId = 6, Name = "Hlebine", IsDistrict = true };
            dbSet.Add(DistrictCity8g);

            DistrictCity DistrictCity9g = new DistrictCity { CountyId = 6, Name = "Kalinovac", IsDistrict = true };
            dbSet.Add(DistrictCity9g);

            DistrictCity DistrictCity10g = new DistrictCity { CountyId = 6, Name = "Kalnik", IsDistrict = true };
            dbSet.Add(DistrictCity10g);

            DistrictCity DistrictCity11g = new DistrictCity { CountyId = 6, Name = "Kloštar Podravski", IsDistrict = true };
            dbSet.Add(DistrictCity11g);

            DistrictCity DistrictCity12g = new DistrictCity { CountyId = 6, Name = "Koprivnički Bregi", IsDistrict = true };
            dbSet.Add(DistrictCity12g);

            DistrictCity DistrictCity13g = new DistrictCity { CountyId = 6, Name = "Koprivnički Ivanec", IsDistrict = true };
            dbSet.Add(DistrictCity13g);

            DistrictCity DistrictCity14g = new DistrictCity { CountyId = 6, Name = "Legrad", IsDistrict = true };
            dbSet.Add(DistrictCity14g);

            DistrictCity DistrictCity15g = new DistrictCity { CountyId = 6, Name = "Molve", IsDistrict = true };
            dbSet.Add(DistrictCity15g);

            DistrictCity DistrictCity16g = new DistrictCity { CountyId = 6, Name = "Novigrad Podravski", IsDistrict = true };
            dbSet.Add(DistrictCity16g);

            DistrictCity DistrictCity17g = new DistrictCity { CountyId = 6, Name = "Novo Virje", IsDistrict = true };
            dbSet.Add(DistrictCity17g);

            DistrictCity DistrictCity18g = new DistrictCity { CountyId = 6, Name = "Peteranec", IsDistrict = true };
            dbSet.Add(DistrictCity18g);

            DistrictCity DistrictCity19g = new DistrictCity { CountyId = 6, Name = "Podravske Sesvete", IsDistrict = true };
            dbSet.Add(DistrictCity19g);

            DistrictCity DistrictCity20g = new DistrictCity { CountyId = 6, Name = "Rasinja", IsDistrict = true };
            dbSet.Add(DistrictCity20g);

            DistrictCity DistrictCity21g = new DistrictCity { CountyId = 6, Name = "Sokolovac", IsDistrict = true };
            dbSet.Add(DistrictCity21g);

            DistrictCity DistrictCity22g = new DistrictCity { CountyId = 6, Name = "Sveti Ivan Žabno", IsDistrict = true };
            dbSet.Add(DistrictCity22g);

            DistrictCity DistrictCity23g = new DistrictCity { CountyId = 6, Name = "Sveti Petar Orehovec", IsDistrict = true };
            dbSet.Add(DistrictCity23g);

            DistrictCity DistrictCity24g = new DistrictCity { CountyId = 6, Name = "Virje", IsDistrict = true };
            dbSet.Add(DistrictCity24g);

            /*Krapinsko-zagorska županija*/

            DistrictCity DistrictCity1h = new DistrictCity { CountyId = 2, Name = "Donja Stubica", IsCity = true };
            dbSet.Add(DistrictCity1h);

            DistrictCity DistrictCity2h = new DistrictCity { CountyId = 2, Name = "Klanjec", IsCity = true };
            dbSet.Add(DistrictCity2h);

            DistrictCity DistrictCity3h = new DistrictCity { CountyId = 2, Name = "Krapina", IsCity = true };
            dbSet.Add(DistrictCity3h);

            DistrictCity DistrictCity4h = new DistrictCity { CountyId = 2, Name = "Oroslavje", IsCity = true };
            dbSet.Add(DistrictCity4h);

            DistrictCity DistrictCity5h = new DistrictCity { CountyId = 2, Name = "Pregrada", IsCity = true };
            dbSet.Add(DistrictCity5h);

            DistrictCity DistrictCity6h = new DistrictCity { CountyId = 2, Name = "Zabok", IsCity = true };
            dbSet.Add(DistrictCity6h);

            DistrictCity DistrictCity7h = new DistrictCity { CountyId = 2, Name = "Zlatar", IsDistrict = true };
            dbSet.Add(DistrictCity7h);

            DistrictCity DistrictCity8h = new DistrictCity { CountyId = 2, Name = "Bedekovčina", IsDistrict = true };
            dbSet.Add(DistrictCity8h);

            DistrictCity DistrictCity9h = new DistrictCity { CountyId = 2, Name = "Budinščina", IsDistrict = true };
            dbSet.Add(DistrictCity9h);

            DistrictCity DistrictCity10h = new DistrictCity { CountyId = 2, Name = "Desinić", IsDistrict = true };
            dbSet.Add(DistrictCity10h);

            DistrictCity DistrictCity11h = new DistrictCity { CountyId = 2, Name = "Đurmanec", IsDistrict = true };
            dbSet.Add(DistrictCity11h);

            DistrictCity DistrictCity12h = new DistrictCity { CountyId = 2, Name = "Gornja Stubica", IsDistrict = true };
            dbSet.Add(DistrictCity12h);

            DistrictCity DistrictCity13h = new DistrictCity { CountyId = 2, Name = "Hrašćina", IsDistrict = true };
            dbSet.Add(DistrictCity13h);

            DistrictCity DistrictCity14h = new DistrictCity { CountyId = 2, Name = "Hum na Sutli", IsDistrict = true };
            dbSet.Add(DistrictCity14h);

            DistrictCity DistrictCity15h = new DistrictCity { CountyId = 2, Name = "Jesenje", IsDistrict = true };
            dbSet.Add(DistrictCity15h);

            DistrictCity DistrictCity16h = new DistrictCity { CountyId = 2, Name = "Konjščina", IsDistrict = true };
            dbSet.Add(DistrictCity16h);

            DistrictCity DistrictCity17h = new DistrictCity { CountyId = 2, Name = "Kraljevec na Sutli", IsDistrict = true };
            dbSet.Add(DistrictCity17h);

            DistrictCity DistrictCity18h = new DistrictCity { CountyId = 2, Name = "Krapinske Toplice", IsDistrict = true };
            dbSet.Add(DistrictCity18h);

            DistrictCity DistrictCity19h = new DistrictCity { CountyId = 2, Name = "Kumrovec", IsDistrict = true };
            dbSet.Add(DistrictCity19h);

            DistrictCity DistrictCity20h = new DistrictCity { CountyId = 2, Name = "Lobor", IsDistrict = true };
            dbSet.Add(DistrictCity20h);

            DistrictCity DistrictCity21h = new DistrictCity { CountyId = 2, Name = "Mače", IsDistrict = true };
            dbSet.Add(DistrictCity21h);

            DistrictCity DistrictCity22h = new DistrictCity { CountyId = 2, Name = "Marija Bistrica", IsDistrict = true };
            dbSet.Add(DistrictCity22h);

            DistrictCity DistrictCity23h = new DistrictCity { CountyId = 2, Name = "Mihovljan", IsDistrict = true };
            dbSet.Add(DistrictCity23h);

            DistrictCity DistrictCity24h = new DistrictCity { CountyId = 2, Name = "Novi Golubovec", IsDistrict = true };
            dbSet.Add(DistrictCity24h);

            DistrictCity DistrictCity25h = new DistrictCity { CountyId = 2, Name = "Petrovsko", IsDistrict = true };
            dbSet.Add(DistrictCity25h);

            DistrictCity DistrictCity26h = new DistrictCity { CountyId = 2, Name = "Radoboj", IsDistrict = true };
            dbSet.Add(DistrictCity26h);

            DistrictCity DistrictCity27h = new DistrictCity { CountyId = 2, Name = "Stubičke Toplice", IsDistrict = true };
            dbSet.Add(DistrictCity27h);

            DistrictCity DistrictCity28h = new DistrictCity { CountyId = 2, Name = "Sveti Križ Začretje", IsDistrict = true };
            dbSet.Add(DistrictCity28h);

            DistrictCity DistrictCity29h = new DistrictCity { CountyId = 2, Name = "Tuhelj", IsDistrict = true };
            dbSet.Add(DistrictCity29h);

            DistrictCity DistrictCity30h = new DistrictCity { CountyId = 2, Name = "Veliko Trgovišće", IsDistrict = true };
            dbSet.Add(DistrictCity30h);

            DistrictCity DistrictCity31h = new DistrictCity { CountyId = 2, Name = "Zagorska Sela", IsDistrict = true };
            dbSet.Add(DistrictCity31h);

            DistrictCity DistrictCity32h = new DistrictCity { CountyId = 2, Name = "Zlatar Bistrica", IsDistrict = true };
            dbSet.Add(DistrictCity32h);

            /*Ličko-senjska županija*/

            DistrictCity DistrictCity1i = new DistrictCity { CountyId = 9, Name = "Gospić", IsCity = true };
            dbSet.Add(DistrictCity1i);

            DistrictCity DistrictCity2i = new DistrictCity { CountyId = 9, Name = "Novalja", IsCity = true };
            dbSet.Add(DistrictCity2i);

            DistrictCity DistrictCity3i = new DistrictCity { CountyId = 9, Name = "Otočac", IsCity = true };
            dbSet.Add(DistrictCity3i);

            DistrictCity DistrictCity4i = new DistrictCity { CountyId = 9, Name = "Senj", IsCity = true };
            dbSet.Add(DistrictCity4i);

            DistrictCity DistrictCity5i = new DistrictCity { CountyId = 9, Name = "Brinje", IsDistrict = true };
            dbSet.Add(DistrictCity5i);

            DistrictCity DistrictCity6i = new DistrictCity { CountyId = 9, Name = "Donji Lapac", IsDistrict = true };
            dbSet.Add(DistrictCity6i);

            DistrictCity DistrictCity7i = new DistrictCity { CountyId = 9, Name = "Karlobag", IsDistrict = true };
            dbSet.Add(DistrictCity7i);

            DistrictCity DistrictCity8i = new DistrictCity { CountyId = 9, Name = "Lovinac", IsDistrict = true };
            dbSet.Add(DistrictCity8i);

            DistrictCity DistrictCity9i = new DistrictCity { CountyId = 9, Name = "Perušić", IsDistrict = true };
            dbSet.Add(DistrictCity9i);

            DistrictCity DistrictCity10i = new DistrictCity { CountyId = 9, Name = "Plitvička jezera", IsDistrict = true };
            dbSet.Add(DistrictCity10i);

            DistrictCity DistrictCity11i = new DistrictCity { CountyId = 9, Name = "Udbina", IsDistrict = true };
            dbSet.Add(DistrictCity11i);

            DistrictCity DistrictCity12i = new DistrictCity { CountyId = 9, Name = "Vrhovine", IsDistrict = true };
            dbSet.Add(DistrictCity12i);

            /*Međimurska županija*/

            DistrictCity DistrictCity1j = new DistrictCity { CountyId = 20, Name = "Čakovec", IsCity = true };
            dbSet.Add(DistrictCity1j);

            DistrictCity DistrictCity2j = new DistrictCity { CountyId = 20, Name = "Mursko Središće", IsCity = true };
            dbSet.Add(DistrictCity2j);

            DistrictCity DistrictCity3j = new DistrictCity { CountyId = 20, Name = "Prelog", IsCity = true };
            dbSet.Add(DistrictCity3j);

            DistrictCity DistrictCity4j = new DistrictCity { CountyId = 20, Name = "Belica", IsDistrict = true };
            dbSet.Add(DistrictCity4j);

            DistrictCity DistrictCity5j = new DistrictCity { CountyId = 20, Name = "Dekanovec", IsDistrict = true };
            dbSet.Add(DistrictCity5j);

            DistrictCity DistrictCity6j = new DistrictCity { CountyId = 20, Name = "Domašinec", IsDistrict = true };
            dbSet.Add(DistrictCity6j);

            DistrictCity DistrictCity7j = new DistrictCity { CountyId = 20, Name = "Donja Dubrava", IsDistrict = true };
            dbSet.Add(DistrictCity7j);

            DistrictCity DistrictCity8j = new DistrictCity { CountyId = 20, Name = "Donji Kraljevec", IsDistrict = true };
            dbSet.Add(DistrictCity8j);

            DistrictCity DistrictCity9j = new DistrictCity { CountyId = 20, Name = "Donji Vidovec", IsDistrict = true };
            dbSet.Add(DistrictCity9j);

            DistrictCity DistrictCity10j = new DistrictCity { CountyId = 20, Name = "Goričan", IsDistrict = true };
            dbSet.Add(DistrictCity10j);

            DistrictCity DistrictCity11j = new DistrictCity { CountyId = 20, Name = "Gornji Mihaljevec", IsDistrict = true };
            dbSet.Add(DistrictCity11j);

            DistrictCity DistrictCity12j = new DistrictCity { CountyId = 20, Name = "Kotoriba", IsDistrict = true };
            dbSet.Add(DistrictCity12j);

            DistrictCity DistrictCity13j = new DistrictCity { CountyId = 20, Name = "Mala Subotica", IsDistrict = true };
            dbSet.Add(DistrictCity13j);

            DistrictCity DistrictCity14j = new DistrictCity { CountyId = 20, Name = "Nedelišće", IsDistrict = true };
            dbSet.Add(DistrictCity14j);

            DistrictCity DistrictCity15j = new DistrictCity { CountyId = 20, Name = "Orehovica", IsDistrict = true };
            dbSet.Add(DistrictCity15j);

            DistrictCity DistrictCity16j = new DistrictCity { CountyId = 20, Name = "Podturen", IsDistrict = true };
            dbSet.Add(DistrictCity16j);

            DistrictCity DistrictCity17j = new DistrictCity { CountyId = 20, Name = "Pribislavec", IsDistrict = true };
            dbSet.Add(DistrictCity17j);

            DistrictCity DistrictCity18j = new DistrictCity { CountyId = 20, Name = "Selnica", IsDistrict = true };
            dbSet.Add(DistrictCity18j);

            DistrictCity DistrictCity19j = new DistrictCity { CountyId = 20, Name = "Strahoninec", IsDistrict = true };
            dbSet.Add(DistrictCity19j);

            DistrictCity DistrictCity20j = new DistrictCity { CountyId = 20, Name = "Sveta Marija", IsDistrict = true };
            dbSet.Add(DistrictCity20j);

            DistrictCity DistrictCity21j = new DistrictCity { CountyId = 20, Name = "Sveti Juraj na Bregu", IsDistrict = true };
            dbSet.Add(DistrictCity21j);

            DistrictCity DistrictCity22j = new DistrictCity { CountyId = 20, Name = "Sveti Martin na Muri", IsDistrict = true };
            dbSet.Add(DistrictCity22j);

            DistrictCity DistrictCity23j = new DistrictCity { CountyId = 20, Name = "Šenkovec", IsDistrict = true };
            dbSet.Add(DistrictCity23j);

            DistrictCity DistrictCity24j = new DistrictCity { CountyId = 20, Name = "Štrigova", IsDistrict = true };
            dbSet.Add(DistrictCity24j);

            DistrictCity DistrictCity25j = new DistrictCity { CountyId = 20, Name = "Vratišinec", IsDistrict = true };
            dbSet.Add(DistrictCity25j);

            /*Osječko-baranjska županija*/

            DistrictCity DistrictCity1k = new DistrictCity { CountyId = 18, Name = "Beli Manastir", IsCity = true };
            dbSet.Add(DistrictCity1k);

            DistrictCity DistrictCity2k = new DistrictCity { CountyId = 18, Name = "Belišće", IsCity = true };
            dbSet.Add(DistrictCity2k);

            DistrictCity DistrictCity3k = new DistrictCity { CountyId = 18, Name = "Donji Miholjac", IsCity = true };
            dbSet.Add(DistrictCity3k);

            DistrictCity DistrictCity4k = new DistrictCity { CountyId = 18, Name = "Đakovo", IsCity = true };
            dbSet.Add(DistrictCity4k);

            DistrictCity DistrictCity5k = new DistrictCity { CountyId = 18, Name = "Našice", IsCity = true };
            dbSet.Add(DistrictCity5k);

            DistrictCity DistrictCity6k = new DistrictCity { CountyId = 18, Name = "Osijek", IsCity = true };
            dbSet.Add(DistrictCity6k);

            DistrictCity DistrictCity7k = new DistrictCity { CountyId = 18, Name = "Valpovo", IsCity = true };
            dbSet.Add(DistrictCity7k);

            DistrictCity DistrictCity8k = new DistrictCity { CountyId = 18, Name = "Antunovac", IsDistrict = true };
            dbSet.Add(DistrictCity8k);

            DistrictCity DistrictCity9k = new DistrictCity { CountyId = 18, Name = "Bilje", IsDistrict = true };
            dbSet.Add(DistrictCity9k);

            DistrictCity DistrictCity10k = new DistrictCity { CountyId = 18, Name = "Bizovac", IsDistrict = true };
            dbSet.Add(DistrictCity10k);

            DistrictCity DistrictCity11k = new DistrictCity { CountyId = 18, Name = "Čeminac", IsDistrict = true };
            dbSet.Add(DistrictCity11k);

            DistrictCity DistrictCity12k = new DistrictCity { CountyId = 18, Name = "Čepin", IsDistrict = true };
            dbSet.Add(DistrictCity12k);

            DistrictCity DistrictCity13k = new DistrictCity { CountyId = 18, Name = "Darda", IsDistrict = true };
            dbSet.Add(DistrictCity13k);

            DistrictCity DistrictCity14k = new DistrictCity { CountyId = 18, Name = "Draž", IsDistrict = true };
            dbSet.Add(DistrictCity14k);

            DistrictCity DistrictCity15k = new DistrictCity { CountyId = 18, Name = "Donja Motičina", IsDistrict = true };
            dbSet.Add(DistrictCity15k);

            DistrictCity DistrictCity16k = new DistrictCity { CountyId = 18, Name = "Drenje", IsDistrict = true };
            dbSet.Add(DistrictCity16k);

            DistrictCity DistrictCity17k = new DistrictCity { CountyId = 18, Name = "Đurđenovac", IsDistrict = true };
            dbSet.Add(DistrictCity17k);

            DistrictCity DistrictCity18k = new DistrictCity { CountyId = 18, Name = "Erdut", IsDistrict = true };
            dbSet.Add(DistrictCity18k);

            DistrictCity DistrictCity19k = new DistrictCity { CountyId = 18, Name = "Ernestinovo", IsDistrict = true };
            dbSet.Add(DistrictCity19k);

            DistrictCity DistrictCity20k = new DistrictCity { CountyId = 18, Name = "Feričanci", IsDistrict = true };
            dbSet.Add(DistrictCity20k);

            DistrictCity DistrictCity21k = new DistrictCity { CountyId = 18, Name = "Gorjani", IsDistrict = true };
            dbSet.Add(DistrictCity21k);

            DistrictCity DistrictCity22k = new DistrictCity { CountyId = 18, Name = "Jagodnjak", IsDistrict = true };
            dbSet.Add(DistrictCity22k);

            DistrictCity DistrictCity23k = new DistrictCity { CountyId = 18, Name = "Kneževi Vinogradi", IsDistrict = true };
            dbSet.Add(DistrictCity23k);

            DistrictCity DistrictCity24k = new DistrictCity { CountyId = 18, Name = "Koška", IsDistrict = true };
            dbSet.Add(DistrictCity24k);

            DistrictCity DistrictCity25k = new DistrictCity { CountyId = 18, Name = "Levanjska Varoš", IsDistrict = true };
            dbSet.Add(DistrictCity25k);

            DistrictCity DistrictCity26k = new DistrictCity { CountyId = 18, Name = "Magadenovac", IsDistrict = true };
            dbSet.Add(DistrictCity26k);

            DistrictCity DistrictCity27k = new DistrictCity { CountyId = 18, Name = "Marijanci", IsDistrict = true };
            dbSet.Add(DistrictCity27k);

            DistrictCity DistrictCity28k = new DistrictCity { CountyId = 18, Name = "Podravska Moslavina", IsDistrict = true };
            dbSet.Add(DistrictCity28k);

            DistrictCity DistrictCity29k = new DistrictCity { CountyId = 18, Name = "Petlovac", IsDistrict = true };
            dbSet.Add(DistrictCity29k);

            DistrictCity DistrictCity30k = new DistrictCity { CountyId = 18, Name = "Petrijevci", IsDistrict = true };
            dbSet.Add(DistrictCity30k);

            DistrictCity DistrictCity31k = new DistrictCity { CountyId = 18, Name = "Podgorač", IsDistrict = true };
            dbSet.Add(DistrictCity31k);

            DistrictCity DistrictCity32k = new DistrictCity { CountyId = 18, Name = "Punitovci", IsDistrict = true };
            dbSet.Add(DistrictCity32k);

            DistrictCity DistrictCity33k = new DistrictCity { CountyId = 18, Name = "Popovac", IsDistrict = true };
            dbSet.Add(DistrictCity33k);

            DistrictCity DistrictCity34k = new DistrictCity { CountyId = 18, Name = "Satnica Đakovačka", IsDistrict = true };
            dbSet.Add(DistrictCity34k);

            DistrictCity DistrictCity35k = new DistrictCity { CountyId = 18, Name = "Semeljci", IsDistrict = true };
            dbSet.Add(DistrictCity35k);

            DistrictCity DistrictCity36k = new DistrictCity { CountyId = 18, Name = "Strizivojna", IsDistrict = true };
            dbSet.Add(DistrictCity36k);

            DistrictCity DistrictCity37k = new DistrictCity { CountyId = 18, Name = "Šodolovci", IsDistrict = true };
            dbSet.Add(DistrictCity37k);

            DistrictCity DistrictCity38k = new DistrictCity { CountyId = 18, Name = "Trnava", IsDistrict = true };
            dbSet.Add(DistrictCity38k);

            DistrictCity DistrictCity39k = new DistrictCity { CountyId = 18, Name = "Viljevo", IsDistrict = true };
            dbSet.Add(DistrictCity39k);

            DistrictCity DistrictCity40k = new DistrictCity { CountyId = 18, Name = "Viškovci", IsDistrict = true };
            dbSet.Add(DistrictCity40k);

            DistrictCity DistrictCity41k = new DistrictCity { CountyId = 18, Name = "Vladislavci", IsDistrict = true };
            dbSet.Add(DistrictCity41k);

            DistrictCity DistrictCity42k = new DistrictCity { CountyId = 18, Name = "Vuka", IsDistrict = true };
            dbSet.Add(DistrictCity42k);

            /*Požeško-slavonska županija*/

            DistrictCity DistrictCity1l = new DistrictCity { CountyId = 11, Name = "Kutjevo", IsCity = true };
            dbSet.Add(DistrictCity1l);

            DistrictCity DistrictCity2l = new DistrictCity { CountyId = 11, Name = "Lipik", IsCity = true };
            dbSet.Add(DistrictCity2l);

            DistrictCity DistrictCity3l = new DistrictCity { CountyId = 11, Name = "Pakrac", IsCity = true };
            dbSet.Add(DistrictCity3l);

            DistrictCity DistrictCity4l = new DistrictCity { CountyId = 11, Name = "Pleternica", IsCity = true };
            dbSet.Add(DistrictCity4l);

            DistrictCity DistrictCity5l = new DistrictCity { CountyId = 11, Name = "Požega", IsCity = true };
            dbSet.Add(DistrictCity5l);

            DistrictCity DistrictCity6l = new DistrictCity { CountyId = 11, Name = "Brestovac", IsDistrict = true };
            dbSet.Add(DistrictCity6l);

            DistrictCity DistrictCity7l = new DistrictCity { CountyId = 11, Name = "Čaglin", IsDistrict = true };
            dbSet.Add(DistrictCity7l);

            DistrictCity DistrictCity8l = new DistrictCity { CountyId = 11, Name = "Jakšić", IsDistrict = true };
            dbSet.Add(DistrictCity8l);

            DistrictCity DistrictCity9l = new DistrictCity { CountyId = 11, Name = "Kaptol", IsDistrict = true };
            dbSet.Add(DistrictCity9l);

            DistrictCity DistrictCity10l = new DistrictCity { CountyId = 11, Name = "Velika", IsDistrict = true };
            dbSet.Add(DistrictCity10l);

            /*Primorsko-goranska županija*/

            DistrictCity DistrictCity1m = new DistrictCity { CountyId = 8, Name = "Rijeka", IsCity = true };
            dbSet.Add(DistrictCity1m);

            DistrictCity DistrictCity2m = new DistrictCity { CountyId = 8, Name = "Bakar", IsCity = true };
            dbSet.Add(DistrictCity2m);

            DistrictCity DistrictCity3m = new DistrictCity { CountyId = 8, Name = "Cres", IsCity = true };
            dbSet.Add(DistrictCity3m);

            DistrictCity DistrictCity4m = new DistrictCity { CountyId = 8, Name = "Crikvenica", IsCity = true };
            dbSet.Add(DistrictCity4m);

            DistrictCity DistrictCity5m = new DistrictCity { CountyId = 8, Name = "Čabar", IsCity = true };
            dbSet.Add(DistrictCity5m);

            DistrictCity DistrictCity6m = new DistrictCity { CountyId = 8, Name = "Delnice", IsCity = true };
            dbSet.Add(DistrictCity6m);

            DistrictCity DistrictCity7m = new DistrictCity { CountyId = 8, Name = "Kastav", IsCity = true };
            dbSet.Add(DistrictCity7m);

            DistrictCity DistrictCity8m = new DistrictCity { CountyId = 8, Name = "Kraljevica", IsCity = true };
            dbSet.Add(DistrictCity8m);

            DistrictCity DistrictCity9m = new DistrictCity { CountyId = 8, Name = "Krk", IsCity = true };
            dbSet.Add(DistrictCity9m);

            DistrictCity DistrictCity10m = new DistrictCity { CountyId = 8, Name = "Mali Lošinj", IsCity = true };
            dbSet.Add(DistrictCity10m);

            DistrictCity DistrictCity11m = new DistrictCity { CountyId = 8, Name = "Novi Vinodolski", IsCity = true };
            dbSet.Add(DistrictCity11m);

            DistrictCity DistrictCity12m = new DistrictCity { CountyId = 8, Name = "Opatija", IsCity = true };
            dbSet.Add(DistrictCity12m);

            DistrictCity DistrictCity13m = new DistrictCity { CountyId = 8, Name = "Rab", IsCity = true };
            dbSet.Add(DistrictCity13m);

            DistrictCity DistrictCity14m = new DistrictCity { CountyId = 8, Name = "Vrbovsko", IsCity = true };
            dbSet.Add(DistrictCity14m);

            DistrictCity DistrictCity15m = new DistrictCity { CountyId = 8, Name = "Baška", IsDistrict = true };
            dbSet.Add(DistrictCity15m);

            DistrictCity DistrictCity16m = new DistrictCity { CountyId = 8, Name = "Brod Moravice", IsDistrict = true };
            dbSet.Add(DistrictCity16m);

            DistrictCity DistrictCity17m = new DistrictCity { CountyId = 8, Name = "Čavle", IsDistrict = true };
            dbSet.Add(DistrictCity17m);

            DistrictCity DistrictCity18m = new DistrictCity { CountyId = 8, Name = "Dobrinj", IsDistrict = true };
            dbSet.Add(DistrictCity18m);

            DistrictCity DistrictCity19m = new DistrictCity { CountyId = 8, Name = "Fužine", IsDistrict = true };
            dbSet.Add(DistrictCity19m);

            DistrictCity DistrictCity20m = new DistrictCity { CountyId = 8, Name = "Jelenje", IsDistrict = true };
            dbSet.Add(DistrictCity20m);

            DistrictCity DistrictCity21m = new DistrictCity { CountyId = 8, Name = "Klana", IsDistrict = true };
            dbSet.Add(DistrictCity21m);

            DistrictCity DistrictCity22m = new DistrictCity { CountyId = 8, Name = "Kostrena", IsDistrict = true };
            dbSet.Add(DistrictCity22m);

            DistrictCity DistrictCity23m = new DistrictCity { CountyId = 8, Name = "Lokve", IsDistrict = true };
            dbSet.Add(DistrictCity23m);

            DistrictCity DistrictCity24m = new DistrictCity { CountyId = 8, Name = "Lopar", IsDistrict = true };
            dbSet.Add(DistrictCity24m);

            DistrictCity DistrictCity25m = new DistrictCity { CountyId = 8, Name = "Lovran", IsDistrict = true };
            dbSet.Add(DistrictCity25m);

            DistrictCity DistrictCity26m = new DistrictCity { CountyId = 8, Name = "Malinska-Dubašnica", IsDistrict = true };
            dbSet.Add(DistrictCity26m);

            DistrictCity DistrictCity27m = new DistrictCity { CountyId = 8, Name = "Matulji", IsDistrict = true };
            dbSet.Add(DistrictCity27m);

            DistrictCity DistrictCity28m = new DistrictCity { CountyId = 8, Name = "Mošćenička Draga", IsDistrict = true };
            dbSet.Add(DistrictCity28m);

            DistrictCity DistrictCity29m = new DistrictCity { CountyId = 8, Name = "Mrkopalj", IsDistrict = true };
            dbSet.Add(DistrictCity29m);

            DistrictCity DistrictCity30m = new DistrictCity { CountyId = 8, Name = "Omišalj", IsDistrict = true };
            dbSet.Add(DistrictCity30m);

            DistrictCity DistrictCity31m = new DistrictCity { CountyId = 8, Name = "Punat", IsDistrict = true };
            dbSet.Add(DistrictCity31m);

            DistrictCity DistrictCity32m = new DistrictCity { CountyId = 8, Name = "Ravna Gora", IsDistrict = true };
            dbSet.Add(DistrictCity32m);

            DistrictCity DistrictCity33m = new DistrictCity { CountyId = 8, Name = "Skrad", IsDistrict = true };
            dbSet.Add(DistrictCity33m);

            DistrictCity DistrictCity34m = new DistrictCity { CountyId = 8, Name = "Vinodolska", IsDistrict = true };
            dbSet.Add(DistrictCity34m);

            DistrictCity DistrictCity35m = new DistrictCity { CountyId = 8, Name = "Viškovo", IsDistrict = true };
            dbSet.Add(DistrictCity35m);

            DistrictCity DistrictCity36m = new DistrictCity { CountyId = 8, Name = "Vrbnik", IsDistrict = true };
            dbSet.Add(DistrictCity36m);

            /*Sisačko-moslavačka županija*/

            DistrictCity DistrictCity1n = new DistrictCity { CountyId = 3, Name = "Glina", IsCity = true };
            dbSet.Add(DistrictCity1n);

            DistrictCity DistrictCity2n = new DistrictCity { CountyId = 3, Name = "Hrvatska Kostajnica", IsCity = true };
            dbSet.Add(DistrictCity2n);

            DistrictCity DistrictCity3n = new DistrictCity { CountyId = 3, Name = "Grad Petrinja", IsCity = true };
            dbSet.Add(DistrictCity3n);

            DistrictCity DistrictCity4n = new DistrictCity { CountyId = 3, Name = "Kutina", IsCity = true };
            dbSet.Add(DistrictCity4n);

            DistrictCity DistrictCity5n = new DistrictCity { CountyId = 3, Name = "Novska", IsCity = true };
            dbSet.Add(DistrictCity5n);

            DistrictCity DistrictCity6n = new DistrictCity { CountyId = 3, Name = "Sisak", IsCity = true };
            dbSet.Add(DistrictCity6n);

            DistrictCity DistrictCity7n = new DistrictCity { CountyId = 3, Name = "Donji Kukuruzari", IsDistrict = true };
            dbSet.Add(DistrictCity7n);

            DistrictCity DistrictCity8n = new DistrictCity { CountyId = 3, Name = "Dvor", IsDistrict = true };
            dbSet.Add(DistrictCity8n);

            DistrictCity DistrictCity9n = new DistrictCity { CountyId = 3, Name = "Gvozd", IsDistrict = true };
            dbSet.Add(DistrictCity9n);

            DistrictCity DistrictCity10n = new DistrictCity { CountyId = 3, Name = "Hrvatska Dubica", IsDistrict = true };
            dbSet.Add(DistrictCity10n);

            DistrictCity DistrictCity11n = new DistrictCity { CountyId = 3, Name = "Jasenovac", IsDistrict = true };
            dbSet.Add(DistrictCity11n);

            DistrictCity DistrictCity12n = new DistrictCity { CountyId = 3, Name = "Lekenik", IsDistrict = true };
            dbSet.Add(DistrictCity12n);

            DistrictCity DistrictCity13n = new DistrictCity { CountyId = 3, Name = "Lipovljani", IsDistrict = true };
            dbSet.Add(DistrictCity13n);

            DistrictCity DistrictCity14n = new DistrictCity { CountyId = 3, Name = "Majur", IsDistrict = true };
            dbSet.Add(DistrictCity14n);

            DistrictCity DistrictCity15n = new DistrictCity { CountyId = 3, Name = "Martinska Ves", IsDistrict = true };
            dbSet.Add(DistrictCity15n);

            DistrictCity DistrictCity16n = new DistrictCity { CountyId = 3, Name = "Popovača", IsDistrict = true };
            dbSet.Add(DistrictCity16n);

            DistrictCity DistrictCity17n = new DistrictCity { CountyId = 3, Name = "Sunja", IsDistrict = true };
            dbSet.Add(DistrictCity17n);

            DistrictCity DistrictCity18n = new DistrictCity { CountyId = 3, Name = "Topusko", IsDistrict = true };
            dbSet.Add(DistrictCity18n);

            DistrictCity DistrictCity19n = new DistrictCity { CountyId = 3, Name = "Velika Ludina", IsDistrict = true };
            dbSet.Add(DistrictCity19n);

            /*Splitsko-dalmatinska županija*/


            DistrictCity DistrictCity1o = new DistrictCity { CountyId = 14, Name = "Hvar", IsCity = true };
            dbSet.Add(DistrictCity1o);

            DistrictCity DistrictCity2o = new DistrictCity { CountyId = 14, Name = "Imotski", IsCity = true };
            dbSet.Add(DistrictCity2o);

            DistrictCity DistrictCity3o = new DistrictCity { CountyId = 14, Name = "Kaštela", IsCity = true };
            dbSet.Add(DistrictCity3o);

            DistrictCity DistrictCity4o = new DistrictCity { CountyId = 14, Name = "Komiža", IsCity = true };
            dbSet.Add(DistrictCity4o);

            DistrictCity DistrictCity5o = new DistrictCity { CountyId = 14, Name = "Makarska", IsCity = true };
            dbSet.Add(DistrictCity5o);

            DistrictCity DistrictCity6o = new DistrictCity { CountyId = 14, Name = "Omiš", IsCity = true };
            dbSet.Add(DistrictCity6o);

            DistrictCity DistrictCity7o = new DistrictCity { CountyId = 14, Name = "Sinj", IsCity = true };
            dbSet.Add(DistrictCity7o);

            DistrictCity DistrictCity8o = new DistrictCity { CountyId = 14, Name = "Solin", IsCity = true };
            dbSet.Add(DistrictCity8o);

            DistrictCity DistrictCity9o = new DistrictCity { CountyId = 14, Name = "Split", IsCity = true };
            dbSet.Add(DistrictCity9o);

            DistrictCity DistrictCity10o = new DistrictCity { CountyId = 14, Name = "Stari Grad", IsCity = true };
            dbSet.Add(DistrictCity10o);

            DistrictCity DistrictCity11o = new DistrictCity { CountyId = 14, Name = "Supetar", IsCity = true };
            dbSet.Add(DistrictCity11o);

            DistrictCity DistrictCity12o = new DistrictCity { CountyId = 14, Name = "Trilj", IsCity = true };
            dbSet.Add(DistrictCity12o);

            DistrictCity DistrictCity13o = new DistrictCity { CountyId = 14, Name = "Trogir", IsCity = true };
            dbSet.Add(DistrictCity13o);

            DistrictCity DistrictCity14o = new DistrictCity { CountyId = 14, Name = "Vis", IsCity = true };
            dbSet.Add(DistrictCity14o);

            DistrictCity DistrictCity15o = new DistrictCity { CountyId = 14, Name = "Vrgorac", IsCity = true };
            dbSet.Add(DistrictCity15o);

            DistrictCity DistrictCity16o = new DistrictCity { CountyId = 14, Name = "Vrlika", IsCity = true };
            dbSet.Add(DistrictCity16o);

            DistrictCity DistrictCity17o = new DistrictCity { CountyId = 14, Name = "Baška Voda", IsDistrict = true };
            dbSet.Add(DistrictCity17o);

            DistrictCity DistrictCity18o = new DistrictCity { CountyId = 14, Name = "Bol", IsDistrict = true };
            dbSet.Add(DistrictCity18o);

            DistrictCity DistrictCity19o = new DistrictCity { CountyId = 14, Name = "Brela", IsDistrict = true };
            dbSet.Add(DistrictCity19o);

            DistrictCity DistrictCity20o = new DistrictCity { CountyId = 14, Name = "Cista Provo", IsDistrict = true };
            dbSet.Add(DistrictCity20o);

            DistrictCity DistrictCity21o = new DistrictCity { CountyId = 14, Name = "Dicmo", IsDistrict = true };
            dbSet.Add(DistrictCity21o);

            DistrictCity DistrictCity22o = new DistrictCity { CountyId = 14, Name = "Dugi Rat", IsDistrict = true };
            dbSet.Add(DistrictCity22o);

            DistrictCity DistrictCity23o = new DistrictCity { CountyId = 14, Name = "Dugopolje", IsDistrict = true };
            dbSet.Add(DistrictCity23o);

            DistrictCity DistrictCity24o = new DistrictCity { CountyId = 14, Name = "Gradac", IsDistrict = true };
            dbSet.Add(DistrictCity24o);

            DistrictCity DistrictCity25o = new DistrictCity { CountyId = 14, Name = "Hrvace", IsDistrict = true };
            dbSet.Add(DistrictCity25o);

            DistrictCity DistrictCity26o = new DistrictCity { CountyId = 14, Name = "Jelsa", IsDistrict = true };
            dbSet.Add(DistrictCity26o);

            DistrictCity DistrictCity27o = new DistrictCity { CountyId = 14, Name = "Klis", IsDistrict = true };
            dbSet.Add(DistrictCity27o);

            DistrictCity DistrictCity28o = new DistrictCity { CountyId = 14, Name = "Lećevica", IsDistrict = true };
            dbSet.Add(DistrictCity28o);

            DistrictCity DistrictCity29o = new DistrictCity { CountyId = 14, Name = "Lokvičići", IsDistrict = true };
            dbSet.Add(DistrictCity29o);

            DistrictCity DistrictCity30o = new DistrictCity { CountyId = 14, Name = "Lovreć", IsDistrict = true };
            dbSet.Add(DistrictCity30o);

            DistrictCity DistrictCity31o = new DistrictCity { CountyId = 14, Name = "Marina", IsDistrict = true };
            dbSet.Add(DistrictCity31o);

            DistrictCity DistrictCity32o = new DistrictCity { CountyId = 14, Name = "Milna", IsDistrict = true };
            dbSet.Add(DistrictCity32o);

            DistrictCity DistrictCity33o = new DistrictCity { CountyId = 14, Name = "Muć", IsDistrict = true };
            dbSet.Add(DistrictCity33o);

            DistrictCity DistrictCity34o = new DistrictCity { CountyId = 14, Name = "Nerežišća", IsDistrict = true };
            dbSet.Add(DistrictCity34o);

            DistrictCity DistrictCity35o = new DistrictCity { CountyId = 14, Name = "Okrug", IsDistrict = true };
            dbSet.Add(DistrictCity35o);

            DistrictCity DistrictCity36o = new DistrictCity { CountyId = 14, Name = "Otok", IsDistrict = true };
            dbSet.Add(DistrictCity36o);

            DistrictCity DistrictCity37o = new DistrictCity { CountyId = 14, Name = "Podbablje", IsDistrict = true };
            dbSet.Add(DistrictCity37o);

            DistrictCity DistrictCity38o = new DistrictCity { CountyId = 14, Name = "Podgora", IsDistrict = true };
            dbSet.Add(DistrictCity38o);

            DistrictCity DistrictCity39o = new DistrictCity { CountyId = 14, Name = "Podstrana", IsDistrict = true };
            dbSet.Add(DistrictCity39o);

            DistrictCity DistrictCity40o = new DistrictCity { CountyId = 14, Name = "Postira", IsDistrict = true };
            dbSet.Add(DistrictCity40o);

            DistrictCity DistrictCity41o = new DistrictCity { CountyId = 14, Name = "Prgomet", IsDistrict = true };
            dbSet.Add(DistrictCity41o);

            DistrictCity DistrictCity42o = new DistrictCity { CountyId = 14, Name = "Primorski Dolac", IsDistrict = true };
            dbSet.Add(DistrictCity42o);

            DistrictCity DistrictCity43o = new DistrictCity { CountyId = 14, Name = "Proložac", IsDistrict = true };
            dbSet.Add(DistrictCity43o);

            DistrictCity DistrictCity44o = new DistrictCity { CountyId = 14, Name = "Pučišća", IsDistrict = true };
            dbSet.Add(DistrictCity44o);

            DistrictCity DistrictCity45o = new DistrictCity { CountyId = 14, Name = "Runovići", IsDistrict = true };
            dbSet.Add(DistrictCity45o);

            DistrictCity DistrictCity46o = new DistrictCity { CountyId = 14, Name = "Seget", IsDistrict = true };
            dbSet.Add(DistrictCity46o);

            DistrictCity DistrictCity47o = new DistrictCity { CountyId = 14, Name = "Selca", IsDistrict = true };
            dbSet.Add(DistrictCity47o);

            DistrictCity DistrictCity48o = new DistrictCity { CountyId = 14, Name = "Sućuraj", IsDistrict = true };
            dbSet.Add(DistrictCity48o);

            DistrictCity DistrictCity49o = new DistrictCity { CountyId = 14, Name = "Sutivan", IsDistrict = true };
            dbSet.Add(DistrictCity49o);

            DistrictCity DistrictCity50o = new DistrictCity { CountyId = 14, Name = "Šestanovac", IsDistrict = true };
            dbSet.Add(DistrictCity50o);

            DistrictCity DistrictCity51o = new DistrictCity { CountyId = 14, Name = "Šolta", IsDistrict = true };
            dbSet.Add(DistrictCity51o);

            DistrictCity DistrictCity52o = new DistrictCity { CountyId = 14, Name = "Tučepi", IsDistrict = true };
            dbSet.Add(DistrictCity52o);

            DistrictCity DistrictCity53o = new DistrictCity { CountyId = 14, Name = "Zadvarje", IsDistrict = true };
            dbSet.Add(DistrictCity53o);

            DistrictCity DistrictCity54o = new DistrictCity { CountyId = 14, Name = "Zagvozd", IsDistrict = true };
            dbSet.Add(DistrictCity54o);

            DistrictCity DistrictCity55o = new DistrictCity { CountyId = 14, Name = "Zmijavci", IsDistrict = true };
            dbSet.Add(DistrictCity55o);

            /*Šibensko-kninska županija*/

            DistrictCity DistrictCity1p = new DistrictCity { CountyId = 16, Name = "Drniš", IsCity = true };
            dbSet.Add(DistrictCity1p);

            DistrictCity DistrictCity2p = new DistrictCity { CountyId = 16, Name = "Knin", IsCity = true };
            dbSet.Add(DistrictCity2p);

            DistrictCity DistrictCity3p = new DistrictCity { CountyId = 16, Name = "Skradin", IsCity = true };
            dbSet.Add(DistrictCity3p);

            DistrictCity DistrictCity4p = new DistrictCity { CountyId = 16, Name = "Šibenik", IsCity = true };
            dbSet.Add(DistrictCity4p);

            DistrictCity DistrictCity5p = new DistrictCity { CountyId = 16, Name = "Vodice", IsCity = true };
            dbSet.Add(DistrictCity5p);

            DistrictCity DistrictCity6p = new DistrictCity { CountyId = 16, Name = "Bilice", IsDistrict = true };
            dbSet.Add(DistrictCity6p);

            DistrictCity DistrictCity7p = new DistrictCity { CountyId = 16, Name = "Civljane", IsDistrict = true };
            dbSet.Add(DistrictCity7p);

            DistrictCity DistrictCity8p = new DistrictCity { CountyId = 16, Name = "Ervenik", IsDistrict = true };
            dbSet.Add(DistrictCity8p);

            DistrictCity DistrictCity9p = new DistrictCity { CountyId = 16, Name = "Kijevo", IsDistrict = true };
            dbSet.Add(DistrictCity9p);

            DistrictCity DistrictCity10p = new DistrictCity { CountyId = 16, Name = "Kistanje", IsDistrict = true };
            dbSet.Add(DistrictCity10p);

            DistrictCity DistrictCity11p = new DistrictCity { CountyId = 16, Name = "Promina", IsDistrict = true };
            dbSet.Add(DistrictCity11p);

            DistrictCity DistrictCity12p = new DistrictCity { CountyId = 16, Name = "Biskupija", IsDistrict = true };
            dbSet.Add(DistrictCity12p);

            DistrictCity DistrictCity13p = new DistrictCity { CountyId = 16, Name = "Pirovac", IsDistrict = true };
            dbSet.Add(DistrictCity13p);

            DistrictCity DistrictCity14p = new DistrictCity { CountyId = 16, Name = "Primošten", IsDistrict = true };
            dbSet.Add(DistrictCity14p);

            DistrictCity DistrictCity15p = new DistrictCity { CountyId = 16, Name = "Rogoznica", IsDistrict = true };
            dbSet.Add(DistrictCity15p);

            DistrictCity DistrictCity16p = new DistrictCity { CountyId = 16, Name = "Ružić", IsDistrict = true };
            dbSet.Add(DistrictCity16p);

            DistrictCity DistrictCity17p = new DistrictCity { CountyId = 16, Name = "Tisno", IsDistrict = true };
            dbSet.Add(DistrictCity17p);

            DistrictCity DistrictCity18p = new DistrictCity { CountyId = 16, Name = "Murter", IsDistrict = true };
            dbSet.Add(DistrictCity18p);

            DistrictCity DistrictCity19p = new DistrictCity { CountyId = 16, Name = "Unešić", IsDistrict = true };
            dbSet.Add(DistrictCity19p);


            /*Varaždinska županija*/

            DistrictCity DistrictCity1r = new DistrictCity { CountyId = 5, Name = "Ivanec", IsCity = true };
            dbSet.Add(DistrictCity1r);

            DistrictCity DistrictCity2r = new DistrictCity { CountyId = 5, Name = "Lepoglava", IsCity = true };
            dbSet.Add(DistrictCity2r);

            DistrictCity DistrictCity3r = new DistrictCity { CountyId = 5, Name = "Ludbreg", IsCity = true };
            dbSet.Add(DistrictCity3r);

            DistrictCity DistrictCity4r = new DistrictCity { CountyId = 5, Name = "Novi Marof", IsCity = true };
            dbSet.Add(DistrictCity4r);

            DistrictCity DistrictCity5r = new DistrictCity { CountyId = 5, Name = "Varaždin", IsCity = true };
            dbSet.Add(DistrictCity5r);

            DistrictCity DistrictCity6r = new DistrictCity { CountyId = 5, Name = "Varaždinske Toplice", IsCity = true };
            dbSet.Add(DistrictCity6r);

            DistrictCity DistrictCity7r = new DistrictCity { CountyId = 5, Name = "Bednja", IsDistrict = true };
            dbSet.Add(DistrictCity7r);

            DistrictCity DistrictCity8r = new DistrictCity { CountyId = 5, Name = "Breznica", IsDistrict = true };
            dbSet.Add(DistrictCity8r);

            DistrictCity DistrictCity9r = new DistrictCity { CountyId = 5, Name = "Breznički Hum", IsDistrict = true };
            dbSet.Add(DistrictCity9r);

            DistrictCity DistrictCity10r = new DistrictCity { CountyId = 5, Name = "Beretinec", IsDistrict = true };
            dbSet.Add(DistrictCity10r);

            DistrictCity DistrictCity11r = new DistrictCity { CountyId = 5, Name = "Cestica", IsDistrict = true };
            dbSet.Add(DistrictCity11r);

            DistrictCity DistrictCity12r = new DistrictCity { CountyId = 5, Name = "Donja Voća", IsDistrict = true };
            dbSet.Add(DistrictCity12r);

            DistrictCity DistrictCity13r = new DistrictCity { CountyId = 5, Name = "Gornji Kneginec", IsDistrict = true };
            dbSet.Add(DistrictCity13r);

            DistrictCity DistrictCity14r = new DistrictCity { CountyId = 5, Name = "Jalžabet", IsDistrict = true };
            dbSet.Add(DistrictCity14r);

            DistrictCity DistrictCity15r = new DistrictCity { CountyId = 5, Name = "Klenovnik", IsDistrict = true };
            dbSet.Add(DistrictCity15r);

            DistrictCity DistrictCity16r = new DistrictCity { CountyId = 5, Name = "Ljubešćica", IsDistrict = true };
            dbSet.Add(DistrictCity16r);

            DistrictCity DistrictCity17r = new DistrictCity { CountyId = 5, Name = "Mali Bukovec", IsDistrict = true };
            dbSet.Add(DistrictCity17r);

            DistrictCity DistrictCity18r = new DistrictCity { CountyId = 5, Name = "Martijanec", IsDistrict = true };
            dbSet.Add(DistrictCity18r);

            DistrictCity DistrictCity19r = new DistrictCity { CountyId = 5, Name = "Maruševec", IsDistrict = true };
            dbSet.Add(DistrictCity19r);

            DistrictCity DistrictCity20r = new DistrictCity { CountyId = 5, Name = "Petrijanec", IsDistrict = true };
            dbSet.Add(DistrictCity20r);

            DistrictCity DistrictCity21r = new DistrictCity { CountyId = 5, Name = "Sračinec", IsDistrict = true };
            dbSet.Add(DistrictCity21r);

            DistrictCity DistrictCity22r = new DistrictCity { CountyId = 5, Name = "Sveti Đurđ", IsDistrict = true };
            dbSet.Add(DistrictCity22r);

            DistrictCity DistrictCity23r = new DistrictCity { CountyId = 5, Name = "Sveti Ilija", IsDistrict = true };
            dbSet.Add(DistrictCity23r);

            DistrictCity DistrictCity24r = new DistrictCity { CountyId = 5, Name = "Trnovec Bartolovečki", IsDistrict = true };
            dbSet.Add(DistrictCity24r);

            DistrictCity DistrictCity25r = new DistrictCity { CountyId = 5, Name = "Veliki Bukovec", IsDistrict = true };
            dbSet.Add(DistrictCity25r);

            DistrictCity DistrictCity26r = new DistrictCity { CountyId = 5, Name = "Vidovec", IsDistrict = true };
            dbSet.Add(DistrictCity26r);

            DistrictCity DistrictCity27r = new DistrictCity { CountyId = 5, Name = "Vinica", IsDistrict = true };
            dbSet.Add(DistrictCity27r);

            DistrictCity DistrictCity28r = new DistrictCity { CountyId = 5, Name = "Visoko", IsDistrict = true };
            dbSet.Add(DistrictCity28r);

            /*Virovitičko-podravska županija*/

            DistrictCity DistrictCity1s = new DistrictCity { CountyId = 10, Name = "Orahovica", IsCity = true };
            dbSet.Add(DistrictCity1s);

            DistrictCity DistrictCity2s = new DistrictCity { CountyId = 10, Name = "Slatina", IsCity = true };
            dbSet.Add(DistrictCity2s);

            DistrictCity DistrictCity3s = new DistrictCity { CountyId = 10, Name = "Virovitica", IsCity = true };
            dbSet.Add(DistrictCity3s);

            DistrictCity DistrictCity4s = new DistrictCity { CountyId = 10, Name = "Crnac", IsDistrict = true };
            dbSet.Add(DistrictCity4s);

            DistrictCity DistrictCity5s = new DistrictCity { CountyId = 10, Name = "Čačinci", IsDistrict = true };
            dbSet.Add(DistrictCity5s);

            DistrictCity DistrictCity6s = new DistrictCity { CountyId = 10, Name = "Čađavica", IsDistrict = true };
            dbSet.Add(DistrictCity6s);

            DistrictCity DistrictCity7s = new DistrictCity { CountyId = 10, Name = "Gradina", IsDistrict = true };
            dbSet.Add(DistrictCity7s);

            DistrictCity DistrictCity8s = new DistrictCity { CountyId = 10, Name = "Lukač", IsDistrict = true };
            dbSet.Add(DistrictCity8s);

            DistrictCity DistrictCity9s = new DistrictCity { CountyId = 10, Name = "Mikleuš", IsDistrict = true };
            dbSet.Add(DistrictCity9s);

            DistrictCity DistrictCity10s = new DistrictCity { CountyId = 10, Name = "Nova Bukovica", IsDistrict = true };
            dbSet.Add(DistrictCity10s);

            DistrictCity DistrictCity11s = new DistrictCity { CountyId = 10, Name = "Pitomača", IsDistrict = true };
            dbSet.Add(DistrictCity11s);

            DistrictCity DistrictCity12s = new DistrictCity { CountyId = 10, Name = "Sopje", IsDistrict = true };
            dbSet.Add(DistrictCity12s);

            DistrictCity DistrictCity13s = new DistrictCity { CountyId = 10, Name = "Suhopolje", IsDistrict = true };
            dbSet.Add(DistrictCity13s);

            DistrictCity DistrictCity14s = new DistrictCity { CountyId = 10, Name = "Špišić Bukovica", IsDistrict = true };
            dbSet.Add(DistrictCity14s);

            DistrictCity DistrictCity15s = new DistrictCity { CountyId = 10, Name = "Voćin", IsDistrict = true };
            dbSet.Add(DistrictCity15s);

            DistrictCity DistrictCity16s = new DistrictCity { CountyId = 10, Name = "Zdenci", IsDistrict = true };
            dbSet.Add(DistrictCity16s);


            /*Vukovarsko-srijemska županija*/

            DistrictCity DistrictCity1t = new DistrictCity { CountyId = 13, Name = "Vukovar", IsCity = true };
            dbSet.Add(DistrictCity1t);

            DistrictCity DistrictCity2t = new DistrictCity { CountyId = 13, Name = "Vinkovci", IsCity = true };
            dbSet.Add(DistrictCity2t);

            DistrictCity DistrictCity3t = new DistrictCity { CountyId = 13, Name = "Ilok", IsCity = true };
            dbSet.Add(DistrictCity3t);

            DistrictCity DistrictCity4t = new DistrictCity { CountyId = 13, Name = "Županja", IsCity = true };
            dbSet.Add(DistrictCity4t);

            DistrictCity DistrictCity5t = new DistrictCity { CountyId = 13, Name = "Otok", IsCity = true };
            dbSet.Add(DistrictCity5t);

            DistrictCity DistrictCity6t = new DistrictCity { CountyId = 13, Name = "Andrijaševci", IsDistrict = true };
            dbSet.Add(DistrictCity6t);

            DistrictCity DistrictCity7t = new DistrictCity { CountyId = 13, Name = "Babina Greda", IsDistrict = true };
            dbSet.Add(DistrictCity7t);

            DistrictCity DistrictCity8t = new DistrictCity { CountyId = 13, Name = "Bogdanovci", IsDistrict = true };
            dbSet.Add(DistrictCity8t);

            DistrictCity DistrictCity9t = new DistrictCity { CountyId = 13, Name = "Borovo", IsDistrict = true };
            dbSet.Add(DistrictCity9t);

            DistrictCity DistrictCity10t = new DistrictCity { CountyId = 13, Name = "Bošnjaci", IsDistrict = true };
            dbSet.Add(DistrictCity10t);

            DistrictCity DistrictCity11t = new DistrictCity { CountyId = 13, Name = "Cerna", IsDistrict = true };
            dbSet.Add(DistrictCity11t);

            DistrictCity DistrictCity12t = new DistrictCity { CountyId = 13, Name = "Drenovci", IsDistrict = true };
            dbSet.Add(DistrictCity12t);

            DistrictCity DistrictCity13t = new DistrictCity { CountyId = 13, Name = "Gradište", IsDistrict = true };
            dbSet.Add(DistrictCity13t);

            DistrictCity DistrictCity14t = new DistrictCity { CountyId = 13, Name = "Gunja", IsDistrict = true };
            dbSet.Add(DistrictCity14t);

            DistrictCity DistrictCity15t = new DistrictCity { CountyId = 13, Name = "Ivankovo", IsDistrict = true };
            dbSet.Add(DistrictCity15t);

            DistrictCity DistrictCity16t = new DistrictCity { CountyId = 13, Name = "Jarmina", IsDistrict = true };
            dbSet.Add(DistrictCity16t);

            DistrictCity DistrictCity17t = new DistrictCity { CountyId = 13, Name = "Lovas", IsDistrict = true };
            dbSet.Add(DistrictCity17t);

            DistrictCity DistrictCity18t = new DistrictCity { CountyId = 13, Name = "Markušica", IsDistrict = true };
            dbSet.Add(DistrictCity18t);

            DistrictCity DistrictCity19t = new DistrictCity { CountyId = 13, Name = "Negoslavci", IsDistrict = true };
            dbSet.Add(DistrictCity19t);

            DistrictCity DistrictCity20t = new DistrictCity { CountyId = 13, Name = "Nijemci", IsDistrict = true };
            dbSet.Add(DistrictCity20t);

            DistrictCity DistrictCity21t = new DistrictCity { CountyId = 13, Name = "Nuštar", IsDistrict = true };
            dbSet.Add(DistrictCity21t);

            DistrictCity DistrictCity22t = new DistrictCity { CountyId = 13, Name = "Privlaka", IsDistrict = true };
            dbSet.Add(DistrictCity22t);

            DistrictCity DistrictCity23t = new DistrictCity { CountyId = 13, Name = "Stari Jankovci", IsDistrict = true };
            dbSet.Add(DistrictCity23t);

            DistrictCity DistrictCity24t = new DistrictCity { CountyId = 13, Name = "Stari Mikanovci", IsDistrict = true };
            dbSet.Add(DistrictCity24t);

            DistrictCity DistrictCity25t = new DistrictCity { CountyId = 13, Name = "Štitar", IsDistrict = true };
            dbSet.Add(DistrictCity25t);

            DistrictCity DistrictCity26t = new DistrictCity { CountyId = 13, Name = "Tompojevci", IsDistrict = true };
            dbSet.Add(DistrictCity26t);

            DistrictCity DistrictCity27t = new DistrictCity { CountyId = 13, Name = "Tordinci", IsDistrict = true };
            dbSet.Add(DistrictCity27t);

            DistrictCity DistrictCity28t = new DistrictCity { CountyId = 13, Name = "Tovarnik", IsDistrict = true };
            dbSet.Add(DistrictCity28t);

            DistrictCity DistrictCity29t = new DistrictCity { CountyId = 13, Name = "Trpinja", IsDistrict = true };
            dbSet.Add(DistrictCity29t);

            DistrictCity DistrictCity30t = new DistrictCity { CountyId = 13, Name = "Vođinci", IsDistrict = true };
            dbSet.Add(DistrictCity30t);

            DistrictCity DistrictCity31t = new DistrictCity { CountyId = 13, Name = "Vrbanja", IsDistrict = true };
            dbSet.Add(DistrictCity31t);

            /*Zadarska županija*/

            DistrictCity DistrictCity1u = new DistrictCity { CountyId = 19, Name = "Zadar", IsCity = true };
            dbSet.Add(DistrictCity1u);

            DistrictCity DistrictCity2u = new DistrictCity { CountyId = 19, Name = "Benkovac", IsCity = true };
            dbSet.Add(DistrictCity2u);

            DistrictCity DistrictCity3u = new DistrictCity { CountyId = 19, Name = "Biograd na Moru", IsCity = true };
            dbSet.Add(DistrictCity3u);

            DistrictCity DistrictCity4u = new DistrictCity { CountyId = 19, Name = "Nin", IsCity = true };
            dbSet.Add(DistrictCity4u);

            DistrictCity DistrictCity5u = new DistrictCity { CountyId = 19, Name = "Obrovac", IsCity = true };
            dbSet.Add(DistrictCity5u);

            DistrictCity DistrictCity6u = new DistrictCity { CountyId = 19, Name = "Pag", IsCity = true };
            dbSet.Add(DistrictCity6u);

            DistrictCity DistrictCity7u = new DistrictCity { CountyId = 19, Name = "Bibinje", IsDistrict = true };
            dbSet.Add(DistrictCity7u);

            DistrictCity DistrictCity8u = new DistrictCity { CountyId = 19, Name = "Galovac", IsDistrict = true };
            dbSet.Add(DistrictCity8u);

            DistrictCity DistrictCity9u = new DistrictCity { CountyId = 19, Name = "Gračac", IsDistrict = true };
            dbSet.Add(DistrictCity9u);

            DistrictCity DistrictCity10u = new DistrictCity { CountyId = 19, Name = "Jasenice", IsDistrict = true };
            dbSet.Add(DistrictCity10u);

            DistrictCity DistrictCity11u = new DistrictCity { CountyId = 19, Name = "Kali", IsDistrict = true };
            dbSet.Add(DistrictCity11u);

            DistrictCity DistrictCity12u = new DistrictCity { CountyId = 19, Name = "Kolan", IsDistrict = true };
            dbSet.Add(DistrictCity12u);

            DistrictCity DistrictCity13u = new DistrictCity { CountyId = 19, Name = "Kukljica", IsDistrict = true };
            dbSet.Add(DistrictCity13u);

            DistrictCity DistrictCity14u = new DistrictCity { CountyId = 19, Name = "Lišane Ostrovičke", IsDistrict = true };
            dbSet.Add(DistrictCity14u);

            DistrictCity DistrictCity15u = new DistrictCity { CountyId = 19, Name = "Novigrad", IsDistrict = true };
            dbSet.Add(DistrictCity15u);

            DistrictCity DistrictCity16u = new DistrictCity { CountyId = 19, Name = "Pakoštane", IsDistrict = true };
            dbSet.Add(DistrictCity16u);

            DistrictCity DistrictCity17u = new DistrictCity { CountyId = 19, Name = "Pašman", IsDistrict = true };
            dbSet.Add(DistrictCity17u);

            DistrictCity DistrictCity18u = new DistrictCity { CountyId = 19, Name = "Polača", IsDistrict = true };
            dbSet.Add(DistrictCity18u);

            DistrictCity DistrictCity19u = new DistrictCity { CountyId = 19, Name = "Poličnik", IsDistrict = true };
            dbSet.Add(DistrictCity19u);

            DistrictCity DistrictCity20u = new DistrictCity { CountyId = 19, Name = "Posedarje", IsDistrict = true };
            dbSet.Add(DistrictCity20u);

            DistrictCity DistrictCity21u = new DistrictCity { CountyId = 19, Name = "Povljana", IsDistrict = true };
            dbSet.Add(DistrictCity21u);

            DistrictCity DistrictCity22u = new DistrictCity { CountyId = 19, Name = "Preko", IsDistrict = true };
            dbSet.Add(DistrictCity22u);

            DistrictCity DistrictCity23u = new DistrictCity { CountyId = 19, Name = "Privlaka", IsDistrict = true };
            dbSet.Add(DistrictCity23u);

            DistrictCity DistrictCity24u = new DistrictCity { CountyId = 19, Name = "Ražanac", IsDistrict = true };
            dbSet.Add(DistrictCity24u);

            DistrictCity DistrictCity25u = new DistrictCity { CountyId = 19, Name = "Sali", IsDistrict = true };
            dbSet.Add(DistrictCity25u);

            DistrictCity DistrictCity26u = new DistrictCity { CountyId = 19, Name = "Stankovci", IsDistrict = true };
            dbSet.Add(DistrictCity26u);

            DistrictCity DistrictCity27u = new DistrictCity { CountyId = 19, Name = "Starigrad", IsDistrict = true };
            dbSet.Add(DistrictCity27u);

            DistrictCity DistrictCity28u = new DistrictCity { CountyId = 19, Name = "Sukošan", IsDistrict = true };
            dbSet.Add(DistrictCity28u);

            DistrictCity DistrictCity29u = new DistrictCity { CountyId = 19, Name = "Sveti Filip i Jakov", IsDistrict = true };
            dbSet.Add(DistrictCity29u);

            DistrictCity DistrictCity30u = new DistrictCity { CountyId = 19, Name = "Škabrnja", IsDistrict = true };
            dbSet.Add(DistrictCity30u);

            DistrictCity DistrictCity31u = new DistrictCity { CountyId = 19, Name = "Tkon", IsDistrict = true };
            dbSet.Add(DistrictCity31u);

            DistrictCity DistrictCity32u = new DistrictCity { CountyId = 19, Name = "Vir", IsDistrict = true };
            dbSet.Add(DistrictCity32u);

            DistrictCity DistrictCity33u = new DistrictCity { CountyId = 19, Name = "Vrsi", IsDistrict = true };
            dbSet.Add(DistrictCity33u);

            DistrictCity DistrictCity34u = new DistrictCity { CountyId = 19, Name = "Zemunik Donji", IsDistrict = true };
            dbSet.Add(DistrictCity34u);
        }
    }
}
