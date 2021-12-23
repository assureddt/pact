using Microsoft.EntityFrameworkCore;

namespace Pact.Kendo.Tests.Containers;

public class FakeContext : DbContext
{
    public FakeContext(DbContextOptions<FakeContext> options) : base(options)
    {
    }

    public DbSet<Basic> Basics { get; set; }
}