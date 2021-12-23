using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Pact.Web.Extensions;

public static class ControllerExtensions
{
    /// <summary>
    /// Return a uniformed json error
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static JsonResult JsonError(this Controller controller, Exception ex, ILogger logger = null)
    {
        ex ??= new ArgumentNullException(nameof(ex));

        logger?.LogError(ex, ex.Message);
        return controller.Json(new { Result = "ERROR", ex.Message });
    }

    /// <summary>
    /// Return a uniformed view error
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ViewResult ViewError(this Controller controller, Exception ex, ILogger logger = null)
    {
        ex ??= new ArgumentNullException(nameof(ex));

        logger?.LogError(ex, ex.Message);
        return controller.View("Error");
    }

    /// <summary>
    /// Return a uniformed view error with error passed to view
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ViewResult ViewErrorMessage(this Controller controller, Exception ex, ILogger logger = null)
    {
        ex ??= new ArgumentNullException(nameof(ex));

        logger?.LogError(ex, ex.Message);
        return controller.View("ErrorMessage", ex);
    }

    /// <summary>
    /// Return a uniformed partial view error
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static PartialViewResult PartialViewErrorModal(this Controller controller, Exception ex, ILogger logger = null)
    {
        ex ??= new ArgumentNullException(nameof(ex));

        logger?.LogError(ex, ex.Message);
        return controller.PartialView("ErrorModal");
    }

    /// <summary>
    /// Returns a generic file stream error
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static FileStreamResult FileStreamError(this Controller controller, Exception ex, ILogger logger = null)
    {
        ex ??= new ArgumentNullException(nameof(ex));

        logger?.LogError(ex, ex.Message);
        return null;
    }

    /// <summary>
    /// Returns a geenric content error
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static FileContentResult FileContentError(this Controller controller, Exception ex, ILogger logger = null)
    {
        ex ??= new ArgumentNullException(nameof(ex));

        logger?.LogError(ex, ex.Message);
        return null;
    }

    /// <summary>
    /// Returns a generic satus code error
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static StatusCodeResult HttpStatusCodeError(this Controller controller, Exception ex, ILogger logger = null)
    {
        ex ??= new ArgumentNullException(nameof(ex));

        logger?.LogError(ex, ex.Message);
        return controller.StatusCode((int)HttpStatusCode.InternalServerError);
    }
}