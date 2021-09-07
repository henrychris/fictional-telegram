using app.Entities;
using app.Data.DTOs;
using AutoMapper;

namespace app.Profiles
{
    public class EpumpDataProfile: Profile
    {
        public EpumpDataProfile()
        {
            CreateMap<EpumpDataDto, EpumpData>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.EpumpId))
            .ForMember(dest => dest.AuthKey, opt => opt.MapFrom(src => src.AuthKey));

            
        }
    }
}