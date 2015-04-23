using System.ComponentModel.DataAnnotations;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class ActiviteitsverslagFeedbackVM
    {
        public Activiteitsverslag Activiteitsverslag { get; set; }
        public int StageId { get; set; }
        public int Week { get; set; }
        [DataType(DataType.MultilineText)]
        public string Feedback { get; set; }
        public string Student { get; set; }
    }

    public class ActiviteitsverslagIndienenVM
    {
        public int Week { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public string Verslag { get; set; }
    }
}