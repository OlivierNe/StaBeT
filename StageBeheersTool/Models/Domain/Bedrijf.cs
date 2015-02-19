using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    //Het bedrijf moet bij registratie volgende informatie ingeven: 
    //Bedrijfsnaam – adres – url - e-mail (aanmeldingsaccount) – telefoon – 
    //bereikbaarheid (wagen – openbaar vervoer – georganiseerd vervoer door bedrijf) 
    //– soort bedrijfsactiviteiten (bank – software ontwikkelaar – openbare diensten ….)
    //Na registratie ontvangt het bedrijf een bevestigings e-mail met gegenereerd wachtwoord 
    //voor de eerste aanmelding. Bij de eerste aanmelding dient een nieuw wachtwoord te worden ingegeven.
    public class Bedrijf
    {
        #region Properties
        public int Id { get; set; }
        public string Email { get; set; } //aanmeldingsaccount
        public string Naam { get; set; } //bedrijfsnaam
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public string Straatnummer { get; set; }
        public string Telefoonnummer { get; set; }
        public string Bereikbaarheid { get; set; } //(wagen – openbaar vervoer – georganiseerd vervoer door bedrijf) 
        public string BedrijfsActiviteiten { get; set; } //(bank – software ontwikkelaar – openbare diensten ….)
        public virtual ICollection<Stageopdracht> Stageopdrachten { get; set; }
        public virtual ICollection<Contactpersoon> Contactpersonen { get; set; }
        #endregion

        #region Constructors

        public Bedrijf()
        {
            Stageopdrachten = new List<Stageopdracht>();
            Contactpersonen = new List<Contactpersoon>();
        }
        #endregion

        #region public methods
        public void AddStageopdracht(Stageopdracht stageopdracht)
        {
            Stageopdrachten.Add(stageopdracht);
        }

        public Stageopdracht FindStageopdrachtById(int id)
        {
            return Stageopdrachten.FirstOrDefault(so => so.Id == id);
        }

        public bool DeleteStageopdracht(Stageopdracht stageopdracht)
        {
            return Stageopdrachten.Remove(stageopdracht);
        }

        public bool DeleteStageopdracht(int id)
        {
            return DeleteStageopdracht(FindStageopdrachtById(id));
        }

        public bool UpdateStageopdracht(Stageopdracht stageopdracht)
        {
            var teUpdatenOpdracht = FindStageopdrachtById(stageopdracht.Id);
            if (stageopdracht != null)
            {
                teUpdatenOpdracht.Omschrijving = stageopdracht.Omschrijving;
                teUpdatenOpdracht.Titel = stageopdracht.Titel;
                teUpdatenOpdracht.Semester = stageopdracht.Semester;
                teUpdatenOpdracht.Specialisatie = stageopdracht.Specialisatie;
                teUpdatenOpdracht.Academiejaar = stageopdracht.Academiejaar;
                teUpdatenOpdracht.AantalStudenten = stageopdracht.AantalStudenten;
                teUpdatenOpdracht.AantalToegewezenStudenten = stageopdracht.AantalToegewezenStudenten;
                return true;
            }
            return false;
        }

        public void AddContactpersoon(Contactpersoon contactpersoon)
        {
            Contactpersonen.Add(contactpersoon);
        }

        public Contactpersoon FindContactpersoonById(int id)
        {
            return Contactpersonen.FirstOrDefault(cp => cp.Id == id);
        }

        public bool UpdateContactpersoon(Contactpersoon contactpersoon)
        {
            var teUpdatenPersoon = FindContactpersoonById(contactpersoon.Id);
            if (contactpersoon != null)
            {
                teUpdatenPersoon.Voornaam = contactpersoon.Voornaam;
                teUpdatenPersoon.Familienaam = contactpersoon.Familienaam;
                teUpdatenPersoon.Gsmnummer = contactpersoon.Gsmnummer;
                teUpdatenPersoon.Telefoonnummer = contactpersoon.Telefoonnummer;
                teUpdatenPersoon.IsStagementor = contactpersoon.IsStagementor;
                teUpdatenPersoon.IsContractOndertekenaar = contactpersoon.IsContractOndertekenaar;
                teUpdatenPersoon.Aanspreektitel = contactpersoon.Aanspreektitel;
                teUpdatenPersoon.Bedrijfsfunctie = contactpersoon.Bedrijfsfunctie;
                teUpdatenPersoon.Email = contactpersoon.Email;
                return true;
            }
            return false;

        }

        public bool DeleteContactpersoon(Contactpersoon contactpersoon)
        {
            return Contactpersonen.Remove(contactpersoon);
        }

        public IEnumerable<Contactpersoon> FindAllStagementors()
        {
            return Contactpersonen.Where(cp => cp.IsStagementor);
        }

        public IEnumerable<Contactpersoon> FindAllContractOndertekenaars()
        {
            return Contactpersonen.Where(cp => cp.IsContractOndertekenaar);
        }

        #endregion


    }
}