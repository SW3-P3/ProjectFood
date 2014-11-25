using System.Collections.Generic;
using Microsoft.Owin;
using Owin;
using ProjectFood.Models;

[assembly: OwinStartupAttribute(typeof(ProjectFood.Startup))]
namespace ProjectFood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
