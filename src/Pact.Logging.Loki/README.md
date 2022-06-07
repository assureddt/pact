# Pact.Logging.Loki 📒
Provides a wrapper extension to the Serilog.Sinks.Grafana.Loki sink.
This facilitates the introduction of a client certificate for authentication with Loki, along with a couple of additional preferred default overrides.

All can be found in [LoggerConfigurationExtensions](./LoggerConfigurationExtensions.cs).

This would typically be used via Serilog configuration in `Program.cs` via:
```c#
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((hostingContext, configuration) =>
                configuration.ReadFrom.Configuration(hostingContext.Configuration))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
```

With the following configuration in `appsettings.json` (notionally):

```json
  "Serilog": {
    "Using": [ "Pact.Logging.Loki" ],
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "GrafanaLokiExtended",
        "Args": {
          "uri": "https://lokiinstanceroot",
          "certificatePath": "/run/auth-certs/tls.crt",
          "certificateKeyPath": "/run/auth-certs/tls.key"
        }
      }
    ],
    "Properties": {
      "app": "remtopic"
    }
  }
```

The 2 certificate paths are the only significant differentiator between this and the wrapped extension.
These are introduced by a HttpClientHandler that's applied to the LokiHttpClient.
Everything else should function in the same manner.

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Logging-Loki-Index)
