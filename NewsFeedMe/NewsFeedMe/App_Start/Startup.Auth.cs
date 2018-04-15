using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace NewsFeedMe.App_Start
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType("ExternalCookie");
            var cookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie",
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                CookieName = ".AspNet.ExternalCookie",
                LogoutPath = new PathString("/Account/LogOff"),
                LoginPath = new PathString("/Account/Login")
            };
            app.UseCookieAuthentication(cookieOptions);

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
                        return Task.FromResult(true);
                    }
                }
            };
            app.UseTwitterAuthentication(twitterOptions);
        }
    }
}