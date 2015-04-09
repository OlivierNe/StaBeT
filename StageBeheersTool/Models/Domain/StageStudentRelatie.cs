using System;

namespace StageBeheersTool.Models.Domain
{
    //n op n relatie tussen student & stage voor extra properties aan de relatie toe te kunnen voegen
    public class StageStudentRelatie
    {
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

        public int StageId { get; set; }
        private Stageopdracht _stage;
        public virtual Stageopdracht Stage
        {
            get { return _stage; }
            set
            {
                if (value != null)
                    StageId = value.Id;
                _stage = value;
            }
        }

        public DateTime? BeginStage { get; set; }
        public DateTime? EindeStage { get; set; }

    }
}