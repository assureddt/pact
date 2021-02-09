# Pact.Web.ErrorHandling 💀
Intended to simplify graceful exception handling in ASP.NET Core+ web applications (requires Razor Pages to be enabled).

The behaviour of the JsonResult responses can be configured via ErrorHandlerSettings, as per the example below.
The example would return a HTTP 200 result (instead of the appropriate status code) with the exception message and the specified formatting of the response body.

In Startup.cs ConfigureServices:
```c#
services.AddRazorPages();
// optional overrides
services.Configure<ErrorHandlerSettings>(opts => {
    opts.AjaxErrorsAsSuccess = false;
    opts.JsonResponseFormatter = model => new {message = model.Details};
});
```

In Startup.cs Configure:
```c#
app.UsePactErrorHandling();
app.UseEndpoints(endpoints => endpoints.MapRazorPages());
```

Then it just needs a Layout defined in the Pages/Error/_ViewStart.cshtml path of the web application:
```c#
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
}
```

The model used in the error page implements [IModel](../Pact.Web/Interfaces/IModel.cs), which you could adopt for your base model & on the layout,
or just inject any dependencies you need directly on your layout.

* The error page currently has content switches for: 400; 401; 403; 404; with anything else described as "Unexpected".
* A Json response is returned if the request is an Ajax call.
* If a [FriendlyException](../Pact.Core/FriendlyException.cs) is being handled, the contained content will be included under the error description.
* Consuming applications can also supply a partial view named "_ErrorSub" which can include custom behaviour (info/links etc.) beneath the error content.

May need some tweaking from a Web API perspective, but is tried-and-tested in typical web app scenarios.

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Web-ErrorHandling-Index)
