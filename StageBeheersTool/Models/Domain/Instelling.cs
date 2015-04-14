
namespace StageBeheersTool.Models.Domain
{
    /// <summary>
    /// Algemene instellingen
    /// </summary>
    public class Instelling
    {
        public const string MailboxStages = "MailboxStages";

        public string Key { get; set; }
        public string Value { get; set; }

        public Instelling() { }

        public Instelling(string key, string value)
        {
            Key = key;
            Value = value;
        }


    }
}