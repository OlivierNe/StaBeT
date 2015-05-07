namespace StageBeheersTool.Models.Domain
{
    public class StandaardEmail
    {
        public int Id { get; set; }
        public string Inhoud { get; set; }
        public string Onderwerp { get; set; }
        public bool Gedeactiveerd { get; set; }
        public EmailType EmailType { get; set; }
    }

    public enum EmailType
    {
        StageopdrachtGoedkeuren,
        StageopdrachtAfkeuren,
        StageopdrachtGewijzigd,
        StageopdrachtAangemaakt,
        StageopdrachtVerwijderd,
        StagedossierIngediend,
        StagedossierAfkeuren,
        StagedossierGoedkeuren
    }
}