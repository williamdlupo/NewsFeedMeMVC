using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using NewsFeedMe.Models;
using System.Threading.Tasks;
using System.Text;
using Tweetinvi;
using System.Web.Configuration;
using Tweetinvi.Parameters;
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
            var authenticateResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("ExternalCookie");

            if (Session["Tweets"] is null || (DateTime)Session["TwitterTimer"] < DateTime.Now.Subtract(new TimeSpan(0, 2, 0)))
            {
                //grab user access token and secret from claim                
                var oauthToken = authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == ("urn:twitter:access_token")).Value;
                var oauthSecret = authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == ("urn:twitter:access_token_secret")).Value;

                string Key = WebConfigurationManager.AppSettings["TwitterKey"];
                string Secret = WebConfigurationManager.AppSettings["TwitterSecret"];

                RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

                //access Twitter API and get user's current  home timeline
                Auth.SetUserCredentials(Key, Secret, oauthToken, oauthSecret);
                var twitterUser = Tweetinvi.User.GetAuthenticatedUser();

                // Get more control over the request with a HomeTimelineParameters
                var homeTimelineParameter = new HomeTimelineParameters
                {
                    MaximumNumberOfTweetsToRetrieve = 100

                };
                Session["Tweets"] = (twitterUser.GetHomeTimeline(homeTimelineParameter)).ToArray();
                Session["TwitterTimer"] = DateTime.Now;
            }

            var userTimeline = Session["Tweets"] as Tweetinvi.Models.ITweet[];

            using (var context = new EntityFramework())
            {
                //get the current userID
                long user = Convert.ToInt64(authenticateResult.Identity.Claims.FirstOrDefault(x => x.Type == "urn:twitter:userid").Value);

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
                };
                feed.Articles = new List<Bookmarked_Article>();

                foreach (var source in feed.FollowedSources) { sourceText.Append(String.Format("{0},", source.PID)); }
                
                //grab 100 headlines based on the users interests
                string newsRequest = String.Format("https://newsapi.org/v2/top-headlines?sources={0}&pagesize=100&apiKey=8919f2f78e174c058c8e9745f90524fa", sourceText.ToString());
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, newsRequest);
                var result = await client.SendAsync(request);

                if (result.IsSuccessStatusCode)
                {
                    var articleJSON = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    int count = 0;
                    foreach (var article in articleJSON["articles"])
                    {
                        feed.Articles.Add(
                            new Bookmarked_Article
                            {
                                AID = count,
                                SourceName = (string)article["source"]["id"],
                                Author = (string)article["author"],
                                Title = (string)article["title"],
                                Description = (string)article["description"],
                                URL = (string)article["url"],
                                URlToImage = (string)article["urlToImage"],
                                PublishedDate = (DateTime)article["publishedAt"]

                            });
                        count++;
                    }
                }

                sourceText.Clear();

                //grab 100 articles based on user interests
                foreach (var cat in feed.FollowedTopics)
                {

                    string categoryRequest = String.Format("https://newsapi.org/v2/top-headlines?country=us&language=us&category={0}&pagesize=100&apiKey=8919f2f78e174c058c8e9745f90524fa", cat.CID.ToString());
                    request = new HttpRequestMessage(HttpMethod.Get, categoryRequest);
                    result = await client.SendAsync(request);

                    if (result.IsSuccessStatusCode)
                    {
                        var articleJSON = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                        int count = 0;

                        foreach (var article in articleJSON["articles"])
                        {
                            feed.Articles.Add(
                                new Bookmarked_Article
                                {
                                    AID = count,
                                    SourceName = (string)article["source"]["name"],
                                    Author = (string)article["author"],
                                    Title = (string)article["title"],
                                    Description = (string)article["description"],
                                    URL = (string)article["url"],
                                    URlToImage = (string)article["urlToImage"],
                                    PublishedDate = (DateTime)article["publishedAt"]

                                });
                            count++;
                        }
                    }
                }

                //shuffle order of articles
                feed.Articles = feed.Articles.OrderBy(x => Guid.NewGuid()).ToList();

                //aggregate news articles and tweets into content block objects
                List<ContentBlock> contentBlocks = new List<ContentBlock>();
                for (int i = 0; i< userTimeline.Length && i< feed.Articles.Count;i+=6)
                {
                    try
                    {
                        var _artList = new List<Bookmarked_Article>() { feed.Articles[i], feed.Articles[i + 1] };
                        var _twList = new List<Tweetinvi.Models.ITweet>() { userTimeline[i], userTimeline[i + 1], userTimeline[i + 2], userTimeline[i + 3], userTimeline[i + 4], userTimeline[i + 5] };

                        contentBlocks.Add(
                            new ContentBlock
                            {
                                Articles = _artList,
                                Tweets = _twList
                            });
                    }
                    catch { break; }
                }
                feed.MixedFeed = contentBlocks;

                return View(feed);
            }
        }
    }
}