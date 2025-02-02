using AutoMapper;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Dashboard;

namespace SciencesTechnology.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                       .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                       .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                       .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                       .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                       .ForMember(dest => dest.DateOfRegistration, opt => opt.MapFrom(src => src.DateOfRegistration))
                       .ForMember(dest => dest.LastUpdatedDate, opt => opt.MapFrom(src => src.LastUpdatedDate))
                       .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                       .ForMember(dest => dest.ProfileImagePath, opt => opt.MapFrom(src => src.ProfileImagePath))
                       .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Credits))
                       .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                       .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                       .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        }
    }
}

