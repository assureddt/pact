# Pact.Core 📦
Provides a collection of general purpose classes and extension methods.

Some examples follow:
## [GenericEqualityComparer](./GenericEqualityComparer.cs)
```c#
var comp = new GenericEqualityComparer<Foo>(f => f.SortOrder);
```

## [FriendlyException](./FriendlyException.cs)
In most circumstances you may not want an end-user to see the details of an internal exception. This exception type can be used in via a type-check to control when you do want to reveal that information.
This is used internally by [Pact.Web.ErrorHandling](../Pact.Web.ErrorHandling) to decorate the resulting error page with the provided content, but the same principle can be applied elsewhere.
Generally, it's not advisable to use exceptions for regular flow-control, but this provides an easy means of bubbling up useful information to the UI in circumstances where it is beneficial.

In Startup.cs ConfigureServices:
```c#
services.UsePactErrorHandling();
```

Then anywhere in your application code:
```c#
throw new FriendlyException("I may be an internal error, but I really want you to know something!");
```

For all of the extension methods, The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Core-Index)
