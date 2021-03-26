using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Interfaces;
using Pact.Web.Extensions;
using AutoMapper;
using System.Collections.Generic;

namespace Pact.Web.Vue.Grid.Controllers
{
    public abstract class BaseCRUDMoveableController<MoveableDatabaseDTO, GridRowDTO, EditDTO> : BaseCRUDController<MoveableDatabaseDTO, GridRowDTO, EditDTO>
        where MoveableDatabaseDTO : class, IMoveableDatabaseObject
        where GridRowDTO : class, IGridRow, new()
        where EditDTO : class, IEdit, new()
    {
        public BaseCRUDMoveableController(DbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpPost]
        public async override Task<JsonResult> Add([FromBody] EditDTO model) => await ProcessAdd(model, ModelState, async (x) => { x.Order = await Context.Set<MoveableDatabaseDTO>().CountAsync(); });

        public async override Task<JsonResult> Remove(int id)
        {
            return await ProcessRemove(id, async (removedItem) =>
            {
                var items = await Context.Set<MoveableDatabaseDTO>().SoftDelete().OrderBy(x => x.Order).ToListAsync();
                await ProcessReordering(items);
            });
        }

        [NonAction]
        public async Task ProcessReordering(List<MoveableDatabaseDTO> items)
        {
            var order = 0;
            foreach (var item in items)
            {
                Context.Attach(item);
                item.Order = order;
                order++;
            }
            await Context.SaveChangesAsync();
        }

        public async virtual Task<JsonResult> Up(int id)
        {
            var item = await Context.Set<MoveableDatabaseDTO>().FirstAsync(x => x.Id == id);
            var swappedItem = await Context.Set<MoveableDatabaseDTO>().FirstOrDefaultAsync(x => x.Order == item.Order - 1);
            return await ProcessMoveUp(item, swappedItem);
        }

        [NonAction]
        public async Task<JsonResult> ProcessMoveUp(MoveableDatabaseDTO original, MoveableDatabaseDTO? swapped)
        {
            if (original.Order == 0 || swapped == null)
                return JsonOK();

            Context.Attach(original);
            Context.Attach(swapped);

            original.Order = original.Order - 1;
            swapped.Order = swapped.Order + 1;

            await Context.SaveChangesAsync();
            return JsonOK();
        }

        public async virtual Task<JsonResult> Down(int id)
        {
            var item = await Context.Set<MoveableDatabaseDTO>().FirstAsync(x => x.Id == id);
            var max = await Context.Set<MoveableDatabaseDTO>().MaxAsync(x => x.Order);
            var swappedItem = await Context.Set<MoveableDatabaseDTO>().FirstOrDefaultAsync(x => x.Order == item.Order + 1);
            return await ProcessMoveDown(item, swappedItem, max);
        }

        [NonAction]
        public async Task<JsonResult> ProcessMoveDown(MoveableDatabaseDTO original, MoveableDatabaseDTO? swapped, int max)
        {
            if (swapped == null || original.Order == max)
                return JsonOK();

            Context.Attach(original);
            Context.Attach(swapped);

            original.Order = original.Order + 1;
            swapped.Order = swapped.Order - 1;

            await Context.SaveChangesAsync();
            return JsonOK();
        }
    }
}