namespace Pact.Web.ErrorHandling.Interfaces;

public interface IErrorDetails
{
    public int? Code { get; }
    public string Details { get; }
}