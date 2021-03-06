﻿using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using MySql.Data.MySqlClient;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using StageBeheersTool.Helpers;

namespace StageBeheersTool.Models.DAL
{
    public class StageopdrachtRepository : IStageopdrachtRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Stageopdracht> _stageopdrachten;
        private readonly DbSet<StagebegeleidingAanvraag> _aanvragen;
        private readonly DbSet<VoorkeurStage> _studentVoorkeurStages;

        public StageopdrachtRepository(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _aanvragen = ctx.StageBegeleidAanvragen;
            _studentVoorkeurStages = ctx.StudentVoorkeurStages;
            _stageopdrachten = ctx.Stageopdrachten;
        }

        public Stageopdracht FindById(int id)
        {
            return _stageopdrachten.Find(id);
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            return _stageopdrachten.IncludeAndOrder();
        }

        /// <summary>
        /// </summary>
        /// <returns>Alle niet beoordeelde stageopdrachten van het huidige en toekomstige academiejaren</returns>
        public IQueryable<Stageopdracht> FindStageopdrachtVoorstellen()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar().Substring(0, 4);
            return _stageopdrachten
                .Where(so => so.Academiejaar.Substring(0, 4).CompareTo(academiejaar) >= 0)
                .Where(so => so.Status == StageopdrachtStatus.NietBeoordeeld)
                .IncludeAndOrder();
        }

        /// <summary>
        /// </summary>
        /// <returns>Alle goedgekeurde stageopdrachten</returns>
        public IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachten()
        {
            return _stageopdrachten
                .Where(so => so.Status == StageopdrachtStatus.Goedgekeurd)
                .IncludeAndOrder();
        }

        /// <summary>
        /// </summary>
        /// <returns>Alle afgekeurde stageopdrachten</returns>
        public IQueryable<Stageopdracht> FindAfgekeurdeStageopdrachten()
        {
            return _stageopdrachten
               .Where(so => so.Status == StageopdrachtStatus.Afgekeurd)
               .IncludeAndOrder();
        }

        /// <summary>
        /// lijst waar studenten uit kunnen kiezen om te solliciteren
        /// </summary>
        /// <returns>Alle goedgekeurde stageopdrachten met minstens 1 plaats vrij</returns>
        public IQueryable<Stageopdracht> FindBeschikbareStageopdrachten()
        {
            return _stageopdrachten.
                Where(IsinHuidigAcademiejaar())
                .Where(so => (so.Status == StageopdrachtStatus.Goedgekeurd
                    || so.Status == StageopdrachtStatus.Toegewezen) && so.Stages.Count < so.AantalStudenten)
                    .IncludeAndOrder();
        }

        /// <summary>
        /// uit deze lijst kunnen docenten kiezen om een stage te begeleiden
        /// </summary>
        /// <returns>Alle toegewezen stageopdrachten van het huidig academiejaar zonder stagebegeleider</returns>
        public IQueryable<Stageopdracht> FindToegewezenStageopdrachtenZonderBegeleider()
        {
            return _stageopdrachten
                .Where(IsinHuidigAcademiejaar())
                .Where(so => so.Status == StageopdrachtStatus.Toegewezen && so.Stages.Count > 0
                    && so.Stagebegeleider == null)
                .IncludeAndOrder();
        }

        /// <summary>
        /// </summary>
        /// <returns>Alle toegewezen stageopdrachten van het huidige academiejaar</returns>
        public IQueryable<Stageopdracht> FindToegewezenStageopdrachten()
        {
            return _stageopdrachten
                .Where(IsinHuidigAcademiejaar())
                .Where(so => so.Status == StageopdrachtStatus.Toegewezen && so.Stages.Count > 0)
                .Where(so => so.Stagebegeleider != null)
                .IncludeAndOrder();
        }

        /// <summary>
        /// lijst stageopdrachten/studenten met ingediend stagedossier. studenten nog niet gekoppeld aan stage
        /// </summary>
        /// <returns></returns>
        public IQueryable<VoorkeurStage> FindAllStudentVoorkeurenMetIngediendStagedossier()
        {
            var huidigAcademiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return _studentVoorkeurStages
                .Include(voorkeur => voorkeur.Student).Include(voorkeur => voorkeur.Stageopdracht)
                .Where(voorkeur => voorkeur.StagedossierIngediend && voorkeur.Stageopdracht.Academiejaar == huidigAcademiejaar)// && voorkeur.Student.Stages.Any(ssr => ssr.Stage.Id == voorkeur.Stageopdracht.Id) == false)
                .OrderBy(voorkeur => voorkeur.Student.Familienaam);
        }

        public VoorkeurStage FindStudentVoorkeurStageByIds(int studentId, int stageId)
        {
            return _studentVoorkeurStages.Include(s => s.Student).Include(s => s.Stageopdracht)
                .SingleOrDefault(s => s.Student.Id == studentId && s.Stageopdracht.Id == stageId);
        }

        public void DeleteVoorkeurstagesVanStudent(Student student)
        {
            _studentVoorkeurStages.RemoveRange(student.VoorkeurStages);
            SaveChanges();
        }

        public IQueryable<Stageopdracht> FindAllFromAcademiejaar(string academiejaar)
        {
            return _stageopdrachten.Where(so => so.Academiejaar == academiejaar).IncludeAndOrder();
        }

        public void Update(Stageopdracht stageopdracht)
        {
            var teUpdatenStageopdracht = FindById(stageopdracht.Id);
            if (teUpdatenStageopdracht == null)
                return;
            teUpdatenStageopdracht.Omschrijving = stageopdracht.Omschrijving;
            teUpdatenStageopdracht.Titel = stageopdracht.Titel;
            teUpdatenStageopdracht.Semester1 = stageopdracht.Semester1;
            teUpdatenStageopdracht.Semester2 = stageopdracht.Semester2;
            teUpdatenStageopdracht.Specialisatie = stageopdracht.Specialisatie;
            teUpdatenStageopdracht.AantalStudenten = stageopdracht.AantalStudenten;
            teUpdatenStageopdracht.Gemeente = stageopdracht.Gemeente;
            teUpdatenStageopdracht.Postcode = stageopdracht.Postcode;
            teUpdatenStageopdracht.Straat = stageopdracht.Straat;
            teUpdatenStageopdracht.Straatnummer = stageopdracht.Straatnummer;

            if (teUpdatenStageopdracht.IsToegewezen() == false)
            {
                teUpdatenStageopdracht.Academiejaar = stageopdracht.Academiejaar;
            }
            
            //stagementor
            teUpdatenStageopdracht.Stagementor = stageopdracht.Stagementor;
            if (stageopdracht.Stagementor != null)
            {
                teUpdatenStageopdracht.StagementorEmail = stageopdracht.Stagementor.Email;
                teUpdatenStageopdracht.StagementorNaam = stageopdracht.Stagementor.Naam;
            }
            else
            {
                teUpdatenStageopdracht.StagementorEmail = null;
                teUpdatenStageopdracht.StagementorNaam = null;
            }
            //contractondertekenaar
            teUpdatenStageopdracht.Contractondertekenaar = stageopdracht.Contractondertekenaar;
            if (stageopdracht.Contractondertekenaar != null)
            {
                teUpdatenStageopdracht.ContractondertekenaarEmail = stageopdracht.Contractondertekenaar.Email;
                teUpdatenStageopdracht.ContractondertekenaarNaam = stageopdracht.Contractondertekenaar.Naam;
            }
            else
            {
                teUpdatenStageopdracht.ContractondertekenaarEmail = null;
                teUpdatenStageopdracht.ContractondertekenaarNaam = null;
            }
            SaveChanges();
        }

        public void Delete(Stageopdracht stageopdracht)
        {
            try
            {
                _stageopdrachten.Remove(stageopdracht);
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1451)
                {
                    throw new ApplicationException(string.Format(Resources.ErrorDeleteStageopdracht, stageopdracht.Titel));
                }
                throw;
            }
        }

        public StagebegeleidingAanvraag FindAanvraagById(int id)
        {
            return _aanvragen
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stage)
                .SingleOrDefault(a => a.Id == id);
        }

        public IQueryable<StagebegeleidingAanvraag> FindAllAanvragen()
        {
            return _aanvragen
                .Where(IsAanvraagInHuidigAcademiejaar())
                .OrderBy(sba => sba.Stage.Titel)
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stage);
        }

        public IQueryable<StagebegeleidingAanvraag> FindAllAanvragenVan(Begeleider begeleider)
        {
            return _aanvragen
                 .Where(sba => sba.Begeleider.Id == begeleider.Id)
                 .Where(IsAanvraagInHuidigAcademiejaar())
                 .OrderBy(sba => sba.Stage.Titel)
                 .Include(sba => sba.Begeleider)
                 .Include(sba => sba.Stage);
        }


        public void AddAanvraag(StagebegeleidingAanvraag aanvraag)
        {
            _aanvragen.Add(aanvraag);
            SaveChanges();
        }

        public void DeleteAanvraag(StagebegeleidingAanvraag aanvraag)
        {
            _aanvragen.Remove(aanvraag);
            SaveChanges();
        }

        public string[] FindAllAcademiejaren()
        {
            return _stageopdrachten.Select(so => so.Academiejaar).Distinct().OrderByDescending(s => s).ToArray();
        }

        public IQueryable<Stageopdracht> FindAllVanAcademiejaar(string academiejaar)
        {
            return _stageopdrachten
                .Where(so => string.IsNullOrEmpty(academiejaar) || so.Academiejaar == academiejaar).IncludeAndOrder();
        }

        public void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                string message = String.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {

                    message +=
                        String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.GetValidationResult());
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message +=
                            String.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new ApplicationException("" + message);
            }
        }

        #region helpers
        private Expression<Func<Stageopdracht, bool>> IsinHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar);
        }

        private Expression<Func<StagebegeleidingAanvraag, bool>> IsAanvraagInHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (aanvraag => aanvraag.Stage.Academiejaar == academiejaar);
        }
        #endregion

    }
}