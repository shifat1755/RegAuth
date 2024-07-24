using Microsoft.EntityFrameworkCore;
using RegAuth.Models.Entities;
namespace RegAuth.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<User>Users { get; set; }
    }
}
