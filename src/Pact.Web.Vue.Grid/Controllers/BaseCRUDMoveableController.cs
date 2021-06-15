using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace Pact.Web.Vue.Grid.Controllers
{
    /// <summary>
    /// Basic CRUD Controller Setup
    /// </summary>
    /// <typeparam name="TMoveableDatabaseDTO">Type of moveable database object</typeparam>
    /// <typeparam name="TGridRowDTO">Type of grid item</typeparam>
    /// <typeparam name="TEditDTO">Type of edit item</typeparam>
    public abstract class BaseCRUDMoveableController<TMoveableDatabaseDTO, TGridRowDTO, TEditDTO> : BaseCRUDController<TMoveableDatabaseDTO, TGridRowDTO, TEditDTO>
        where TMoveableDatabaseDTO : class, IMoveable
        where TGridRowDTO : class, IGridRow, new()
        where TEditDTO : class, IEdit, new()
    {
        protected BaseCRUDMoveableController(DbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        /// <summary>
        /// Updates numeric order on list of items
        /// </summary>
        /// <param name="items">list of items to correct</param>
        /// <returns></returns>
        protected async Task ProcessReordering(List<TMoveableDatabaseDTO> items)
        {
            var order = 0;
            foreach (var item in items.OrderBy(x => x.Order).ToList())
            {
                Context.Attach(item);
                item.Order = order;
                order++;
            }
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Moves item up
        /// </summary>
        /// <param name="original">item being moved up</param>
        /// <param name="swapped">item being moved down</param>
        /// <returns></returns>
        protected async Task<JsonResult> ProcessMoveUp(TMoveableDatabaseDTO original, TMoveableDatabaseDTO? swapped)
        {
            if (original.Order == 0 || swapped == null)
                return JsonOK();

            Context.Attach(original);
            Context.Attach(swapped);

            original.Order = original.Order - 1;
            swapped.Order = swapped.Order + 1;

            await Context.SaveChangesAsync();

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }

        /// <summary>
        /// Moves item down
        /// </summary>
        /// <param name="original">item being moved down</param>
        /// <param name="swapped">item being moved up</param>
        /// <param name="max">max items in this list</param>
        /// <returns></returns>
        protected async Task<JsonResult> ProcessMoveDown(TMoveableDatabaseDTO original, TMoveableDatabaseDTO? swapped, int max)
        {
            if (swapped == null || original.Order == max)
                return JsonOK();

            Context.Attach(original);
            Context.Attach(swapped);

            original.Order = original.Order + 1;
            swapped.Order = swapped.Order - 1;

            await Context.SaveChangesAsync();

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }
    }
}