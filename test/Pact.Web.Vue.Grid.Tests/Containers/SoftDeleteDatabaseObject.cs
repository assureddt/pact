using Pact.Web.Vue.Grid.Interfaces;

namespace Pact.Web.Vue.Grid.Tests.Containers;

public class SoftDeleteDatabaseObject : ISoftDelete
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool SoftDelete { get; set; }
}