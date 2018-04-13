using AutoMapper;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Dtos;

namespace mars_marking_svc.Utils
{
    public class AutoMapperProfilesUtil : Profile
    {
        public AutoMapperProfilesUtil()
        {
            CreateMap<MarkSessionModel, MarkSessionForReturnDto>()
                .ForMember(destination => destination.MarkSessionId,
                    option => { option.MapFrom(src => src.Id.ToString()); }
                ).ForMember(destination => destination.DependantResources,
                    option => { option.MapFrom(src => src.DependantResources); }
                );
        }
    }
}