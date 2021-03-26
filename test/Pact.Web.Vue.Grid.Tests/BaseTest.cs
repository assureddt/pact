using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq.AutoMock;
using System;
using System.Data.Common;

namespace Pact.Web.Vue.Grid.Tests
{
    public class BaseTest : IDisposable
    {
        public AutoMocker mocker;

        private readonly DbConnection _connection;
        private readonly DbContextOptions<FakeContext> _contextOptions;
        public readonly FakeContext _context;
        public readonly MapperConfiguration _mappingConfig;

        public BaseTest()
        {
            _contextOptions = new DbContextOptionsBuilder<FakeContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options;

            _connection = RelationalOptionsExtension.Extract(_contextOptions).Connection;

            var services = new ServiceCollection();

            services.AddDbContext<FakeContext>(o => o.UseSqlite(CreateInMemoryDatabase()));

            services.AddLogging();

            var provider = services.BuildServiceProvider();

            _context = provider.GetRequiredService<FakeContext>(); ;
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            mocker = new AutoMocker(Moq.MockBehavior.Default);
            mocker.Use(_context);
            var _mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Map());
            });
            var mapper = _mappingConfig.CreateMapper();
            mocker.Use(mapper);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
