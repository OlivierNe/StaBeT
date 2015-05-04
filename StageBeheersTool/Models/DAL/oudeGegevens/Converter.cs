using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNet.Identity;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using OudeGegevens.Models;
using StageBeheersTool.Models.Identity;

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

            //var website = new UriBuilder(stagebedrijf.website).Uri;

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

        public static Student ToStudent(student student, ApplicationUserManager manager = null)
        {
            string[] formats = { "dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "dd/MM/yy" };
            DateTime gebdatum;
            DateTime? geboortedatum;
            var geboorteplaats = student.gebplaats;

            if (student.gebdatum != null &&
                DateTime.TryParseExact(student.gebdatum.Trim(), formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out gebdatum))
            {
                geboortedatum = gebdatum;
            }
            else if (student.gebplaats != null &&
                DateTime.TryParseExact(student.gebplaats.Trim(), formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out gebdatum))//gebdatum en gebplaats omgewisseld in oude gegevens
            {
                geboortedatum = gebdatum;
                geboorteplaats = student.gebdatum;
            }
            else
            {
                geboortedatum = null;
            }

            if (manager != null)
            {
                var acadj = AcademiejaarHelper.HuidigAcademiejaar();
                if (acadj == student.acjaar)
                {
                    var user = new ApplicationUser()
                    {
                        Email = student.email,
                        UserName = student.email,
                        EmailConfirmed = true
                    };
                    manager.Create(user);
                    manager.AddToRole(user.Id, Role.Student);
                }
            }

            return new Student()
            {
                HogentEmail = student.email ?? "geenEmail" + Guid.NewGuid(),
                Email = student.email2,
                Voornaam = student.voornaam,
                Familienaam = student.naam,
                Gemeente = student.gemeente != null ? student.gemeente.Trim() : student.gemeente,
                Straat = student.straat,
                Postcode = student.pc,
                Gsm = student.GSM,
                Telefoon = student.telefoon,
                Geboortedatum = geboortedatum,
                Geboorteplaats = geboorteplaats
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
                    return StageopdrachtStatus.Goedgekeurd;
                case "C": // C = Created
                    return StageopdrachtStatus.NietBeoordeeld;
                case "NA": //Not Approved (?)
                    return StageopdrachtStatus.Afgekeurd;
                case "G": //Granted
                case null:
                    return StageopdrachtStatus.Toegewezen;
                default: //betaald, ...
                    return StageopdrachtStatus.Goedgekeurd;
            }
        }

    }
}