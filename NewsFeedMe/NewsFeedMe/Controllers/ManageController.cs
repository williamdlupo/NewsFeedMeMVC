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
        public async Task<ActionResult> Following()
        {
            ViewBag.Message = TempData["result"] as string;

            using (var context = new EntityFramework())
            {
                long user = context.Users.Where(x => x.ScreenName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();

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
                                        .Select(x => new Category { CID = x.CID, Country = x.Country })).ToList(),
                    FollowedTopics = ((from category in context.Set<Category>()
                                       where (
                                               (from subcat in context.User_Category
                                                where subcat.UserID.Equals(user)
                                                select subcat.CategoryID)
                                                .ToList())
                                      .Contains(category.ID)
                                       select category.CID)
                                      .ToList().Select(x => new Category { CID = x })).ToList(),
                    FollowedSources = ((from source in context.Set<Publisher>()
                                        where (
                                                (from subcat in context.User_Publisher
                                                 where subcat.UserID.Equals(user)
                                                 select subcat.PublisherID)
                                                 .ToList())
                                       .Contains(source.PID)
                                        select new { source.PID, source.Name })
                                      .ToList().Select(x => new Publisher { PID = x.PID, Name = x.Name })).ToList()
                };

                return View(following);
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> DeleteFollowing(DeleteFollowingModel[] deleteList)
        {
            using (var context = new EntityFramework())
            {
                long user = context.Users.Where(x => x.ScreenName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();

                List<Category> AllTopics = ((from top in context.Set<Category>()
                                             select new
                                             {
                                                 top.ID,
                                                 top.CID,
                                                 top.Country
                                             }).ToList()
                                        .Select(x => new Category { ID = x.ID, CID = x.CID, Country = x.Country })).ToList();

                foreach (var topic in deleteList)
                {
                    //test if the id is a topic vs a news source
                    if (AllTopics.Any(x => x.CID.Equals(topic.Id)))
                    {
                        context.DeleteUser_Category(user, AllTopics.Where(x => x.CID.Equals(topic.Id)).Select(y => y.ID).First());
                        await context.SaveChangesAsync();
                    }

                    else
                    {
                        context.DeleteUser_Publisher(user, topic.Id);
                        await context.SaveChangesAsync();
                    }
                }
                
                TempData["result"] = "Interests saved!";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveFollowing(SaveFollowingModel[] topicList)
        {
            using (var context = new EntityFramework())
            {
                long user = context.Users.Where(x => x.ScreenName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();

                if (topicList.Where(x => x.Type.Equals("category")).ToList().Count > 0)
                {
                    int id = 0;
                    foreach (SaveFollowingModel category in topicList.Where(x => x.Type.Equals("category")).ToList())
                    {
                        context.User_Category.Add(
                            new User_Category
                            {
                                Id = id,
                                UserID = user,
                                CategoryID = context.Categories.Select(x => new { x.ID, x.CID }).Where(x => x.CID.Equals(category.Id)).FirstOrDefault().ID
                            });
                        await context.SaveChangesAsync();
                        id++;
                    }
                }
                if (topicList.Where(x => x.Type.Equals("publisher")).ToList().Count > 0)
                {
                    int id = 0;
                    foreach (SaveFollowingModel publisher in topicList.Where(x => x.Type.Equals("publisher")).ToList())
                    {
                        context.User_Publisher.Add(
                            new User_Publisher
                            {
                                ID = id,
                                UserID = user,
                                PublisherID = publisher.Id
                            });
                        await context.SaveChangesAsync();
                        id++;
                    }
                }

                TempData["result"] = "Interests saved!";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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