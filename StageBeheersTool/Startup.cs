using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StageBeheersTool.Startup))]
namespace StageBeheersTool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
