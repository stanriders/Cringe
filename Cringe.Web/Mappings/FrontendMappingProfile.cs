using AutoMapper;
using Cringe.Types.Database;
using Cringe.Web.Pages.Players;

namespace Cringe.Web.Mappings
{
    public class FrontendMappingProfile : Profile
    {
        public FrontendMappingProfile()
        {
            CreateMap<PlayerEditModel, Player>().ReverseMap();
        }
    }
}
