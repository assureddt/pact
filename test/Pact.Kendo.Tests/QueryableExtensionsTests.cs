using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pact.Kendo.Tests.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace Pact.Kendo.Tests
{
    public class QueryableExtensionsTests
    {
        private FakeContext GetFakeContext()
        {
            var options = new DbContextOptionsBuilder<FakeContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            return new FakeContext(options);
        }


        [Fact]
        public void Kendo_CoverAll_No_Sort()
        {
            var items = new List<Basic>();
            items.Add(new Basic { Id = 1, Name = "Cat" });
            items.Add(new Basic { Id = 2, Name = "Dog" });

            var request = new KendoDataRequest
            {
                Page = 0,
                PageSize = 5,
                Skip = 0,
                Take = 5
            };

            var results = items.AsQueryable().Kendo(request);
            Assert.True(results.Count() == 2);
        }

        [Fact]
        public void Kendo_CoverAll_Sort()
        {
            var items = new List<Basic>();
            items.Add(new Basic { Id = 1, Name = "Cat" });
            items.Add(new Basic { Id = 2, Name = "Dog" });

            var request = new KendoDataRequest
            {
                Page = 0,
                PageSize = 5,
                Skip = 0,
                Take = 5,
                Sort = new List<KendoDataRequestSort>
                {
                    new KendoDataRequestSort {Dir = "ASC", Field = "Name"}
                }
            };

            var results = items.AsQueryable().Kendo(request);
            Assert.True(results.First().Name == "Cat");
        }

        [Fact]
        public void SoftDelete_All_Gone()
        {
            var items = new List<Basic>();
            items.Add(new Basic { Id = 1, Name = "Cat", SoftDelete = true });
            items.Add(new Basic { Id = 2, Name = "Dog", SoftDelete = true });

            var result = items.AsQueryable().SoftDelete();

            Assert.Empty(result);
        }

        [Fact]
        public void SoftDelete_Correct_Removed()
        {
            var items = new List<Basic>();
            items.Add(new Basic { Id = 1, Name = "Cat", SoftDelete = true });
            items.Add(new Basic { Id = 2, Name = "Dog" });

            var result = items.AsQueryable().SoftDelete();

            var resultItem = Assert.Single(result);
            Assert.True(resultItem.Id == 2);
        }

        [Fact]
        public void SoftDelete_None_Removed()
        {
            var items = new List<Basic>();
            items.Add(new Basic { Id = 1, Name = "Cat" });
            items.Add(new Basic { Id = 2, Name = "Dog" });

            var result = items.AsQueryable().SoftDelete();

            Assert.True(result.Count() == 2);
        }

        [Fact]
        public async Task KendoResult_Standard()
        {
            using (var context = GetFakeContext())
            {
                await context.Basics.AddAsync(new Basic { Id = 1, Name = "Cat" });
                await context.Basics.AddAsync(new Basic { Id = 2, Name = "Dog" });
                await context.SaveChangesAsync();

                var request = new KendoDataRequest
                {
                    Page = 0,
                    PageSize = 5,
                    Skip = 0,
                    Take = 5
                };

                var result = await context.Basics.KendoResultAsync(request);

                Assert.IsType<JsonResult>(result);
                var resultValue = Assert.IsType<KendoResult<Basic>>(result.Value);
                Assert.True(resultValue.Result == "OK");
                Assert.True(resultValue.Count == 2);
                Assert.True(resultValue.Records.Count == 2);
            }
        }

        [Fact]
        public async Task KendoResult_Skip_Take()
        {
            using (var context = GetFakeContext())
            {
                await context.Basics.AddAsync(new Basic { Id = 1, Name = "Cat" });
                await context.Basics.AddAsync(new Basic { Id = 2, Name = "Dog" });
                await context.Basics.AddAsync(new Basic { Id = 3, Name = "Apple" });
                await context.Basics.AddAsync(new Basic { Id = 4, Name = "Fish" });
                await context.Basics.AddAsync(new Basic { Id = 5, Name = "Cake" });
                await context.SaveChangesAsync();

                var request = new KendoDataRequest
                {
                    Page = 0,
                    PageSize = 5,
                    Skip = 1,
                    Take = 1,
                    Sort = new List<KendoDataRequestSort>
                {
                    new KendoDataRequestSort {Dir = "ASC", Field = "Name"}
                }
                };

                var result = await context.Basics.KendoResultAsync(request);

                Assert.IsType<JsonResult>(result);
                var resultValue = Assert.IsType<KendoResult<Basic>>(result.Value);
                Assert.True(resultValue.Result == "OK");
                Assert.True(resultValue.Count == 5);

                var resultItem = Assert.Single(resultValue.Records);
                Assert.True(resultItem.Name == "Dog");
            }
        }
    }
}
