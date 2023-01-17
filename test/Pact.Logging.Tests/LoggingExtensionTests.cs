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
        dict.Keys.ShouldContain("SecurePassword");
        dict["Id"].ShouldBe(1);
        dict["Name"].ShouldBe("Test");
        dict["SecurePassword"].ShouldBe("[Redacted]");
    }

    [Fact]
    public void GetPropertyDictionary_NotFiltered_OK()
    {
        // arrange
        var obj = new MyClass { Id = 1, Name = "Test", SecurePassword = "blah" };

        // act
        var dict = obj.GetLogPropertyDictionary(Array.Empty<string>());

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
        var obj2 = new MyClass { Id = 1, Name = "Tested", When = DateTime.Now };

        // act
        var dict = obj2.GetDifference(obj);

        // assert
        dict.Keys.ShouldContain("Id");
        dict.Keys.ShouldContain("Name");
        dict.Keys.ShouldContain("When");
        dict["Id"].OriginalValue.ShouldBe(1);
        dict["Name"].NewValue.ShouldBe("Tested");
        dict["Name"].OriginalValue.ShouldBe("Test");
        dict["When"].NewValue.ShouldBe(obj2.When);
        dict["When"].OriginalValue.ShouldBe(null);
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
        dict["Id"].OriginalValue.ShouldBe(1);
        dict["Name"].NewValue.ShouldBe("Tested");
        dict["Name"].OriginalValue.ShouldBe("Test");
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
        dict["Id"].OriginalValue.ShouldBe(1);
        dict["Name"].NewValue.ShouldBe("Tested");
        dict["Name"].OriginalValue.ShouldBe("Test");
    }

    [Fact]
    public void GetDiff_OriginalNull_OK()
    {
        // arrange
        var obj2 = new Dictionary<string, object>
        {
            { "Id", 1 },
            { "Name", "Tested" }
        };

        // act
        var dict = obj2.GetDifference();

        // assert
        dict.Keys.ShouldContain("Id");
        dict.Keys.ShouldContain("Name");
        dict["Id"].OriginalValue.ShouldBe(null);
        dict["Id"].NewValue.ShouldBe(1);
        dict["Name"].NewValue.ShouldBe("Tested");
        dict["Name"].OriginalValue.ShouldBe(null);
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
    public void LogDiff_New_OK()
    {
        // arrange
        var obj2 = new MyClass { Id = 1, Name = "Tested" };
        var logger = new Mock<ILogger<LoggingExtensionTests>>();
        var logObj = logger.Object;

        // act
        logObj.LogDifference(obj2, "Difference");

        // assert
        logger.Verify(m => m.BeginScope(It.IsAny<Dictionary<string, object>>()));
        logger.Verify(m => m.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        logger.VerifyNoOtherCalls();
    }

    [Fact]
    public void LogDiff_New_Debug_OK()
    {
        // arrange
        var obj2 = new MyClass { Id = 1, Name = "Tested" };
        var logger = new Mock<ILogger<LoggingExtensionTests>>();
        var logObj = logger.Object;

        // act
        logObj.LogDifference(LogLevel.Debug, obj2, "Difference");

        // assert
        logger.Verify(m => m.BeginScope(It.IsAny<Dictionary<string, object>>()));
        logger.Verify(m => m.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
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

    [Fact]
    public void IsSupported_DateTime()
    {
        DateTime.Now.IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_NullableDateTime()
    {
        ((DateTime?)DateTime.Now).IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_Long()
    {
        1L.IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_NullableLong()
    {
        ((long?)1).IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_Int()
    {
        1.IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_NullableInt()
    {
        ((int?)1).IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_Double()
    {
        1.0D.IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_NullableDouble()
    {
        ((double?)1.0D).IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_Float()
    {
        1F.IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_NullableFloat()
    {
        ((float?)1.0F).IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_Boolean()
    {
        true.IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_NullableBoolean()
    {
        ((bool?)true).IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsSupported_String()
    {
        "string".IsSupportedLogProperty().ShouldBeTrue();
    }

    [Fact]
    public void IsNotSupported_Class()
    {
        new MyClass().IsSupportedLogProperty().ShouldBeFalse();
    }

    private class MyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecurePassword { get; set; }
        public DateTime? When { get; set; }
    }
}