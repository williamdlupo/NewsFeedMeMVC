using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsFeedMe.Controllers
{
    public class FeedController : Controller
    {
        public FeedController()
        {
            
        }

        // GET: Feed
        public ActionResult Home()
        {
            return View();
        }
    }
}