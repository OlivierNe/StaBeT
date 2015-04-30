using System.Collections.Generic;
using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IStudentRepository
    {
        void Add(Student student);
        void AddAll(IList<Student> studenten);
        Student FindByEmail(string hoGentEmail);
        Student FindById(int id);
        Student FindByNaam(string voornaam, string familienaam);
        IQueryable<Student> FindAll();
        IQueryable<Student> FindStudentenMetToegewezenStage();
        void SaveChanges();
        void Update(Student student);
        void Delete(Student student);
    }
}