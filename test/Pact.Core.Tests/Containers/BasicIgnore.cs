using Pact.Core.Models;

namespace Pact.Core.Tests.Containers;

public class BasicIgnore
{
    public int Id { get; set; }
    public string Name { get; set; }

    [IgnoreFilter]
    public string Ignore { get; set; }
}