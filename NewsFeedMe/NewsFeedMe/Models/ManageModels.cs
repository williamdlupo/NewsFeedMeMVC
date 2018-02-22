using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsFeedMe.Models
{
    public class FollowingModel
    {
        public List<Publisher> AllSources;
        public List<Category> AllTopics;
        public List<Publisher> FollowedSources;
        public List<Category> FollowedTopics;
    }
}