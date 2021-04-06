# Pact.Web.ErrorHandling 💀
Intended to simplify graceful exception handling in ASP.NET Core+ web applications (requires Razor Pages to be enabled).

By default, the handler is configured to return all Json responses as HTTP 200 with a bespoke expected body format.
However, the behaviour of the JsonResult responses can be configured via ErrorHandlerSettings, as per the example below.
The example would return an appropriate status code (instead of the default HTTP 200 result).

In Startup.cs ConfigureServices:
```c#
services.AddRazorPages();
// optional overrides
services.Configure<ErrorHandlerSettings>(opts => {
    opts.JsonErrorsAsSuccess = false;
    opts.JsonResponseFormatter = model => new {message = model.Details};
});
```

A pre-configured alternative for the above preference is provided by `ErrorHandlerSettings.WithJsonStatusCodes`.

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
* A Json response is returned if the request does not include `text/html` as a value in the `Accept` header.
* If a [FriendlyException](../Pact.Core/FriendlyException.cs) is being handled, the contained content will be included under the error description.
* Consuming applications can also supply a partial view named "_ErrorSub" which can include custom behaviour (info/links etc.) beneath the error content.

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Web-ErrorHandling-Index)
