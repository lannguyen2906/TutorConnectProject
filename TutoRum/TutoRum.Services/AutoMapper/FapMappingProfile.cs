using AutoMapper;
using TutoRum.Data.Models;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.AutoMapper
{
    public class FapMappingProfile : Profile
    {
        public FapMappingProfile()
        {
            CreateMap<Faq, FaqDto>()
                .ForMember(dest => dest.AdminFullname, opt => opt.MapFrom(src => src.Admin.AspNetUser.Fullname))
                .ForMember(dest => dest.AdminPosition, opt => opt.MapFrom(src => src.Admin.Position));

            CreateMap<FaqCreateDto, Faq>()
          
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) 
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<FaqUpdateDto, Faq>();
        }
    }
}
