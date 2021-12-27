using Microsoft.EntityFrameworkCore;
using api.Models;
namespace api {
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {  
            modelBuilder.Entity<Transaction>().ToTable("transactions");            
        }
        public DbSet<Transaction> Transactions { get; set; }
       
    }
}
