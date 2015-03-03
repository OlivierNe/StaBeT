using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        }

        public Student FindByEmail(string hogentEmail)
        {
            return studenten.FirstOrDefault(student => student.HogentEmail == hogentEmail);
        }

        public Student FindById(int id)
        {
            return studenten.FirstOrDefault(s => s.Id == id);
        }

        public IQueryable<Student> FindAll()
        {
            return studenten;
        }

        public void SaveChanges()
        {
            ctx.SaveChanges();
        }

        public void Update(Student student, Student newStudent)
        {
            student.Voornaam = newStudent.Voornaam;
            student.Familienaam = newStudent.Familienaam;
            student.Keuzepakket = newStudent.Keuzepakket;
            student.Email = newStudent.Email;
            student.Gsmnummer = newStudent.Gsmnummer;
            student.Postcode = newStudent.Postcode;
            student.Gemeente = newStudent.Gemeente;
            student.Straat = newStudent.Straat;
            student.Straatnummer = newStudent.Straatnummer;
            student.FotoUrl = newStudent.FotoUrl;
        }


    }
}