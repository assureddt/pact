using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pact.Core.Extensions
{
    public static class ObjectExtensions
    {
        private static MemberInfo GetMemberInfo(this object obj) =>
            obj?.GetType().GetMember(obj.ToString()).FirstOrDefault();

        /// <summary>
        /// Gets an attribute on an object
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>The attribute of type T that exists on the enum value, null if not present</returns>
        public static T GetAttributeOfType<T>(this object obj) where T : Attribute => obj.GetMemberInfo()?.GetAttributeOfType<T>();

        /// <summary>
        /// Gets Name attribute metadata for an object, falling back to some alternatives if absent
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetName(this object obj) => obj.GetMemberInfo()?.GetName();

        /// <summary>
        /// Gets Description attribute metadata for an object, falling back to some alternatives if absent
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDescription(this object obj) => obj.GetMemberInfo()?.GetDescription();

        /// <summary>
        /// Gets Order attribute metadata for an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? GetOrder(this object obj) => obj.GetMemberInfo()?.GetOrder();

        /// <summary>
        /// Returns a deep copy of the object
        /// Object must be serializable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", nameof(source));

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
                return default(T);

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
