using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pact.Core.Extensions;
using Pact.Web.Vue.Grid.Interfaces;

namespace Pact.Web.Vue.Grid.Controllers
{
    /// <summary>
    /// Default Moveable CRUD Setup
    /// </summary>
    /// <typeparam name="TMoveableDatabaseDTO">Type of moveable database object</typeparam>
    /// <typeparam name="TGridRowDTO">Type of grid item</typeparam>
    /// <typeparam name="TEditDTO">Type of edit item</typeparam>
    public abstract class DefaultMoveableCRUDController<TMoveableDatabaseDTO, TGridRowDTO, TEditDTO> : BaseCRUDMoveableController<TMoveableDatabaseDTO, TGridRowDTO, TEditDTO>
        where TMoveableDatabaseDTO : class, IMoveable
        where TGridRowDTO : class, IGridRow, new()
        where TEditDTO : class, IEdit, new()
    {
        public DefaultMoveableCRUDController(DbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public virtual ViewResult Index() => View("Index");

        public virtual async Task<JsonResult> Read(int page, int size, string order, int direction, string filter) =>
            await GetGridDataSet(page, size, order, direction, filter);

        public virtual async Task<JsonResult> Data(int id) => await ProcessData(id);

        [HttpPost]
        public virtual async Task<JsonResult> Add([FromBody] TEditDTO model) =>
            await ProcessAdd(model, ModelState, async (x) => { x.Order = await Context.Set<TMoveableDatabaseDTO>().CountAsync(); });

        [HttpPost]
        public virtual async Task<JsonResult> Edit([FromBody] TEditDTO model) => await ProcessEdit(model, ModelState);

        public virtual async Task<JsonResult> Remove(int id)
        {
            return await ProcessRemove(id, async (_) =>
            {
                var items = await Context.Set<TMoveableDatabaseDTO>().SoftDelete().OrderBy(x => x.Order).ToListAsync();
                await ProcessReordering(items);
            });
        }

        public virtual async Task<JsonResult> Up(int id)
        {
            var item = await Context.Set<TMoveableDatabaseDTO>().FirstAsync(x => x.Id == id);
            var swappedItem = await Context.Set<TMoveableDatabaseDTO>().FirstOrDefaultAsync(x => x.Order == item.Order - 1);
            return await ProcessMoveUp(item, swappedItem);
        }

        public virtual async Task<JsonResult> Down(int id)
        {
            var item = await Context.Set<TMoveableDatabaseDTO>().FirstAsync(x => x.Id == id);
            var max = await Context.Set<TMoveableDatabaseDTO>().MaxAsync(x => x.Order);
            var swappedItem = await Context.Set<TMoveableDatabaseDTO>().FirstOrDefaultAsync(x => x.Order == item.Order + 1);
            return await ProcessMoveDown(item, swappedItem, max);
        }
    }
}