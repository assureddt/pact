using Microsoft.EntityFrameworkCore;

namespace Pact.EntityFrameworkCore.Tests.Containers
{
    public class FakeContext : DbContext
    {
        public FakeContext(DbContextOptions<FakeContext> options) : base(options)
        {
        }

        public DbSet<Basic> Basics { get; set; }
        public DbSet<BasicIgnore> Ignores { get; set; }
        public DbSet<BasicFilter> Filters { get; set; }
    }
}
