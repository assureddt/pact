using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Pact.Core.Javascript;

public class JavascriptEnums : IJavascriptEnums
{
    private readonly ILogger<JavascriptEnums> _logger;

    public JavascriptEnums(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<JavascriptEnums>();
    }

    public string Enums(params Type[] enums)
    {
        try {
            var contentParts = enums.Select(x => ConvertEnumToJson(x)).ToList();
            return $"z.enums = {{ {string.Join(",", contentParts)} }};";

        }
        catch(Exception ex)
        {
            _logger.LogCritical(ex.Message, ex);
            return null;
        }
    }

    public string ConvertEnumToJson(Type e, string varName = null)
    {
        try
        {
            varName ??= ToCamelCase(e.Name);

            var ret = varName + ": {";
            foreach (var val in Enum.GetValues(e))
            {
                ret += ToCamelCase(Enum.GetName(e, val)) + ":" + ((int)val) + ",";
            }
            ret += "}";
            return ret;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ConvertEnumToJson {Name}", e.Name);
            return null;
        }  
    }

    private static string ToCamelCase(string s)
    {
        return s.Substring(0, 1).ToLower() + s.Substring(1);
    }
}