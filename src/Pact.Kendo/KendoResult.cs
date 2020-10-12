using System.Collections.Generic;

namespace Pact.Kendo
{
    public class KendoResult<T> where T : class
    {
        public string Result { get; set; }
        public List<T> Records { get; set; }
        public int TotalRecordCount { get; set; }
    }
}
