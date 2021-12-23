namespace Pact.Web.Vue.Grid.Models;

public class SingleDataResult<T> where T : class
{
    public string Result { get; set; }
    public T Record { get; set; }
}