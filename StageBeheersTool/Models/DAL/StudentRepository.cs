using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class StudentRepository : IStudentRepository
    {
        private StageToolDbContext ctx;
        private DbSet<Student> studenten;

        public StudentRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            studenten = ctx.Studenten;
        }

        public void Add(Student student)
        {
            studenten.Add(student);
            SaveChanges();
        }

        public Student FindByEmail(string hogentEmail)
        {
            return studenten.SingleOrDefault(student => student.HogentEmail == hogentEmail);
        }

        public Student FindById(int id)
        {
            return studenten.SingleOrDefault(s => s.Id == id);
        }

        public IQueryable<Student> FindAll()
        {
            return studenten.OrderBy(student => student.Familienaam).OrderBy(student => student.Voornaam);
        }

        public IQueryable<Student> FindStudentenMetStageopdrachtEnBegeleider()
        {
            return FindAll().Include(student => student.Stageopdracht)
                .Where(student => student.Stageopdracht != null &&
                    student.Stageopdracht.Status == StageopdrachtStatus.Goedgekeurd &&
                    student.Stageopdracht.Stagebegeleider != null);
        }

        public void Update(Student student, Student model)
        {
            student.Voornaam = model.Voornaam;
            student.Familienaam = model.Familienaam;
            student.Keuzepakket = model.Keuzepakket;
            student.Email = model.Email;
            student.Gsmnummer = model.Gsmnummer;
            student.Postcode = model.Postcode;
            student.Gemeente = model.Gemeente;
            student.Straat = model.Straat;
            student.Straatnummer = model.Straatnummer;
            student.FotoUrl = model.FotoUrl;
            SaveChanges();
        }
        public void SaveChanges()
        {
            try
            {
                ctx.SaveChanges();
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