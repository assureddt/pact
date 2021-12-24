using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests;

[Collection("JsonSerializerSequential")]
public class GenericExtensionTests
{
    [Fact]
    public void GetValue_OK()
    {
        // arrange
        var item = new {Id = 1, Name = "Test"};

        // assert
        item.GetPropertyValue(x => x.Name).ShouldBe("Test");
    }

    [Fact]
    public void SetValue_Settable_Changed()
    {
        // arrange
        var item = new MyClass {Id = 1, Name = "Test"};

        // act
        item.SetPropertyValue(x => x.Name, "Edited");

        // assert
        item.Name.ShouldBe("Edited");
    }

    [Fact]
    public void SetValue_NotSettable_DoesNothing()
    {
        // arrange
        var item = new {Id = 1, Name = "Test"};

        // act
        item.SetPropertyValue(x => x.Name, "Edited");

        // assert
        item.Name.ShouldBe("Test");
    }
}

public class MyClass
{
    public int Id { get; set; }
    public string Name { get; set; }
}