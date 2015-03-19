

namespace OudeGegevens.Models
{
    public class voorkeurstudentstage
    {
        public int stageID { get; set; }
        public string student1ID { get; set; }
        public string student2ID { get; set; }
        public short voorkeur { get; set; }
        public bool wel { get; set; }
        public bool niet { get; set; }
    }
}
