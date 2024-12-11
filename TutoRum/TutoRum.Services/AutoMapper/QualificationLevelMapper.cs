using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.AutoMapper
{
    public class  QualificationLevelMapper : Profile
    {
        public QualificationLevelMapper()
        {
            CreateMap<QualificationLevel, QualificationLevelDto>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level ?? "N/A")); 
        }
    }
}