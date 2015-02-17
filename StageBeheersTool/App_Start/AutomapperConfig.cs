using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.App_Start
{
   public class AutomapperConfig {
       public static void Configure()
       {
           Mapper.CreateMap<RegisterBedrijfViewModel, Bedrijf>();

       }
   }
}