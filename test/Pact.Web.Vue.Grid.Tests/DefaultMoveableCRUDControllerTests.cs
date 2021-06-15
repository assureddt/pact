using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Tests.Containers;
using System.Threading.Tasks;
using Xunit;
using Pact.Web.Vue.Grid.Controllers;
using Shouldly;
using Pact.Web.Vue.Grid.Models;

namespace Pact.Web.Vue.Grid.Tests
{
    public class DefaultMoveableCRUDControllerTests : BaseTest
    {
        internal class TestController : DefaultMoveableCRUDController<OrderDatabaseObject, GridRowOutput, EditOutput>
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
        public async Task Up_Basic()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake B",
                Order = 1,
                Id = 2
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Up(2);

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            (await _context.Orders.FirstAsync(x => x.Id == 1)).Order.ShouldBe(1);
            (await _context.Orders.FirstAsync(x => x.Id == 2)).Order.ShouldBe(0);
        }

        [Fact]
        public async Task Up_Check_Post_Action_Called()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake B",
                Order = 1,
                Id = 2
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Up(2);

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            testController.WasPostChangeActionCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task Up_At_Bottom()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Up(1);

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            (await _context.Orders.FirstAsync(x => x.Id == 1)).Order.ShouldBe(0);
        }


        [Fact]
        public async Task Down_Basic()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake B",
                Order = 1,
                Id = 2
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Down(1);

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            (await _context.Orders.FirstAsync(x => x.Id == 1)).Order.ShouldBe(1);
            (await _context.Orders.FirstAsync(x => x.Id == 2)).Order.ShouldBe(0);
        }

        [Fact]
        public async Task Down_Check_Post_Action_Called()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake B",
                Order = 1,
                Id = 2
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Down(1);

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            testController.WasPostChangeActionCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task Down_At_Top()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Down(1);

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            (await _context.Orders.FirstAsync(x => x.Id == 1)).Order.ShouldBe(0);
        }

        [Fact]
        public async Task Check_Order_Is_Correct_On_Added()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Add(new EditOutput { Name = "Cake B" });

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            (await _context.Orders.FirstAsync(x => x.Id == 1)).Order.ShouldBe(0);
            (await _context.Orders.FirstAsync(x => x.Id == 2)).Order.ShouldBe(1);
        }

        [Fact]
        public async Task Check_Order_Is_Correct_On_Remove()
        {
            // arrange
            var testController = mocker.CreateInstance<TestController>();

            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake A",
                Order = 0,
                Id = 1
            });
            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake B",
                Order = 1,
                Id = 2
            });
            await _context.Orders.AddAsync(new OrderDatabaseObject
            {
                Name = "Cake C",
                Order = 2,
                Id = 3
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // act
            var result = await testController.Remove(2);

            // assert
            var dataItems = result.Value.ShouldBeAssignableTo<GeneralJsonOK>();
            dataItems.ShouldNotBeNull().Result.ShouldBe("OK");

            (await _context.Orders.FirstAsync(x => x.Id == 1)).Order.ShouldBe(0);
            (await _context.Orders.FirstAsync(x => x.Id == 3)).Order.ShouldBe(1);
        }
    }
}
