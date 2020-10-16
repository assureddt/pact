using Newtonsoft.Json;
using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void FromJson_OK()
        {
            // apply
            var result = "{ \"Id\": 1, \"Name\": \"Test\" }".FromJson<MyClass>();

            // check
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(1),
                () => result.Name.ShouldBe("Test"));
        }

        [Fact]
        public void FromJson_BadArray_Throws()
        {
            // check
            Assert.Throws<JsonReaderException>(() => "{ \"Id\": 1, \"Name\": \"Test\" }, { \"Id\": 2, \"Name\": \"Test2\" }".FromJson<MyClass>());
        }

        [Fact]
        public void FromJson_Relaxed_OK()
        {
            // apply
            var result = "{ \"Id\": 1, \"Name\": \"Test\" }, { \"Id\": 2, \"Name\": \"Test2\" }".FromJson<MyClass[]>(true);

            // check
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
            // check
            "My-Name-Is-George_& I was born in 1964".StripNonAlphaNumeric("").ShouldBe("MyNameIsGeorgeIwasbornin1964");
        }

        [Fact]
        public void Filename_OK()
        {
            // check
            "Some@Of:these\\should-not_be;here, right?!".MakeSafeFilename().ShouldBe("Some@Of_these_should-not_be;here, right_!");
        }

        [Fact]
        public void EmailAddresses_OK()
        {
            // check
            "fred.smith@test.com <freddy smith, sales>, jon.smith@test.com".GetEmailAddresses().ShouldBe(new [] {"fred.smith@test.com", "jon.smith@test.com"});
        }

        [Fact]
        public void IsUppercaseCharacter_OK()
        {
            // check
            'F'.IsBasicUppercaseLetter().ShouldBe(true);
        }

        [Fact]
        public void IsUppercaseCharacter_Nope()
        {
            // check
            ';'.IsBasicUppercaseLetter().ShouldBe(false);
        }

        [Fact]
        public void ToNullable_OK()
        {
            // check
            "5".ToNullable<int>().ShouldBe(5);
        }

        [Fact]
        public void ToNullable_Null()
        {
            // check
            "a".ToNullable<int>().ShouldBe(null);
        }

        [Fact]
        public void Masked_Simple_AsExpected()
        {
            // check
            "sensitiveinfohere".Masked(4).ShouldBe("sensitiveinfo****");
        }

        [Fact]
        public void Masked_Inverted_AsExpected()
        {
            // check
            "sensitiveinfohere".Masked(4, true).ShouldBe("*************here");
        }

        [Fact]
        public void Masked_Specific_AsExpected()
        {
            const string source = "sensitiveinfohere";
            // check
            source.Masked('X', 1, source.Length - 1).ShouldBe("sXXXXXXXXXXXXXXXX");
        }

        [Fact]
        public void Ellipsis_AsExpected()
        {
            // check
            "large sentence that we don't want to wrap".Ellipsis(16).ShouldBe("large sentenc...");
        }
    }
}
