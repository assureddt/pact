using System.Reflection;
using System.Threading.Tasks;
using Pact.Core.Extensions;
using Pact.Core.Helpers;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests;

public class StopwatchExtensionTests
{
    [Fact]
    public void Time_Sync_AsExpected()
    {
        // arrange
        var items = new[]
        {
            new[] {1, 2},
            new[] {1, 2}
        };

        // act
        var result = StopwatchHelper.Time(() =>
        {
            items.CartesianProduct();
        });

        // assert
        result.TotalMilliseconds.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Time_Async_AsExpected()
    {
        // arrange
        var file = System.IO.File.OpenRead(Assembly.GetExecutingAssembly().Location);

        var buff = new byte[1024];
        var read = 0;

        // act
        var result = await StopwatchHelper.TimeAsync(async () =>
        {
            read = await file.ReadAsync(buff, 0, buff.Length);
        });

        // assert
        result.TotalMilliseconds.ShouldBeGreaterThan(0);
        buff.ShouldContain(x => x != 0);
        read.ShouldBe(buff.Length);
    }
}