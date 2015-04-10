using System;

namespace StageBeheersTool.Models.Domain
{
    //n op n relatie tussen student & stage voor extra properties aan de relatie toe te kunnen voegen
    public class Stage
    {
        #region Properties
        public int StudentId { get; set; }
        private Student _student;
        public virtual Student Student
        {
            get { return _student; }
            set
            {
                if (value != null)
                    StudentId = value.Id;
                _student = value;
            }
        }

        public int StageopdrachtId { get; set; }
        private Stageopdracht _stageopdracht;
        public virtual Stageopdracht Stageopdracht
        {
            get { return _stageopdracht; }
            set
            {
                if (value != null)
                    StageopdrachtId = value.Id;
                _stageopdracht = value;
            }
        }

        public DateTime Begindatum { get; set; }
        public DateTime Einddatum { get; set; }
        public int Semester { get; set; }
        public bool AangepasteStageperiode { get; set; }

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
        public void SetAangepasteStageperiode(DateTime begindatum, DateTime einddatum, int semester)
        {
            if (begindatum.CompareTo(einddatum) >= 0)
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
        }

        public void SetStageperiode(AcademiejaarInstellingen academiejaarInstellingen, int semester)
        {
            if (semester == 1)
            {
                if (academiejaarInstellingen == null || academiejaarInstellingen.Semester1Begin == null ||
                    academiejaarInstellingen.Semester1Einde == null)
                {
                    throw new ApplicationException(Resources.ErrorGeenStageperiodesIngesteld);
                }
                Begindatum = (DateTime)academiejaarInstellingen.Semester1Begin;
                Einddatum = (DateTime)academiejaarInstellingen.Semester1Einde;
            }
            else if (semester == 2)
            {
                if (academiejaarInstellingen == null || academiejaarInstellingen.Semester2Begin == null ||
                   academiejaarInstellingen.Semester2Einde == null)
                {
                    throw new ApplicationException(Resources.ErrorGeenStageperiodesIngesteld);
                }
                Begindatum = (DateTime)academiejaarInstellingen.Semester2Begin;
                Einddatum = (DateTime)academiejaarInstellingen.Semester2Einde;
            }
            else
            {
                throw new ApplicationException(Resources.ErrorOngeldigSemester);
            }
        }

        #endregion

    }
}