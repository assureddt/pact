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
            // setup
            var initial = 1;

            // apply
            8.Times(() => initial++);

            // assert
            initial.ShouldBe(9);
        }
    }
}
