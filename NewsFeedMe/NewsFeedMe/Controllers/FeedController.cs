using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewsFeedMe.Models;

namespace NewsFeedMe.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        public FeedController()
        {

        }

        // GET: Feed
        public ActionResult Home()
        {
            using (var context = new EntityFramework())
            {
                //get the current user
                int user = context.Users.Where(x => x.ScreenName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();

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
                                      .ToList().Select(x => new Publisher { PID = x.PID, Name = x.Name })).ToList(),
                    Publisher_Articles = ((from x in context.Set<Publisher_Article>()
                                           where (
                                                     (from sub in context.User_Publisher
                                                      where sub.UserID.Equals(user)
                                                      select sub.PublisherID)
                                                      .ToList())
                                         .Contains(x.PID)
                                           select new
                                           {
                                               x.AID,
                                               x.Author,
                                               x.Description,
                                               x.Title,
                                               x.URL,
                                               x.Publisher,
                                               x.PublishedAt,
                                               x.URlToImage
                                           }).ToList().Select(x => new Publisher_Article
                                           {
                                               AID = x.AID,
                                               Author = x.Author,
                                               Description = x.Description,
                                               Title = x.Title,
                                               URL = x.URL,
                                               Publisher = x.Publisher,
                                               PublishedAt = x.PublishedAt,
                                               URlToImage = x.URlToImage
                                           })).ToList()

                };

                //get the articles that correspond to the list of topics the user follows


                return View(feed);
            }
        }
    }
}