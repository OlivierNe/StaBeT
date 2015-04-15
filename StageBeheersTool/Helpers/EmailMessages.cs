using Microsoft.AspNet.Identity;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Helpers
{
    public class EmailMessages
    {
        public static IdentityMessage StageopdrachtGoedkeurenMail(Stageopdracht stageopdracht)
        {
            return new IdentityMessage
            {
                Destination = stageopdracht.Bedrijf.Email,
                Subject = "Stageopdracht goedgekeurd.",
                Body = "Stageopdracht " + stageopdracht.Titel + " werd goedgekeurd."
            };
        }

        public static IdentityMessage StageopdrachtAfkeurenMail(Stageopdracht stageopdracht, string reden, string onderwerp = "Stageopdracht afgekeurd.")
        {
            return new IdentityMessage
            {
                Destination = stageopdracht.Bedrijf.Email,
                Subject = onderwerp,
                Body = reden
            };
        }

        public static IdentityMessage StageopdrachtGewijzigd(Stageopdracht stageopdracht)
        {
            return new IdentityMessage
            {
                Destination = stageopdracht.Bedrijf.Email,
                Subject = "Stageopdracht Gewijzigd",
                Body = "Stageopdracht " + stageopdracht.Titel + " werd gewijzigd."
            };
        }

        public static IdentityMessage StageopdrachtAangemaakt(Stageopdracht stageopdracht, Instelling stagesMailbox)
        {
            string email = stagesMailbox != null ? stagesMailbox.Value : "";
            return new IdentityMessage
            {
                Destination = email,
                Subject = "Nieuwe stageopdracht aangemaakt",
                Body = "Nieuwe stageopdracht aangemaakt. Titel:" + stageopdracht.Titel
            };
        }

        public static IdentityMessage BedrijfGeregistreerd(string email, string wachtwoord)
        {
            return new IdentityMessage
            {
                Destination = email,
                Subject = "Registratie",
                Body = string.Format("<strong>Account aangemaakt: <br/><strong><ul><li>" +
                                    "Login: {0}</li><li>Wachtwoord: {1}</li></ul>",
                                    email, wachtwoord)
            };
        }

        public static IdentityMessage StageopdrachtVerwijderd(Stageopdracht stageopdracht, Instelling stagesMailbox)
        {
            string email = stagesMailbox != null ? stagesMailbox.Value : "";
            return new IdentityMessage
            {
                Destination = email,
                Subject = "Stageopdracht verwijderd",
                Body = "Stageopdracht verwijderd. Titel:" + stageopdracht.Titel
            };
        }

        public static IdentityMessage StagedossierIngediend(Stageopdracht stageopdracht, Student student, Instelling stagesMailbox)
        {
            string email = stagesMailbox != null ? stagesMailbox.Value : "";
            return new IdentityMessage
            {
                Destination = email,
                Subject = "Stageopdracht ingediend",
                Body = "<strong>Stageopdracht ingediend<strong><br/><ul><li>Stageopdracht: " + stageopdracht.Titel +
                "</li><li>Student: " + student.Naam + "</li></ul>"

            };
        }
    }
}