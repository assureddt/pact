namespace Pact.Web.Vue.Grid.Interfaces;

public interface IMoveable: IDatabaseObject
{
    public int Order { get; set; }
}