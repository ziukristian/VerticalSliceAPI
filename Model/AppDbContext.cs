using Microsoft.EntityFrameworkCore;
using VerticalSliceAPI.Entities;

namespace VerticalSliceAPI.Model
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TestDB");
        }
    }
}
