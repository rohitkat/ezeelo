using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gandhibagh.Startup))]
namespace Gandhibagh
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
