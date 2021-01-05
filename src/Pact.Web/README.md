# Pact.Web 🕸
Provides a number of useful extension methods and services for web applications.

Some examples follow:
## [TempDataService](./TempDataService/TempDataService.cs)
Simplifies the storing of objects in TempData
```c#
var svc = provider.GetService<ITempDataService>();
var theThing = new Thing();
svc.Set("ThingKey", theThing);
...
var retrievedThing = svc.Get<Thing>("ThingKey");
```

## [LoggingActionFilter](./Filters/LoggingActionFilter.cs) & [LoggingPageFilter](./Filters/LoggingPageFilter.cs) 
When registered in the pipeline, these enrich the logging context with information from web requests/
In Startup.cs ConfigureServices:
```c#
services.AddControllers(options => options.AddLogEnrichmentFilters());
```

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Web-Index)
