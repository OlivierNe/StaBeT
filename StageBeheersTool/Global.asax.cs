using StageBeheersTool.App_Start;
using StageBeheersTool.Models.DAL;
using System.Data.Entity;
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
            var init = new StageToolDbInitializer();
            Database.SetInitializer<StageToolDbContext>(init);
            var ctx = new StageToolDbContext();
            ctx.Database.Initialize(true);
            //init.AddOudeGegevens(ctx);
        }
    }
}
