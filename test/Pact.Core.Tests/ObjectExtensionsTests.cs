using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Pact.Core.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void DeepCopy()
        {
            var input = new List<string>() { "Cake" };

            var result = Extensions.ObjectExtensions.DeepCopy(input);

            Assert.True(!ReferenceEquals(result, input));
            Assert.True(string.Equals(result.First(), input.First()));
        }
    }
}
