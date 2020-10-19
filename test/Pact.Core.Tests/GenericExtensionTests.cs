using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    public class GenericExtensionTests
    {
        [Fact]
        public void GetJson_AsExpected()
        {
            // arrange
            var item = new {Id = 1, Name = "Test"};

            // assert
            item.ToJson().ShouldBe("{\"Id\":1,\"Name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Indented_AsExpected()
        {
            // arrange
            var item = new {Id = 1, Name = "Test"};

            // assert
            item.ToJson(true).ShouldBe("{\r\n  \"Id\": 1,\r\n  \"Name\": \"Test\"\r\n}");
        }

        [Fact]
        public void GetJson_Escaped_AsExpected()
        {
            // arrange
            var item = new {Id = 1, Name = "<p>Test</p>"};

            // assert
            item.ToJson(stringEscape: true).ShouldBe("{\"Id\":1,\"Name\":\"\\u003cp\\u003eTest\\u003c/p\\u003e\"}");
        }

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
}
