using Pact.Web.Vue.Grid.Interfaces;

namespace Pact.Web.Vue.Grid.Tests.Containers
{
    public class BasicDatabaseObject : IDatabaseObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
