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

        public string Semester
        {
            get
            {
                if (Semester1 && Semester2)
                    return "1 & 2";
                else if (Semester1)
                    return "1";
                return "2";
            }
        }

        public bool Semester1 { get; set; }
        public bool Semester2 { get; set; }
        public int AantalStudenten { get; set; }
        public int AantalToegewezenStudenten { get; set; }
        public string Academiejaar { get; set; }
        public virtual Contactpersoon Contractondertekenaar { get; set; }
        public virtual Contactpersoon Stagementor { get; set; }
        public virtual Begeleider Stagebegeleider { get; set; }
        public virtual Bedrijf Bedrijf { get; set; }
        public virtual ICollection<Student> Studenten { get; set; }
        public StageopdrachtStatus Status { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public string Straatnummer { get; set; }
        #endregion

        #region Constructors
        public Stageopdracht()
        {
            Status = StageopdrachtStatus.NietBeoordeeld;
            Studenten = new List<Student>();
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

        public bool IsVolledigIngenomen()
        {
            return AantalToegewezenStudenten == AantalStudenten;
        }
        #endregion


    }
}