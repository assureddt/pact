using System.Collections.Generic;
using System.Linq;
using Pact.Core.Comparers;
using Pact.Core.Extensions;
using Pact.Core.Tests.Containers;
using Shouldly;
using Xunit;
using CollectionExtensions = Pact.Core.Extensions.CollectionExtensions;

namespace Pact.Core.Tests;

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

    [Fact]
    public void UpdateOrder_Dec_Normalized_Uninitialized_AllApplied()
    {
        // arrange
        var items = new[]
        {
            new MyClass { Id = 1, Order = 0 },
            new MyClass { Id = 2, Order = 0 },
            new MyClass { Id = 3, Order = 0 }
        };

        // act
        items.UpdateOrderAndNormalize(2, CollectionExtensions.OrderShift.Decrement, x => x.Id, x => x.Order);

        // assert
        items[0].Id.ShouldBe(1);
        items[0].Order.ShouldBe(2);
        items[1].Id.ShouldBe(2);
        items[1].Order.ShouldBe(1);
        items[2].Id.ShouldBe(3);
        items[2].Order.ShouldBe(3);
    }

    [Fact]
    public void UpdateOrder_Dec_Normalized_AllApplied()
    {
        // arrange
        var items = new[]
        {
            new MyClass { Id = 2, Order = 1 },
            new MyClass { Id = 1, Order = 8 },
            new MyClass { Id = 3, Order = 1008 }
        };

        // act
        items.UpdateOrderAndNormalize(1, CollectionExtensions.OrderShift.Decrement, x => x.Id, x => x.Order);

        // assert
        items[0].Id.ShouldBe(2);
        items[0].Order.ShouldBe(2);
        items[1].Id.ShouldBe(1);
        items[1].Order.ShouldBe(1);
        items[2].Id.ShouldBe(3);
        items[2].Order.ShouldBe(3);
    }

    [Fact]
    public void UpdateOrder_Dec_NotNormalized_AllApplied()
    {
        // arrange
        var items = new[]
        {
            new MyClass { Id = 2, Order = 1 },
            new MyClass { Id = 1, Order = 8 },
            new MyClass { Id = 3, Order = 1008 }
        };

        // act
        items.UpdateOrderAndNormalize(1, CollectionExtensions.OrderShift.Decrement, x => x.Id, x => x.Order, false);

        // assert
        items[0].Id.ShouldBe(2);
        items[0].Order.ShouldBe(8);
        items[1].Id.ShouldBe(1);
        items[1].Order.ShouldBe(1);
        items[2].Id.ShouldBe(3);
        items[2].Order.ShouldBe(1008);
    }


    [Fact]
    public void UpdateOrder_Inc_Normalized_AllApplied()
    {
        // arrange
        var items = new[]
        {
            new MyClass { Id = 2, Order = 1 },
            new MyClass { Id = 1, Order = 8 },
            new MyClass { Id = 3, Order = 1008 }
        };

        // act
        items.UpdateOrderAndNormalize(1, CollectionExtensions.OrderShift.Increment, x => x.Id, x => x.Order);

        // assert
        items[0].Id.ShouldBe(2);
        items[0].Order.ShouldBe(1);
        items[1].Id.ShouldBe(1);
        items[1].Order.ShouldBe(3);
        items[2].Id.ShouldBe(3);
        items[2].Order.ShouldBe(2);
    }

    [Fact]
    public void UpdateOrder_Inc_NotNormalized_AllApplied()
    {
        // arrange
        var items = new[]
        {
            new MyClass { Id = 2, Order = 1 },
            new MyClass { Id = 1, Order = 8 },
            new MyClass { Id = 3, Order = 1008 }
        };

        // act
        items.UpdateOrderAndNormalize(1, CollectionExtensions.OrderShift.Increment, x => x.Id, x => x.Order, false);

        // assert
        items[0].Id.ShouldBe(2);
        items[0].Order.ShouldBe(1);
        items[1].Id.ShouldBe(1);
        items[1].Order.ShouldBe(1008);
        items[2].Id.ShouldBe(3);
        items[2].Order.ShouldBe(8);
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
        public int Id { get; init; }
        public MyHierarchy [] Children { get; init; }
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

    [Fact]
    public void GenericComparer_AsExpected()
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
        var comparer = GenericComparer<MyClass>.Create(x => x.Order);

        // assert
        comparer.Compare(items[0], items[1]).ShouldBeGreaterThan(0);
        comparer.Compare(items[1], items[2]).ShouldBeLessThan(0);
        comparer.Compare(items[2], items[3]).ShouldBeLessThan(0);
        comparer.Compare(items[3], items[4]).ShouldBe(0);
    }

    [Fact]
    public void SoftDelete_All_Gone()
    {
        var items = new List<Basic>
        {
            new() { Id = 1, Name = "Cat", SoftDelete = true },
            new() { Id = 2, Name = "Dog", SoftDelete = true }
        };

        var result = items.SoftDelete();

        Assert.Empty(result);
    }

    [Fact]
    public void SoftDelete_Correct_Removed()
    {
        var items = new List<Basic>
        {
            new() { Id = 1, Name = "Cat", SoftDelete = true },
            new() { Id = 2, Name = "Dog" }
        };

        var result = items.SoftDelete();

        var resultItem = Assert.Single(result);
        Assert.True(resultItem.Id == 2);
    }

    [Fact]
    public void SoftDelete_None_Removed()
    {
        var items = new List<Basic>
        {
            new() { Id = 1, Name = "Cat" },
            new() { Id = 2, Name = "Dog" }
        };

        var result = items.SoftDelete();

        Assert.True(result.Count() == 2);
    }

    [Theory]
    [InlineData("dog")]
    [InlineData("doG")]
    [InlineData("Dog")]
    [InlineData("   og   ")]
    public void TextFilter_No_Attributes(string filter)
    {
        var items = new List<Basic>
        {
            new() { Id = 1, Name = "Cat" },
            new() { Id = 2, Name = "Dog" },
            new() { Id = 3, Name = "Apple" },
            new() { Id = 4, Name = "Fish" },
            new() { Id = 5, Name = "Cake" }
        };

        var result = items.TextFilter(filter);

        var resultItem = Assert.Single(result);
        Assert.True(resultItem.Name == "Dog");
    }

    [Fact]
    public void TextFilter_Ignore_Attribute()
    {
        var items = new List<BasicIgnore>
        {
            new() { Id = 1, Name = "Cat", Ignore = "Dog" },
            new() { Id = 2, Name = "Dog", Ignore = "Fish" },
            new() { Id = 3, Name = "Apple", Ignore = "Fish" },
            new() { Id = 4, Name = "Fish", Ignore = "Dog" },
            new() { Id = 5, Name = "Cake", Ignore = "Fish" }
        };

        var result = items.TextFilter("og");

        var resultItem = Assert.Single(result);
        Assert.True(resultItem.Name == "Dog" && resultItem.Id == 2);
    }

    [Fact]
    public void TextFilter_Filter_Attribute()
    {
        var items = new List<BasicFilter>
        {
            new() { Id = 1, Name = "Cat", Filter = "Dog" },
            new() { Id = 2, Name = "Dog", Filter = "Fish" },
            new() { Id = 3, Name = "Apple", Filter = "Fish" },
            new() { Id = 4, Name = "Fish", Filter = "Dog" },
            new() { Id = 5, Name = "Cake", Filter = "Fish" }
        };

        var result = items.TextFilter("og");

        Assert.True(result.Count() == 2);
        Assert.True(result.Any(x => x.Id == 1) && result.Any(x => x.Id == 4));
    }

    [Fact]
    public void SoftDelete_Queryable_All_Gone()
    {
        var items = new List<Basic>
        {
            new() { Id = 1, Name = "Cat", SoftDelete = true },
            new() { Id = 2, Name = "Dog", SoftDelete = true }
        };

        var result = items.AsQueryable().SoftDelete();

        Assert.Empty(result);
    }

    [Fact]
    public void SoftDelete_Queryable_Correct_Removed()
    {
        var items = new List<Basic>
        {
            new() { Id = 1, Name = "Cat", SoftDelete = true },
            new() { Id = 2, Name = "Dog" }
        };

        var result = items.AsQueryable().SoftDelete();

        var resultItem = Assert.Single(result);
        Assert.True(resultItem.Id == 2);
    }

    [Fact]
    public void SoftDelete_Queryable_None_Removed()
    {
        var items = new List<Basic>
        {
            new() { Id = 1, Name = "Cat" },
            new() { Id = 2, Name = "Dog" }
        };

        var result = items.AsQueryable().SoftDelete();

        Assert.True(result.Count() == 2);
    }
}