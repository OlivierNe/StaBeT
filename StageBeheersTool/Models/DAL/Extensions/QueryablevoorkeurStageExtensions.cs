using System;
using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Extensions
{
    public static class QueryablevoorkeurStageExtensions
    {
        public static IQueryable<VoorkeurStage> WithFilter(this IQueryable<VoorkeurStage> voorkeurStages,
            string naam = null, string voornaam = null)
        {
            if (String.IsNullOrWhiteSpace(naam) == false)
            {
                voorkeurStages = voorkeurStages.Where(vs => vs.Student.Familienaam.ToLower().Contains(naam.ToLower()));
            }
            if (String.IsNullOrWhiteSpace(voornaam) == false)
            {
                voorkeurStages = voorkeurStages.Where(vs => vs.Student.Voornaam.ToLower().Contains(voornaam.ToLower()));
            }
            return voorkeurStages;
        }
    }
}