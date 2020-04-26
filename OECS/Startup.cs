using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OECS.Startup))]
namespace OECS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}