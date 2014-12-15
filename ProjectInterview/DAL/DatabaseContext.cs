using ProjectInterview.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ProjectInterview.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Blog> blogs { get; set; }
        public DbSet<NewsArticle> newsArticles { get; set; }
        public DbSet<Book> books { get; set; }
  
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}