using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("GStoreWeb", typeof(GStoreWeb.Startup))]
namespace GStoreWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
