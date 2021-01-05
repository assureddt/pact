# Pact.Web.ErrorHandling 💀
Intended to simplify graceful exception handling in ASP.NET Core+ web applications (requires Razor Pages).

In Startup.cs ConfigureServices:
```c#
services.AddRazorPages();
```

In Startup.cs Configure:
```c#
app.UsePactErrorHandling();
```

Then it just needs a Layout defined in the Pages/Error/_ViewStart.cshtml path of the web application:
```c#
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
}
```

... and the model used in the layout to implement [IModel](./Interfaces/IModel)

* The error page currently has content switches for: 400; 401; 403; 404; with anything else described as "Unexpected".
* A Json response is returned if the request is an Ajax call.
* If a [FriendlyException](../Pact.Core/FriendlyException.cs) is being handled, the contained content will be included under the error description.
* Consuming applications can also supply a partial view named "_ErrorSub" which can include custom behaviour (info/links etc.) beneath the error content.

May need some tweaking from a Web API perspective, but is tried-and-tested in typical web app scenarios.

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Web-ErrorHandling-Index)
