using AutoMapper;
using MovieTickets.Models;
using MovieTickets.ViewModels;

namespace MovieTickets.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserProfileVM>()
                    .ForMember(dest => dest.OldPassword, src => src.MapFrom(src => src.PasswordHash));

        }
    }
}
