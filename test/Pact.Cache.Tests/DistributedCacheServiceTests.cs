using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Pact.Cache.Tests
{
    public class DistributedCacheServiceTests
    {
        [Fact]
        public async Task GetAsync_OK()
        {
            // arrange
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = await svc.GetAsync("test");

            // assert
            cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAsyncGeneric_OK()
        {
            // arrange
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = await svc.GetAsync<MyClass>("test");

            // assert
            cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public void Get_OK()
        {
            // arrange
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = svc.Get("test");

            // assert
            cache.Verify(m => m.Get("test"));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetOrCreateAsync_Generic_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("{\"Id\":1,\"Name\":\"Test\"}");
            var cache = new Mock<IDistributedCache>();
            cache.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[])null);
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = await svc.GetOrCreateAsync("test", opts => Task.FromResult(new MyClass {Id = 1, Name = "Test"}));

            // assert
            cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
            cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetOrCreateAsync_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("blob");
            var cache = new Mock<IDistributedCache>();
            cache.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[])null);
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = await svc.GetOrCreateAsync("test", opts => Task.FromResult("blob"));

            // assert
            cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
            cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetOrCreate_Generic_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("{\"Id\":1,\"Name\":\"Test\"}");
            var cache = new Mock<IDistributedCache>();
            cache.Setup(m => m.Get(It.IsAny<string>())).Returns((byte[])null);
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = svc.GetOrCreate("test", opts => new MyClass { Id = 1, Name = "Test" });

            // assert
            cache.Verify(m => m.Get("test"));
            cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetOrCreate_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("blob");
            var cache = new Mock<IDistributedCache>();
            cache.Setup(m => m.Get(It.IsAny<string>())).Returns((byte[])null);
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = svc.GetOrCreate("test", opts => "blob");

            // assert
            cache.Verify(m => m.Get("test"));
            cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SetAsync_Generic_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("{\"Id\":1,\"Name\":\"Test\"}");
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = await svc.SetAsync("test", opts => Task.FromResult(new MyClass { Id = 1, Name = "Test" }));

            // assert
            cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SetAsync_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("blob");
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = await svc.SetAsync("test", opts => Task.FromResult("blob"));

            // assert
            cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public void Set_Generic_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("{\"Id\":1,\"Name\":\"Test\"}");
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = svc.Set("test", opts => new MyClass { Id = 1, Name = "Test" });

            // assert
            cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public void Set_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("blob");
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            var _ = svc.Set("test", opts => "blob");

            // assert
            cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RemoveAsync_OK()
        {
            // arrange
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            await svc.RemoveAsync("test");

            // assert
            cache.Verify(m => m.RemoveAsync("test", It.IsAny<CancellationToken>()));
            cache.VerifyNoOtherCalls();
        }

        [Fact]
        public void Remove_OK()
        {
            // arrange
            var cache = new Mock<IDistributedCache>();
            var svc = new DistributedCacheService(cache.Object, new NullLogger<DistributedCacheService>());

            // act
            svc.Remove("test");

            // assert
            cache.Verify(m => m.Remove("test"));
            cache.VerifyNoOtherCalls();
        }

        private class MyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}