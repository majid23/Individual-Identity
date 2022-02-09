using AutoMapper;
using Individual_Identity.Core.Domain;
using Individual_Identity.Services.Models;

namespace Individual_Identity.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
