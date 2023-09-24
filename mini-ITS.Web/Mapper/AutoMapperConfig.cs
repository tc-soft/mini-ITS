using AutoMapper;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Web.Mapper
{
    public static class AutoMapperConfig
    {
        public static IMapper GetMapper() => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Users, UsersDto>().ReverseMap();
                cfg.CreateMap<Groups, GroupsDto>().ReverseMap();
                cfg.CreateMap<EnrollmentsDescription, EnrollmentsDescriptionDto>().ReverseMap();
                cfg.CreateMap<EnrollmentsPicture, EnrollmentsPictureDto>().ReverseMap();
            }
        ).CreateMapper();
    }
}