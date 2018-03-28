using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Tweetinvi;

namespace NewsFeedMe.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.Message = TempData["result"] as string;
            ViewBag.errorMessage = TempData["error"] as string;
            return View("Login");
        }

        public ActionResult TwitterAuth(string returnUrl)
        {
            // Request a redirect to the external challenge class
            return new ChallengeResult("Twitter",
              Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var authenticateResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("ExternalCookie");

            if (authenticateResult != null)
            {
                long userid = Convert.ToInt64(authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == "urn:twitter:userid").Value);

                if (!UserExists(userid))
                {
                    var oauthToken = authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == ("urn:twitter:access_token")).Value;
                    var oauthSecret = authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == ("urn:twitter:access_token_secret")).Value;

                    string Key = WebConfigurationManager.AppSettings["TwitterKey"];
                    string Secret = WebConfigurationManager.AppSettings["TwitterSecret"];

                    //authorize user with Twitter and load user data 
                    Auth.SetUserCredentials(Key, Secret, oauthToken, oauthSecret);
                    var twitterUser = Tweetinvi.User.GetAuthenticatedUser();
                  
                    //load twitter user data into User class
                    var user = new User { Id = twitterUser.Id, Access_Token = oauthToken, Secret = oauthSecret, ExternalService = "Twitter", ScreenName = twitterUser.ScreenName, ProfilePictureURL = twitterUser.ProfileImageUrl400x400 };

                    using (var context = new EntityFramework())
                    {
                        //new user is saved to DB with EntityFramework
                        var result = context.Users.Add(user);
                        await context.SaveChangesAsync();
                    }
                    
                    AuthenticationManager.SignIn();
                    TempData["result"] = "You're all signed up!";
                    return RedirectToAction("Following", "Manage");

                }
                else
                {
                    return RedirectToAction("Home", "Feed");
                }
            }
            TempData["error"] = "Your session expired! Please log in again";
            return RedirectToAction("LogOff");
        }

        private bool UserExists(long userid)
        {
            bool result = false;
            try
            {
                using (var context = new EntityFramework())
                {
                    result = (from user in context.Users
                              where user.Id == userid
                              select user.Id).Any();
                }
            }
            catch { throw; }
            return result;
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