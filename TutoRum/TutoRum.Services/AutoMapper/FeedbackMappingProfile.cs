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
    public class FeedbackMappingProfile : Profile
    {
        public FeedbackMappingProfile()
        {
            
            CreateMap<Feedback, FeedbackDto>().ReverseMap();

           
            CreateMap<CreateFeedbackDto, Feedback>();
        }
    }
}