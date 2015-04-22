﻿using System;

namespace StageBeheersTool.Models.Domain
{
    public class Stage
    {
        #region Properties
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int StageopdrachtId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Stageopdracht Stageopdracht { get; set; }

        public DateTime? Begindatum { get; set; }
        public DateTime? Einddatum { get; set; }
        public int Semester { get; set; }
        public bool AangepasteStageperiode { get; set; }
        public virtual AcademiejaarInstellingen AcademiejaarInstellingen { get; set; }

        public bool StagecontractOpgesteld { get; set; }
        public bool GetekendStagecontract { get; set; }


        //not mapped properties
        public Bedrijf Bedrijf
        {
            get { return Stageopdracht.Bedrijf; }
        }

        public Begeleider Begeleider
        {
            get { return Stageopdracht.Stagebegeleider; }
        }
        #endregion

        #region Constructors
        public Stage()
        {
        }

        public Stage(Stageopdracht stageopdracht, Student student)
        {
            Stageopdracht = stageopdracht;
            Student = student;
        }
        #endregion

        #region methods
        public void SetAangepasteStageperiode(DateTime? begindatum, DateTime? einddatum, int semester)
        {
            if (begindatum == null || einddatum == null)
            {
                throw new ApplicationException(Resources.ErrorSetStageperiode);
            }
            if (((DateTime)begindatum).CompareTo((DateTime)einddatum) >= 0)
            {
                throw new ApplicationException(Resources.ErrorBegindatumNaEinddatum);
            }
            if (semester > 2 || semester < 1)
            {
                throw new ApplicationException(Resources.ErrorOngeldigSemester);
            }
            Begindatum = begindatum;
            Einddatum = einddatum;
            AangepasteStageperiode = true;
            Semester = semester;
        }

        public bool HeeftAangepasteStageperiode()
        {
            return AangepasteStageperiode;
        }

        public DateTime? GetEinddatum()
        {
            if (AangepasteStageperiode)
            {
                return Einddatum;
            }
            if (AcademiejaarInstellingen == null)
            {
                return null;
            }
            if (Semester == 1)
            {
                return AcademiejaarInstellingen.Semester1Einde;
            }
            return AcademiejaarInstellingen.Semester2Einde;
        }

        public DateTime? GetBeginDatum()
        {
            if (AangepasteStageperiode)
            {
                return Begindatum;
            }
            if (AcademiejaarInstellingen == null)
            {
                return null;
            }
            if (Semester == 1)
            {
                return AcademiejaarInstellingen.Semester1Begin;
            }
            return AcademiejaarInstellingen.Semester1Einde;
        }

        #endregion



    }
}
