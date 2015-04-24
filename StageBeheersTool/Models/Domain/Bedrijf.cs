using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Telefoon { get; set; }

        public Uri WebsiteUri
        {
            get
            {
                return string.IsNullOrWhiteSpace(Website) ? null : new UriBuilder(Website).Uri;
            }
        }

        public string Website { get; set; }
        public string Bereikbaarheid { get; set; } //(wagen – openbaar vervoer – georganiseerd vervoer door bedrijf) 
        public string Bedrijfsactiviteiten { get; set; } //(bank – software ontwikkelaar – openbare diensten ….)
        public virtual ICollection<Stageopdracht> Stageopdrachten { get; set; }
        public virtual ICollection<Contactpersoon> Contactpersonen { get; set; }

        public string Adres
        {
            get
            {
                return string.Format("{0} {1} {2}", Postcode, Gemeente, Straat);
            }
        }

        #endregion

        #region Constructors
        public Bedrijf()
        {
            Stageopdrachten = new List<Stageopdracht>();
            Contactpersonen = new List<Contactpersoon>();
        }

        public Bedrijf(string naam, string email)
            : this()
        {
            this.Naam = naam;
            this.Email = email;
        }
        #endregion

        #region public methods
        public void AddStageopdracht(Stageopdracht stageopdracht)
        {
            if (stageopdracht.Contractondertekenaar != null)
            {
                stageopdracht.ContractondertekenaarEmail = stageopdracht.Contractondertekenaar.Email;
                stageopdracht.ContractondertekenaarNaam = stageopdracht.Contractondertekenaar.Naam;
            }
            if (stageopdracht.Stagementor != null)
            {
                stageopdracht.StagementorEmail = stageopdracht.Stagementor.Email;
                stageopdracht.StagementorNaam = stageopdracht.Stagementor.Naam;
            }
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
            contactpersoon.Bedrijf = this;
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
            teUpdatenPersoon.Gsm = contactpersoon.Gsm;
            teUpdatenPersoon.Telefoon = contactpersoon.Telefoon;
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

        public bool MagStageopdrachtWijzigen(Stageopdracht stageopdracht, AcademiejaarInstellingen academiejaarInstellingen)
        {
            if (stageopdracht == null)
            {
                return false;
            }
            if (FindStageopdrachtById(stageopdracht.Id) == null)
            {
                return false;
            }
            //if (stageopdracht.Academiejaar == AcademiejaarHelper.HuidigAcademiejaar() == false)
            //{
            //    return false;
            //}
            //if (stageopdracht.IsAfgekeurd())
            //{
            //return true;
            //}
            if (stageopdracht.IsBeoordeeld() == false)
            {
                return true;
            }
            if (stageopdracht.IsGoedgekeurd())
            {
                if (academiejaarInstellingen == null)
                {
                    return true;
                }
                var deadline = academiejaarInstellingen.DeadlineBedrijfStageEdit;
                if (deadline == null)
                {
                    return true;
                }
                if (DateTime.Now.Date <= ((DateTime)deadline).Date)
                {
                    return true;
                }
            }
            return false;
        }

        public void KoppelContactpersoonLosVanOpdrachten(Contactpersoon contactpersoon)
        {
            var huidigAcademiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            foreach (var so in Stageopdrachten)
            {
                if (so.Stagementor != null && so.Stagementor.Equals(contactpersoon))
                {
                    if (so.Academiejaar == huidigAcademiejaar)
                    {
                        so.StagementorNaam = null;
                        so.StagementorEmail = null;
                    }
                    else
                    {
                        so.StagementorNaam = contactpersoon.Naam;
                        so.StagementorEmail = contactpersoon.Email;
                    }
                    so.Stagementor = null;
                }
                if (so.Contractondertekenaar != null && so.Contractondertekenaar.Equals(contactpersoon))
                {
                    if (so.Academiejaar == huidigAcademiejaar)
                    {
                        so.ContractondertekenaarNaam = null;
                        so.ContractondertekenaarEmail = null;
                    }
                    else
                    {
                        so.ContractondertekenaarNaam = contactpersoon.Naam;
                        so.ContractondertekenaarEmail = contactpersoon.Email;
                    }
                    so.Contractondertekenaar = null;
                }
            }
        }

        public Student FindStudent(int id)
        {
            return Stageopdrachten.SelectMany(so => so.Stages)
                    .Select(stage => stage.Student)
                    .FirstOrDefault(student => student.Id == id);
        }

        public bool HeeftGeldigEmail()
        {
            return new EmailAddressAttribute().IsValid(Email);
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