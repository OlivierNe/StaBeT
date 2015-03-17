using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.App_Start
{
    public class AutomapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<RegisterBedrijfViewModel, Bedrijf>();
            Mapper.CreateMap<StageopdrachtCreateVM, Stageopdracht>();
            Mapper.CreateMap<Stageopdracht, StageopdrachtCreateVM>();
            Mapper.CreateMap<StageopdrachtEditVM, Stageopdracht>();
            Mapper.CreateMap<Stageopdracht, StageopdrachtEditVM>();
            Mapper.CreateMap<ContactpersoonCreateVM, Contactpersoon>();
            Mapper.CreateMap<ContactpersoonEditVM, Contactpersoon>();
            Mapper.CreateMap<Contactpersoon, ContactpersoonCreateVM>();
            Mapper.CreateMap<Contactpersoon, ContactpersoonEditVM>();
            Mapper.CreateMap<Student, StudentEditVM>();
            Mapper.CreateMap<StudentEditVM, Student>();
            Mapper.CreateMap<EditBedrijfVM, Bedrijf>();
            Mapper.CreateMap<Bedrijf, EditBedrijfVM>();
            Mapper.CreateMap<Begeleider, BegeleiderEditVM>();
            Mapper.CreateMap<BegeleiderEditVM, Begeleider>();
            Mapper.CreateMap<Stageopdracht, StageopdrachtAfkeurenVM>();
            Mapper.CreateMap<AcademiejaarInstellingen, AcademiejaarInstellingenVM>();
            Mapper.CreateMap<AcademiejaarInstellingenVM, AcademiejaarInstellingen>();
        }
    }
}