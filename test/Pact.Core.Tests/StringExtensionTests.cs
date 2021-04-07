using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    [Collection("JsonSerializerSequential")]
    public class StringExtensionTests
    {
        [Fact]
        public void FromJson_Microsoft_OK()
        {
            JsonSerialization.Serializer = JsonImplementation.Microsoft;

            // act
            var result = "{ \"Id\": 1, \"Name\": \"Test\" }".FromJson<MyClass>();

            // assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(1),
                () => result.Name.ShouldBe("Test"));
        }

        [Fact]
        public void FromJson_Microsoft_CaseSensitive_Fail()
        {
            JsonSerialization.Serializer = JsonImplementation.Microsoft;

            // act
            var result = "{ \"id\": 1, \"name\": \"test\" }".FromJson<MyClass>(caseInsensitive: false);

            // assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(0),
                () => result.Name.ShouldBe(null));
        }

        [Fact]
        public void FromJson_Microsoft_CaseInSensitive_OK()
        {
            JsonSerialization.Serializer = JsonImplementation.Microsoft;

            // act
            var result = "{ \"id\": 1, \"name\": \"Test\" }".FromJson<MyClass>();

            // assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(1),
                () => result.Name.ShouldBe("Test"));
        }

        [Fact]
        public void FromJson_Microsoft_BadArray_Throws()
        {
            JsonSerialization.Serializer = JsonImplementation.Microsoft;

            // assert
            Assert.Throws<JsonException>(() => "{ \"Id\": 1, \"Name\": \"Test\" }, { \"Id\": 2, \"Name\": \"Test2\" }".FromJson<MyClass>());
        }

        [Fact]
        public void FromJson_Microsoft_Relaxed_OK()
        {
            JsonSerialization.Serializer = JsonImplementation.Microsoft;

            // act
            var result = "{ \"Id\": 1, \"Name\": \"Test\" }, { \"Id\": 2, \"Name\": \"Test2\" }".FromJson<MyClass[]>(true);

            // assert
            result.ShouldSatisfyAllConditions(
                () => result[0].Id.ShouldBe(1),
                () => result[0].Name.ShouldBe("Test"),
                () => result[1].Id.ShouldBe(2),
                () => result[1].Name.ShouldBe("Test2"));
        }

        [Fact]
        public void FromJson_Newtonsoft_OK()
        {
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;

            // act
            var result = "{ \"Id\": 1, \"Name\": \"Test\" }".FromJson<MyClass>();

            // assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(1),
                () => result.Name.ShouldBe("Test"));
        }

        [Fact]
        public void FromJson_Newtonsoft_CaseSensitive_Ignored()
        {
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;

            // act
            var result = "{ \"id\": 1, \"name\": \"test\" }".FromJson<MyClass>(caseInsensitive: false);

            // assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(1),
                () => result.Name.ShouldBe("test"));
        }

        [Fact]
        public void FromJson_Newtonsoft_CaseInSensitive_OK()
        {
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;

            // act
            var result = "{ \"id\": 1, \"name\": \"Test\" }".FromJson<MyClass>();

            // assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(1),
                () => result.Name.ShouldBe("Test"));
        }

        [Fact]
        public void FromJson_Newtonsoft_BadArray_Throws()
        {
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;

            // assert
            Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => "{ \"Id\": 1, \"Name\": \"Test\" }, { \"Id\": 2, \"Name\": \"Test2\" }".FromJson<MyClass>());
        }

        [Fact]
        public void FromJson_Newtonsoft_Relaxed_OK()
        {
            JsonSerialization.Serializer = JsonImplementation.Newtonsoft;

            // act
            var result = "{ \"Id\": 1, \"Name\": \"Test\" }, { \"Id\": 2, \"Name\": \"Test2\" }".FromJson<MyClass[]>(true);

            // assert
            result.ShouldSatisfyAllConditions(
                () => result[0].Id.ShouldBe(1),
                () => result[0].Name.ShouldBe("Test"),
                () => result[1].Id.ShouldBe(2),
                () => result[1].Name.ShouldBe("Test2"));
        }

        private class MyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public void Alphanumerics_OK()
        {
            // assert
            "My-Name-Is-George_& I was born in 1964".StripNonAlphaNumeric("").ShouldBe("MyNameIsGeorgeIwasbornin1964");
        }

        [Fact]
        public void Filename_OK()
        {
            // assert
            "Some@Of:these\\/should-not_be;here, right?!".MakeSafeFilename()
                .ShouldBe(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "Some@Of:these\\_should-not_be;here, right?!"
                    : "Some@Of_these__should-not_be;here, right_!");
        }

        [Fact]
        public void EmailAddress_OK()
        {
            // assert
            "fred.smith@test.com".GetEmailAddresses().ShouldBe(new [] {"fred.smith@test.com"});
        }

        [Fact]
        public void EmailAddresses_OK()
        {
            // assert
            "fred.smith@test.com <freddy smith, sales>, jon.smith@test.com".GetEmailAddresses().ShouldBe(new[] { "fred.smith@test.com", "jon.smith@test.com" });
        }


        [Fact]
        public void IsUppercaseCharacter_OK()
        {
            // assert
            'F'.IsBasicUppercaseLetter().ShouldBeTrue();
        }

        [Fact]
        public void IsUppercaseCharacter_Nope()
        {
            // assert
            ';'.IsBasicUppercaseLetter().ShouldBeFalse();
        }

        [Fact]
        public void ToNullable_OK()
        {
            // assert
            "5".ToNullable<int>().ShouldBe(5);
        }

        [Fact]
        public void ToNullable_Null()
        {
            // assert
            "a".ToNullable<int>().ShouldBe(null);
        }

        [Fact]
        public void Masked_Simple_AsExpected()
        {
            // assert
            "sensitiveinfohere".Masked(4).ShouldBe("sensitiveinfo****");
        }

        [Fact]
        public void Masked_Inverted_AsExpected()
        {
            // assert
            "sensitiveinfohere".Masked(4, true).ShouldBe("*************here");
        }

        [Fact]
        public void Masked_Specific_AsExpected()
        {
            const string source = "sensitiveinfohere";
            // assert
            source.Masked('X', 1, source.Length - 1).ShouldBe("sXXXXXXXXXXXXXXXX");
        }

        [Fact]
        public void Ellipsis_AsExpected()
        {
            // assert
            "large sentence that we don't want to wrap".Ellipsis(16).ShouldBe("large sentenc...");
        }

        [Fact]
        public void ParseIntegerArray_Messy_AsExpected()
        {
            // assert
            "  1,,,34;,,   26; 45 ,".ParseIntegerArray().ShouldBe(new [] {1, 34, 26, 45});
        }

        [Fact]
        public void ParseIntegerArray_Empty_ReturnsEmpty()
        {
            // assert
            "".ParseIntegerArray().ShouldBe(Array.Empty<int>());
        }

        [Fact]
        public void ParseIntegerArray_Whitespace_ReturnsEmpty()
        {
            // assert
            "   ".ParseIntegerArray().ShouldBe(Array.Empty<int>());
        }

        [Fact]
        public void ParseIntegerArray_Null_ReturnsEmpty()
        {
            // assert
            ((string)null).ParseIntegerArray().ShouldBe(Array.Empty<int>());
        }

        [Fact]
        public void ParseIntegerArray_Bad_ThrowsException()
        {
            // assert
            Assert.Throws<FormatException>(() => "  1,,,34;,,BAD   26; 45 ,".ParseIntegerArray());
        }
    }
}
