using CandyWebMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CandyWebMVC.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasMany(u => u.UserAddresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserID);

            modelBuilder.Entity<User>()
           .HasKey(u => u.CPFID);

            modelBuilder.Entity<LoginModel>()
                .HasKey(l => l.LoginId);

            modelBuilder.Entity<LoginModel>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<LoginModel>(l => l.CPFID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Products> Products { get; set; }
        public DbSet<Cart> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoginModel> loginModels { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
