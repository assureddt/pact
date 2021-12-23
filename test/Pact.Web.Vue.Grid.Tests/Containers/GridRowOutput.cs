using Pact.Web.Vue.Grid.Interfaces;

namespace Pact.Web.Vue.Grid.Tests.Containers;

public class GridRowOutput : IGridRow
{
    public int Id { get; set; }
    public string Name { get; set; }
}