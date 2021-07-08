using Microsoft.EntityFrameworkCore;
using Pact.EntityFrameworkCore.Extensions;
using Pact.EntityFrameworkCore.Tests.Containers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pact.EntityFrameworkCore.Tests
{
    public class QueryableExtensionsTests
    {
        private static FakeContext GetFakeContext()
        {
            var options = new DbContextOptionsBuilder<FakeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            return new FakeContext(options);
        }

        [Theory]
        [InlineData("dog")]
        [InlineData("doG")]
        [InlineData("Dog")]
        [InlineData("   og   ")]
        public async Task TextFilter_No_Attributes(string filter)
        {
            await using var context = GetFakeContext();
            await context.Basics.AddAsync(new Basic { Id = 1, Name = "Cat" });
            await context.Basics.AddAsync(new Basic { Id = 2, Name = "Dog" });
            await context.Basics.AddAsync(new Basic { Id = 3, Name = "Apple" });
            await context.Basics.AddAsync(new Basic { Id = 4, Name = "Fish" });
            await context.Basics.AddAsync(new Basic { Id = 5, Name = "Cake" });
            await context.SaveChangesAsync();

            var result = await context.Basics.TextFilter(filter).ToListAsync();

            var resultItem = Assert.Single(result);
            Assert.True(resultItem.Name == "Dog");
        }

        [Fact]
        public async Task TextFilter_Ignore_Attribute()
        {
            await using var context = GetFakeContext();
            await context.Ignores.AddAsync(new BasicIgnore { Id = 1, Name = "Cat", Ignore = "Dog" });
            await context.Ignores.AddAsync(new BasicIgnore { Id = 2, Name = "Dog", Ignore = "Fish" });
            await context.Ignores.AddAsync(new BasicIgnore { Id = 3, Name = "Apple", Ignore = "Fish" });
            await context.Ignores.AddAsync(new BasicIgnore { Id = 4, Name = "Fish", Ignore = "Dog" });
            await context.Ignores.AddAsync(new BasicIgnore { Id = 5, Name = "Cake", Ignore = "Fish" });
            await context.SaveChangesAsync();

            var result = await context.Ignores.TextFilter("og").ToListAsync();

            var resultItem = Assert.Single(result);
            Assert.True(resultItem.Name == "Dog" && resultItem.Id == 2);
        }

        [Fact]
        public async Task TextFilter_Filter_Attribute()
        {
            await using var context = GetFakeContext();
            await context.Filters.AddAsync(new BasicFilter { Id = 1, Name = "Cat", Filter = "Dog" });
            await context.Filters.AddAsync(new BasicFilter { Id = 2, Name = "Dog", Filter = "Fish" });
            await context.Filters.AddAsync(new BasicFilter { Id = 3, Name = "Apple", Filter = "Fish" });
            await context.Filters.AddAsync(new BasicFilter { Id = 4, Name = "Fish", Filter = "Dog" });
            await context.Filters.AddAsync(new BasicFilter { Id = 5, Name = "Cake", Filter = "Fish" });
            await context.SaveChangesAsync();

            var result = await context.Filters.TextFilter("og").ToListAsync();

            Assert.True(result.Count == 2);
            Assert.True(result.Any(x => x.Id == 1) && result.Any(x => x.Id == 4));
        }
    }
}
