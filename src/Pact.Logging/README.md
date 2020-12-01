# Pact.Logging 📒
Provides a number of useful extension methods relating to Microsoft.Extensions.Logging.
Some of these are centered around the use of Serilog to provide context enrichmnent of logs, with exclusion of noisy endpoints.
The others mostly offer the ability to log property differences in the state of an object between 2 points.

All can be found in [LoggingExtensions](./LoggingExtensions.cs).

The log enrichment would typically be enabled in the Configure method of Startup.cs:
```c#
app.UseSerilogRequestLoggingWithPactDefaults("/health");
```

An example of differential logging would be:
```c#
var originalProps = object.GetLogPropertyDictionary();
// edit object here
Logger.LogDifference(originalProps, object, this.MethodName());
```

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Logging-Index)
