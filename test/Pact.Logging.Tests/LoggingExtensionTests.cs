using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Pact.Logging.Tests;

public class LoggingExtensionTests
{
    [Fact]
    public void GetPropertyDictionary_OK()
    {
        // arrange
        var obj = new MyClass { Id = 1, Name = "Test", SecurePassword = "blah"};
            
        // act
        var dict = obj.GetLogPropertyDictionary();

        // assert
        dict.Keys.ShouldContain("Id");
        dict.Keys.ShouldContain("Name");
        dict.Keys.ShouldNotContain("SecurePassword");
        dict["Id"].ShouldBe(1);
        dict["Name"].ShouldBe("Test");
    }

    [Fact]
    public void GetPropertyDictionary_NotFiltered_OK()
    {
        // arrange
        var obj = new MyClass { Id = 1, Name = "Test", SecurePassword = "blah" };

        // act
        var dict = obj.GetLogPropertyDictionary(false);

        // assert
        dict.Keys.ShouldContain("Id");
        dict.Keys.ShouldContain("Name");
        dict.Keys.ShouldContain("SecurePassword");
        dict["Id"].ShouldBe(1);
        dict["Name"].ShouldBe("Test");
        dict["SecurePassword"].ShouldBe("blah");
    }

    [Fact]
    public void GetDiff_OK()
    {
        // arrange
        var obj = new MyClass { Id = 1, Name = "Test" };
        var obj2 = new MyClass { Id = 1, Name = "Tested" };

        // act
        var dict = obj2.GetDifference(obj);

        // assert
        dict.Keys.ShouldContain("Id");
        dict.Keys.ShouldContain("Name");
        dict.Keys.ShouldContain("__Name");
        dict.Keys.ShouldNotContain("SecurePassword");
        dict["Id"].ShouldBe(1);
        dict["Name"].ShouldBe("Tested");
        dict["__Name"].ShouldBe("Test");
    }
        
    [Fact]
    public void GetDiff_ViaDict_OK()
    {
        // arrange
        var obj = new Dictionary<string, object>
        {
            { "Id", 1 },
            { "Name", "Test" }
        };
        var obj2 = new MyClass { Id = 1, Name = "Tested" };

        // act
        var dict = obj2.GetDifference(obj);

        // assert
        dict.Keys.ShouldContain("Id");
        dict.Keys.ShouldContain("Name");
        dict.Keys.ShouldContain("__Name");
        dict.Keys.ShouldNotContain("SecurePassword");
        dict["Id"].ShouldBe(1);
        dict["Name"].ShouldBe("Tested");
        dict["__Name"].ShouldBe("Test");
    }

    [Fact]
    public void GetDiff_Via2Dict_OK()
    {
        // arrange
        var obj = new Dictionary<string, object>
        {
            { "Id", 1 },
            { "Name", "Test" }
        };
        var obj2 = new Dictionary<string, object>
        {
            { "Id", 1 },
            { "Name", "Tested" }
        };

        // act
        var dict = obj2.GetDifference(obj);

        // assert
        dict.Keys.ShouldContain("Id");
        dict.Keys.ShouldContain("Name");
        dict.Keys.ShouldContain("__Name");
        dict.Keys.ShouldNotContain("SecurePassword");
        dict["Id"].ShouldBe(1);
        dict["Name"].ShouldBe("Tested");
        dict["__Name"].ShouldBe("Test");
    }

    [Fact]
    public void LogDiff_OK()
    {
        // arrange
        var obj = new Dictionary<string, object>
        {
            { "Id", 1 },
            { "Name", "Test" }
        };
        var obj2 = new MyClass { Id = 1, Name = "Tested" };
        var logger = new Mock<ILogger<LoggingExtensionTests>>();
        var logObj = logger.Object;

        // act
        logObj.LogDifference(obj, obj2, "Difference");

        // assert
        logger.Verify(m => m.BeginScope(It.IsAny<Dictionary<string, object>>()));
        logger.Verify(m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        logger.VerifyNoOtherCalls();
    }

    [Fact]
    public void Itsa_Me_Mario()
    {
        // act & assert
        this.MethodName().ShouldBe("Itsa_Me_Mario");
    }

    [Fact]
    public void Itsa_Me_Mario_Too()
    {
        // act & assert
        this.FullMethodName().ShouldBe("Pact.Logging.Tests.LoggingExtensionTests.Itsa_Me_Mario_Too");
    }

    private class MyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecurePassword { get; set; }
    }
}