# Pact.Impersonation 🎭
Provides a basis for file system access requests via impersonation, with an implementation for Microsoft Windows in [WindowsImpersonator](./WindowsImpersonator.cs).
An action that needs to be executed in an impersonated context is passed in as a parameter to the request, along with the provided impersonation account details (ideally configured via Environment Settings or equivalent).

An example usage of the ExecuteAction method follows:
```c#
_impersonator.ExecuteAction(_settings.ImpersonationSettings, () =>
{
    if (File.Exists(filePath)) data = File.ReadAllBytes(filePath);
});
```

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Impersonation-Index)
