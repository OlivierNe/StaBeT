
namespace StageBeheersTool.Models.Domain
{
    public class StudentVoorkeurStage
    {
        public int StudentId { get; set; }
        private Student _student;
        public virtual Student Student
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
        private Stageopdracht _stage;
        public virtual Stageopdracht Stageopdracht
        {
            get { return _stage; }
            set
            {
                _stage = value;
                if (value != null)
                {
                    StageopdrachtId = value.Id;
                }
            }
        }

        public bool StagedossierIngediend { get; set; }

        public StagedossierStatus Status { get; set; }

        public StudentVoorkeurStage(Student student, Stageopdracht stageopdracht)
            : this()
        {
            this.Student = student;
            this.Stageopdracht = stageopdracht;
        }

        public StudentVoorkeurStage()
        {
            Status = StagedossierStatus.Ingediend;
        }
    }
}
