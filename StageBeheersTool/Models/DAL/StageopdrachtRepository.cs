﻿using System.Linq.Expressions;
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
        private readonly StageToolDbContext _ctx;
        private readonly DbSet<Stageopdracht> _stageopdrachten;
        private readonly DbSet<StageBegeleidAanvraag> _aanvragen;
        private readonly IUserService _userService;

        public StageopdrachtRepository(StageToolDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
            _aanvragen = ctx.StageBegeleidAanvragen;
            _stageopdrachten = ctx.Stageopdrachten;
        }

        public Stageopdracht FindById(int id)
        {
            Stageopdracht stageopdracht;
            if (_userService.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            else if (_userService.IsStudent())
            {
                stageopdracht = _stageopdrachten.Where(IsGeldig())
                    .IncludeAndOrder()
                    .SingleOrDefault(so => so.Id == id);
            }
            else // admin/begeleider
            {
                stageopdracht = _stageopdrachten.IncludeAndOrder()
                    .SingleOrDefault(so => so.Id == id);
            }
            return stageopdracht;
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            IQueryable<Stageopdracht> stageopdrachten;
            if (_userService.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                stageopdrachten = bedrijf.Stageopdrachten.AsQueryable();
            }
            else if (_userService.IsStudent())
            {
                stageopdrachten = _stageopdrachten.Where(IsGeldig());
            }
            else // admin/begeleider
            {
                stageopdrachten = _stageopdrachten
                    .Where(IsGoedgekeurdEnVanHuidigAcademiejaar());
            }
            return stageopdrachten.IncludeAndOrder();
        }

        public IQueryable<Stageopdracht> FindStageopdrachtVoorstellen()
        {
            return _stageopdrachten.Where(IsNietBeoordeeldEnVanHuidigAcademiejaar()).IncludeAndOrder();
        }

        public IQueryable<Stageopdracht> FindAllFromAcademiejaar(string academiejaar)
        {
            return _stageopdrachten.Where(so => so.Academiejaar == academiejaar);
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
            teUpdatenStageopdracht.Academiejaar = stageopdracht.Academiejaar;
            teUpdatenStageopdracht.AantalStudenten = stageopdracht.AantalStudenten;
            teUpdatenStageopdracht.Stagementor = stageopdracht.Stagementor;
            teUpdatenStageopdracht.Contractondertekenaar = stageopdracht.Contractondertekenaar;
            teUpdatenStageopdracht.Gemeente = stageopdracht.Gemeente;
            teUpdatenStageopdracht.Postcode = stageopdracht.Postcode;
            teUpdatenStageopdracht.Straat = stageopdracht.Straat;
            teUpdatenStageopdracht.Straatnummer = stageopdracht.Straatnummer;
            SaveChanges();
        }

        public void Delete(Stageopdracht stageopdracht)
        {
            _stageopdrachten.Remove(stageopdracht);
            SaveChanges();
        }

        public StageBegeleidAanvraag FindAanvraagById(int id)
        {
            return _aanvragen
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stageopdracht)
                .SingleOrDefault(a => a.Id == id);
        }

        public IQueryable<StageBegeleidAanvraag> FindAllAanvragen()
        {
            return _aanvragen
                .Where(IsAanvraagInHuidigAcademiejaar())
                .OrderBy(sba => sba.Begeleider.Familienaam)
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stageopdracht);
        }

        public IQueryable<StageBegeleidAanvraag> FindAllAanvragenFrom(Begeleider begeleider)
        {
            return _aanvragen
                 .Where(sba => sba.Begeleider.Id == begeleider.Id)
                 .Where(IsAanvraagInHuidigAcademiejaar())
                 .OrderBy(sba => sba.Begeleider.Familienaam)
                 .Include(sba => sba.Begeleider)
                 .Include(sba => sba.Stageopdracht);
        }

        public IQueryable<Stageopdracht> FindStageopdrachtenVanBegeleider()
        {
            var begeleider = _userService.FindBegeleider();
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return _stageopdrachten.Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail &&
                            so.Academiejaar == academiejaar).IncludeAndOrder();
        }


        public IQueryable<Stageopdracht> FindGeldigeBegeleiderStageopdrachten()
        {
            return _stageopdrachten
                .Where(IsGoedgekeurdEnVanHuidigAcademiejaar())
                .Where(so => so.Stagebegeleider == null)
                .IncludeAndOrder();
        }

        public void AddAanvraag(StageBegeleidAanvraag aanvraag)
        {
            _aanvragen.Add(aanvraag);
            SaveChanges();
        }

        public void DeleteAanvraag(StageBegeleidAanvraag aanvraag)
        {
            _aanvragen.Remove(aanvraag);
            SaveChanges();
        }

        public string[] FindAllAcademiejaren()
        {
            return _stageopdrachten.Select(so => so.Academiejaar).Distinct().OrderBy(s => s).ToArray();
        }

        public IQueryable<Stageopdracht> FindAllVanAcademiejaar(string academiejaar)
        {
            return _stageopdrachten
                .Where(so => so.Academiejaar == academiejaar).IncludeAndOrder();
        }

        public string[] FindAllAcademiejarenVanBegeleider()
        {
            var begeleider = _userService.FindBegeleider();
            return _stageopdrachten
                .Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail)
                .Select(so => so.Academiejaar).Distinct().OrderBy(s => s).ToArray();
        }

        public IQueryable<Stageopdracht> FindMijnStagesVanAcademiejaar(string academiejaar)
        {
            var begeleider = _userService.FindBegeleider();
            return _stageopdrachten
                .Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail &&
                so.Academiejaar == academiejaar).IncludeAndOrder();
        }

        public void SaveChanges()
        {
            try
            {
                _ctx.SaveChanges();
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

        #region private helpers
        /*private Expression<Func<Stageopdracht, bool>> Filter(int? semester, int? aantalStudenten,
            string specialisatie, string bedrijf, string locatie, string student)
        {
            return so => (semester == null || ((so.Semester1 && semester == 1) || (so.Semester2 && semester == 2))) &&
                         (aantalStudenten == null || so.AantalStudenten == aantalStudenten) &&
                         (string.IsNullOrEmpty(specialisatie) ||
                          so.Specialisatie.ToLower().Contains(specialisatie.ToLower())) &&
                         (string.IsNullOrEmpty(bedrijf) || so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                         (string.IsNullOrEmpty(locatie) || so.Gemeente.ToLower().Contains(locatie.ToLower())) &&
                         (string.IsNullOrEmpty(student) || so.Studenten.Any(s =>
                             (s.Familienaam != null && s.Familienaam.ToLower().Contains(student.ToLower())) ||
                                   (s.Voornaam != null && s.Voornaam.ToLower().Contains(student.ToLower()))));
        }

        private Expression<Func<Stageopdracht, bool>> StudentEnBedrijfFilter(string student, string bedrijf)
        {
            return so => (string.IsNullOrEmpty(bedrijf) || so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                         (string.IsNullOrEmpty(student) || so.Studenten.Any(s =>
                             (s.Familienaam != null && s.Familienaam.ToLower().Contains(student.ToLower())) ||
                                   (s.Voornaam != null && s.Voornaam.ToLower().Contains(student.ToLower()))));
        }
        */
        private Expression<Func<Stageopdracht, bool>> IsGeldig()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                         so.Status == StageopdrachtStatus.Goedgekeurd && so.Studenten.Count != so.AantalStudenten);
        }

        private Expression<Func<Stageopdracht, bool>> IsGoedgekeurdEnVanHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                          so.Status == StageopdrachtStatus.Goedgekeurd);
        }

        private Expression<Func<Stageopdracht, bool>> IsNietBeoordeeldEnVanHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                          so.Status == StageopdrachtStatus.NietBeoordeeld);
        }

        private Expression<Func<StageBegeleidAanvraag, bool>> IsAanvraagInHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (aanvraag => aanvraag.Stageopdracht.Academiejaar == academiejaar);
        }

        #endregion




    }


}