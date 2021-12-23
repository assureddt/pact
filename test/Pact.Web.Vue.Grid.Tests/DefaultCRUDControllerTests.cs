using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Tests.Containers;
using System.Threading.Tasks;
using Xunit;
using Pact.Web.Vue.Grid.Controllers;
using Shouldly;
using Pact.Web.Vue.Grid.Models;
using System.Linq;

namespace Pact.Web.Vue.Grid.Tests;

public class DefaultCRUDControllerTests : BaseTest
{
    //This is the rolled up class used for all the tests in this file..
    internal class TestController : DefaultCRUDController<BasicDatabaseObject, GridRowOutput, EditOutput>
    {
        public TestController(FakeContext context, IMapper mapper) : base(context, mapper)
        {
            PostChangeAction = () => {
                WasPostChangeActionCalled = true;
                return Task.CompletedTask;
            };
        }

        public bool WasPostChangeActionCalled;
    }

    [Fact]
    public void CheckIndex()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        // act
        var result = testController.Index();

        // assert
        result.ViewName.ShouldBe("Index");
    }

    [Fact]
    public async Task Read()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.Basics.AddAsync(new BasicDatabaseObject
        {
            Name = "Cake A"
        });
        await _context.SaveChangesAsync();

        // act
        var result = await testController.Read(0, 10, "name", 0, null);

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GenericGridResult<GridRowOutput>>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");
        dataItems.Count.ShouldBe(1);
        dataItems.Records.First().Name.ShouldBe("Cake A");
    }

    [Fact]
    public async Task Data()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.Basics.AddAsync(new BasicDatabaseObject
        {
            Name = "Cake A",
        });
        await _context.SaveChangesAsync();

        // act
        var result = await testController.Data(1);

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<SingleDataResult<EditOutput>>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");
        dataItems.Record.Name.ShouldBe("Cake A");
    }

    [Fact]
    public async Task Add_Basic()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        // act
        var result = await testController.Add(new EditOutput
        {
            Name = "Cake C"
        });

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

        (await _context.Basics.CountAsync()).ShouldBe(1);
        (await _context.Basics.FirstAsync()).Name.ShouldBe("Cake C");
    }

    [Fact]
    public async Task Add_Missing_Server_Validation()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();
        testController.ModelState.AddModelError("test", "not shown");

        // act
        var result = await testController.Add(new EditOutput());

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonMessage>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("FAIL");
    }

    [Fact]
    public async Task Add_Check_Post_Action_Called()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        // act
        var result = await testController.Add(new EditOutput
        {
            Name = "Cake C"
        });

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

        testController.WasPostChangeActionCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task Edit_Basic()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.Basics.AddAsync(new BasicDatabaseObject
        {
            Name = "Cake A",
            Id = 1
        });
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // act
        var result = await testController.Edit(new EditOutput
        {
            Id = 1,
            Name = "Cake C"
        });

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

        (await _context.Basics.CountAsync()).ShouldBe(1);
        (await _context.Basics.FirstAsync()).Name.ShouldBe("Cake C");
    }

    [Fact]
    public async Task Edit_Missing_Server_Validation()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();
        testController.ModelState.AddModelError("test", "not shown");

        // act
        var result = await testController.Edit(new EditOutput());

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonMessage>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("FAIL");
    }

    [Fact]
    public async Task Edit_Check_Post_Action_Called()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.Basics.AddAsync(new BasicDatabaseObject
        {
            Name = "Cake A",
            Id = 1
        });
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // act
        var result = await testController.Edit(new EditOutput
        {
            Id = 1,
            Name = "Cake C"
        });

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

        testController.WasPostChangeActionCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task Remove_Basic()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.Basics.AddAsync(new BasicDatabaseObject
        {
            Name = "Cake A",
            Id = 1
        });
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // act
        var result = await testController.Remove(1);

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

        (await _context.Basics.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task Remove_Check_Post_Action_Called()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.Basics.AddAsync(new BasicDatabaseObject
        {
            Name = "Cake A",
            Id = 1
        });
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // act
        var result = await testController.Remove(1);

        // assert
        var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
        dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

        testController.WasPostChangeActionCalled.ShouldBeTrue();
    }
}