using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsFeedMe.Controllers
{
    public class FeedController : Controller
    {
        private int UserID;

        public FeedController()
        {
            UserID = Convert.ToInt32(System.Security.Claims.ClaimsPrincipal.Current.Claims.FirstOrDefault(x => x.Type.EndsWith("twitter:userid")).Value);
        }

        // GET: Feed
        public ActionResult Home()
        {
            return View();
        }
    }
}