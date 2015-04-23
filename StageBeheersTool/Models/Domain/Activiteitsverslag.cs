using System;

namespace StageBeheersTool.Models.Domain
{
    public class Activiteitsverslag
    {
        public int Id { get; set; }
        public DateTime? DatumIngave { get; set; }
        public int Week { get; set; }
        public string Verslag { get; set; }
        public string Feedback { get; set; }

        public Stage Stage { get; set; }

        public bool HeeftFeedbackGekregen()
        {
            return !String.IsNullOrEmpty(Feedback);
        }

        public bool IsIngediend()
        {
            return DatumIngave != null;
        }
    }
}