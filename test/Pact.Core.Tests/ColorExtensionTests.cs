using System.Drawing;
using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    public class ColorExtensionTests
    {
        [Fact]
        public void CssHex_AsExpected()
        {
            // setup
            var col = Color.OrangeRed;

            // apply
            var result = col.ToCssHex();

            // assert
            result.ShouldBe("#FF4500");
        }

        [Fact]
        public void CssHex_WithAlpha_AsExpected()
        {
            // setup
            var col = Color.MediumPurple;

            // apply
            var result = col.ToCssHex(true);

            // assert
            result.ShouldBe("#FF9370DB");
        }

        [Fact]
        public void RandomColors_AsExpected()
        {
            // apply
            var result = ColorExtensions.GetRandomKnownColors(8);

            // assert
            result.Count.ShouldBe(8);
            result.ShouldBeUnique();
        }
    }
}
