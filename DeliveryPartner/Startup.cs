using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeliveryPartner.Startup))]
namespace DeliveryPartner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
