using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Tests.Containers;

namespace Pact.Web.Vue.Grid.Tests
{
    public class FakeContext : DbContext
    {
        public FakeContext(DbContextOptions<FakeContext> options) : base(options)
        {
        }

        public FakeContext()
        {

        }

        public DbSet<BasicDatabaseObject> Basics { get; set; }
        public DbSet<OrderDatabaseObject> Orders { get; set; }
        public DbSet<SoftDeleteDatabaseObject> SoftDeletes { get; set; }
    }
}
