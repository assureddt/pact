using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Tests.Containers;
using System.Threading.Tasks;
using Xunit;
using Pact.Web.Vue.Grid.Controllers;
using Shouldly;
using Pact.Web.Vue.Grid.Models;
using System.Linq;

namespace Pact.Web.Vue.Grid.Tests
{
    public class BaseCRUDControllerTests : BaseTest
    {
        //This is the rolled up class used for all the tests in this file..
        internal class TestController : BaseCRUDController<BasicDatabaseObject, GridRowOutput, EditOutput>
        {
            public TestController(FakeContext context, IMapper mapper) : base(context, mapper)
            {
            }
        }

        [Fact]
        public void CheckIndex()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            // act
            var reuslt = testController.Index();

            // assert
            reuslt.ViewName.ShouldBe("Index");
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
            var reuslt = await testController.Read(0, 10, "name", 0, null);

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GenericGridResult<GridRowOutput>>();
            dataItems.Result.ShouldBe("OK");
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
            var reuslt = await testController.Data(1);

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<SingleDataResult<EditOutput>>();
            dataItems.Result.ShouldBe("OK");
            dataItems.Record.Name.ShouldBe("Cake A");
        }

        [Fact]
        public async Task Add()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            // act
            var reuslt = await testController.Add(new EditOutput
            {
                Name = "Cake C"
            });

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.Result.ShouldBe("OK");

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
            var reuslt = await testController.Add(new EditOutput());

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GeneralJsonMessage>();
            dataItems.Result.ShouldBe("FAIL");
        }

        [Fact]
        public async Task Edit()
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
            var reuslt = await testController.Edit(new EditOutput
            {
                Id = 1,
                Name = "Cake C"
            });

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.Result.ShouldBe("OK");

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
            var reuslt = await testController.Edit(new EditOutput());

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GeneralJsonMessage>();
            dataItems.Result.ShouldBe("FAIL");
        }

        [Fact]
        public async Task Remove()
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
            var reuslt = await testController.Remove(1);

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.Result.ShouldBe("OK");

            (await _context.Basics.CountAsync()).ShouldBe(0);
        }
    }
}
