using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Stageopdracht
    {

        #region Properties
        public int Id { get; set; }
        public string Titel { get; set; }
        public string Omschrijving { get; set; }
        public virtual Specialisatie Specialisatie { get; set; }

        public string ToonSpecialisatie
        {
            get
            {
                if (Specialisatie == null) { return ""; }
                else { return Specialisatie.Naam; }
            }
        }

        public int Semester { get; set; }
        public int AantalStudenten { get; set; }
        public int AantalToegewezenStudenten { get; set; }
        public string Academiejaar { get; set; }
        public virtual Contactpersoon ContractOndertekenaar { get; set; }
        public virtual Contactpersoon Stagementor { get; set; }
        public virtual Bedrijf Bedrijf { get; set; }
        public StageopdrachtStatus Status { get; set; }
        #endregion

        #region Constructors
        public Stageopdracht()
        {
            Status = StageopdrachtStatus.NietBeoordeeld;
        }
        #endregion

        #region Public methods
        public bool IsGoedgekeurd()
        {
            return Status == StageopdrachtStatus.Goedgekeurd;
        }

        public bool IsInHuidigAcademiejaar()
        {
            var beginJaar = int.Parse(Academiejaar.Substring(0, 4));
            var eindJaar = int.Parse(Academiejaar.Substring(5, 4));
            if ((beginJaar == DateTime.Now.Year && DateTime.Now.Month >= 9)
                || (eindJaar == (beginJaar + 1)
                && DateTime.Now.Month < 9))
            {
                return true;
            }
            return false;
        }

        public bool isVolledigIngenomen()
        {
            return AantalToegewezenStudenten < AantalStudenten;
        }
        #endregion


    }
}