using System.Linq;
using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    public class CollectionExtensionTests
    {
        [Fact]
        public void NormalizeOrder_AllApplied()
        {
            // arrange
            var items = new[]
            {
                new MyClass { Id = 1, Order = 8 },
                new MyClass { Id = 2, Order = 1 },
                new MyClass { Id = 3, Order = 1008 }
            };

            // act
            items.NormalizeOrder(x => x.Order);

            // assert
            items[0].Id.ShouldBe(1);
            items[0].Order.ShouldBe(2);
            items[1].Id.ShouldBe(2);
            items[1].Order.ShouldBe(1);
            items[2].Id.ShouldBe(3);
            items[2].Order.ShouldBe(3);
        }

        private class MyClass
        {
            public int Id { get; set; }
            public int Order { get; set; }
        }

        [Fact]
        public void FormatListForTooltip_AsExpected()
        {
            // arrange
            var items = new[]
            {
                new { Id = 1, Name = "Wibble" },
                new { Id = 2, Name = "Wobble" },
                new { Id = 3, Name = "Wubble" }
            };

            // act
            var result = items.FormatListForTooltip(x => x.Name);

            // assert
            result.ShouldBe("<ul><li>Wibble</li><li>Wobble</li><li>Wubble</li></ul>");
        }

        [Fact]
        public void PropertyCsv_AsExpected()
        {
            // arrange
            var items = new[]
            {
                new { Id = 1, Name = "Wibble" },
                new { Id = 2, Name = "Wobble" },
                new { Id = 3, Name = "Wubble" }
            };

            // act
            var result = items.PropertyCsv(x => x.Id.ToString());

            // assert
            result.ShouldBe("1;2;3");
        }

        [Fact]
        public void Cartesian_AsExpected()
        {
            // arrange
            var items = new[]
            {
                new[] {1, 2},
                new[] {1, 2}
            };

            // act
            var result = items.CartesianProduct();

            // assert
            var expected = new[]
            {
                new[] {1, 1},
                new[] {1, 2},
                new[] {2, 1},
                new[] {2, 2}
            };
            result.ShouldBe(expected);
        }

        [Fact]
        public void Flatten_IncludesAll()
        {
            // arrange
            var items = new[]
            {
                new MyHierarchy { Id = 1, Children = new [] {new MyHierarchy {Id = 2}} },
                new MyHierarchy { Id = 2, Children = new [] {new MyHierarchy {Id = 3}, new MyHierarchy {Id = 4}} }
            };

            // act
            var flattened = items.Flatten(x => x.Children).ToArray();

            // assert
            flattened[0].Id.ShouldBe(1);
            flattened[1].Id.ShouldBe(2);
            flattened[2].Id.ShouldBe(2);
            flattened[3].Id.ShouldBe(3);
            flattened[4].Id.ShouldBe(4);
        }

        private class MyHierarchy
        {
            public int Id { get; set; }
            public MyHierarchy [] Children { get; set; }
        }

        [Fact]
        public void OrderByIndex_AsExpected()
        {
            // arrange
            var items = new[]
            {
                new MyClass { Id = 1, Order = 8 },
                new MyClass { Id = 2, Order = 1 },
                new MyClass { Id = 3, Order = 1008 }
            };

            // act
            var reordered = items.OrderByIndex(x => x.Order, new [] {1008, 1, 8}).ToArray();

            // assert
            reordered[0].Id.ShouldBe(3);
            reordered[1].Id.ShouldBe(2);
            reordered[2].Id.ShouldBe(1);
        }

        [Fact]
        public void Distinct_AsExpected()
        {
            // arrange
            var items = new[]
            {
                new MyClass { Id = 1, Order = 8 },
                new MyClass { Id = 2, Order = 1 },
                new MyClass { Id = 2, Order = 44 },
                new MyClass { Id = 3, Order = 1008 },
                new MyClass { Id = 3, Order = 1008 }
            };

            // act
            var result = items.Distinct(x => x.Id).ToArray();

            // assert
            result.Length.ShouldBe(3);
            result[0].Id.ShouldBe(1);
            result[1].Id.ShouldBe(2);
            result[1].Order.ShouldBe(1);
            result[2].Id.ShouldBe(3);
        }

        [Fact]
        public void Except_AsExpected()
        {
            // arrange
            var items = new[]
            {
                new MyClass { Id = 1, Order = 8 },
                new MyClass { Id = 2, Order = 1 },
                new MyClass { Id = 2, Order = 44 },
                new MyClass { Id = 3, Order = 1008 },
                new MyClass { Id = 3, Order = 1008 }
            };

            var exclusions = new[]
            {
                new MyClass {Id = 2}
            };

            // act
            var result = items.Except(exclusions,  x => x.Id).ToArray();

            // assert
            result.Length.ShouldBe(2);
            result[0].Id.ShouldBe(1);
            result[1].Id.ShouldBe(3);
        }

        [Fact]
        public void Shuffle_AsExpected()
        {
            // arrange
            var items = new[]
            {
                new MyClass { Id = 1, Order = 8 },
                new MyClass { Id = 2, Order = 1 },
                new MyClass { Id = 2, Order = 44 },
                new MyClass { Id = 3, Order = 1008 },
                new MyClass { Id = 3, Order = 1008 }
            };

            const int iterations = 1000;
            var results = new int[iterations];

            // act
            // NOTE: by definition, the outcome is random, so we can't really unit test it properly
            // settling for the assertion if we shuffle it 1000 times then there should be no chance of the 1st element always having ID 1
            for (var i = 0; i < iterations; i++)
            {
                items.Shuffle();
                results[i] = items.First().Id;
            }

            // assert
            results.Distinct().Count().ShouldBeGreaterThan(1);
        }
    }
}
