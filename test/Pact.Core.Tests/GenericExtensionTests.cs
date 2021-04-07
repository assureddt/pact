using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    [Collection("JsonSerializerSequential")]
    public class GenericExtensionTests
    {
        [Fact]
        public void GetJson_Microsoft_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Microsoft;
            var item = new {Id = 1, Name = "Test"};

            // assert
            item.ToJson().ShouldBe("{\"Id\":1,\"Name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Microsoft_CaseSensitive_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Microsoft;
            var item = new { Id = 1, Name = "Test" };

            // assert
            // should behave any differently
            item.ToJson(caseInsensitive: false).ShouldBe("{\"Id\":1,\"Name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Microsoft_CaseSensitive_Lower_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Microsoft;
            var item = new { id = 1, name = "Test" };

            // assert
            // should behave any differently
            item.ToJson(caseInsensitive: false).ShouldBe("{\"id\":1,\"name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Microsoft_CaseInsensitive_Lower_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Microsoft;
            var item = new { id = 1, name = "Test" };

            // assert
            // should behave any differently
            item.ToJson().ShouldBe("{\"id\":1,\"name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Microsoft_Indented_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Microsoft;
            var item = new {Id = 1, Name = "Test"};

            // assert
            item.ToJson(true).ShouldMatch("{\\s+\"Id\":\\s+1,\\s+\"Name\":\\s+\"Test\"\\s+}");
        }

        [Fact]
        public void GetJson_Microsoft_Escaped_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Microsoft;
            var item = new {Id = 1, Name = "<p>Test</p>"};

            // assert
            item.ToJson(stringEscape: true).ShouldBe("{\"Id\":1,\"Name\":\"\\u003Cp\\u003ETest\\u003C/p\\u003E\"}");
        }

        [Fact]
        public void GetJson_Newtonsoft_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;
            var item = new { Id = 1, Name = "Test" };

            // assert
            item.ToJson().ShouldBe("{\"Id\":1,\"Name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Newtonsoft_CaseSensitive_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;
            var item = new { Id = 1, Name = "Test" };

            // assert
            // should behave any differently
            item.ToJson(caseInsensitive: false).ShouldBe("{\"Id\":1,\"Name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Newtonsoft_CaseSensitive_Lower_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;
            var item = new { id = 1, name = "Test" };

            // assert
            // should behave any differently
            item.ToJson(caseInsensitive: false).ShouldBe("{\"id\":1,\"name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Newtonsoft_CaseInsensitive_Lower_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;
            var item = new { id = 1, name = "Test" };

            // assert
            // should behave any differently
            item.ToJson().ShouldBe("{\"id\":1,\"name\":\"Test\"}");
        }

        [Fact]
        public void GetJson_Newtonsoft_Indented_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;
            var item = new { Id = 1, Name = "Test" };

            // assert
            item.ToJson(true).ShouldMatch("{\\s+\"Id\":\\s+1,\\s+\"Name\":\\s+\"Test\"\\s+}");
        }

        [Fact]
        public void GetJson_Newtonsoft_Escaped_AsExpected()
        {
            // arrange
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;
            var item = new { Id = 1, Name = "<p>Test</p>" };

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
