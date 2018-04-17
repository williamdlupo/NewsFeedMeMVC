﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EntityFramework : DbContext
    {
        public EntityFramework()
            : base("name=EntityFramework")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<User_Category> User_Category { get; set; }
        public virtual DbSet<User_Publisher> User_Publisher { get; set; }
        public virtual DbSet<Bookmarked_Article> Bookmarked_Article { get; set; }
    
        public virtual int InsertUser(Nullable<long> id, string accessToken, string secret, string service, string screenName, string profilePic)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(long));
    
            var accessTokenParameter = accessToken != null ?
                new ObjectParameter("AccessToken", accessToken) :
                new ObjectParameter("AccessToken", typeof(string));
    
            var secretParameter = secret != null ?
                new ObjectParameter("Secret", secret) :
                new ObjectParameter("Secret", typeof(string));
    
            var serviceParameter = service != null ?
                new ObjectParameter("Service", service) :
                new ObjectParameter("Service", typeof(string));
    
            var screenNameParameter = screenName != null ?
                new ObjectParameter("ScreenName", screenName) :
                new ObjectParameter("ScreenName", typeof(string));
    
            var profilePicParameter = profilePic != null ?
                new ObjectParameter("ProfilePic", profilePic) :
                new ObjectParameter("ProfilePic", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertUser", idParameter, accessTokenParameter, secretParameter, serviceParameter, screenNameParameter, profilePicParameter);
        }
    
        public virtual int InsertUser_Category(Nullable<long> userID, Nullable<int> categoryID)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(long));
    
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertUser_Category", userIDParameter, categoryIDParameter);
        }
    
        public virtual int InsertUser_Publisher(Nullable<long> userID, string publisherID)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(long));
    
            var publisherIDParameter = publisherID != null ?
                new ObjectParameter("PublisherID", publisherID) :
                new ObjectParameter("PublisherID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertUser_Publisher", userIDParameter, publisherIDParameter);
        }
    
        public virtual int Seed_Category(string cID, string country)
        {
            var cIDParameter = cID != null ?
                new ObjectParameter("CID", cID) :
                new ObjectParameter("CID", typeof(string));
    
            var countryParameter = country != null ?
                new ObjectParameter("Country", country) :
                new ObjectParameter("Country", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Seed_Category", cIDParameter, countryParameter);
        }
    
        public virtual int Seed_Publisher(string pID, string name, string description, string uRL)
        {
            var pIDParameter = pID != null ?
                new ObjectParameter("PID", pID) :
                new ObjectParameter("PID", typeof(string));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var descriptionParameter = description != null ?
                new ObjectParameter("Description", description) :
                new ObjectParameter("Description", typeof(string));
    
            var uRLParameter = uRL != null ?
                new ObjectParameter("URL", uRL) :
                new ObjectParameter("URL", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Seed_Publisher", pIDParameter, nameParameter, descriptionParameter, uRLParameter);
        }
    
        public virtual int DeleteUser_Category(Nullable<long> userID, Nullable<int> categoryID)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(long));
    
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteUser_Category", userIDParameter, categoryIDParameter);
        }
    
        public virtual int DeleteUser_Publisher(Nullable<long> userID, string publisherID)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(long));
    
            var publisherIDParameter = publisherID != null ?
                new ObjectParameter("PublisherID", publisherID) :
                new ObjectParameter("PublisherID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteUser_Publisher", userIDParameter, publisherIDParameter);
        }
    
        public virtual int InsertBookmarked_Article(string sourceName, Nullable<long> userID, string author, string title, string description, string uRL, string uRLToImage, Nullable<System.DateTime> date)
        {
            var sourceNameParameter = sourceName != null ?
                new ObjectParameter("SourceName", sourceName) :
                new ObjectParameter("SourceName", typeof(string));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(long));
    
            var authorParameter = author != null ?
                new ObjectParameter("Author", author) :
                new ObjectParameter("Author", typeof(string));
    
            var titleParameter = title != null ?
                new ObjectParameter("Title", title) :
                new ObjectParameter("Title", typeof(string));
    
            var descriptionParameter = description != null ?
                new ObjectParameter("Description", description) :
                new ObjectParameter("Description", typeof(string));
    
            var uRLParameter = uRL != null ?
                new ObjectParameter("URL", uRL) :
                new ObjectParameter("URL", typeof(string));
    
            var uRLToImageParameter = uRLToImage != null ?
                new ObjectParameter("URLToImage", uRLToImage) :
                new ObjectParameter("URLToImage", typeof(string));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertBookmarked_Article", sourceNameParameter, userIDParameter, authorParameter, titleParameter, descriptionParameter, uRLParameter, uRLToImageParameter, dateParameter);
        }
    }
}
