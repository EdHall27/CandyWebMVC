using CandyWebMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CandyWebMVC.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) :base(options)
        {
        }

        public DbSet<Products> Products { get; set; }
    }
}
