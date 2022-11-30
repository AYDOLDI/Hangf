using Hangf.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hangf
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> People => Set<Person>();
    }
}
