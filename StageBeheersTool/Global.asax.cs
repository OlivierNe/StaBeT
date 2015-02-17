using StageBeheersTool.App_Start;
using StageBeheersTool.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace StageBeheersTool
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutomapperConfig.Configure();
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
  
            Database.SetInitializer<StageBeheersTool.Models.DAL.StageToolDbContext>(new StageBeheersTool.Models.DAL.StageToolDbInitializer());
            var bedrijven = new StageToolDbContext().Bedrijven.ToList();
        }
    }
}
