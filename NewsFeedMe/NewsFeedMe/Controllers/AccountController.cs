using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TweetSharp;

namespace NewsFeedMe.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult TwitterAuth(string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult("Twitter",
              Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var claim = System.Security.Claims.ClaimsPrincipal.Current.Claims;

            var oauthToken = claim.FirstOrDefault(x => x.Type.EndsWith("twitter:access_token")).Value;
            var oauthSecret = claim.FirstOrDefault(x => x.Type.EndsWith("twitter:access_token_secret")).Value;

            string Key = WebConfigurationManager.AppSettings["TwitterKey"];
            string Secret = WebConfigurationManager.AppSettings["TwitterSecret"];
            try
            {
                TwitterService service = new TwitterService(Key, Secret);

                service.AuthenticateWith(oauthToken, oauthSecret);
                VerifyCredentialsOptions option = new VerifyCredentialsOptions();

                //According to Access Tokens get user profile details  
                TwitterUser user = service.VerifyCredentials(option);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                throw;
            }
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            Session.RemoveAll();
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        // Implementation copied from a standard MVC Project, with some stuff
        // that relates to linking a new external login to an existing identity
        // account removed.
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}