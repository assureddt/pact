using Microsoft.AspNetCore.Mvc;
using Pact.Kendo.Tests.Containers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Pact.Kendo.Tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void Kendo_CoverAll_No_Sort()
        {
            var items = new List<Basic>
            {
                new() { Id = 1, Name = "Cat" },
                new() { Id = 2, Name = "Dog" }
            };

            var request = new KendoDataRequest
            {
                Page = 0,
                PageSize = 5,
                Skip = 0,
                Take = 5
            };

            var results = items.Kendo(request);
            Assert.True(results.Count() == 2);
        }

        [Fact]
        public void Kendo_CoverAll_Sort()
        {
            var items = new List<Basic>
            {
                new() { Id = 1, Name = "Cat" },
                new() { Id = 2, Name = "Dog" }
            };

            var request = new KendoDataRequest
            {
                Page = 0,
                PageSize = 5,
                Skip = 0,
                Take = 5,
                Sort = new List<KendoDataRequestSort>
                {
                    new() {Dir = "ASC", Field = "Name"}
                }
            };

            var results = items.Kendo(request);
            Assert.True(results.First().Name == "Cat");
        }

        [Fact]
        public void Kendo_Multi_Sort()
        {
            var items = new List<Basic>
            {
                new() { Id = 1, Name = "Cat", Order = 0 },
                new() { Id = 2, Name = "Dog", Order = 1 },
                new() { Id = 3, Name = "Aardvark", Order = 1 }
            };

            var request = new KendoDataRequest
            {
                Page = 0,
                PageSize = 5,
                Skip = 0,
                Take = 5,
                Sort = new List<KendoDataRequestSort>
                {
                    new() {Dir = "ASC", Field = "Order"},
                    new() {Dir = "ASC", Field = "Name"}
                }
            };

            var results = items.Kendo(request);
            Assert.True(results.First().Name == "Cat");
            Assert.True(results.Skip(1).First().Name == "Aardvark");
            Assert.True(results.Skip(2).First().Name == "Dog");
        }

        [Fact]
        public void KendoResult_Standard()
        {
            var items = new List<Basic>
            {
                new() { Id = 1, Name = "Cat" },
                new() { Id = 2, Name = "Dog" }
            };

            var request = new KendoDataRequest
            {
                Page = 0,
                PageSize = 5,
                Skip = 0,
                Take = 5
            };

            var result = items.KendoResult(request);

            Assert.IsType<JsonResult>(result);
            var resultValue = Assert.IsType<KendoResult<Basic>>(result.Value);
            Assert.True(resultValue.Result == "OK");
            Assert.True(resultValue.Count == 2);
            Assert.True(resultValue.Records.Count == 2);
        }

        [Theory]
        [InlineData("ASC")]
        [InlineData("DESC")]
        public void KendoResult_Skip_Take(string direction)
        {
            var items = new List<Basic>
            {
                new() { Id = 1, Name = "Cat" },
                new() { Id = 2, Name = "Dog" },
                new() { Id = 3, Name = "Apple" },
                new() { Id = 4, Name = "Fish" },
                new() { Id = 5, Name = "Cake" }
            };

            var request = new KendoDataRequest
            {
                Page = 0,
                PageSize = 5,
                Skip = 1,
                Take = 1,
                Sort = new List<KendoDataRequestSort>
                {
                    new() {Dir = direction, Field = "Name"}
                }
            };

            var result = items.KendoResult(request);

            Assert.IsType<JsonResult>(result);
            var resultValue = Assert.IsType<KendoResult<Basic>>(result.Value);
            Assert.True(resultValue.Result == "OK");
            Assert.True(resultValue.Count == 5);

            var resultItem = Assert.Single(resultValue.Records);
            Assert.True(resultItem.Name == "Dog");
        }
    }
}
