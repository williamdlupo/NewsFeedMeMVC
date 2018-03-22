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
                int userid = Convert.ToInt32(authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == "urn:twitter:userid").Value);

                if (!UserExists(userid))
                {
                    var oauthToken = authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == ("urn:twitter:access_token")).Value;
                    var oauthSecret = authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == ("urn:twitter:access_token_secret")).Value;

                    string Key = WebConfigurationManager.AppSettings["TwitterKey"];
                    string Secret = WebConfigurationManager.AppSettings["TwitterSecret"];

                    TwitterService service = new TwitterService(Key, Secret);

                    service.AuthenticateWith(oauthToken, oauthSecret);
                    VerifyCredentialsOptions option = new VerifyCredentialsOptions();

                    //Use Access Tokens to access user Twitter data  
                    TwitterUser twitterdata = service.VerifyCredentials(option);

                    //load twitter user data into User class
                    var user = new User { Id = Convert.ToInt32(twitterdata.Id), Access_Token = oauthToken, Secret = oauthSecret, ExternalService = "Twitter", ScreenName = twitterdata.ScreenName.ToString(), ProfilePictureURL = twitterdata.ProfileImageUrlHttps };

                    using (var context = new EntityFramework())
                    {
                        //new user is saved to DB with EntityFramework
                        var result = context.Users.Add(user);
                        context.SaveChanges();
                    }

                    Session["ProfilePicture"] = user.ProfilePictureURL;
                    AuthenticationManager.SignIn();
                    TempData["result"] = "You're all signed up!";
                    return RedirectToAction("Following", "Manage");

                }
                else
                {
                    using (var context = new EntityFramework())
                    {
                        int user = context.Users.Where(x => x.ScreenName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();

                        var userstatus = (from db in context.Users
                                          where db.Id == user
                                          select new { db.ScreenName, db.ProfilePictureURL }).FirstOrDefault();

                        Session["ProfilePicture"] = userstatus.ProfilePictureURL;
                    }
                    return RedirectToAction("Home", "Feed");
                }
            }
            TempData["error"] = "Your session expired! Please log in again";
            return RedirectToAction("LogOff");
        }

        private bool UserExists(int userid)
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