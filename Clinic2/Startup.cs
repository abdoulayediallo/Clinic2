using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Clinic2.Startup))]
namespace Clinic2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
