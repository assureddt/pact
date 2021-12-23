using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pact.Web.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Web.Tests;

public class SelectListItemExtensionTests
{
    [Fact]
    public void WithDefault_Applied()
    {
        // arrange
        var items = new[]
        {
            new SelectListItem { Value = "1", Text = "One" },
            new SelectListItem { Value = "2", Text = "Two" },
            new SelectListItem { Value = "3", Text = "Three" }
        };

        // act
        var result = items.WithDefault("Wibble");

        // assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(4);
        result.First().Value.ShouldBeEmpty();
        result.First().Text.ShouldBe("Wibble");
    }

    [Fact]
    public void ToDefault_IfNotValid_Applied()
    {
        // arrange
        var items = new[]
        {
            new SelectListItem { Value = "1", Text = "One" },
            new SelectListItem { Value = "2", Text = "Two" },
            new SelectListItem { Value = "3", Text = "Three" }
        };

        // act & assert
        "Wibble".GetDefaultIfNotValid(items).ShouldBe("1");
    }

    [Fact]
    public void IfValid_Unchanged()
    {
        // arrange
        var items = new[]
        {
            new SelectListItem { Value = "1", Text = "One" },
            new SelectListItem { Value = "2", Text = "Two" },
            new SelectListItem { Value = "3", Text = "Three" }
        };

        // act & assert
        "3".GetDefaultIfNotValid(items).ShouldBe("3");
    }

    [Fact]
    public void Generic_ToDefault_IfNotValid_Applied()
    {
        // arrange
        var items = new[]
        {
            new SelectListItem { Value = "1", Text = "One" },
            new SelectListItem { Value = "2", Text = "Two" },
            new SelectListItem { Value = "3", Text = "Three" }
        };

        // act & assert
        4.GetDefaultIfNotValid(items).ShouldBe(1);
    }

    [Fact]
    public void Generic_IfValid_Unchanged()
    {
        // arrange
        var items = new[]
        {
            new SelectListItem { Value = "1", Text = "One" },
            new SelectListItem { Value = "2", Text = "Two" },
            new SelectListItem { Value = "3", Text = "Three" }
        };

        // act & assert
        3.GetDefaultIfNotValid(items).ShouldBe(3);
    }

    [Fact]
    public void GenericNullable_ToDefault_IfNotValid_Applied()
    {
        // arrange
        var items = new[]
        {
            new SelectListItem { Value = null, Text = "Null" },
            new SelectListItem { Value = "1", Text = "One" },
            new SelectListItem { Value = "2", Text = "Two" },
            new SelectListItem { Value = "3", Text = "Three" }
        };

        // act & assert
        ((int?)4).GetDefaultIfNotValid(items).ShouldBe(null);
    }

    [Fact]
    public void GenericNullable_IfValid_Unchanged()
    {
        // arrange
        var items = new[]
        {
            new SelectListItem { Value = null, Text = "Null" },
            new SelectListItem { Value = "1", Text = "One" },
            new SelectListItem { Value = "2", Text = "Two" },
            new SelectListItem { Value = "3", Text = "Three" }
        };

        // act & assert
        ((int?)3).GetDefaultIfNotValid(items).ShouldBe(3);
    }
}