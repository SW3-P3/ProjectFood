using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectFood.Startup))]
namespace ProjectFood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // derp
            ConfigureAuth(app);
        }
    }
}
