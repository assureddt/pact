using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using Pact.Core.Extensions;
using System.Text.RegularExpressions;

namespace Pact.Web.TempDataService
{
    ///<inheritdoc/>
    public class TempDataService : ITempDataService
    {
        public TempDataService(IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionary)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionary;
        }

        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string UrlCacheString = "TempDataService.URLItem:";

        private ITempDataDictionary GetTempData()
        {
            return _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext);
        }

        ///<inheritdoc/>
        public void Set<T>(string key, T value) where T : class
        {
            GetTempData()[key] = value.ToJson();
        }

        ///<inheritdoc/>
        public T Get<T>(string key) where T : class
        {
            if (!GetTempData().TryGetValue(key, out var value) || value is not string json)
                return default;

            GetTempData().Remove(key);
            return json.FromJson<T>();

        }

        ///<inheritdoc/>
        public string StoreOnKeyToken<T>(string key, T value) where T : class
        {
            var token = "Z" + Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "-");
            var cacheKey = UrlCacheString + key + ":" + token;
            Set(cacheKey, value);
            return token;
        }

        ///<inheritdoc/>
        public T GetFromKeyToken<T>(string key, string token) where T : class
        {
            var cacheKey = UrlCacheString + key + ":" + token;
            return Get<T>(cacheKey);
        }

        ///<inheritdoc/>
        public void Clear()
        {
            GetTempData().Clear();
        }
    }
}
