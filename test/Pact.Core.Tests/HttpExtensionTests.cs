using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pact.Core.Extensions;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    public class HttpExtensionTests
    {
        [Fact]
        public async Task HttpClient_Post_Called()
        {
            // arrange
            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.Expect("http://test/api/testmethod")
                .WithHeaders("Content-Type: application/json")
                .WithContent("{\"Id\":1,\"Name\":\"Test\"}")
                .Respond(HttpStatusCode.OK); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();

            // act
            var result = await client.PostAsJsonAsync("http://test/api/testmethod", new MyClass { Id = 1, Name = "Test"});

            // assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            mockHttp.GetMatchCount(request).ShouldBe(1);
        }

        [Fact]
        public async Task HttpClient_Put_Called()
        {
            // arrange
            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.Expect("http://test/api/testmethod")
                .WithHeaders("Content-Type: application/json")
                .WithContent("{\"Id\":1,\"Name\":\"Test\"}")
                .Respond(HttpStatusCode.OK); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();

            // act
            var result = await client.PutAsJsonAsync("http://test/api/testmethod", new MyClass { Id = 1, Name = "Test" });

            // assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            mockHttp.GetMatchCount(request).ShouldBe(1);
        }

        [Fact]
        public async Task HttpContent_Json_Get_OK()
        {
            // arrange
            var content = new StringContent("{\"Id\":1,\"Name\":\"Test\"}");

            // act
            var result = await content.ReadAsJsonAsync<MyClass>();

            // assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Test");
        }

        [Fact]
        public void HttpContext_Safe_Match_True()
        {
            // arrange
            const string ip = "92.233.254.58";
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse(ip);

            // act & assert
            context.IsSafe(ip).ShouldBeTrue();
        }

        [Fact]
        public void HttpContext_Safe_NoMatch_False()
        {
            // arrange
            const string ip = "92.233.254.58";
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse(ip);

            // act & assert
            context.IsSafe("127.0.0.1").ShouldBeFalse();
        }

        [Fact]
        public void HttpContext_Local_Loopback_True()
        {
            // arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            // act & assert
            context.IsLocal().ShouldBeTrue();
        }

        [Fact]
        public void HttpContext_Local_Remote_False()
        {
            // arrange
            const string ip = "92.233.254.58";
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse(ip);

            // act & assert
            context.IsLocal().ShouldBeFalse();
        }

        [Fact]
        public void HttpRequest_Local_Loopback_True()
        {
            // arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            // act & assert
            context.Request.IsLocal().ShouldBeTrue();
        }

        [Fact]
        public void HttpRequest_Local_Remote_False()
        {
            // arrange
            const string ip = "92.233.254.58";
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse(ip);

            // act & assert
            context.Request.IsLocal().ShouldBeFalse();
        }

        [Fact]
        public void HttpRequest_IsAjax_Yes_True()
        {
            // arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

            // act & assert
            context.Request.IsAjaxRequest().ShouldBeTrue();
        }

        [Fact]
        public void HttpRequest_IsAjax_No_False()
        {
            // arrange
            var context = new DefaultHttpContext();

            // act & assert
            context.Request.IsAjaxRequest().ShouldBeFalse();
        }

        private class MyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
