using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ProjectInterview.Models;
using ProjectInterview.DAL;

namespace ProjectInterview.DAL
{
    public class DatabaseInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            /*
            var blogs = new List<Blog>
            {
            new Blog{Title="Test title 1", Url="http://www.testurl1.ca", Excerpt="Just a test - Just a test - Just a test - Just a test - Just a test - Just a test - Just a test", PublishDate=DateTime.Parse("2014-11-20")},
            new Blog{Title="Test title 2", Url="http://www.testurl2.ca", Excerpt="Just a test - Just a test - Just a test - Just a test - Just a test - Just a test - Just a test", PublishDate=DateTime.Parse("2014-11-20")},
            new Blog{Title="Test title 3", Url="http://www.testurl3.ca", Excerpt="Just a test - Just a test - Just a test - Just a test - Just a test - Just a test - Just a test", PublishDate=DateTime.Parse("2014-11-20")},
            new Blog{Title="Test title 4", Url="http://www.testurl4.ca", Excerpt="Just a test - Just a test - Just a test - Just a test - Just a test - Just a test - Just a test", PublishDate=DateTime.Parse("2014-11-20")},
            new Blog{Title="Test title 5", Url="http://www.testurl5.ca", Excerpt="Just a test - Just a test - Just a test - Just a test - Just a test - Just a test - Just a test", PublishDate=DateTime.Parse("2014-11-20")},
            new Blog{Title="Test title 6", Url="http://www.testurl6.ca", Excerpt="Just a test - Just a test - Just a test - Just a test - Just a test - Just a test - Just a test", PublishDate=DateTime.Parse("2014-11-20")}
            };

            blogs.ForEach(s => context.blogs.Add(s));
            context.SaveChanges();
             */
         }
    }
}