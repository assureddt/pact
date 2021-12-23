namespace Pact.Web.Interfaces;

/// <summary>
/// Formalizes the expectation that your page models will supply a title (to move away from the awkward ViewData["Title"] norm)
/// </summary>
public interface ITitleModel
{
    /// <summary>
    /// A page title for presentation
    /// </summary>
    string Title { get; }
}