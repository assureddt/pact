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
When registered in the pipeline, these enrich the logging context with information from web requests.
In Startup.cs ConfigureServices:
```c#
services.AddControllers(options => options.AddLogEnrichmentFilters());
```

## [Content Security Policy](./Extensions/SecurityHeaderExtensions.cs)
Provides some default enablements of CSP & Feature Policy headers for web requests. 
In Startup.cs Configure:
```c#
app.UseCspWithPactDefaults();
```
The Pact defaults are notably configured with Script Nonces enabled (which overrides the Inline Scripts enablement if the browser supports it, hence both applied).
Internally, we're using the [NetEscapades.AspNetCore.SecurityHeaders](https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaders) library.

**If enabling Script Nonces, wou will need to follow the instructions there to enable the `asp-add-nonce` Tag Helper and decorate your script declarations accordingly.**
1. Add a package reference to NetEscapades.AspNetCore.SecurityHeaders.TagHelpers to your web project.
2. Add: `@addTagHelper *, NetEscapades.AspNetCore.SecurityHeaders.TagHelpers` to your `_ViewImports.cshtml`
3. Add the `asp-add-nonce` tag helper attribute to *all* of your script blocks. e.g. `<script type="text/javascript" asp-add-nonce src="@Url.Content("~/js/global.js")" asp-append-version="true"></script>`

Recommended reading:
* [Andrew Lock (NetEscapades)](https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaders/blob/master/README.md)
* [Troy Hunt (HIBP)](https://www.troyhunt.com/locking-down-your-website-scripts-with-csp-hashes-nonces-and-report-uri/)

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Web-Index)
