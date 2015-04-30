namespace StageBeheersTool.Models.Domain
{
    public class Foto
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public byte[] FotoData { get; set; }
        public string ContentType { get; set; }
    }
}
