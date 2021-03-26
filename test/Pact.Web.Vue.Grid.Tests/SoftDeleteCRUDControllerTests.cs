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
    public class SoftDeleteCRUDControllerTests : BaseTest
    {
        //This is the rolled up class used for all the tests in this file..
        internal class TestController : BaseCRUDController<SoftDeleteDatabaseObject, GridRowOutput, EditOutput>
        {
            public TestController(FakeContext context, IMapper mapper) : base(context, mapper)
            {
            }
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
            var reuslt = await testController.Read(0, 10, "name", 0, null);

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GenericGridResult<GridRowOutput>>();
            dataItems.Result.ShouldBe("OK");
            dataItems.Count.ShouldBe(1);
            dataItems.Records.First().Name.ShouldBe("Cake A");
        }


        [Fact]
        public async Task Remove()
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
            var reuslt = await testController.Remove(1);

            // assert
            var dataItems = reuslt.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.Result.ShouldBe("OK");

            (await _context.SoftDeletes.CountAsync()).ShouldBe(1);
            (await _context.SoftDeletes.FirstAsync()).SoftDelete.ShouldBe(true);
        }
    }
}
