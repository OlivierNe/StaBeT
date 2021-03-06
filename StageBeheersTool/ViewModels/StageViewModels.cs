﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public bool ToonEditStageopdracht { get; set; }
        public bool ToonDetails { get; set; }
        public bool ToonEvaluatieformulier { get; set; }
        public bool ToonEvaluatieformulierBekijken { get; set; }

        public string Student { get; set; }
        public string Stageopdracht { get; set; }
        public string Bedrijf { get; set; }
        public string Begeleider { get; set; }
        public string Title { get; set; }
    }

    public class StagesToewijzenListVM
    {
        public IEnumerable<VoorkeurStage> VoorkeurStages { get; set; }

        //student
        public string Voornaam { get; set; }
        public string Naam { get; set; }
    }

    public class StageDetailsVM
    {
        public Stage Stage { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Begindatum { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Einddatum { get; set; }
        [UIHint("NullableDateTime")]
        [DisplayName("Stagecontract opgesteld")]
        public DateTime? DatumStagecontractOpgesteld { get { return Stage.DatumStagecontractOpgesteld; } }
        [UIHint("NullableDateTime")]
        [DisplayName("Getekend stagecontract")]
        public DateTime? DatumGetekendStagecontract { get { return Stage.DatumGetekendStagecontract; } }
        [DisplayName("Aangepaste stageperiode")]
        public bool AangepasteStageperiode { get { return Stage.AangepasteStageperiode; } }

        public bool ToonEdit { get; set; }
        public bool ToonVerwijderen { get; set; }
    }

    public class StageEditVM : StageAanStudentToewijzenVM
    {
        public int Id { get; set; }
        [Display(Name = "Stagecontract opgesteld")]
        public bool StagecontractOpgesteld { get; set; }
        [Display(Name = "Getekend stagecontract")]
        public bool GetekendStagecontract { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? DatumStagecontractOpgesteld { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? DatumGetekendStagecontract { get; set; }
    }

    public class StagedossierAfkeurenVM
    {
        public int StageId { get; set; }
        public int StudentId { get; set; }
        public string Titel { get; set; }
        public string Aan { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public string Reden { get; set; }
        public string Overzicht { get; set; }
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

        private IDictionary<string, string> _kolommen;
        public IDictionary<string, string> Kolommen
        {
            get
            {
                //kolomnaam + property stage
                return _kolommen ?? (_kolommen = new Dictionary<string, string>
                {
                    {"Bedrijf", "Bedrijf.Naam"},
                    {"Bedrijf E-mail","Bedrijf.Email"},
                    {"Bedrijf adres", "Bedrijf.Adres"},
                    {"Bedrijf gemeente", "Bedrijf.Gemeente"},
                    {"Bedrijfstelefoon", "Bedrijf.Telefoon"},
                    {"Stageplaats", "Stageopdracht.Stageplaats"},
                    {"Stageopdracht omschrijving", "Stageopdracht.Omschrijving"},
                    {"Stageopdracht titel", "Stageopdracht.Titel"},
                    {"Studentnaam", "Student.Familienaam"},
                    {"Studentvoornaam", "Student.Voornaam"},
                    {"Studentadres", "Student.Adres"},
                    {"Studentgemeente", "Student.Gemeente"},
                    {"Studentgeboortedatum", "Student.GeboortedatumToString"},
                    {"Studentemail1", "Student.HogentEmail"},
                    {"Studentemail2", "Student.Email"},
                    {"Studenttelefoon", "Student.Telefoon"},
                    {"Stagebegeleider", "Stageopdracht.Stagebegeleider.Naam"},
                    {"Stagebegleideremail", "Stageopdracht.Stagebegeleider.HogentEmail"},
                    {"Stagementor", "Stageopdracht.StagementorNaam"},
                    {"Stagementoremail", "Stageopdracht.StagementorEmail"},
                    {"Stagementortelefoon", "Stageopdracht.Stagementor.Telefoon"},
                    {"Stagementorfunctie", "Stageopdracht.Stagementor.Bedrijfsfunctie"},
                    {"Contractondertekenaar", "Stageopdracht.ContractondertekenaarNaam"},
                    {"Contractondertekenaaremail", "Stageopdracht.ContractondertekenaarEmail"},
                    {"Contractondertekenaartelefoon", "Stageopdracht.Contractondertekenaar.Telefoon"},
                    {"Contractondertekenaarfunctie", "Stageopdracht.Contractondertekenaar.Bedrijfsfunctie"},
                    {"Stageperiode", "Stageperiode"}
                });
                //TODO:verlenging contract, vrije dag voor bachelproef op, campus, studentstamboek
            }
            set { _kolommen = value; }
        }

        public StageLijstExcelVM()
        {
            TabbladNaam = "Stages";
            Bestandsnaam = "Stages";
        }

        public void InitSelectLists(IEnumerable<Begeleider> stagebegeleiders, string[] academiejaren)
        {
            StagebegeleiderSelectList = new SelectList(stagebegeleiders, "Id", "Naam", SelectedStagebegeleiderId != null ? SelectedStagebegeleiderId.ToString() : "");
            AcademiejaarSelectList = new SelectList(academiejaren);
            Opties = new SelectList(Kolommen.Keys);
        }
    }
}