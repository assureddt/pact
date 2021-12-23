using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Pact.Cache.Tests
{
    [Collection("JsonSerializerSequential")]
    public class IntegrationTests
    {
        private readonly IOptions<MemoryDistributedCacheOptions> _opts = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());

        [Fact]
        public async Task GetStringAsync_Missing_Null()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());

            // act & assert
            (await svc.GetAsync<string>("test")).ShouldBeNull();
        }

        [Fact]
        public async Task GetReferenceAsync_Missing_Null()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());

            // act & assert
            (await svc.GetAsync<MyClass>("test")).ShouldBeNull();
        }

        [Fact]
        public async Task GetValueAsync_Missing_Null()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());

            // act & assert
            (await svc.GetValueAsync<int>("test")).ShouldBeNull();
        }
        
        [Fact]
        public async Task GetStringAsync_Found_OK()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            const string val = "testval";
            await svc.SetAsync("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            (await svc.GetAsync<string>("test")).ShouldBe(val);
        }

        [Fact]
        public async Task GetReferenceAsync_Found_OK()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            var val = new MyClass {Id = 80, Name = "testval"};
            await svc.SetAsync("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            (await svc.GetAsync<MyClass>("test")).ShouldBeEquivalentTo(val);
        }

        [Fact]
        public async Task GetValueAsync_Found_OK()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            const int val = 80;
            await svc.SetValueAsync("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            (await svc.GetValueAsync<int>("test")).ShouldBe(val);
        }

        [Fact]
        public async Task GetValueAsync_WrongType_Null()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            const double val = 80.5D;
            await svc.SetValueAsync("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            (await svc.GetValueAsync<int>("test")).ShouldBeNull();
            (await svc.GetValueAsync<double>("test")).ShouldBe(val);
        }

        [Fact]
        public async Task GetValueAsync_Convertible_OK()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            const double val = 80.0D;
            await svc.SetValueAsync("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            (await svc.GetValueAsync<int>("test")).ShouldBe(80);
            (await svc.GetValueAsync<double>("test")).ShouldBe(val);
        }

        [Fact]
        public void GetString_Missing_Null()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());

            // act & assert
            svc.Get<string>("test").ShouldBeNull();
        }

        [Fact]
        public void GetReference_Missing_Null()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());

            // act & assert
            svc.Get<MyClass>("test").ShouldBeNull();
        }

        [Fact]
        public void GetValue_Missing_Null()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());

            // act & assert
            svc.GetValue<int>("test").ShouldBeNull();
        }
        
        [Fact]
        public void GetString_Found_OK()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            const string val = "testval";
            svc.Set("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            svc.Get<string>("test").ShouldBe(val);
        }

        [Fact]
        public void GetReference_Found_OK()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            var val = new MyClass {Id = 80, Name = "testval"};
            svc.Set("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            svc.Get<MyClass>("test").ShouldBeEquivalentTo(val);
        }

        [Fact]
        public void GetValue_Found_OK()
        {
            // arrange
            var svc = new DistributedCacheService(new MemoryDistributedCache(_opts), new NullLogger<DistributedCacheService>());
            var val = 80;
            svc.SetValue("test", val, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});

            // act & assert
            svc.GetValue<int>("test").ShouldBe(val);
        }
        
        private class MyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
