using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IStudentRepository
    {
        void Add(Student student);
        Student FindByEmail(string hoGentEmail);
        Student FindById(int id);
        IQueryable<Student> FindAll();
        IQueryable<Student> FindStudentenMetToegewezenStage();
        void SaveChanges();
        void Update(Student student);
        void Delete(Student student);
    }
}