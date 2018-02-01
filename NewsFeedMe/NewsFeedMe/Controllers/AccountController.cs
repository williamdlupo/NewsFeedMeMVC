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

        //
        //Twitter authentication with TweetSharp
        [HttpGet]
        public ActionResult TwitterAuth()
        {
            string Key = WebConfigurationManager.AppSettings["TwitterKey"];
            string Secret = WebConfigurationManager.AppSettings["TwitterSecret"];
            TwitterService service = new TwitterService(Key, Secret);

            //Obtaining a request token  
            OAuthRequestToken requestToken = service.GetRequestToken("https://localhost:44363/Account/TwitterCallback");
            Uri uri = service.GetAuthenticationUrl(requestToken);

            //Redirecting the user to Twitter  
            return Redirect(uri.ToString());
        }

        //
        //Function to handle callback from Twitter once user has authenticated
        public ActionResult TwitterCallback(string oauth_token, string oauth_verifier)
        {
            var requestToken = new OAuthRequestToken
            {
                Token = oauth_token
            };

            string Key = WebConfigurationManager.AppSettings["TwitterKey"];
            string Secret = WebConfigurationManager.AppSettings["TwitterSecret"];
            try
            {
                TwitterService service = new TwitterService(Key, Secret);

                //Get Access Tokens  
                OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);
                service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
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
    }
}