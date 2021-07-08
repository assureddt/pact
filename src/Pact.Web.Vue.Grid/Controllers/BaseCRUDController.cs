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
    /// <summary>
    /// Basic CRUD Controller Setup
    /// </summary>
    /// <typeparam name="TDatabaseDTO">Type of database object</typeparam>
    /// <typeparam name="TGridRowDTO">Type of grid item</typeparam>
    /// <typeparam name="TEditDTO">Type of edit item</typeparam>
    public abstract class BaseCRUDController<TDatabaseDTO, TGridRowDTO, TEditDTO> : Controller
        where TDatabaseDTO : class, IDatabaseObject
        where TGridRowDTO : class, IGridRow, new()
        where TEditDTO : class, IEdit, new()
    {
        protected readonly DbContext Context;
        protected readonly IMapper Mapper;

        protected BaseCRUDController(DbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        /// <summary>
        /// Used to help setup a general post change function
        /// The general idea this is used for a general action post a data set change like clearing caching.
        /// </summary>
        protected Func<Task> PostChangeAction { get; set; }

        /// <summary>
        /// Gets a generic grid data set 
        /// </summary>
        /// <param name="page">page of data to return</param>
        /// <param name="size">size of paged data set</param>
        /// <param name="order">column to order data set by</param>
        /// <param name="direction">order of sort 'ASC' or 'DESC'</param>
        /// <param name="filter">generic text filter applied to source data set</param>
        /// <param name="whereClause">extra sql where clause</param>
        /// <returns></returns>
        protected async Task<JsonResult> GetGridDataSet(int page, int size, string order, int direction, string filter, Expression<Func<TDatabaseDTO, bool>> whereClause = null)
        {
            var query = Context.Set<TDatabaseDTO>().SoftDelete();
            if (whereClause != null)
                query = query.Where(whereClause);
            var items = await query.OrderBy(order + " " + (direction == 0 ? "ASC" : "DESC"))
                .TextFilter(filter)
                .ProjectTo<TGridRowDTO>(Mapper.ConfigurationProvider).ToListAsync();

            return new JsonResult(new GenericGridResult<TGridRowDTO> { Result = "OK", Records = items.Skip(page * size).Take(size).ToList(), Count = items.Count });
        }

        /// <summary>
        /// Get a single items data on id
        /// </summary>
        /// <param name="id">id of data to return</param>
        /// <returns></returns>
        protected async Task<JsonResult> ProcessData(int id)
        {
            var databaseObject = await Context.Set<TDatabaseDTO>().FirstAsync(x => x.Id == id);
            var data = Mapper.Map<TDatabaseDTO, TEditDTO>(databaseObject);
            return new JsonResult(new SingleDataResult<TEditDTO> { Result = "OK", Record = data });
        }

        /// <summary>
        /// Adds model to database
        /// </summary>
        /// <param name="model">item to be created</param>
        /// <param name="modelState">controller model state</param>
        /// <param name="customStep">custom step before data is saved</param>
        /// <returns></returns>
        protected async Task<JsonResult> ProcessAdd(TEditDTO model, ModelStateDictionary modelState, Func<TDatabaseDTO, Task> customStep = null)
        {
            if (!modelState.IsValid) return JsonMessage("Submitted model didn't validate, please check entered values");

            var databaseObject = Mapper.Map<TDatabaseDTO>(model);

            if (customStep != null)
                await customStep(databaseObject);

            await Context.Set<TDatabaseDTO>().AddAsync(databaseObject);
            await Context.SaveChangesAsync();

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }

        /// <summary>
        /// Edits model
        /// </summary>
        /// <param name="model">item to be updated</param>
        /// <param name="modelState">controller model state</param>
        /// <param name="customStep">custom step before data is saved</param>
        /// <returns></returns>
        protected async Task<JsonResult> ProcessEdit(TEditDTO model, ModelStateDictionary modelState, Func<TDatabaseDTO, Task> customStep = null)
        {
            if (!modelState.IsValid) return JsonMessage("Submitted model didn't validate, please check entered values");
            var databaseObject = await Context.Set<TDatabaseDTO>().FirstAsync(x => x.Id == model.Id);
            Context.Attach(databaseObject);
            Mapper.Map(model, databaseObject);

            if (customStep != null)
                await customStep(databaseObject);

            await Context.SaveChangesAsync();

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }

        /// <summary>
        /// Removes item from database
        /// </summary>
        /// <param name="id">id of item to be removed</param>
        /// <param name="customStep">custom step post item being removed</param>
        /// <returns></returns>
        protected async Task<JsonResult> ProcessRemove(int id, Func<TDatabaseDTO, Task> customStep = null)
        {
            var item = await Context.Set<TDatabaseDTO>().FirstAsync(x => x.Id == id);

            if(item is ISoftDelete softDeleteItem)
            {
                Context.Attach(softDeleteItem);
                softDeleteItem.SoftDelete = true;
            }
            else
                Context.Set<TDatabaseDTO>().Remove(item);
            await Context.SaveChangesAsync();

            if (customStep != null)
                await customStep(item);

            if (PostChangeAction != null)
                await PostChangeAction();

            return JsonOK();
        }

        protected JsonResult JsonOK() => Json(new GeneralJsonOK { Result = "OK" });
        protected JsonResult JsonMessage(string message) => Json(new GeneralJsonMessage { Result = "FAIL", Message = message });
    }
}