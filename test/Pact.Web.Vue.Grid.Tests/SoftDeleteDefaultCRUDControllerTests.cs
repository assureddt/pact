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

public class SoftDeleteDefaultCRUDControllerTests : BaseTest
{
    //This is the rolled up class used for all the tests in this file..
    internal class TestController : DefaultCRUDController<SoftDeleteDatabaseObject, GridRowOutput, EditOutput>
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
    public async Task Read()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.SoftDeletes.AddAsync(new SoftDeleteDatabaseObject
        {
            Name = "Cake A"
        });
        await _context.SoftDeletes.AddAsync(new SoftDeleteDatabaseObject
        {
            Name = "Cake B",
            SoftDelete = true
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
    public async Task Remove_Basic()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.SoftDeletes.AddAsync(new SoftDeleteDatabaseObject
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

        (await _context.SoftDeletes.CountAsync()).ShouldBe(1);
        (await _context.SoftDeletes.FirstAsync()).SoftDelete.ShouldBe(true);
    }

    [Fact]
    public async Task Remove_Check_Post_Action_Called()
    {
        // arrange
        var testController = mocker.CreateInstance<TestController>();

        await _context.SoftDeletes.AddAsync(new SoftDeleteDatabaseObject
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