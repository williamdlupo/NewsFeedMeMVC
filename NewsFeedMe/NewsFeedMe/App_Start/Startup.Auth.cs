using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace NewsFeedMe.App_Start
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var cookieOptions = new CookieAuthenticationOptions
            {
                SlidingExpiration = true,
                ExpireTimeSpan = System.TimeSpan.FromMinutes(10),
                LoginPath = new PathString("/Account/Login")
            };

            app.UseCookieAuthentication(cookieOptions);

            app.SetDefaultSignInAsAuthenticationType(cookieOptions.AuthenticationType);

            var twitterOptions = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions
            {
                ConsumerKey = WebConfigurationManager.AppSettings["TwitterKey"],
                ConsumerSecret = WebConfigurationManager.AppSettings["TwitterSecret"],
                Provider = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationProvider
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_token", context.AccessToken, null, "Twitter"));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_token_secret", context.AccessTokenSecret, null, "Twitter"));
                        return Task.FromResult(0);
                    }
                }
            };
            app.UseTwitterAuthentication(twitterOptions);
        }
    }
}