using CandyWebMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CandyWebMVC.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasMany(u => u.UserAdress).WithOne(e => e.User).HasForeignKey(e => e.Userid);
        }


        public DbSet<Products> Products { get; set; }
        public DbSet<Cart> CartItems { get; set; }
        public DbSet<User> User { get; set; }
    }
}
