
namespace StageBeheersTool.Models.Domain
{
    public class VoorkeurStage
    {
        #region properties
        public int StudentId { get; set; }
        private Student _student;
        public Student Student
        {
            get { return _student; }
            set
            {
                _student = value;
                if (value != null)
                {
                    StudentId = value.Id;
                }
            }
        }

        public int StageopdrachtId { get; set; }
        private Stageopdracht _stageopdracht;
        public Stageopdracht Stageopdracht
        {
            get { return _stageopdracht; }
            set
            {
                _stageopdracht = value;
                if (value != null)
                {
                    StageopdrachtId = value.Id;
                }
            }
        }

        public bool StagedossierIngediend { get; set; }

        public StagedossierStatus Status { get; set; }

        #endregion

        #region constructors
        public VoorkeurStage(Student student, Stageopdracht stageopdracht)
            : this()
        {
            Student = student;
            Stageopdracht = stageopdracht;
        }

        public VoorkeurStage()
        {
            Status = StagedossierStatus.Ingediend;
        }

        #endregion

        public bool HeeftGoedgekeurdStagedossier()
        {
            return StagedossierIngediend && Status == StagedossierStatus.Goedgekeurd;
        }
    }
}
