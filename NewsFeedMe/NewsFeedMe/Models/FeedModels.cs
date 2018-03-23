using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsFeedMe.Models
{
    public class FeedModel
    {
        public List<Publisher> FollowedSources;
        public List<Category> FollowedTopics;
        public List<Publisher_Article> Publisher_Articles;
    }
}