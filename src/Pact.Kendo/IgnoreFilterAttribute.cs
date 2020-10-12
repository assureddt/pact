using System;

namespace Pact.Kendo
{
    /// <summary>
    /// If present on a property we ignore 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreFilterAttribute : Attribute
    {
    }
}
