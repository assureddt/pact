# Pact.Localization 🌍
Builds on the behaviour of the framework-provided Microsoft.AspNetCore.Localization functionality by introducing context-based decisions into the selection of culture to present to the user.
This involves a replacement RequestLocalizationMiddleware: [DynamicLocalizationMiddleware](./DynamicLocalizationMiddleware.cs), which inserts the dynamic check into the process via an additional "Culture Resolver" service, which needs to implement [ISupportedCulturesResolver](./ISupportedCulturesResolver.cs).
That implementation will typically look up the Identity on the HttpContext to infer specifics about the user's account configuration (which may include languages explicitly made available to them).

DI service extensions for adding the middleware can be found in [ServiceCollectionExtensions](./ServiceCollectionExtensions.cs).

An example implementation of the GetSupportedCulturesAsync method follows:
```c#
public async Task<IList<CultureInfo>> GetSupportedCulturesAsync(HttpContext context)
{
    var groupId = context.GetContextItem<SurveySession>()?.GroupId;

    Logger.LogTrace("Resolving supported cultures for user: {User}", context.User?.Identity?.Name);

    var langs = await GetLanguagesAsync();
    var contextLanguages = langs.Select(x => x.CountryCode.ToLowerInvariant()).ToList();

    if (groupId != null)
    {
        contextLanguages = (await GetGroupLanguagesAsync(groupId.Value)).ToList();

        if (!contextLanguages.Any())
            contextLanguages.Add(Constants.DefaultCulture);
    }

    var prefixes = contextLanguages.Select(x => x.Substring(0, 2)).Distinct().ToList();
    contextLanguages.AddRange(prefixes);

    return contextLanguages.Select(x => new CultureInfo(x)).ToList();
}
```

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Localization-Index)
