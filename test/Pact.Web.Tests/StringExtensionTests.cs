using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Pact.Core.Extensions;
using Pact.Web.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Web.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void Attachment_OK()
        {
            const string expected = "Some@Of_these_should-not_be;here, right_!.pdf";
            const string expected2 = "Some%40Of_these_should-not_be%3Bhere%2C%20right_%21.pdf";

            // assert
            "Some@Of:these\\should-not_be;here, right?!.pdf".MakeSafeAttachmentHeader()
                .ShouldBe($"attachment; filename=\"{expected}\"; filename*=UTF-8''{expected2}");
        }

        [Fact]
        public void DescriptionFor_OK()
        {
            // arrange
            var helpMock = new Mock<IHtmlHelper<MyClass>>();
            var help = helpMock.Object;

            var result = help.DescriptionFor(d => d.Name);

            // assert
            result.ShouldBe("Wibble");
        }

        public class MyClass
        {
            public int Id { get; set; }
            [Display(Name = "Dabble", Description = "Wibble")]
            public string Name { get; set; }
        }

        [Fact]
        public void ForJs_OK()
        {
            // arrange
            var helpMock = new Mock<IHtmlHelper>();
            var help = helpMock.Object;

            var _ = help.ForJs("test o'trace");

            // assert
            helpMock.Verify(m => m.Raw(It.Is<string>(x => x == "\"test o\\u0027trace\"")));
            helpMock.VerifyNoOtherCalls();
        }


        [Fact]
        public void ForJs_HtmlContent_OK()
        {
            // arrange
            var content = new HtmlString("test o'trace");
            var helpMock = new Mock<IHtmlHelper>();
            var help = helpMock.Object;

            var _ = help.ForJs(content);

            // assert
            helpMock.Verify(m => m.Raw(It.Is<string>(x => x == "\"test o\\u0027trace\"")));
            helpMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetLines_OK()
        {
            // arrange
            var content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                Proin velit justo, luctus quis odio non, hendrerit scelerisque enim. Nunc convallis, magna vel maximus tempus, orci elit dictum nisl, non vehicula nibh mauris eget ipsum.
                Vestibulum egestas vulputate elit et mattis. Vivamus cursus nulla et felis blandit rhoncus. Integer luctus eleifend sem, id pellentesque quam.
                Sed rhoncus urna eu turpis efficitur, a pretium tortor porttitor. Maecenas in sapien ac ipsum tincidunt consequat sit amet vel nisl.
                Aenean fringilla nunc vel dui sollicitudin, vitae tristique nunc dictum. Mauris hendrerit arcu in elit tempor vulputate. Nunc at nibh elit.";

            // act
            var result = content.GetLines(50, 500);

            // assert
            result.Length.ShouldBe(10);
            result.First().StartsWith("Lorem").ShouldBeTrue();
            result.Last().EndsWith("...").ShouldBeTrue();
            result.Max(x => x.Length).ShouldBeLessThanOrEqualTo(50);
        }
    }
}
