using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Interfaces;
using Pact.Web.Vue.Grid.Models;
using Pact.Core.Extensions;
using Pact.EntityFrameworkCore.Extensions;
using AutoMapper;

namespace Pact.Web.Vue.Grid.Controllers
{
    public abstract class BaseCRUDController<DatabaseDTO, GridRowDTO, EditDTO> : Controller
        where DatabaseDTO : class, IDatabaseObject
        where GridRowDTO : class, IGridRow, new()
        where EditDTO : class, IEdit, new()
    {
        public readonly DbContext Context;
        public readonly IMapper Mapper;

        public BaseCRUDController(DbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        /// <summary>
        /// Used to help setup a general post change function
        /// The general idea this is used for a general action post a dataset change like clearing caching.
        /// </summary>
        public Func<Task> PostChangeAction { get; set; }

        public ViewResult Index() => View("Index");

        public async virtual Task<JsonResult> Read(int page, int size, string order, int direction, string filter) => await GetGridDataSet(page, size, order, direction, filter);

        [NonAction]
        public async Task<JsonResult> GetGridDataSet(int page, int size, string order, int direction, string filter, Expression<Func<DatabaseDTO, bool>> whereClause = null)
        {
            var query = Context.Set<DatabaseDTO>().SoftDelete();
            if (whereClause != null)
                query = query.Where(whereClause);
            var items = await query.OrderBy(order + " " + (direction == 0 ? "ASC" : "DESC"))
                .TextFilter(filter)
                .ProjectTo<GridRowDTO>(Mapper.ConfigurationProvider).ToListAsync();

            return new JsonResult(new GenericGridResult<GridRowDTO> { Result = "OK", Records = items.Skip(page * size).Take(size).ToList(), Count = items.Count });
        }

        public async virtual Task<JsonResult> Data(int id)
        {
            var databaseObject = await Context.Set<DatabaseDTO>().FirstAsync(x => x.Id == id);
            var data = Mapper.Map<DatabaseDTO, EditDTO>(databaseObject);
            return new JsonResult(new SingleDataResult<EditDTO> { Result = "OK", Record = data });
        }

        [HttpPost]
        public async virtual Task<JsonResult> Add([FromBody] EditDTO model) => await ProcessAdd(model, ModelState);

        [NonAction]
        public async Task<JsonResult> ProcessAdd(EditDTO model, ModelStateDictionary modelState, Func<DatabaseDTO, Task> customStep = null)
        {
            if (!modelState.IsValid) return JsonMessage("Submitted model didn't validate, please check entered values");

            var databaseObject = Mapper.Map<DatabaseDTO>(model);

            if (customStep != null)
                await customStep(databaseObject);

            await Context.Set<DatabaseDTO>().AddAsync(databaseObject);
            await Context.SaveChangesAsync();

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }

        [HttpPost]
        public async virtual Task<JsonResult> Edit([FromBody] EditDTO model) => await ProcessEdit(model, ModelState);

        [NonAction]
        public async Task<JsonResult> ProcessEdit(EditDTO model, ModelStateDictionary modelState, Func<DatabaseDTO, Task> customStep = null)
        {
            if (!modelState.IsValid) return JsonMessage("Submitted model didn't validate, please check entered values");
            var databaseObject = await Context.Set<DatabaseDTO>().FirstAsync(x => x.Id == model.Id);
            Context.Attach(databaseObject);
            Mapper.Map(model, databaseObject);

            if (customStep != null)
                await customStep(databaseObject);

            await Context.SaveChangesAsync();

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }

        public async virtual Task<JsonResult> Remove(int id) => await ProcessRemove(id);

        [NonAction]
        public async Task<JsonResult> ProcessRemove(int id, Func<DatabaseDTO, Task> customStep = null)
        {
            var item = await Context.Set<DatabaseDTO>().FirstAsync(x => x.Id == id);

            if(item is ISoftDelete)
            {
                var softDeleteItem = (ISoftDelete)item;
                Context.Attach(softDeleteItem);
                softDeleteItem.SoftDelete = true;
                
            }
            else
                Context.Set<DatabaseDTO>().Remove(item);
            await Context.SaveChangesAsync();

            if (customStep != null)
                await customStep(item);

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }

        public JsonResult JsonOK() => Json(new GeneralJsonOK { Result = "OK" });
        public JsonResult JsonMessage(string message) => Json(new GeneralJsonMessage { Result = "FAIL", Message = message });
    }
}