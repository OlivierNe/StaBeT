using Microsoft.AspNet.Identity.EntityFramework;
using StageBeheersTool.App_Start;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.DAL;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StageBeheersTool.Models.Domain;

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
            //var umanager = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));
            //var user = new ApplicationUser() {UserName = "begeleider1@test.be", Email = "begeleider1@test.be"};
            //umanager.CreateAsync(user, "wachtwoord");
            //umanager.AddToRole(user.Id, Role.Begeleider);
            //ctx.Begeleiders.Add(new Begeleider() {HogentEmail = "begeleider1@test.be"});
            //ctx.SaveChanges();
        }
    }
}
