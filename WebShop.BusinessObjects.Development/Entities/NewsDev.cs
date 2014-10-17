using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Syrilium.Modules.BusinessObjects;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class NewsDev
    {
        public static void FillData(DbSet<News> dbSet)
        {

            News news1 = new News { Title = "Ovrhe: Dnevno pet nekretnina na dražbi", Autor="Ovrhe.hr", IntroductionText = "Do kraja godine sudovi će u Hrvatskoj kroz ovršni postupak pokušati prodati tristotinjak kuća, stanova i manjih zgrada s nekoliko stambenih jedinica čiji vlasnici nisu uspjeli platiti svoje obveze, bilo da je riječ o otplati kredita ili nekih drugih dugova.", Text = "Do kraja godine sudovi će u Hrvatskoj kroz ovršni postupak pokušati prodati tristotinjak kuća, stanova i manjih zgrada s nekoliko stambenih jedinica čiji vlasnici nisu uspjeli platiti svoje obveze, bilo da je riječ o otplati kredita ili nekih drugih dugova. Procijenjena vrijednost te imovine bez koje u gotovo svim slučajevima ostaju građani, a ne poduzeća, je oko 250 milijuna kuna, piše Novi list. No ne znači da će se nekretnine prodati za tu cijenu, jer se za njihovu prodaju organiziraju dražbe na kojima zainteresirani licitiraju, a sud nastoji postići što veću cijenu. Ako nitko ne želi kupiti nekretninu po njezinoj procijenjenoj vrijednosti, cijena se spušta ovisno o tome koji se krug dražbe organizira, pa se stan ili kuća mogu prodati za tri četvrtine, polovicu ili primjerice jednu trećinu vrijednosti. Samo u ovom tjednu održat će se dražbe za sedamdesetak kuća i desetak stanova. Do kraja godine, u nešto više od šezdesetak dana, na bubanj će ukupno 54 stana iz cijele Hrvatske te 230 kuća, ako u međuvremenu sudovi ne donesu nove odluke o ovrhama i taj broj se ne poveća. Procijenjena vrijednost tih stanova je gotovo 36 milijuna kuna, a kuće su vrijedne više od 190 milijuna kuna. U ovrhama se prodaje i dvadesetak manjih zgrada za više od 20 milijuna kuna, a vlasnici su opet uglavnom fizičke osobe koje su se upustile u jednokratni građevinski pothvat ili sud obiteljsku kuću s više stanova koju su vlasnici gradili za sebe, tretira kao zgradu. Sve to vidljivo je iz Očevidnika nekretnina koji se vodi pri Hrvatskoj gospodarskoj komori. U taj očevidnik trenutačno su ubilježene 553 nekretnine, što znači da je u 55 posto slučajeva, gdje se nakon ovrhe prodaje imovina, riječ o kućama i stanovima. Da bez svoje imovine zbog novčanih problema ostaju građani pokazuje i dosadašnja statistika HGK prema kojoj su od 2005. godine do početka listopada ove godine u 93 posto slučajeva fizičke osobe, odnosno građani i dijelom obrtnici, ostajali bez svoje imovine u ovršnim postupcima. Naplatu svojih potraživanja u najvećem broju slučajeva, 40 posto, tražile su banke, kartičarske kuće, osiguravajuća društva i štedno-kreditne zadruge. Nakon toga dug u 25 posto slučajeva su utjerivali drugi građani, a u 25 posto slučajeva državne institucije. Najjeftiniji stan koji se prodaje na dražbi procijenjen je na 41 tisuću kuna, a nalazi se u općini Pleternica i ima 35 četvornih metra. Najskuplji je pak stan od 175 četvornih metra u zagrebačkim Remetama kojeg je vještak u sudskom postupku procijenio na gotovo dva milijuna kuna. Što se tiče kuća, najvrjednija iz očevidnika je kuća iz Velike Gorice od gotovo 400 kvadrata s dvorištem i procijenjena je na 22 milijuna kuna, ali se na dražbi prodaje samo jedna četvrtina koja je u vlasništvu osobe protiv koje se provodi ovrha. Osim toga, trenutno je na dražbi i stotinjak livada i oranica, a dijelom je to posljedica problema koje poljoprivrednici imaju u otplati kredita koje su podizali radi širenja ili poboljšanja proizvodnje. U očevidniku nekretnina najavljuju se i dražbe za pokretnine, ali trenutno ih je 13, a među njima je i prodaja zrakoplova tvrtke AIR Adria u stečaju a početna cijena mu je nešto veća od tri milijuna kuna.", ImageUrl = "1.jpg", Date = DateTime.Now , Visible=true };
            dbSet.Add(news1);
            News news2 = new News { Title = "Vlada popravlja ovršni zakon 2", Text = "Opis2", ImageUrl = "2.jpg", Date = DateTime.Now , Visible=true };
            dbSet.Add(news2);
            News news3 = new News { Title = "Vlada popravlja ovršni zakon 3", Text = "Opis3", ImageUrl = "3.jpg", Date = DateTime.Now, Visible = true };
            dbSet.Add(news3);
            News news4 = new News { Title = "Vlada popravlja ovršni zakon 4", Text = "Opis4", ImageUrl = "4.jpg", Date = DateTime.Now, Visible = true };
            dbSet.Add(news4);
        }
    }
}
