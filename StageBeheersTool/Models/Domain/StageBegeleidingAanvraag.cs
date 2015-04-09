
namespace StageBeheersTool.Models.Domain
{
    public class StagebegeleidingAanvraag
    {
        public int Id { get; set; }
        public virtual Begeleider Begeleider { get; set; }
        public virtual Stageopdracht Stage { get; set; }
        public StagebegeleidAanvraagStatus Status { get; set; }
        public bool IsGoedgekeurd
        {
            get
            {
                return Status == StagebegeleidAanvraagStatus.Goedgekeurd;
            }
        }
        public bool IsAfgekeurd
        {
            get
            {
                return Status == StagebegeleidAanvraagStatus.Afgekeurd;
            }
        }

        public StagebegeleidingAanvraag()
        {
            Status = StagebegeleidAanvraagStatus.NietBeoordeeld;
        }

        public bool StageHeeftAlBegeleider()
        {
            return Stage.Stagebegeleider != null;
        }
    }
}