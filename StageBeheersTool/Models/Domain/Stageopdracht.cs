using System.Collections.Generic;
using System.Linq;
using StageBeheersTool.Helpers;

namespace StageBeheersTool.Models.Domain
{
    public class Stageopdracht
    {
        #region Properties
        public int Id { get; set; }
        public string Titel { get; set; }
        public string Omschrijving { get; set; }
        public string Specialisatie { get; set; }

        public string Semester
        {
            get
            {
                if (Semester1 && Semester2)
                    return "1 & 2";
                if (Semester1)
                    return "1";
                if (Semester2)
                    return "2";
                return null;
            }
        }

        public string ToonStudenten
        {
            get
            {
                var retVal = "";
                if (Stages.Count == 0)
                {
                    return "";
                }
                var studenten = Stages.ToList();
                int i = 0;
                while (i < (studenten.Count - 1))
                {
                    retVal += studenten[i].Student.Naam + ", ";
                    i++;
                }
                retVal += studenten[i].Student.Naam;
                return retVal;
            }
        }

        public string Stageplaats
        {
            get
            {
                string stageplaats = string.Format("{0} {1}\n {2} {3}", Postcode, Gemeente, Straat, Straatnummer);
                if (string.IsNullOrWhiteSpace(stageplaats))
                {
                    return Bedrijf.Adres;
                }
                return stageplaats;
            }
        }

        public bool Semester1 { get; set; }
        public bool Semester2 { get; set; }
        public int AantalStudenten { get; set; }
        public string Academiejaar { get; set; }
        public virtual Contactpersoon Contractondertekenaar { get; set; }
        public virtual Contactpersoon Stagementor { get; set; }
        public virtual Begeleider Stagebegeleider { get; set; }
        public virtual Bedrijf Bedrijf { get; set; }
        public virtual ICollection<Stage> Stages { get; set; }
        public virtual ICollection<VoorkeurStage> StudentVoorkeurStages { get; set; }

        public StageopdrachtStatus Status { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public string Straatnummer { get; set; }

        //extra properties in geval een contactpersoon verwijderd wordt (voor archief) 
        public string StagementorNaam { get; set; }
        public string ContractondertekenaarNaam { get; set; }
        public string StagementorEmail { get; set; }
        public string ContractondertekenaarEmail { get; set; }
        #endregion

        #region Constructors
        public Stageopdracht()
        {
            Status = StageopdrachtStatus.NietBeoordeeld;
            Stages = new List<Stage>();
        }
        #endregion

        #region Public methods

        public bool IsBeoordeeld()
        {
            return Status != StageopdrachtStatus.NietBeoordeeld;
        }
        public bool IsGoedgekeurd()
        {
            return Status == StageopdrachtStatus.Goedgekeurd || StageopdrachtStatus.Toegewezen == Status;
        }

        public bool IsAfgekeurd()
        {
            return Status == StageopdrachtStatus.Afgekeurd;
        }

        /// <summary>
        /// Is minstens aan 1 student toegewezen
        /// </summary>
        /// <returns></returns>
        public bool IsToegewezen()
        {
            return Status == StageopdrachtStatus.Toegewezen;
        }

        public bool IsInHuidigAcademiejaar()
        {
            return AcademiejaarHelper.HuidigAcademiejaar() == Academiejaar;
        }

        public bool IsVolledigIngenomen()
        {
            return Stages.Count >= AantalStudenten;
        }

        public bool HeeftStageBegeleider()
        {
            return Stagebegeleider != null;
        }

        public int AantalToegewezenStudenten()
        {
            return Stages.Count;
        }

        public bool IsBeschikbaar()
        {
            if (IsInHuidigAcademiejaar() == false)
            {
                return false;
            }
            if (IsVolledigIngenomen())
            {
                return false;
            }
            if (IsAfgekeurd())
            {
                return false;
            }
            return true;
        }
        #endregion

    }
}