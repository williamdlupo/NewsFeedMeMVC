//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewsFeedMe
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bookmarked_Article
    {
        public int AID { get; set; }
        public string SourceName { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string URlToImage { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
        public long UserID { get; set; }
    
        public virtual User User { get; set; }
    }
}
