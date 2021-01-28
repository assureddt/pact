# Pact.Kendo 🤺
Provides a collection of general extension methods, helpers and business objects for use with Kendo UI.

The general idea is you have a data collection which is either IEnumerable<T> or IQueryable<T> which you can then call one of the extension methods on.


An example for using this from a controller:
## [KendoResultAsync](./QueryableExtensions.cs)
```c#
public async Task<JsonResult> GetClients(KendoDataRequest r)
{
    try
    {
        return await Context.Clients.KendoResultAsync(r);
    }
    catch (Exception exc)
    {
        return JsonError(exc);
    }
}
```

For all of the extension methods, The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Kendo-Index)