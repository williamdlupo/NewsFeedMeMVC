using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using NewsFeedMe.Models;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NewsFeedMe.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        public FeedController()
        {

        }

        // GET: Feed
        public async Task<ActionResult> Home()
        {
            using (var context = new EntityFramework())
            {
                //get the current user
                int user = context.Users.Where(x => x.ScreenName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();

                HttpClient client = new HttpClient();
                StringBuilder sourceText = new StringBuilder();

                    //get the current topics the user follows
                    FeedModel feed = new FeedModel
                {
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
                    //Publisher_Articles = ((from x in context.Set<Publisher_Article>()
                    //                       where (
                    //                                 (from sub in context.User_Publisher
                    //                                  where sub.UserID.Equals(user)
                    //                                  select sub.PublisherID)
                    //                                  .ToList())
                    //                     .Contains(x.PID)
                    //                       select new
                    //                       {
                    //                           x.AID,
                    //                           x.Author,
                    //                           x.Description,
                    //                           x.Title,
                    //                           x.URL,
                    //                           x.Publisher,
                    //                           x.PublishedAt,
                    //                           x.URlToImage
                    //                       }).ToList().Select(x => new Publisher_Article
                    //                       {
                    //                           AID = x.AID,
                    //                           Author = x.Author,
                    //                           Description = x.Description,
                    //                           Title = x.Title,
                    //                           URL = x.URL,
                    //                           Publisher = x.Publisher,
                    //                           PublishedAt = x.PublishedAt,
                    //                           URlToImage = x.URlToImage
                    //                       })).ToList()

                };
                foreach(var source in feed.FollowedSources) { sourceText.Append(String.Format("{0},", source.PID)); }
                string newsRequest = String.Format("https://newsapi.org/v2/top-headlines?sources={0}&pagesize=100&apiKey=8919f2f78e174c058c8e9745f90524fa", sourceText.ToString());
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, newsRequest);
                var result = await client.SendAsync(request);

                if (result.IsSuccessStatusCode)
                {
                    var articleJSON = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    int count = 0;
                    feed.Publisher_Articles = new List<Publisher_Article>();
                    foreach (var article in articleJSON["articles"])
                    {
                        feed.Publisher_Articles.Add(
                            new Publisher_Article
                            {
                                AID = count,
                                PID = (string)article["source"]["id"],
                                Author = (string)article["author"],
                                Title = (string)article["title"],
                                Description = (string)article["description"],
                                URL = (string)article["url"],
                                URlToImage = (string)article["urlToImage"],
                                PublishedAt = (DateTime)article["publishedAt"]

                            });
                        count++;
                    }
                }
                //get the articles that correspond to the list of topics the user follows


                return View(feed);
            }
        }
    }
}