using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(bat.Startup))]
namespace bat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
