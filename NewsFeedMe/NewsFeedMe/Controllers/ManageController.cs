using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace NewsFeedMe.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        [HttpGet]
        public ActionResult Content()
        {
            ViewBag.Message = TempData["result"] as string;
            return View();
        }

        public async Task<ActionResult> TestContent(string contentID, string publisherID)
        {
            var claim = System.Security.Claims.ClaimsPrincipal.Current.Claims;

            using (var context = new EntityFramework())
            {
                try
                {
                    int userid = Convert.ToInt32(claim.FirstOrDefault(x => x.Type.EndsWith("twitter:userid")).Value);
                    var interest = new User_Interest { UserID = userid, CategoryID = contentID, PublisherID = publisherID };

                    var result = context.User_Interest.Add(interest);
                    await context.SaveChangesAsync();

                    TempData["result"] = "Content selection saved!";
                    return RedirectToAction("Content");
                }
                catch { throw; }
            }
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