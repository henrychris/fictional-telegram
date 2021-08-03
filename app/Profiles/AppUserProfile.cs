using System;
using app.Entities;
using app.Data.DTOs;
using AutoMapper;

namespace app.Profiles
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<TelegramUserDto, AppUser>()
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.first_name))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.AuthDate, opt => opt.MapFrom(src => UnixTimeStampToDateTime(src.auth_date)))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.hash));
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}