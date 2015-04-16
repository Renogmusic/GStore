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

			Microsoft.AspNet.SignalR.HubConfiguration config = new Microsoft.AspNet.SignalR.HubConfiguration();
			config.EnableDetailedErrors = true;
			app.MapSignalR(config);
        }
    }
}
