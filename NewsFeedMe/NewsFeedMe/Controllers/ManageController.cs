using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsFeedMe.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ActionResult Content()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }
        
        public ActionResult Bookmarks()
        {
            return View();
        }
    }
}