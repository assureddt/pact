using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    public class IntegerExtensionTests
    {
        [Fact]
        public void Times_Applied()
        {
            // arrange
            var initial = 1;

            // act
            8.Times(() => initial++);

            // assert
            initial.ShouldBe(9);
        }
    }
}
