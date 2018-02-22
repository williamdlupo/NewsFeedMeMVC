using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using NewsFeedMe.Models;

namespace NewsFeedMe.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private HttpClient client = new HttpClient();

        [HttpGet]
        public ActionResult Following()
        {
            ViewBag.Message = TempData["result"] as string;

            using (var context = new EntityFramework())
            {
                FollowingModel following = new FollowingModel
                {
                    //get list of all possible news sources and topics to follow
                    AllSources = ((from pub in context.Set<Publisher>()
                                   select new
                                   {
                                       pub.PID,
                                       pub.Name,
                                       pub.Description,
                                       pub.URL
                                   }).ToList()
                                        .Select(x => new Publisher { PID = x.PID, Name = x.Name, Description = x.Description, URL = x.URL })).ToList(),
                    AllTopics = ((from top in context.Set<Category>()
                                  select new
                                  {
                                      top.CID,
                                      top.Country
                                  }).ToList()
                                        .Select(x => new Category { CID = x.CID, Country = x.Country })).ToList()

                    //get lists of currently followed sources and topics
                };

                return View(following);
            }
        }

        public async Task<ActionResult> TestContent(string publisherID)
        {
            var claim = System.Security.Claims.ClaimsPrincipal.Current.Claims;

            using (var context = new EntityFramework())
            {
                try
                {
                    int userid = Convert.ToInt32(claim.FirstOrDefault(x => x.Type.EndsWith("twitter:userid")).Value);
                    var interest = new User_Publisher { UserID = userid, PublisherID = publisherID };

                    var result = context.User_Publisher.Add(interest);
                    await context.SaveChangesAsync();

                    TempData["result"] = "Content selection saved!";
                    return RedirectToAction("Following");
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

        //seeds the publisher table with data with NewsAPI sources
        public async Task<ActionResult> SeedPublisher()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://newsapi.org/v2/sources?language=en&country=us&apiKey=8919f2f78e174c058c8e9745f90524fa");

            var result = await client.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                var publisherJSON = JObject.Parse(result.Content.ReadAsStringAsync().Result);

                using (var context = new EntityFramework())
                {
                    foreach (var source in publisherJSON["sources"])
                    {
                        Publisher publisher = new Publisher { PID = (string)source["id"], Name = (string)source["name"], Description = (string)source["description"], URL = (string)source["url"] };
                        context.Publishers.Add(publisher);
                        await context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction("Content");
        }
    }
}