using System;
using System.Collections.Generic;
using System.Linq;
using StageBeheersTool.Helpers;

namespace StageBeheersTool.Models.Domain
{

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
        public string Bedrijfsactiviteiten { get; set; } //(bank – software ontwikkelaar – openbare diensten ….)
        public virtual ICollection<Stageopdracht> Stageopdrachten { get; set; }
        public virtual ICollection<Contactpersoon> Contactpersonen { get; set; }

        public string Adres
        {
            get
            {
                return string.Format("{0} {1}, {2} {3}", Postcode, Gemeente, Straat, Straatnummer);
            }
        }

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


        public bool UpdateStageopdracht(Stageopdracht stageopdracht)
        {
            var teUpdatenOpdracht = FindStageopdrachtById(stageopdracht.Id);
            if (teUpdatenOpdracht != null)
            {
                teUpdatenOpdracht.Omschrijving = stageopdracht.Omschrijving;
                teUpdatenOpdracht.Titel = stageopdracht.Titel;
                teUpdatenOpdracht.Semester1 = stageopdracht.Semester1;
                teUpdatenOpdracht.Semester2 = stageopdracht.Semester2;
                teUpdatenOpdracht.Specialisatie = stageopdracht.Specialisatie;
                teUpdatenOpdracht.Academiejaar = stageopdracht.Academiejaar;
                teUpdatenOpdracht.AantalStudenten = stageopdracht.AantalStudenten;
                teUpdatenOpdracht.Stagementor = stageopdracht.Stagementor;
                teUpdatenOpdracht.Contractondertekenaar = stageopdracht.Contractondertekenaar;
                teUpdatenOpdracht.Gemeente = stageopdracht.Gemeente;
                teUpdatenOpdracht.Postcode = stageopdracht.Postcode;
                teUpdatenOpdracht.Straat = stageopdracht.Straat;
                teUpdatenOpdracht.Straatnummer = stageopdracht.Straatnummer;
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
            if (teUpdatenPersoon == null)
                return false;
            teUpdatenPersoon.Voornaam = contactpersoon.Voornaam;
            teUpdatenPersoon.Familienaam = contactpersoon.Familienaam;
            teUpdatenPersoon.Gsmnummer = contactpersoon.Gsmnummer;
            teUpdatenPersoon.Telefoonnummer = contactpersoon.Telefoonnummer;
            teUpdatenPersoon.IsStagementor = contactpersoon.IsStagementor;
            teUpdatenPersoon.IsContractondertekenaar = contactpersoon.IsContractondertekenaar;
            teUpdatenPersoon.Aanspreektitel = contactpersoon.Aanspreektitel;
            teUpdatenPersoon.Bedrijfsfunctie = contactpersoon.Bedrijfsfunctie;
            teUpdatenPersoon.Email = contactpersoon.Email;
            return true;
        }

        public IEnumerable<Contactpersoon> FindAllStagementors()
        {
            return Contactpersonen.Where(cp => cp.IsStagementor);
        }

        public IEnumerable<Contactpersoon> FindAllContractOndertekenaars()
        {
            return Contactpersonen.Where(cp => cp.IsContractondertekenaar);
        }


        public bool ContactpersoonHeeftStageopdrachten(Contactpersoon contactpersoon)
        {
            foreach (var so in Stageopdrachten)
            {
                if (so.Stagementor.Equals(contactpersoon) || so.Contractondertekenaar.Equals(contactpersoon))
                {
                    return true;
                }
            }
            return false;
        }

        public bool MagWijzigen(Stageopdracht opdracht, DateTime? deadline)
        {
            if (opdracht == null)
            {
                return false;
            }
            if (deadline == null)
            {
                return true;
            }
            if (!opdracht.Academiejaar.Equals(AcademiejaarHelper.HuidigAcademiejaar()))
            {
                return false;
            }
            if (DateTime.Now.Date <= ((DateTime)deadline).Date)
            {
                return true;
            }
            return false;
        }

        protected bool Equals(Bedrijf other)
        {
            return string.Equals(other.Email, Email) && string.Equals(other.Naam, Naam);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
                return Equals((Bedrijf)obj);
        }

        public override int GetHashCode()
        {
            return (Email != null ? Email.GetHashCode() : 0);
        }
        #endregion

    }
}