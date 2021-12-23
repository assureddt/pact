using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace Pact.Cache.Tests;

[Collection("JsonSerializerSequential")]
public class DualLayerCacheServiceTests
{
    private readonly IOptions<CacheSettings> _opts =
        new OptionsWrapper<CacheSettings>(new CacheSettings {DefaultMemoryExpirySeconds = 60});

    [Fact]
    public async Task GetAsync_OK()
    {
        // arrange
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var _ = await svc.GetAsync<string>("test");

        // assert
        cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
        cache.VerifyNoOtherCalls();
        entry.VerifyNoOtherCalls();
        object val;
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAsyncGeneric_OK()
    {
        // arrange
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var _ = await svc.GetAsync<MyClass>("test");

        // assert
        cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
        cache.VerifyNoOtherCalls();
        entry.VerifyNoOtherCalls();
        object val;
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void Get_OK()
    {
        // arrange
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var _ = svc.Get<string>("test");

        // assert
        cache.Verify(m => m.Get("test"));
        cache.VerifyNoOtherCalls();
        entry.VerifyNoOtherCalls();
        object val;
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void Get_InMem_SkipDistributed_OK()
    {
        // arrange
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        object val = "blob";
        memory.Setup(m => m.TryGetValue("test", out val)).Returns(true);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var _ = svc.Get<string>("test");

        // assert
        cache.VerifyNoOtherCalls();
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrCreateAsync_Generic_OK()
    {
        // arrange
        const string encoded = "{\"Id\":1,\"Name\":\"Test\"}";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        cache.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[])null);
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var obj = new MyClass {Id = 1, Name = "Test"};
        var _ = await svc.GetOrCreateAsync("test", _ => Task.FromResult(obj));

        // assert
        cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
        cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        object val;
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrCreateAsync_OK()
    {
        // arrange
        const string encoded = "\"blob\"";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        cache.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[])null);
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        const string obj = "blob";
        var _ = await svc.GetOrCreateAsync("test", _ => Task.FromResult(obj));

        // assert
        cache.Verify(m => m.GetAsync("test", It.IsAny<CancellationToken>()));
        cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        object val;
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetOrCreate_Generic_OK()
    {
        // arrange
        const string encoded = "{\"Id\":1,\"Name\":\"Test\"}";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        cache.Setup(m => m.Get(It.IsAny<string>())).Returns((byte[])null);
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var obj = new MyClass { Id = 1, Name = "Test" };
        var _ = svc.GetOrCreate("test", _ => obj);

        // assert
        cache.Verify(m => m.Get("test"));
        cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        object val;
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetOrCreate_OK()
    {
        // arrange
        const string encoded = "\"blob\"";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        cache.Setup(m => m.Get(It.IsAny<string>())).Returns((byte[])null);
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        const string obj = "blob";
        var _ = svc.GetOrCreate("test", _ => obj);

        // assert
        cache.Verify(m => m.Get("test"));
        cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        object val;
        memory.Verify(m => m.TryGetValue("test", out val));
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SetAsync_Generic_OK()
    {
        // arrange
        const string encoded = "{\"Id\":1,\"Name\":\"Test\"}";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var obj = new MyClass { Id = 1, Name = "Test" };
        var _ = await svc.SetAsync("test", _ => Task.FromResult(obj));

        // assert
        cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SetAsync_OK()
    {
        // arrange
        const string encoded = "\"blob\"";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        const string obj = "blob";
        var _ = await svc.SetAsync("test", _ => Task.FromResult(obj));

        // assert
        cache.Verify(m => m.SetAsync("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void Set_Generic_OK()
    {
        // arrange
        const string encoded = "{\"Id\":1,\"Name\":\"Test\"}";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var obj = new MyClass { Id = 1, Name = "Test" };
        var _ = svc.Set("test", _ => obj);

        // assert
        cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void Set_OK()
    {
        // arrange
        const string encoded = "\"blob\"";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        const string obj = "blob";
        var _ = svc.Set("test", _ => obj);

        // assert
        cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.Value.DefaultMemoryExpirySeconds));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void Set_ExcessiveMemDurationCappedAtHalf_OK()
    {
        // arrange
        const string encoded = "\"blob\"";
        var expectedBytes = Encoding.UTF8.GetBytes(encoded);
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(),
            new OptionsWrapper<CacheSettings>(new CacheSettings { DefaultMemoryExpirySeconds = 60*60*24 }));

        // act
        var obj = "blob";
        var _ = svc.Set("test", opts =>
        {
            opts.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);
            return obj;
        });

        // assert
        cache.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), It.IsAny<DistributedCacheEntryOptions>()));
        cache.VerifyNoOtherCalls();
        entry.VerifySet(m => m.Value = obj);
        entry.VerifySet(m => m.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6));
        entry.Verify(m => m.Dispose());
        entry.VerifyNoOtherCalls();
        memory.Verify(m => m.CreateEntry("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveAsync_OK()
    {
        // arrange
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        await svc.RemoveAsync("test");

        // assert
        cache.Verify(m => m.RemoveAsync("test", It.IsAny<CancellationToken>()));
        cache.VerifyNoOtherCalls();
        memory.Verify(m => m.Remove("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public void Remove_OK()
    {
        // arrange
        var cache = new Mock<IDistributedCache>();
        var memory = new Mock<IMemoryCache>();
        var entry = new Mock<ICacheEntry>();
        memory.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var svc = new DualLayerCacheService(cache.Object, memory.Object, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        svc.Remove("test");

        // assert
        cache.Verify(m => m.Remove("test"));
        cache.VerifyNoOtherCalls();
        memory.Verify(m => m.Remove("test"));
        memory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAsyncGeneric_Integration_OK()
    {
        // arrange
        var cache = new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        await cache.SetStringAsync("test", "{ \"id\": 1, \"Name\": \"Test\" }");

        var memory = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        var svc = new DualLayerCacheService(cache, memory, new NullLogger<DualLayerCacheService>(), _opts);

        // act
        var result = await svc.GetAsync<MyClass>("test");

        // assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
        result.Name.ShouldBe("Test");
    }

    private class MyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}