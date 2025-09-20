using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_minya.Infrastructure.Entities;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Domain.Entities;

namespace tourism_minya.Infrastructure.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterUserDto, ApplicationUser>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

            CreateMap<TourismType, TourismTypeDTO>();

            CreateMap<TourismTypeDTO, TourismType>();

            CreateMap<Center, CenterDto>().ReverseMap();
        }
    }
}
