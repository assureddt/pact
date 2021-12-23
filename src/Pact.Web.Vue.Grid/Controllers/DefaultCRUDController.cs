using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pact.Web.Vue.Grid.Interfaces;
using AutoMapper;

namespace Pact.Web.Vue.Grid.Controllers;

/// <summary>
/// Default CRUD Setup
/// </summary>
/// <typeparam name="TDatabaseDTO">Type of database object</typeparam>
/// <typeparam name="TGridRowDTO">Type of grid item</typeparam>
/// <typeparam name="TEditDTO">Type of edit item</typeparam>
public abstract class DefaultCRUDController<TDatabaseDTO, TGridRowDTO, TEditDTO> : BaseCRUDController<TDatabaseDTO, TGridRowDTO, TEditDTO>
    where TDatabaseDTO : class, IDatabaseObject
    where TGridRowDTO : class, IGridRow, new()
    where TEditDTO : class, IEdit, new()
{
    public DefaultCRUDController(DbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public virtual ViewResult Index() => View("Index");

    public virtual async Task<JsonResult> Read(int page, int size, string order, int direction, string filter) => 
        await GetGridDataSet(page, size, order, direction, filter);

    public virtual async Task<JsonResult> Data(int id) => await ProcessData(id);

    [HttpPost]
    public virtual async Task<JsonResult> Add([FromBody] TEditDTO model) => await ProcessAdd(model, ModelState);

    [HttpPost]
    public virtual async Task<JsonResult> Edit([FromBody] TEditDTO model) => await ProcessEdit(model, ModelState);

    public virtual async Task<JsonResult> Remove(int id) => await ProcessRemove(id);
}