using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
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
            if (Request.IsAuthenticated) { return RedirectToAction("Home", "Feed"); }

            ViewBag.Message = TempData["result"] as string;
            ViewBag.errorMessage = TempData["error"] as string;
            return View();
        }

        public ActionResult TwitterAuth(string returnUrl)
        {
            // Request a redirect to the external challenge class
            return new ChallengeResult("Twitter",
              Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var claim = System.Security.Claims.ClaimsPrincipal.Current.Claims;
            
            using (var context = new EntityFramework())
            {
                try
                {
                    int userid = Convert.ToInt32(claim.FirstOrDefault(x => x.Type.EndsWith("twitter:userid")).Value);
                    
                    var userstatus = from db in context.Users
                                     where db.Id == userid
                                     select new { db.ScreenName, db.ProfilePictureURL };
                    
                    //Determine if user already exists - if not, create the user
                    if (userstatus.Count() == 0)
                    {
                        var oauthToken = claim.FirstOrDefault(x => x.Type.EndsWith("twitter:access_token")).Value;
                        var oauthSecret = claim.FirstOrDefault(x => x.Type.EndsWith("twitter:access_token_secret")).Value;

                        string Key = WebConfigurationManager.AppSettings["TwitterKey"];
                        string Secret = WebConfigurationManager.AppSettings["TwitterSecret"];

                        try
                        {
                            TwitterService service = new TwitterService(Key, Secret);

                            service.AuthenticateWith(oauthToken, oauthSecret);
                            VerifyCredentialsOptions option = new VerifyCredentialsOptions();

                            //Use Access Tokens to access user Twitter data  
                            TwitterUser twitterdata = service.VerifyCredentials(option);

                            //load twitter user data into User class
                            var user = new User { Id = Convert.ToInt32(twitterdata.Id), Access_Token = oauthToken, Secret = oauthSecret, ExternalService = "Twitter", ScreenName = twitterdata.ScreenName.ToString(), ProfilePictureURL = twitterdata.ProfileImageUrlHttps };


                            //new user is saved to DB with EntityFramework
                            var result = context.Users.Add(user);
                            await context.SaveChangesAsync();

                            Session["ProfilePicture"] = user.ProfilePictureURL;

                            TempData["result"] = "You're all signed up!";
                            return RedirectToAction("Following", "Manage");
                        }

                        //something went wrong, throw error and redirect back to login page.
                        catch
                        {
                            TempData["error"] = "Something went wrong";
                            return RedirectToAction("LogOff");
                        }
                    }

                    Session["ProfilePicture"] = userstatus.FirstOrDefault().ProfilePictureURL;
                    return RedirectToAction("Home", "Feed");
                }
                catch { throw; }
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