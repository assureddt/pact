using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using Pact.Web.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Web.Tests
{
    public class SessionExtensionTests
    {
        [Fact]
        public void Set_Integer_OK()
        {
            // arrange
            var sessMock = new Mock<ISession>();
            var sess = sessMock.Object;

            // act
            sess.Set("test", 3);

            // assert
            var expectedBytes = Encoding.UTF8.GetBytes("3");
            sessMock.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes))));
            sessMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Set_Object_OK()
        {
            // arrange
            var sessMock = new Mock<ISession>();
            var sess = sessMock.Object;
            var obj = new MyClass { Id = 1, Name = "test"};
            // act
            sess.Set("test", obj);

            // assert
            var expectedBytes = Encoding.UTF8.GetBytes("{\"Id\":1,\"Name\":\"test\"}");
            sessMock.Verify(m => m.Set("test", It.Is<byte[]>(x => x.SequenceEqual(expectedBytes))));
            sessMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Get_Object_OK()
        {
            // arrange
            var expectedBytes = Encoding.UTF8.GetBytes("{\"Id\":1,\"Name\":\"test\"}");
            var sessMock = new Mock<ISession>();
            sessMock.Setup(m => m.TryGetValue("test", out expectedBytes)).Returns(true);
            var sess = sessMock.Object;

            // act
            var val = sess.Get<MyClass>("test");

            // assert
            sessMock.Verify(m => m.TryGetValue("test", out expectedBytes));
            sessMock.VerifyNoOtherCalls();
            val.ShouldNotBeNull();
            val.Id.ShouldBe(1);
            val.Name.ShouldBe("test");
        }

        private class MyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
