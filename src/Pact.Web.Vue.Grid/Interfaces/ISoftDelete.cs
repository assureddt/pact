namespace Pact.Web.Vue.Grid.Interfaces
{
    public interface ISoftDelete : IDatabaseObject
    {
        public bool SoftDelete { get; set; }
    }
}
