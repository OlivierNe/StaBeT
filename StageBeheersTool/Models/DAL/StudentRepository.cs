using System.Data.Entity.Infrastructure;
using MySql.Data.MySqlClient;
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
            this._dbContext = ctx;
            this._studenten = ctx.Studenten;
        }

        public bool Add(Student student)
        {
            try
            {
                _studenten.Add(student);
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1062)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public Student FindByEmail(string hogentEmail)
        {
            return _studenten.SingleOrDefault(student => student.HogentEmail == hogentEmail);
        }

        public Student FindById(int id)
        {
            return _studenten.SingleOrDefault(s => s.Id == id);
        }

        public IQueryable<Student> FindAll()
        {
            return _studenten.OrderBy(student => student.Familienaam);
        }

        public IQueryable<Student> FindStudentenMetStageopdrachtEnBegeleider()
        {
            throw new NotSupportedException();
            //return FindAll().Include(student => student.Stageopdrachten)
            //    .Where(student => student.Stageopdrachten != null &&
            //        student.Stageopdracht.Status == StageopdrachtStatus.Goedgekeurd &&
            //        student.Stageopdracht.Stagebegeleider != null);
        }

        public void Update(Student student)
        {
            var teUpdatenStudent = FindById(student.Id);
            if (teUpdatenStudent == null)
                return;
            teUpdatenStudent.Voornaam = student.Voornaam;
            teUpdatenStudent.Familienaam = student.Familienaam;
            teUpdatenStudent.Keuzepakket = student.Keuzepakket;
            teUpdatenStudent.Email = student.Email;
            teUpdatenStudent.Gsm = student.Gsm;
            teUpdatenStudent.Postcode = student.Postcode;
            teUpdatenStudent.Gemeente = student.Gemeente;
            teUpdatenStudent.Straat = student.Straat;
            teUpdatenStudent.FotoUrl = student.FotoUrl;
            SaveChanges();
        }

        public void Delete(Student student)
        {
            _studenten.Remove(student);
            SaveChanges();
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
        
    }
}