using System.Collections.Generic;

namespace Pact.Kendo
{
    /// <summary>
    /// The result object from kendo server side actions
    /// This is in the format expected by keno ui when serialized to json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KendoResult<T> where T : class
    {
        /// <summary>
        /// String response to the server process 
        /// </summary>
        public string Result { get; set; } = "OK";

        /// <summary>
        /// The resulting records
        /// </summary>
        public List<T> Records { get; set; }

        /// <summary>
        /// The total number of records server side before filtering or pagination
        /// </summary>
        public int Count { get; set; }
    }
}
