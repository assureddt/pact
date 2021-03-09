namespace Pact.Core
{
    public static class JsonSerialization
    {
        /// <summary>
        /// Pure static switch between Json serializers for internal Pact behaviour as a short-term fallback
        /// to Newtonsoft while we address some lacking areas (e.g. polymorphism)
        /// </summary>
        /// <remarks>
        /// Very conscious that this is untidy (a global static flag and still having the newtonsoft ref if you aren't using it), but:
        /// a) don't really want to be specifying which implementation to use on every method call (and that's all we have as an alternative for extension methods)
        /// b) you really won't want to be mixing both implementations in the same code-base (let alone the same app)
        /// c) the dumber we make it, the less likely you'll want to stick with it!
        /// </remarks>
        /// <example>
        /// <code>
        /// JsonSerialization.Serializer = JsonImplementation.Newtonsoft;
        /// </code>
        /// </example>
        public static JsonImplementation Serializer { get; set; } = JsonImplementation.Microsoft;
    }

    public enum JsonImplementation
    {
        Microsoft,
        Newtonsoft
    }
}
