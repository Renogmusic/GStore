using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("GStore", typeof(GStore.Startup))]
namespace GStore
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
