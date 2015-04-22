using System;

namespace StageBeheersTool.Models.Domain
{
    public class Activiteitsverslag
    {
        public int Id { get; set; }
        public DateTime DatumIngave { get; set; }
        public int Week { get; set; }
        public string Verslag { get; set; }

    }
}