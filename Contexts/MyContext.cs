using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Contexts
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options){}
        
        public DbSet<User> Users {get;set;}
        public DbSet<Wedding> Weddings{get;set;}
        public DbSet<Association> Associations{get;set;}
    }
}