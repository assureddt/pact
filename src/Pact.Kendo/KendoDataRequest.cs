using System.Collections.Generic;

namespace Pact.Kendo;

/// <summary>
/// A class used to support kendo server side requests.
/// This is in the format kendo ui sends over json
/// </summary>
public class KendoDataRequest
{
    /// <summary>
    /// Number of items to take if pagination is turn on
    /// </summary>
    public int Take { get; set; }

    /// <summary>
    /// Number of items to skip if pagination is turn on
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    /// Current page if pagination is turn on
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The size of the page if pagination is turn on
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The information to sort on if server side sorting is enabled
    /// </summary>
    public List<KendoDataRequestSort> Sort { get; set; }

    /// <summary>
    /// If this has a value the data source should be filtered server side on this value
    /// </summary>
    public string TextFilter { get; set; }
}

/// <summary>
/// A class used to support kendo server side requests.
/// This is in the format kendo ui sends over json
/// </summary>
public class KendoDataRequestSort
{
    /// <summary>
    /// The field to sort on.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// The direction to sort oun
    /// </summary>
    public string Dir { get; set; }

    public override string ToString()
    {
        return Field + " " + Dir.ToUpper();
    }
}