using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NewsFeedMe.App_Start.Startup))]

namespace NewsFeedMe.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
