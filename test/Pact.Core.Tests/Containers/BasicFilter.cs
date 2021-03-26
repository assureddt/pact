using Pact.Core.Models;

namespace Pact.Core.Tests.Containers
{
    public class BasicFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Filter]
        public string Filter { get; set; }
    }
}
