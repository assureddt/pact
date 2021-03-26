namespace Pact.Web.Vue.Grid.Interfaces
{
    public interface IMoveableDatabaseObject: IDatabaseObject
    {
        public int Order { get; set; }
    }
}
