using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Web.Configuration;

namespace NewsFeedMe.App_Start
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var cookieOptions = new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Account/Login")
            };

            app.UseCookieAuthentication(cookieOptions);

            app.SetDefaultSignInAsAuthenticationType(cookieOptions.AuthenticationType);

            app.UseTwitterAuthentication(
               consumerKey: WebConfigurationManager.AppSettings["TwitterKey"],
               consumerSecret: WebConfigurationManager.AppSettings["TwitterSecret"]);
        }
    }
}