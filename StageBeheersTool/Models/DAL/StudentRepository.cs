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

        public Student FindByEmail(string email)
        {
            return studenten.FirstOrDefault(student => student.Email == email);
        }

        public IQueryable<Student> FindAll()
        {
            return studenten;
        }

        public void SaveChanges()
        {
            ctx.SaveChanges();
        }
    }
}