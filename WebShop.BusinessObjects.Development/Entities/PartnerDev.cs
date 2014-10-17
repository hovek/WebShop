using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syrilium.Modules.BusinessObjects;
using System.Data.Entity;

namespace WebShop.BusinessObjects.Development.Entities
{
	public class PartnerDev
	{
		public static void FillData(DbSet<Partner> dbSet)
		{
			dbSet.Add(new Partner
			{
				About = "<p>Orijentacija Volksbank d.d. je financijski servis i potpora srednjim&nbsp;i malim tvrtkama, posebice onima koje su uvozno i izvozno poslovno orijentirane, predstavni&scaron;tvima inozemnih tvrtki, obrtnicima, privatnim osobama slobodnih profesija, građanima.<br /> Zahvaljujući međunarodnoj mreži, Volksbank d.d. pruža svojim klijentima ne samo financijsku podr&scaron;ku u Hrvatskoj već im pomaže u njihovim naporima za uključivanje u poslove s inozemstvom.</p>",
				Address = "Varšavska 9",
				City = "Zagreb",
				Email = "info@volksbank.hr",
				Fax = "0800 0600",
				Logo = "../../Resources/Upload/Images/Partner/Logo/ErsteLogo.png",
				Name = "ERSTE BANKA d.d. ",
				Phone = "0800 0600",
				PostalCode = "10000",
				Services = "<p>razne oblike &scaron;tednje (kunska i devizna po viđenju</a>, oročeni kunski i devizni depoziti te kunski depoziti s valutnom klauzulom, VB KLUB &scaron;tednja, bonus &scaron;tednja), kredite (nenamjenski: gotovinski, hipotekarni; namjenski: stambeni krediti, krediti za kupnju motornih vozila, razni potro&scaron;ački krediti&hellip;), stambenu &scaron;tednju u suradnji s W&uuml;stenrot stambenom &scaron;tedionicom, tekuće i žiro račune, SMS bankarstvo, trajni nalog, revolving</p>",
				URL = "http://www.volksbank.hr",
				WorkDescription = "",
                Premium = true,
                Visible = true
			});
			dbSet.Add(new Partner
			{
				About = "<p>Orijentacija Volksbank d.d. je financijski servis i potpora srednjim&nbsp;i malim tvrtkama, posebice onima koje su uvozno i izvozno poslovno orijentirane, predstavni&scaron;tvima inozemnih tvrtki, obrtnicima, privatnim osobama slobodnih profesija, građanima.<br /> Zahvaljujući međunarodnoj mreži, Volksbank d.d. pruža svojim klijentima ne samo financijsku podr&scaron;ku u Hrvatskoj već im pomaže u njihovim naporima za uključivanje u poslove s inozemstvom.</p>",
				Address = "Varšavska 9",
				City = "Zagreb",
				Email = "info@volksbank.hr",
				Fax = "0800 0600",
                Logo = "../../Resources/Upload/Images/Partner/Logo/HypoLogo.png",
				Name = "HYPO BANKA d.d. ",
				Phone = "0800 0600",
				PostalCode = "10000",
				Services = "<p>razne oblike &scaron;tednje (kunska i devizna po viđenju</a>, oročeni kunski i devizni depoziti te kunski depoziti s valutnom klauzulom, VB KLUB &scaron;tednja, bonus &scaron;tednja), kredite (nenamjenski: gotovinski, hipotekarni; namjenski: stambeni krediti, krediti za kupnju motornih vozila, razni potro&scaron;ački krediti&hellip;), stambenu &scaron;tednju u suradnji s W&uuml;stenrot stambenom &scaron;tedionicom, tekuće i žiro račune, SMS bankarstvo, trajni nalog, revolving</p>",
				URL = "http://www.volksbank.hr",
				WorkDescription = "",
                 Premium = true,
                Visible = true
			});
			dbSet.Add(new Partner
			{
				About = "<p>Privredna banka Zagreb d. d. u samom je vrhu hrvatskog bankarstva s dugim kontinuitetom bankarskog poslovanja.Osnovana je 1966. godine te je pravna slijednica Banke NRH osnovane 1962. U svim etapama svoje povijesti Privredna banka Zagreb bila je nositelj najvećih investicijskih programa u razvoju turizma, poljoprivrede, industrijalizacije, brodogradnje, elektrifikacije i cestogradnje, te je postala sinonimom za gospodarsku vitalnost, kontinuitet i identitet Hrvatske.</p>",
                Address = "Račkoga 6, 10000 Zagreb, Hrvatska",
				City = "Zagreb",
                Email = "http://www.pbz.hr/",
                Fax = "01 636 0063",
                Logo = "../../Resources/Upload/Images/Partner/Logo/PBZLogo.png",
                Name = "Privredna banka Zagreb d.d. ",
                Phone = "01 636 0000",
				PostalCode = "10000",
				Services = "<p></p>",
				URL = "http://www.volksbank.hr",
				WorkDescription = "",
                 Premium = true,
                Visible = true
			});
			dbSet.Add(new Partner
			{
				About = "<p>Orijentacija Volksbank d.d. je financijski servis i potpora srednjim&nbsp;i malim tvrtkama, posebice onima koje su uvozno i izvozno poslovno orijentirane, predstavni&scaron;tvima inozemnih tvrtki, obrtnicima, privatnim osobama slobodnih profesija, građanima.<br /> Zahvaljujući međunarodnoj mreži, Volksbank d.d. pruža svojim klijentima ne samo financijsku podr&scaron;ku u Hrvatskoj već im pomaže u njihovim naporima za uključivanje u poslove s inozemstvom.</p>",
				Address = "Varšavska 9",
				City = "Zagreb",
				Email = "info@volksbank.hr",
				Fax = "0800 0600",
                Logo = "../../Resources/Upload/Images/Partner/Logo/VolksbankLogo.png",
				Name = "VOLKSBANK d.d. ",
				Phone = "0800 0600",
				PostalCode = "10000",
				Services = "<p>razne oblike &scaron;tednje (kunska i devizna po viđenju</a>, oročeni kunski i devizni depoziti te kunski depoziti s valutnom klauzulom, VB KLUB &scaron;tednja, bonus &scaron;tednja), kredite (nenamjenski: gotovinski, hipotekarni; namjenski: stambeni krediti, krediti za kupnju motornih vozila, razni potro&scaron;ački krediti&hellip;), stambenu &scaron;tednju u suradnji s W&uuml;stenrot stambenom &scaron;tedionicom, tekuće i žiro račune, SMS bankarstvo, trajni nalog, revolving</p>",
				URL = "http://www.volksbank.hr",
				WorkDescription = "",
                 Premium = true,
                Visible = true
			});
			dbSet.Add(new Partner
			{
				About = "<p>Orijentacija Volksbank d.d. je financijski servis i potpora srednjim&nbsp;i malim tvrtkama, posebice onima koje su uvozno i izvozno poslovno orijentirane, predstavni&scaron;tvima inozemnih tvrtki, obrtnicima, privatnim osobama slobodnih profesija, građanima.<br /> Zahvaljujući međunarodnoj mreži, Volksbank d.d. pruža svojim klijentima ne samo financijsku podr&scaron;ku u Hrvatskoj već im pomaže u njihovim naporima za uključivanje u poslove s inozemstvom.</p>",
				Address = "Varšavska 9",
				City = "Zagreb",
				Email = "info@volksbank.hr",
				Fax = "0800 0600",
                Logo = "../../Resources/Upload/Images/Partner/Logo/ZabaLogo.png",
				Name = "ZAGREBACKA BANKA d.d. ",
				Phone = "0800 0600",
				PostalCode = "10000",
				Services = "<p>razne oblike &scaron;tednje (kunska i devizna po viđenju</a>, oročeni kunski i devizni depoziti te kunski depoziti s valutnom klauzulom, VB KLUB &scaron;tednja, bonus &scaron;tednja), kredite (nenamjenski: gotovinski, hipotekarni; namjenski: stambeni krediti, krediti za kupnju motornih vozila, razni potro&scaron;ački krediti&hellip;), stambenu &scaron;tednju u suradnji s W&uuml;stenrot stambenom &scaron;tedionicom, tekuće i žiro račune, SMS bankarstvo, trajni nalog, revolving</p>",
				URL = "http://www.volksbank.hr",
				WorkDescription = "",
                 Premium = true,
                Visible = true
			});


		}
	}
}