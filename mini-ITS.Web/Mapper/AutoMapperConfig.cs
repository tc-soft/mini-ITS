using System;
using System.Net;
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
            cfg.CreateMap<EnrollmentsDescription, EnrollmentsDescriptionDto>()
                .ForMember(m => m.Description,
                    c => c.MapFrom(s => WebUtility.HtmlDecode(s.Description))
                );
            cfg.CreateMap<EnrollmentsDescriptionDto, EnrollmentsDescription>()
                .ForMember(m => m.DateAddDescription,
                    opt => opt.Condition((src, dest, srcMember) => dest.DateAddDescription == DateTime.MinValue)
                )
                .ForMember(m => m.UserAddDescription,
                    opt => opt.Condition((src, dest, srcMember) => dest.UserAddDescription == Guid.Empty)
                )
                .ForMember(m => m.UserAddDescriptionFullName,
                    opt => opt.Condition((src, dest, srcMember) => string.IsNullOrEmpty(dest.UserAddDescriptionFullName))
                );
            cfg.CreateMap<EnrollmentsPicture, EnrollmentsPictureDto>();
            cfg.CreateMap<EnrollmentsPictureDto, EnrollmentsPicture>()
                .ForMember(m => m.DateAddPicture,
                    opt => opt.Condition((src, dest, srcMember) => dest.DateAddPicture == DateTime.MinValue)
                )
                .ForMember(m => m.UserAddPicture,
                    opt => opt.Condition((src, dest, srcMember) => dest.UserAddPicture == Guid.Empty)
                )
                .ForMember(m => m.UserAddPictureFullName,
                    opt => opt.Condition((src, dest, srcMember) => string.IsNullOrEmpty(dest.UserAddPictureFullName))
                );
            cfg.CreateMap<Enrollments, Enrollments>();
            cfg.CreateMap<Enrollments, EnrollmentsDto>()
                .ForMember(m => m.Description,
                    c => c.MapFrom(s => WebUtility.HtmlDecode(s.Description))
                );
            cfg.CreateMap<EnrollmentsDto, Enrollments>()
                .ForMember(m => m.DateAddEnrollment,
                    opt => opt.Condition((src, dest, srcMember) => dest.DateAddEnrollment == DateTime.MinValue)
                );
        }
        ).CreateMapper();
    }
}