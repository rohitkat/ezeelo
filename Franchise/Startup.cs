using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Franchise.Startup))]
namespace Franchise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
