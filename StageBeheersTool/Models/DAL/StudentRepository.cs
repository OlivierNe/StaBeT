using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using MySql.Data.MySqlClient;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace StageBeheersTool.Models.DAL
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Student> _studenten;

        public StudentRepository(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _studenten = ctx.Studenten;
        }

        public void Add(Student student)
        {
            try
            {
                _studenten.Add(student);
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1062)
                {
                    throw new ApplicationException(string.Format(
                        Resources.ErrorStudentCreateHogentEmailBestaatAl, student.HogentEmail));
                }
                throw;
            }
        }

        public void AddAll(IList<Student> studenten)
        {
            foreach (var student in studenten)
            {
                var oudeStudent = FindByEmail(student.HogentEmail);
                if (oudeStudent == null)
                {

                    _studenten.Add(student);
                }
                else
                {
                    UpdateStudent(oudeStudent, student);
                }
            }
            SaveChanges();
        }

        public Student FindByEmail(string hogentEmail)
        {
            return _studenten.SingleOrDefault(student => student.HogentEmail == hogentEmail);
        }

        public Student FindByNaam(string voornaam, string familienaam)
        {
            return _studenten.FirstOrDefault(student => student.Voornaam.ToLower() == voornaam.ToLower()
                && student.Familienaam.ToLower() == familienaam.ToLower());
        }

        public Student FindById(int id)
        {
            return _studenten.SingleOrDefault(s => s.Id == id);
        }

        public IQueryable<Student> FindAll()
        {
            return _studenten.OrderBy(student => student.Familienaam);
        }

        public IQueryable<Student> FindStudentenMetToegewezenStage()
        {
            var huidigAcademiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return _studenten.Include(student => student.Stages)
                .Where(student => student.Stages.Any(stage =>
                    stage.Stageopdracht.Academiejaar == huidigAcademiejaar))
                .OrderBy(student => student.Familienaam);
        }

        public void Update(Student student)
        {
            var teUpdatenStudent = FindById(student.Id);
            if (teUpdatenStudent == null)
                return;
            UpdateStudent(teUpdatenStudent, student);
            SaveChanges();
        }

        private void UpdateStudent(Student teUpdatenStudent, Student student)
        {
            teUpdatenStudent.Voornaam = student.Voornaam;
            teUpdatenStudent.Familienaam = student.Familienaam;
            teUpdatenStudent.Keuzepakket = student.Keuzepakket;
            teUpdatenStudent.Email = student.Email;
            teUpdatenStudent.Gsm = student.Gsm;
            teUpdatenStudent.Postcode = student.Postcode;
            teUpdatenStudent.Gemeente = student.Gemeente;
            teUpdatenStudent.Straat = student.Straat;
            teUpdatenStudent.Geboortedatum = student.Geboortedatum;
            teUpdatenStudent.Geboorteplaats = student.Geboorteplaats;
            if (teUpdatenStudent.Foto != null)
            {
                teUpdatenStudent.Foto.FotoData = student.Foto.FotoData;
                teUpdatenStudent.Foto.ContentType = student.Foto.ContentType;
                teUpdatenStudent.Foto.Naam = student.Foto.Naam;
            }
            else
            {
                teUpdatenStudent.Foto = student.Foto;
            }
        }

        public void Delete(Student student)
        {
            try
            {
                _studenten.Remove(student);
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1451)
                {
                    throw new ApplicationException(string.Format(
                        Resources.ErrorDeleteStudent, student.Naam));
                }
                throw;
            }
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
                    message = eve.ValidationErrors.Aggregate(message, (current, ve) => current +
                        String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                }
                throw new ApplicationException("" + message);
            }
        }

    }
}