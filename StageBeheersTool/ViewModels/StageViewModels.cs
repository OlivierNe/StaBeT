using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class StageListVM
    {
        public IEnumerable<Stage> Stages { get; set; }

        public bool ToonZoeken { get; set; }
        public bool ToonBegeleider { get; set; }
        public bool ToonActiviteitverslagen { get; set; }
        public bool ToonEdit { get; set; }

        public string Student { get; set; }
        public string Stageopdracht { get; set; }
        public string Bedrijf { get; set; }
        public string Begeleider { get; set; }
        public string Title { get; set; }
    }

    public class StageDetailsVM
    {
        public Stage Stage { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Begindatum { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Einddatum { get; set; }

        public bool ToonEdit { get; set; }
        public bool ToonVerwijderen { get; set; }

        [Display(Name = "Aangepaste stageperiode")]
        public bool AangepasteStageperiode
        {
            get { return Stage.AangepasteStageperiode; }
        }
    }

    public class StageEditVM : StageAanStudentToewijzenVM
    {
        public int Id { get; set; }
        [Display(Name = "Stagecontract opgesteld")]
        public bool StagecontractOpgesteld { get; set; }
        [Display(Name = "Getekend stagecontract")]
        public bool GetekendStagecontract { get; set; }
    }

    public class StageAanStudentToewijzenVM : IValidatableObject
    {
        public int StageopdrachtId { get; set; }
        public int StudentId { get; set; }
        public Stageopdracht Stageopdracht { get; set; }
        public Student Student { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Begindatum { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Einddatum { get; set; }

        [Range(1, 2)]
        public int Semester { get; set; }

        public SelectList SemesterSelectList
        {
            get
            {
                var semesters = new List<int>();
                if (Stageopdracht.Semester2)
                {
                    semesters.Add(2);
                }
                if (Stageopdracht.Semester1)
                {
                    semesters.Add(1);
                }
                return new SelectList(semesters, Semester == 0 ? semesters[0] : Semester);
            }
        }

        public bool AangepasteStageperiode { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Semester1Begin { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Semester1Einde { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Semester2Begin { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Semester2Einde { get; set; }

        public string Overzicht { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            if (AangepasteStageperiode && (Einddatum == null || Begindatum == null))
            {
                errors.Add(new ValidationResult(Resources.ErrorVerplichtBeginEnEindDatum));
            }
            return errors;
        }

        public void SetStageperiodes(AcademiejaarInstellingen academiejaarInstellingen)
        {
            if (academiejaarInstellingen != null)
            {
                Semester1Begin = academiejaarInstellingen.Semester1Begin;
                Semester1Einde = academiejaarInstellingen.Semester1Einde;
                Semester2Begin = academiejaarInstellingen.Semester2Begin;
                Semester2Einde = academiejaarInstellingen.Semester2Einde;
            }
        }
    }

    public class StageLijstExcelVM
    {
        [Display(Name = "Stagebegeleider")]
        public int? SelectedStagebegeleiderId { get; set; }
        [Display(Name = "Academiejaar")]
        public string SelectedAcademiejaar { get; set; }

        public SelectList StagebegeleiderSelectList { get; set; }
        public SelectList AcademiejaarSelectList { get; set; }

        [Display(Name = "Tabblad naam")]
        [Required]
        public string TabbladNaam { get; set; }
        [Required]
        public string Bestandsnaam { get; set; }

        public SelectList Opties { get; set; }
        public string[] SelectedOpties { get; set; }

        public StageLijstExcelVM()
        {
            TabbladNaam = "Stages";
            Bestandsnaam = "Stages";
        }

        public void InitSelectLists(IEnumerable<Begeleider> stagebegeleiders, string[] academiejaren)
        {
            StagebegeleiderSelectList = new SelectList(stagebegeleiders, "Id", "Naam", SelectedStagebegeleiderId != null ? SelectedStagebegeleiderId.ToString() : "");
            AcademiejaarSelectList = new SelectList(academiejaren);
            Opties = new SelectList(new[] { "Bedrijf", "Stageplaats", "Titel", "Omschrijving", "Student", "Begeleider" });
        }
    }
}