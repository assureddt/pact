using System.Collections.Generic;

namespace Pact.Kendo
{
    public class KendoDataRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<KendoDataRequestSort> Sort { get; set; }
        public string TextFilter { get; set; }
    }

    public class KendoDataRequestSort
    {
        public string Field { get; set; }
        public string Dir { get; set; }

        public override string ToString()
        {
            return Field + " " + Dir.ToUpper();
        }
    }
}