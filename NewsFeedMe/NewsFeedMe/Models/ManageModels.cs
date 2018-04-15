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

    public class SaveFollowingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class DeleteFollowingModel
    {
        public string Id { get; set; }
    }
}