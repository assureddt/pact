using System.ComponentModel.DataAnnotations;
using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests;

public class EnumExtensionTests
{
    [Fact]
    public void GetAttribute_OK()
    {
        Fruit.Apple.GetAttributeOfType<DisplayAttribute>().Description.ShouldBe("Pomme");
    }

    [Fact]
    public void GetName_OK()
    {
        Fruit.Pear.GetName().ShouldBe("Pear");
    }

    [Fact]
    public void GetDescription_OK()
    {
        Fruit.Pineapple.GetDescription().ShouldBe("Ananas");
    }

    [Fact]
    public void GetOrder_OK()
    {
        Fruit.Pear.GetOrder().ShouldBe(1);
    }

    [Fact]
    public void Values_OK()
    {
        // NOTE: these will follow the order attribute
        EnumExtensions.Values<Fruit>().ShouldBe(new [] {Fruit.Pear, Fruit.Apple, Fruit.Pineapple});
    }

    [Fact]
    public void GetWithOrder_OK()
    {
        // NOTE: these will follow the order attribute
        typeof(Fruit).GetWithOrder<Fruit>().ShouldBe(new [] {Fruit.Pear, Fruit.Apple, Fruit.Pineapple});
    }

    private enum Fruit
    {
        [Display(Name = "Apple", Description = "Pomme", Order = 2)]
        Apple,
        [Display(Name = "Pear", Description = "Poire", Order = 1)]
        Pear,
        [Display(Name = "Pineapple", Description = "Ananas", Order = 3)]
        Pineapple
    }
}