using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Pact.Core.Extensions;

public static class MemberInfoExtensions
{
    /// <summary>
    /// Gets an attribute from member info
    /// </summary>
    /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
    /// <param name="memInfo">The member info</param>
    /// <returns>The attribute of type T that exists on the enum value, null if not present</returns>
    public static T GetAttributeOfType<T>(this MemberInfo memInfo) where T : Attribute
    {
        var attributes = memInfo?.GetCustomAttributes(typeof(T), false);

        return (T)attributes?.FirstOrDefault();
    }

    /// <summary>
    /// Gets Name attribute metadata for member info, falling back to some alternatives if absent
    /// </summary>
    /// <param name="memInfo">The member info</param>
    /// <returns></returns>
    public static string GetName(this MemberInfo memInfo)
    {
        return memInfo?.GetAttributeOfType<DisplayAttribute>()?.Name ??
               memInfo?.GetAttributeOfType<DisplayAttribute>()?.Description ??
               memInfo?.GetAttributeOfType<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    /// Gets Description attribute metadata for member info, falling back to some alternatives if absent
    /// </summary>
    /// <param name="memInfo">The member info</param>
    /// <returns></returns>
    public static string GetDescription(this MemberInfo memInfo)
    {
        return memInfo?.GetAttributeOfType<DisplayAttribute>()?.Description ??
               memInfo?.GetAttributeOfType<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    /// Gets Order attribute metadata for member info
    /// </summary>
    /// <param name="memInfo">The member info</param>
    /// <returns></returns>
    public static int? GetOrder(this MemberInfo memInfo)
    {
        return memInfo?.GetAttributeOfType<DisplayAttribute>()?.Order;
    }
}