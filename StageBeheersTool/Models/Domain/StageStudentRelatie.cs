
using System;

namespace StageBeheersTool.Models.Domain
{
    //n op n relatie tussen student & stage voor extra properties aan de relatie toe te kunnen voegen
    public class StageStudentRelatie
    {
        private Student _student;

        public virtual Student Student
        {
            get { return _student; }
            set
            {
                StudentId = value.Id;
                _student = value;
            }
        }

        private Stageopdracht _stage;

        public virtual Stageopdracht Stage
        {
            get { return _stage; }
            set
            {
                StageId = value.Id;
                _stage = value;
            }
        }

        public DateTime? BeginStage { get; set; }
        public DateTime? EindeStage { get; set; }

        public int StudentId { get; set; }
        public int StageId { get; set; }

    }
}