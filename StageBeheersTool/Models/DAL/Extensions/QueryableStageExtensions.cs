using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Extensions
{
    public static class QueryableStageExtensions
    {
        public static IQueryable<Stage> WithFilter(this IQueryable<Stage> stages,
            string stageopdracht, string bedrijf, string student, string begeleider)
        {
            if (string.IsNullOrWhiteSpace(stageopdracht) == false)
            {
                stages = stages.Where(stage => stage.Stageopdracht.Titel.ToLower().Contains(stageopdracht.ToLower()));
            }
            if (string.IsNullOrWhiteSpace(bedrijf) == false)
            {
                stages = stages.Where(stage => stage.Stageopdracht.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower()));
            }
            if (string.IsNullOrWhiteSpace(begeleider) == false)
            {
                stages = stages.Where(stage => (stage.Stageopdracht.Stagebegeleider != null) && (
                    (stage.Stageopdracht.Stagebegeleider.Voornaam.ToLower().Contains(begeleider.ToLower())) ||
                    (stage.Stageopdracht.Stagebegeleider.Familienaam.ToLower().Contains(begeleider.ToLower()))));
            }
            if (string.IsNullOrWhiteSpace(student) == false)
            {
                stages = stages.Where(stage => (stage.Student.Voornaam.ToLower().Contains(student.ToLower())) ||
                    (stage.Student.Familienaam.ToLower().Contains(student.ToLower())));
            }
            return stages;
        }

        public static IQueryable<Stage> WithFilter(this IQueryable<Stage> stages,
           int? begeleiderId, string academiejaar)
        {
            if (begeleiderId != null)
            {
                stages = stages.Where(stage => stage.Stageopdracht.Stagebegeleider.Id == begeleiderId);
            }
            if (string.IsNullOrWhiteSpace(academiejaar) == false)
            {
                stages = stages.Where(stage => stage.Stageopdracht.Academiejaar == academiejaar);
            }

            return stages;
        }

    }
}