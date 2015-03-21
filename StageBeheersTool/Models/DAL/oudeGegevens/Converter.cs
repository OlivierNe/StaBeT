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
                Telefoonnummer = stagebedrijf.tel,
                //website, fax toevoegen?
                Bereikbaarheid = stagebedrijf.bereikbaarheid,
                Bedrijfsactiviteiten = stagebedrijf.sector,
                Contactpersonen = contactpersonen,
                Email = (!String.IsNullOrEmpty(email)) ? email : ""
                //Stageopdrachten = GetStageopdrachten(stagebedrijf.stage)
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
                Telefoonnummer = relatie.tel,
                Gsmnummer = relatie.GSMnummer
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
                Gsmnummer = student.GSM,
                Telefoonnummer = student.telefoon
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
                Telefoonnummer = docent.telefoon,
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
                //mentor, contractondertekenaar, docent
                Status = ToStageopdrachtStatus(stage.statusOpdracht),
                Academiejaar = stage.acjaar
                //Stagebegeleider = begeleider,
                // Studenten = studentenVanDezeStage
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
        /*
        public IList<Bedrijf> Bedrijven(ICollection<stagebedrijf> stagebedrijven)
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
                     ,
                    // Stageopdrachten = GetStageopdrachten(stagebedrijf.stage)
                 });
            }
            return bedrijven;
        }

        public static StageopdrachtStatus ToStageopdrachtStatus(string status)
        {
            switch (status)
            {
                case "A":
                case "C": // C = ?
                case "BETAALD":
                    return StageopdrachtStatus.Afgekeurd;
                case "G":
                    return StageopdrachtStatus.Goedgekeurd;
                default: //C, betaald, NA, null
                    return StageopdrachtStatus.NietBeoordeeld;
            }
        }

        public IList<Contactpersoon> Contactpersonen(ICollection<relatie> relaties)
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

       /* public IList<Stageopdracht> GetStageopdrachten(ICollection<stage> stages)
        {
            var stageopdrachten = new List<Stageopdracht>();
            var begeleiders = stageDbContext.Begeleiders.ToList();
            var studenten = stageDbContext.Studenten.ToList();

            foreach (var stage in stages)
            {
                var begeleider = begeleiders
                    .FirstOrDefault(b => b.HogentEmail == (stage.docent == null ? null : stage.docent.email));
                var oudeStudenten = stage.studenten;
                var studentenVanDezeStage = new List<Student>();
                foreach (var oudeStudent in oudeStudenten)
                {
                    studentenVanDezeStage.Add(studenten.FirstOrDefault(s => s.Voornaam == oudeStudent.voornaam && s.Familienaam == oudeStudent.naam));
                }
                stageopdrachten.Add(new Stageopdracht()
                {
                    Titel = stage.titel,
                    Omschrijving = stage.omschrijving ?? "",
                    AantalStudenten = stage.aantalStudenten ?? 0,
                    Straat = stage.straat,
                    Gemeente = stage.gemeente,
                    Postcode = stage.pc,
                    //mentor, contractondertekenaar, docent
                    Status = GetStageopdrachtStatus(stage.statusOpdracht),
                    Academiejaar = stage.acjaar,
                    Stagebegeleider = begeleider,
                    Studenten = studentenVanDezeStage
                });

            }
            return stageopdrachten;
        }*/


        /*
        public IList<Begeleider> GetBegeleiders(ICollection<docent> docenten)
        {
            var begeleiders = new List<Begeleider>();
            foreach (var docent in docenten)
            {
                begeleiders.Add(new Begeleider()
                {
                    HogentEmail = docent.email,
                    Voornaam = docent.voornaam,
                    Familienaam = docent.naam,
                    Straat = docent.straat,
                    Gemeente = docent.gemeente,
                    Postcode = docent.pc,
                    Telefoonnummer = docent.telefoon,
                    Aanspreking = docent.aanspreking
                });
            }
            return begeleiders;
        }


        public IList<Student> GetStudenten(List<student> studentenOud)
        {
            var studenten = new List<Student>();
            foreach (var student in studentenOud)
            {
                var newStudent = new Student()
                {
                    HogentEmail = student.email ?? "geenEmail" + Guid.NewGuid(),
                    Email = student.email2,
                    Voornaam = student.voornaam,
                    Familienaam = student.naam,
                    Gemeente = student.gemeente,
                    Straat = student.straat,
                    Postcode = student.pc,
                    Gsmnummer = student.GSM,
                    Telefoonnummer = student.telefoon
                };
               // stageDbContext.Studenten.AddOrUpdate(newStudent);
                studenten.Add(newStudent);
            }
            return studenten;
        }*/
    }
}
