namespace Pact.Web.Interfaces
{
    /// <summary>
    /// Ease-of access interface on a page model for adding alert messages (in the 4 standard Bootstrap formats).
    /// The expectation is that these will be held in TempData via the [TempData] attribute for use in the layout following a redirect
    /// </summary>
    public interface IAlertsModel
    {
        /// <summary>
        /// An error message
        /// </summary>
        string Error { get; set; }

        /// <summary>
        /// An informative message
        /// </summary>
        string Info { get; set; }

        /// <summary>
        /// A success message
        /// </summary>
        string Success { get; set; }

        /// <summary>
        /// A warning message
        /// </summary>
        string Warning { get; set; }
    }
}
