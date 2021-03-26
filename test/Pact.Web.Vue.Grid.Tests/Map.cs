using AutoMapper;
using Pact.Web.Vue.Grid.Tests.Containers;

namespace Pact.Web.Vue.Grid.Tests
{
    public class Map : Profile
    {
        public Map()
        {
            CreateMap<BasicDatabaseObject, GridRowOutput>();
            CreateMap<BasicDatabaseObject, EditOutput>().ReverseMap();

            CreateMap<OrderDatabaseObject, GridRowOutput>();
            CreateMap<OrderDatabaseObject, EditOutput>().ReverseMap();

            CreateMap<SoftDeleteDatabaseObject, GridRowOutput>();
            CreateMap<SoftDeleteDatabaseObject, EditOutput>().ReverseMap();
        }
    }
}
