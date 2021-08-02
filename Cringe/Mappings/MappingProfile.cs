
using AutoMapper;
using Cringe.Types.Database;

namespace Cringe.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ScoreBase, SubmittedScore>().ReverseMap();
            CreateMap<ScoreBase, RecentScore>().ReverseMap();
        }
    }
}
