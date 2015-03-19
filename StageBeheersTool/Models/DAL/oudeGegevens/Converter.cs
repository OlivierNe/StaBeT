using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StageBeheersTool.Models.Domain;
using OudeGegevens.Models;

namespace StageBeheersTool.OudeGegevens
{
    public class Converter
    {
        public static IList<Bedrijf> Bedrijven(ICollection<stagebedrijf> stagebedrijven)
        {
            IList<Bedrijf> bedrijven = new List<Bedrijf>();
            foreach (var stagebedrijf in stagebedrijven)
            {
                bedrijven.Add(new Bedrijf()
                 {
                     Naam = stagebedrijf.naam,
                     Straat = stagebedrijf.straat,
                     Gemeente = stagebedrijf.gemeente,
                     Postcode = stagebedrijf.pc,
                     Telefoonnummer = stagebedrijf.tel,
                     //website, fax ?
                     Bereikbaarheid = stagebedrijf.bereikbaarheid,
                     Bedrijfsactiviteiten = stagebedrijf.sector,
                     Contactpersonen = Contactpersonen(stagebedrijf.relatie),
                     Email = "geenEmail" + Guid.NewGuid()
                     //geen email
                 });
            }
            return bedrijven;
        }

        public static IList<Contactpersoon> Contactpersonen(ICollection<relatie> relaties)
        {
            IList<Contactpersoon> contactpersonen = new List<Contactpersoon>();
            foreach (var relatie in relaties)
            {
                contactpersonen.Add(new Contactpersoon()
                {
                    Familienaam = relatie.naam,
                    Voornaam = relatie.voornaam,
                    Bedrijfsfunctie = relatie.functie,
                    Email = relatie.email,
                    Aanspreektitel = relatie.aanspreektitel,
                    Telefoonnummer = relatie.tel,
                    Gsmnummer = relatie.GSMnummer
                });
            }
            return contactpersonen;
        }





    }
}
