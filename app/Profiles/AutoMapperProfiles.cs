using app.Entities;
using app.Data.DTOs;
using AutoMapper;
using System;

namespace app.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<EpumpDataDto, EpumpData>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => Convert.ToInt64(src.user.chatId)))
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.user.companyId))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.user.id))
            .ForMember(dest => dest.AuthKey, opt => opt.MapFrom(src => src.token))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user.email))
            .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}