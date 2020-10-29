using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Merchant.Startup))]
namespace Merchant
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
