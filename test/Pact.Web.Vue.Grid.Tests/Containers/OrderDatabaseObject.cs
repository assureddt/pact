using Pact.Web.Vue.Grid.Interfaces;

namespace Pact.Web.Vue.Grid.Tests.Containers;

public class OrderDatabaseObject : IMoveable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
}