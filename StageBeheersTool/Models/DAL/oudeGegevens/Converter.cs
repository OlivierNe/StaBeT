using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OudeGegevens;
using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Domain;
using OudeGegevens.Models;

namespace StageBeheersTool.OudeGegevens
{
    public class Converter
    {
        public static Bedrijf ToBedrijf(stagebedrijf stagebedrijf)
        {
            var contactpersonen = stagebedrijf.relatie.Select(ToContactpersoon).ToList();
            String email = "";
            foreach (var contact in contactpersonen)
            {
                if (contact.Email != String.Empty) email = contact.Email;
                break;
            }
            return new Bedrijf()
            {
                Naam = stagebedrijf.naam,
                Straat = stagebedrijf.straat,
                Gemeente = stagebedrijf.gemeente,
                Postcode = stagebedrijf.pc,
                Telefoon = stagebedrijf.tel,
                Website = stagebedrijf.website,
                Bereikbaarheid = stagebedrijf.bereikbaarheid,
                Bedrijfsactiviteiten = stagebedrijf.sector,
                Contactpersonen = contactpersonen,
                Email = (!String.IsNullOrEmpty(email)) ? email : ""
            };
        }

        public static Contactpersoon ToContactpersoon(relatie relatie)
        {
            return new Contactpersoon()
            {
                Familienaam = relatie.naam,
                Voornaam = relatie.voornaam,
                Bedrijfsfunctie = relatie.functie,
                Email = relatie.email,
                Aanspreektitel = relatie.aanspreektitel,
                Telefoon = relatie.tel,
                Gsm = relatie.GSMnummer
            };
        }

        public static Student ToStudent(student student)
        {
            return new Student()
            {
                HogentEmail = student.email ?? "geenEmail" + Guid.NewGuid(),
                Email = student.email2,
                Voornaam = student.voornaam,
                Familienaam = student.naam,
                Gemeente = student.gemeente,
                Straat = student.straat,
                Postcode = student.pc,
                Gsm = student.GSM,
                Telefoon = student.telefoon
            };
        }

        public static Begeleider ToBegeleider(docent docent)
        {
            return new Begeleider()
            {
                HogentEmail = docent.email,
                Voornaam = docent.voornaam,
                Familienaam = docent.naam,
                Straat = docent.straat,
                Gemeente = docent.gemeente,
                Postcode = docent.pc,
                Telefoon = docent.telefoon,
                Aanspreking = docent.aanspreking
            };
        }

        public static Stageopdracht ToStageopdracht(stage stage)
        {
            return new Stageopdracht()
            {
                Titel = stage.titel,
                Omschrijving = stage.omschrijving ?? "",
                AantalStudenten = stage.aantalStudenten ?? 0,
                Straat = stage.straat,
                Gemeente = stage.gemeente,
                Postcode = stage.pc,
                Status = ToStageopdrachtStatus(stage.statusOpdracht),
                Academiejaar = stage.acjaar,
                Specialisatie = stage.typeStage,
                Semester2 = true
            };
        }

        public static StageopdrachtStatus ToStageopdrachtStatus(string status)
        {
            switch (status)
            {
                case "A": //Approved
                case "C": // C = Created
                case "BETAALD":
                    return StageopdrachtStatus.Afgekeurd;
                case "G": //Granted
                    return StageopdrachtStatus.Goedgekeurd;
                default: //C, betaald, NA, null
                    return StageopdrachtStatus.NietBeoordeeld;
            }
        }

    }
}
