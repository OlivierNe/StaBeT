using AutoMapper;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;

namespace StageBeheersTool.App_Start
{
    public class AutomapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<StageopdrachtCreateVM, Stageopdracht>();
            Mapper.CreateMap<Stageopdracht, StageopdrachtCreateVM>();
            Mapper.CreateMap<StageopdrachtEditVM, Stageopdracht>();
            Mapper.CreateMap<Stageopdracht, StageopdrachtEditVM>();
            Mapper.CreateMap<Stageopdracht, StageopdrachtAfkeurenVM>();

            Mapper.CreateMap<ContactpersoonCreateVM, Contactpersoon>();
            Mapper.CreateMap<ContactpersoonEditVM, Contactpersoon>();
            Mapper.CreateMap<Contactpersoon, ContactpersoonCreateVM>();
            Mapper.CreateMap<Contactpersoon, ContactpersoonEditVM>();

            Mapper.CreateMap<Student, StudentCreateVM>();
            Mapper.CreateMap<StudentCreateVM, Student>();
            Mapper.CreateMap<Student, StudentEditVM>();
            Mapper.CreateMap<StudentEditVM, Student>();
            Mapper.CreateMap<Student, StudentDetailsVM>();
            Mapper.CreateMap<Student, StudentJsonVM>();

            Mapper.CreateMap<RegisterBedrijfViewModel, Bedrijf>();
            Mapper.CreateMap<EditBedrijfVM, Bedrijf>();
            Mapper.CreateMap<Bedrijf, EditBedrijfVM>();
            Mapper.CreateMap<Bedrijf, BedrijfInfoVM>();

            Mapper.CreateMap<BegeleiderCreateVM, Begeleider>();
            Mapper.CreateMap<Begeleider, BegeleiderCreateVM>();
            Mapper.CreateMap<Begeleider, BegeleiderEditVM>();
            Mapper.CreateMap<BegeleiderEditVM, Begeleider>();
            Mapper.CreateMap<Begeleider, BegeleiderDetailsVM>();
            Mapper.CreateMap<Begeleider, BegeleiderJsonVM>();

            Mapper.CreateMap<AcademiejaarInstellingen, AcademiejaarInstellingenVM>();
            Mapper.CreateMap<AcademiejaarInstellingenVM, AcademiejaarInstellingen>();

            Mapper.CreateMap<AdminVm, ApplicationUser>();
            Mapper.CreateMap<ApplicationUser, AdminVm>();

        }
    }
}