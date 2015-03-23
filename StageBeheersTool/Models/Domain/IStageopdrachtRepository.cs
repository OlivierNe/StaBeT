using System.Collections;
using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IStageopdrachtRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// <para>- bedrijf: returns null als stageopdracht niet van het ingelogde bedrijf is</para>
        /// <para>- student: returns goedgekeurde stageopdracht van huidig academiejaar die nog niet volledig ingenomen is</para>
        /// <para>- admin/begeleider: returns stageopdracht of null als ze niet bestaat</para>
        /// </returns>
        Stageopdracht FindById(int id);

        /// <summary>
        /// <para>- bedrijf: alleen stageopdrachten van huidig ingelogd bedrijf</para>
        /// <para>- student: goedgekeurde van het huidig academiejaar die nog niet volledig ingenomen zijn</para>
        /// <para>- admin/begeleider: alle goedgekeurde van het huidige academiejaar</para>
        /// </summary>
        /// <returns></returns>
        IQueryable<Stageopdracht> FindAll();

        /// <summary>
        /// nog niet beoordeelde stageopdrachten van bedrijven voor het huidig academiejaar
        /// </summary>
        /// <returns></returns>
        IQueryable<Stageopdracht> FindStageopdrachtVoorstellen();

        /// <summary>
        /// <para>- bedrijf: alleen stageopdrachten van huidig ingelogd bedrijf</para>
        /// <para>- student: goedgekeurde van het huidig academiejaar die nog niet volledig ingenomen zijn</para>
        /// <para>- admin/begeleider: alle goedgekeurde van het huidige academiejaar</para>
        /// </summary>
        /// <returns></returns>
        IQueryable<Stageopdracht> FindAllWithFilter(int? semester, int? aantalStudenten, string specialisatie, string bedrijf, string locatie, string student);

        /// <summary>
        /// </summary>
        /// <param name="academiejaar"></param>
        /// <returns>Alle stageopdrachten van een academiejaar</returns>
        IQueryable<Stageopdracht> FindAllFromAcademiejaar(string academiejaar);

        /// <summary>
        /// Alle goedgekeurde stageopdrachten van het huidige academiejaar die nog geen begeleider hebben
        /// </summary>
        /// <returns></returns>
        IQueryable<Stageopdracht> FindGeldigeBegeleiderStageopdrachtenWithFilter(int? semester, int? aantalStudenten, string specialisatie, string bedrijf, string locatie, string student);

        void Update(Stageopdracht stageopdracht);
        void Delete(Stageopdracht stageopdracht);

        StageBegeleidAanvraag FindAanvraagById(int id);
    
        /// <summary>
        /// alle begeleider aanvragen van het huidige academiejaar
        /// </summary>
        /// <returns></returns>
        IQueryable<StageBegeleidAanvraag> FindAllAanvragen();

        /// <summary>
        /// alle begeleider aanvragen van het huidige academiejaar van een begeleider
        /// </summary>
        /// <returns></returns>
        IQueryable<StageBegeleidAanvraag> FindAllAanvragenFrom(Begeleider begeleider);

        void AddAanvraag(StageBegeleidAanvraag aanvraag);
        void DeleteAanvraag(StageBegeleidAanvraag aanvraag);

        /// <summary>
        /// </summary>
        /// <returns>
        /// Lijst van alle academiejaren waar minstens 1 stageopdracht van aanwezig is
        /// </returns>
        string[] FindAllAcademiejaren();

        IQueryable<Stageopdracht> FindAllVanAcademiejaar(string academiejaar);

        void SaveChanges();
    }
}