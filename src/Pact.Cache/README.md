# Pact.Cache 💨
Provides a [DistributedCacheService](Pact.Cache/DistributedCacheService.cs) implementation that extends the default DistributedCache with some advanced functionality.
This implementation remains storage-agnostic but has been successfully used with both Redis & SqlServer storage in high-utililzation production environments.
The primary improvements are the availability of a GetOrCreate method (and Async variant) and built-in exception handling on all calss with a warning log message (making the assumption that cache failures are likely transient/non-critical.

Usage in a DI container is just a matter of registering the provided service against the [interface](Pact.Cache/IDistributedCacheService.cs), along with your choice of storage service, but some defaults are made available for Redis & SqlServer in the provided [AddDistributedCache](Pact.Cache/ServiceCollectionExtensions.cs) extension method.

An example usage of the GetOrCreateAsync method follows:
```c#
return Cache.GetOrCreateAsync("FooCacheKey",
    async obj =>
    {
        obj.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

        return _fooService.GetAllTheThings();
    });
```
The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Cache-Index)
