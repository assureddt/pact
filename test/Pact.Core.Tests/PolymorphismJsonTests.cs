using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests;

[Collection("JsonSerializerSequential")]
public class PolymorphismJsonTests
{
    [Fact]
    public void Poly_Serialize_As_Base_Microsoft_Missing_Extended_OK()
    {
        // arrange
        var val = new MyClass {Id = 80, Name = "testval", Perc = 90.0D};

        // act & assert
        val.ToJson<MyBaseClass>().ShouldBe("{\"Id\":80,\"Name\":\"testval\"}");
    }

    [Fact]
    public void Poly_Deserialize_As_Base_Microsoft_Missing_Extended_OK()
    {
        // arrange

        var val = new MyClass {Id = 80, Name = "testval", Perc = 90.0D};
        var asString = val.ToJson<MyBaseClass>();

        // act
        var result = asString.FromJson<MyClass>();
                
        // assert
        result.Id.ShouldBe(val.Id);
        result.Name.ShouldBe(val.Name);
        // this is the difference
        result.Perc.ShouldNotBe(val.Perc);
    }

    private class MyBaseClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class MyClass : MyBaseClass
    {
        public double Perc { get; set; }
    }
}