﻿using System;

namespace StageBeheersTool.Models.Domain
{
    /// <summary>
    /// Instellingen voor 1 academiejaar
    /// </summary>
    public class AcademiejaarInstellingen
    {
        public string Academiejaar { get; set; }
        public DateTime? Semester1Begin { get; set; }
        public DateTime? Semester1Einde { get; set; }
        public DateTime? Semester2Begin { get; set; }
        public DateTime? Semester2Einde { get; set; }
        public DateTime? DeadlineBedrijfStageEdit { get; set; }
        public string VrijeDagen { get; set; }
        public DateTime? DatumAfstudeerbeurs { get; set; }
        public DateTime? Stageterugkomdag { get; set; }

        public string StageterugkomdagToString
        {
            get { return Stageterugkomdag == null ? "" : ((DateTime)Stageterugkomdag).ToString("dd/MM/yyyy"); }
        }

        public string DatumAfstudeerbeursToString
        {
            get { return DatumAfstudeerbeurs == null ? "" : ((DateTime)DatumAfstudeerbeurs).ToString("dddd d MMMM yyyy"); }
        }

        public string StageperiodeSemester1()
        {
            if (Semester1Begin != null && Semester1Einde != null)
            {
                return string.Format("{0} - {1}",
                    ((DateTime)Semester1Begin).ToString("dd/MM/yyyy"),
                    ((DateTime)Semester1Einde).ToString("dd/MM/yyyy"));
            }
            return "";
        }

        public string StageperiodeSemester2()
        {
            if (Semester2Begin != null && Semester2Einde != null)
            {
                return string.Format("{0} - {1}",
                    ((DateTime)Semester2Begin).ToString("dd/MM/yyyy"),
                    ((DateTime)Semester2Einde).ToString("dd/MM/yyyy"));
            }
            return "";
        }

        public string DeadlineBedrijfStageEditToString()
        {
            if (DeadlineBedrijfStageEdit == null)
            {
                return "";
            }
            return String.Format(Resources.WarningBedrijfMagStageWijzigenTot, ((DateTime)DeadlineBedrijfStageEdit).ToString("dd/MM/yyyy"));
        }
    }
}