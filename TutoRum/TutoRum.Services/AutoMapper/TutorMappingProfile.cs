using AutoMapper;
using TutoRum.Data.Models;
using TutoRum.Services.ViewModels;

public class TutorMappingProfile : Profile
{
    public TutorMappingProfile()   
    {
        CreateMap<Tutor, TutorDto>()
            .ForMember(dest => dest.Certificates,
                opt => opt.MapFrom(src => src.Certificates.Select(c => new CertificateDTO
                {
                    CertificateId = c.CertificateId,
                    ImgUrl = c.ImgUrl,
                    Description = c.Description,
                    IssueDate = c.IssueDate,
                    ExpiryDate = c.ExpiryDate,
                    IsVerified = c.IsVerified,
                    EntityType = c.EntityType,
                    Status = c.Status

                })))
            .ForMember(dest => dest.TutorSubjects,
                opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => new TutorSubjectDto
                {
                    TutorSubjectId = ts.TutorSubjectId,
                    TutorId = ts.TutorId,
                    Rate = ts.Rate,
                    Description = ts.Description,
                    IsVerified = ts.IsVerified,
                    EntityType = ts.EntityType,
                    SubjectType = ts.SubjectType,
                    Status = ts.Status,
                    RateRangeId = ts.RateRangeId,
                    Subject = ts.Subject != null ? new SubjectDTO
                    {
                        SubjectId = ts.Subject.SubjectId,
                        SubjectName = ts.Subject.SubjectName
                    } : null
                })))
            .ForMember(dest => dest.Schedules,
                opt => opt.MapFrom(src => src.Schedules
                    .Where(s => s.TutorLearnerSubjectId == null)
                    .GroupBy(s => s.DayOfWeek)
                    .Select(group => new ScheduleDTO
                    {
                        DayOfWeek = group.Key,
                        FreeTimes = group.Select(g => new FreeTimeDTO
                        {
                            StartTime = g.StartTime,
                            EndTime = g.EndTime
                        }).ToList()
                    }).ToList()))
            .ForMember(dest => dest.TeachingLocations,
                opt => opt.MapFrom(src => src.TutorTeachingLocations
                    .GroupBy(tl => tl.TeachingLocation.CityId)
                    .Select(group => new TeachingLocationViewDTO
                    {
                        CityId = group.Key,
                        Districts = group.Select(tl => new DistrictDTO
                        {
                            TeachingLocationId = tl.TeachingLocationId,
                            DistrictId = tl.TeachingLocation.DistrictId
                        }).Distinct().ToList()
                    }).ToList()))
             .ForMember(dest => dest.Experience,
            opt => opt.MapFrom(src => src.Experience))
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.TutorNavigation.Fullname))
            .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.TutorNavigation.AvatarUrl))
            .ForMember(dest => dest.AddressID,
                opt => opt.MapFrom(src => src.TutorNavigation.AddressId))
            .ForMember(dest => dest.AddressDetail,
                opt => opt.MapFrom(src => src.TutorNavigation.AddressDetail))
            .ForMember(dest => dest.videoUrl,
                opt => opt.MapFrom(src => src.videoUrl))
            .ForMember(dest => dest.EducationalLevelID,
                opt => opt.MapFrom(src => src.EducationalLevel))
            .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
            .ForMember(dest => dest.IsAccepted,
                opt => opt.MapFrom(src => src.IsAccepted))
            .ForMember(dest => dest.IsVerified,
                opt => opt.MapFrom(src => src.IsVerified))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status));

        CreateMap<Tutor, TutorSummaryDto>()
           .ForMember(dest => dest.Certificates,
               opt => opt.MapFrom(src => src.Certificates.Select(c => new CertificateDTO
               {
                   CertificateId = c.CertificateId,
                   ImgUrl = c.ImgUrl,
                   Description = c.Description,
                   IssueDate = c.IssueDate,
                   ExpiryDate = c.ExpiryDate,
               })))
           .ForMember(dest => dest.TutorSubjects,
               opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => new TutorSubjectDto
               {
                   TutorSubjectId = ts.TutorSubjectId,
                   TutorId = ts.TutorId,
                   SubjectId = ts.SubjectId,
                   Rate = ts.Rate,
                   Description = ts.Description,
                   Subject = ts.Subject != null ? new SubjectDTO
                   {
                       SubjectId = ts.Subject.SubjectId,
                       SubjectName = ts.Subject.SubjectName
                   } : null
               })))
           .ForMember(dest => dest.Schedules,
               opt => opt.MapFrom(src => src.Schedules
                   .GroupBy(s => s.DayOfWeek)
                   .Select(group => new ScheduleDTO
                   {
                       DayOfWeek = group.Key,
                       FreeTimes = group.Select(g => new FreeTimeDTO
                       {
                           StartTime = g.StartTime,
                           EndTime = g.EndTime
                       }).ToList()
                   }).ToList()))
           .ForMember(dest => dest.TeachingLocations,
               opt => opt.MapFrom(src => src.TutorTeachingLocations
                   .GroupBy(tl => tl.TeachingLocation.CityId)
                   .Select(group => new TeachingLocationViewDTO
                   {
                       CityId = group.Key,
                       Districts = group.Select(tl => new DistrictDTO
                       {
                           DistrictId = tl.TeachingLocation.DistrictId
                       }).Distinct().ToList()
                   }).ToList()))

           .ForMember(dest => dest.Experience,
            opt => opt.MapFrom(src => src.Experience))
           .ForMember(dest => dest.FullName,
               opt => opt.MapFrom(src => src.TutorNavigation.Fullname))
           .ForMember(dest => dest.AvatarUrl,
               opt => opt.MapFrom(src => src.TutorNavigation.AvatarUrl))
           .ForMember(dest => dest.AddressId,
               opt => opt.MapFrom(src => src.TutorNavigation.AddressId));
                

        CreateMap<Tutor, AdminTutorDto>()
            .ForMember(dest => dest.Certificates,
                opt => opt.MapFrom(src => src.Certificates.Select(c => new CertificateDTO
                {
                    CertificateId = c.CertificateId,
                    ImgUrl = c.ImgUrl,
                    Description = c.Description
                })))
            .ForMember(dest => dest.TutorSubjects,
                opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => new TutorSubjectDto
                {
                    TutorSubjectId = ts.TutorSubjectId,
                    TutorId = ts.TutorId,
                    SubjectId = ts.SubjectId,
                    Rate = ts.Rate,
                    Description = ts.Description,
                    Subject = ts.Subject != null ? new SubjectDTO
                    {
                        SubjectId = ts.Subject.SubjectId,
                        SubjectName = ts.Subject.SubjectName
                    } : null
                })))
            .ForMember(dest => dest.Schedules,
                opt => opt.MapFrom(src => src.Schedules
                    .GroupBy(s => s.DayOfWeek)
                    .Select(group => new ScheduleDTO
                    {
                        DayOfWeek = group.Key,
                        FreeTimes = group.Select(g => new FreeTimeDTO
                        {
                            StartTime = g.StartTime,
                            EndTime = g.EndTime
                        }).ToList()
                    }).ToList()))
            .ForMember(dest => dest.TeachingLocations,
                opt => opt.MapFrom(src => src.TutorTeachingLocations
                    .GroupBy(tl => tl.TeachingLocation.CityId)
                    .Select(group => new TeachingLocationViewDTO
                    {
                        CityId = group.Key,
                        Districts = group.Select(tl => new DistrictDTO
                        {
                            DistrictId = tl.TeachingLocation.DistrictId
                        }).Distinct().ToList()
                    }).ToList()))
             .ForMember(dest => dest.Experience,
            opt => opt.MapFrom(src => src.Experience))
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.TutorNavigation.Fullname))
            .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.TutorNavigation.AvatarUrl))
            .ForMember(dest => dest.AddressId,
                opt => opt.MapFrom(src => src.TutorNavigation.AddressId))
             .ForMember(dest => dest.CreatedDate,
                opt => opt.MapFrom(src => src.CreatedDate));

        CreateMap<TutorTeachingLocations, TutorTeachingLocationDto>();
        CreateMap<Tutor, TutorsDTO>();
       CreateMap<Tutor, TutorRatingDto>()
            .ForMember(dest => dest.Feedbacks, opt => opt.MapFrom(src => 
                src.TutorSubjects
                    .SelectMany(ts => ts.TutorLearnerSubjects)
                    .SelectMany(tls => tls.Feedbacks)
            ));
    }
}
