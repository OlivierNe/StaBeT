using System;
using System.Collections.Generic;
using System.Linq;

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

        public DateTime? DatumStagecontractOpgesteld { get; set; }
        public DateTime? DatumGetekendStagecontract { get; set; }

        public virtual ICollection<Activiteitsverslag> Activiteitsverslagen { get; set; }
        public virtual ICollection<Evaluatieantwoord> EvaluatieAntwoorden { get; set; }

        public string Stageperiode
        {
            get
            {
                return String.Format("{0} - {1}", BeginStageperiode, EindeStageperiode);
            }
        }

        public string BeginStageperiode
        {
            get
            {
                string beginStageperiode = "";
                if (Begindatum == null || Einddatum == null)
                {
                    if (AcademiejaarInstellingen == null) return "";
                    if (Semester == 1 && AcademiejaarInstellingen.Semester1Begin != null)
                    {
                        beginStageperiode = ((DateTime)AcademiejaarInstellingen.Semester1Begin).ToString("dd/MM/yyyy");
                    }
                    else if (Semester == 2 && AcademiejaarInstellingen.Semester2Begin != null)
                    {
                        beginStageperiode = ((DateTime)AcademiejaarInstellingen.Semester2Begin).ToString("dd/MM/yyyy");
                    }
                }
                else
                {
                    beginStageperiode = ((DateTime)Begindatum).ToString("dd/MM/yyyy");
                }
                return beginStageperiode;
            }
        }

        public string EindeStageperiode
        {
            get
            {
                string eindeStageperiode = "";
                if (Begindatum == null || Einddatum == null)
                {
                    if (AcademiejaarInstellingen == null) return "";
                    if (Semester == 1 && AcademiejaarInstellingen.Semester1Einde != null)
                    {
                        eindeStageperiode = ((DateTime)AcademiejaarInstellingen.Semester1Einde).ToString("dd/MM/yyyy");
                    }
                    else if (Semester == 2 && AcademiejaarInstellingen.Semester2Einde != null)
                    {
                        eindeStageperiode = ((DateTime)AcademiejaarInstellingen.Semester2Einde).ToString("dd/MM/yyyy");
                    }
                }
                else
                {
                    eindeStageperiode = ((DateTime)Einddatum).ToString("dd/MM/yyyy");
                }
                return eindeStageperiode;
            }
        }

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
            Activiteitsverslagen = new List<Activiteitsverslag>();
        }

        public Stage(Stageopdracht stageopdracht, Student student)
            : this()
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

        public Activiteitsverslag GetActiviteitsverslagVanWeek(int week)
        {
            return Activiteitsverslagen.FirstOrDefault(verslag => verslag.Week == week);
        }

        public void InitializeActiviteitsverslagen(int aantalWekenStage)
        {
            if (Activiteitsverslagen == null)
            {
                Activiteitsverslagen = new List<Activiteitsverslag>();
            }
            for (var i = 0; i < aantalWekenStage; i++)
            {
                if (Activiteitsverslagen.All(verslag => verslag.Week != i + 1))
                {
                    Activiteitsverslagen.Add(new Activiteitsverslag { Week = i + 1 });
                }
            }
        }

        public void AddEvaluatieAntwoord(Evaluatievraag evaluatievraag, string antwoord)
        {
            if (evaluatievraag.IsGeldigAntwoord(antwoord) == false)
            {
                return;
            }
            var oudEvaluatieantwoord = EvaluatieAntwoorden
                .FirstOrDefault(a => a.Evaluatievraag.Equals(evaluatievraag));
            if (oudEvaluatieantwoord != null)
            {
                oudEvaluatieantwoord.Antwoord = antwoord;
            }
            else
            {
                EvaluatieAntwoorden.Add(new Evaluatieantwoord { Evaluatievraag = evaluatievraag, Antwoord = antwoord });
            }
        }
        #endregion
    }
}
