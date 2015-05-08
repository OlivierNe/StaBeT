using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class AcademiejaarInstellingenVM : IValidatableObject
    {
        [RegularExpression("[0-9]{4}-[0-9]{4}", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "ErrorOngeldigAcademiejaarFormaat")]
        [Required]
        public string Academiejaar { get; set; }
        [UIHint("NullableDateTime")]
        [Display(Name = "DisplayBeginSem1", ResourceType = typeof(Resources))]
        public DateTime? Semester1Begin { get; set; }

        [Display(Name = "DisplayEindeSem1", ResourceType = typeof(Resources))]
        [UIHint("NullableDateTime")]
        public DateTime? Semester1Einde { get; set; }

        [Display(Name = "DisplayBeginSem2", ResourceType = typeof(Resources))]
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Begin { get; set; }

        [Display(Name = "DisplayEindeSem2", ResourceType = typeof(Resources))]
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Einde { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "DisplayDeadlineBedrijfMagStageWijzigen")]
        [UIHint("NullableDateTime")]
        public DateTime? DeadlineBedrijfStageEdit { get; set; }

        [Display(Name = "Datum afstudeerbeurs")]
        [UIHint("NullableDateTime")]
        public DateTime? DatumAfstudeerbeurs { get; set; }

        [Display(Name = "Datum Stageterugkomdag")]
        [UIHint("NullableDateTime")]
        public DateTime? Stageterugkomdag { get; set; }

        private string _vrijeDagen;
        [Display(Name = "Vrije dagen")]
        [DataType(DataType.MultilineText)]
        public string VrijeDagen
        {
            get
            {
                if (_vrijeDagen == null)
                {
                    _vrijeDagen = "";
                    var culture = new CultureInfo("nl-BE");
                    const string dateFormat = "dddd d MMMM yyyy";
                    var jaar = DateTime.Now.Year;
                    _vrijeDagen += "- " + new DateTime(jaar, 11, 2).ToString(dateFormat, culture) + " (Allerzielen)\n";
                    _vrijeDagen += "- " + new DateTime(jaar, 11, 11).ToString(dateFormat, culture) + " (Wapenstilstand)\n";
                    jaar++;
                    int firstDig = jaar / 100;
                    int remain19 = jaar % 19;
                    int temp = (firstDig - 15) / 2 + 202 - 11 * remain19;
                    switch (firstDig)
                    {
                        case 21:
                        case 24:
                        case 25:
                        case 27:
                        case 28:
                        case 29:
                        case 30:
                        case 31:
                        case 32:
                        case 34:
                        case 35:
                        case 38:
                            temp = temp - 1;
                            break;
                        case 33:
                        case 36:
                        case 37:
                        case 39:
                        case 40:
                            temp = temp - 2;
                            break;
                    }
                    temp = temp % 30;
                    int tA = temp + 21;
                    if ((temp == 29) || (temp == 28 & remain19 > 10))
                        tA -= 1;
                    int tB = (tA - 19) % 7;
                    int tC = (40 - firstDig) % 4;
                    if (tC == 3 || tC > 1)
                        tC++;
                    temp = jaar % 100;
                    int tD = (temp + temp / 4) % 7;
                    int tE = ((20 - tB - tC - tD) % 7) + 1;
                    int d = tA + tE;
                    int m = 3;
                    if (d > 31)
                    {
                        d -= 31;
                        m++;
                    }
                    var paasmaandag = new DateTime(jaar, m, d + 1);
                    _vrijeDagen += "- " + paasmaandag.ToString(dateFormat, culture) + " (paasmaandag)\n";
                    _vrijeDagen += "- " + paasmaandag.AddDays(38).ToString(dateFormat, culture) + " (Hemelvaart)\n";
                    var pinkstermaandag = paasmaandag.AddDays(48);
                    while (pinkstermaandag.DayOfWeek != DayOfWeek.Monday)
                    {
                        pinkstermaandag = pinkstermaandag.AddDays(1);
                    }
                    _vrijeDagen += "- " + pinkstermaandag.ToString(dateFormat, culture) + " (Pinkstermaandag)\n";
                }
                return
                    _vrijeDagen;
            }
            set { _vrijeDagen = value; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            if (Semester1Begin != null && Semester1Einde != null
                && (DateTime.Compare((DateTime)Semester1Begin, (DateTime)Semester1Einde) > 0))
            {
                errors.Add(new ValidationResult(Resources.ErrorStageperiodeSem1));
            }
            if (Semester2Begin != null && Semester2Einde != null
                && (DateTime.Compare((DateTime)Semester2Begin, (DateTime)Semester2Einde) > 0))
            {
                errors.Add(new ValidationResult(Resources.ErrorStageperiodeSem2));
            }
            var beginJaar = int.Parse(Academiejaar.Substring(0, 4));
            var eindJaar = int.Parse(Academiejaar.Substring(5, 4));
            if (beginJaar != (eindJaar - 1))
            {
                errors.Add(new ValidationResult(Resources.ErrorOngeldigAcademiejaarFormaat));
            }
            if (beginJaar < (DateTime.Now.Year - 1))
            {
                errors.Add(new ValidationResult(Resources.ErrorAcademiejaarInVerleden));
            }
            return errors;
        }
    }

    public class InstellingenVM
    {
        [EmailAddress]
        [Display(Name = "Mailbox stages")]
        public string MailboxStages { get; set; }
        [Display(Name = "Aantal weken stage")]
        public int AantalWekenStage { get; set; }
        [Range(1, 31)]
        public int Dag { get; set; }
        [Range(1, 12)]
        public int Maand { get; set; }

        public SelectList Maanden { get; set; }

        public InstellingenVM()
        {
            var maanden = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                maanden.Add(new SelectListItem
                {
                    Value = i.ToString(CultureInfo.InvariantCulture),
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i)
                });

            }
            Maanden = new SelectList(maanden, "Value", "Text", Maand);
        }

        public void InitInstellingen(IEnumerable<Instelling> instellingen)
        {
            foreach (var instelling in instellingen)
            {
                switch (instelling.Key)
                {
                    case Instelling.MailboxStages:
                        MailboxStages = instelling.Value;
                        break;
                    case Instelling.AantalWekenStage:
                        AantalWekenStage = instelling.IntValue;
                        break;
                    case Instelling.BeginNieuwAcademiejaar:
                        DateTime datum = instelling.DateTimeValue;
                        Dag = datum.Day;
                        Maand = datum.Month;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class StandaardEmailVM
    {
        public int Id { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        [AllowHtml]
        public string Inhoud { get; set; }
        [Required]
        public string Onderwerp { get; set; }
        public bool Gedeactiveerd { get; set; }
        public EmailType EmailType { get; set; }

        public bool HeeftReden
        {
            get
            {
                return EmailType == EmailType.StagedossierAfkeuren
                    || EmailType == EmailType.StageopdrachtAfkeuren;
            }
        }
    }
}