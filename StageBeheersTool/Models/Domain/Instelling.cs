
using System;

namespace StageBeheersTool.Models.Domain
{
    /// <summary>
    /// Algemene instellingen
    /// </summary>
    public class Instelling
    {
        public const string MailboxStages = "MailboxStages";
        public const string AantalWekenStage = "AantalWekenStage";
        public const string BeginNieuwAcademiejaar = "BeginNieuwAcademiejaar";

        public string Key { get; set; }
        public string Value { get; set; }

        public int IntValue
        {
            get
            {
                int intValue;
                var result = int.TryParse(Value, out intValue);
                return result ? intValue : 0;
            }
        }

        public DateTime DateTimeValue
        {
            get
            {
                DateTime date;
                DateTime.TryParse(Value, out date);
                return date;
            }
        }

        public Instelling() { }

        public Instelling(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}