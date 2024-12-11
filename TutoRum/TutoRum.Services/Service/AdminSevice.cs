using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class AdminSevice : IAdminSevice
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IScheduleService _scheduleService;
        private readonly INotificationService _notificationService;



        public AdminSevice(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager, IMapper mapper, IScheduleService scheduleService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _scheduleService = scheduleService;
            _notificationService = notificationService;
        }

        public async Task AssignRoleAdmin(AssignRoleAdminDto dto, ClaimsPrincipal user)
        {
            // Get the current user based on the provided ClaimsPrincipal
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Find the user by their username or email from the DTO (depending on what identifier you are using)
            var targetUser = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (targetUser == null)
            {
                throw new Exception("Target user not found.");
            }

            // Check if the user is already an admin
            if (await _userManager.IsInRoleAsync(targetUser, AccountRoles.Admin))
            {
                throw new Exception("User is already an admin.");
            }
            else
            {
                // Retrieve the current roles of the user
                var currentRoles = await _userManager.GetRolesAsync(targetUser);

                // Remove the user from all current roles
                var removeResult = await _userManager.RemoveFromRolesAsync(targetUser, currentRoles);
                if (!removeResult.Succeeded)
                {
                    throw new Exception("Failed to remove user's current roles.");
                }

                // Add the user to the "Admin" role
                var result = await _userManager.AddToRoleAsync(targetUser, AccountRoles.Admin);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to add user to the Admin role.");
                }
            }

            // Optionally, update the `Admin` entity if your domain model requires
            var admin = new Admin
            {
                AdminId = targetUser.Id,
                Position = dto.Position,
                HireDate = DateTime.UtcNow,
                Salary = dto.Salary,
                SupervisorId = dto.SupervisorId
            };

            _unitOfWork.Admins.Add(admin);
            await _unitOfWork.CommitAsync();
        }


        public async Task SetVerificationStatusAsync(EntityTypeName entityType, Guid guidId, int id, bool isVerified, string reason, ClaimsPrincipal user)
        {

            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }


            switch (entityType)
            {
                case EntityTypeName.Tutor:
                    var tutor = _unitOfWork.Tutors.GetSingleByGuId(guidId);
                    if (tutor == null)
                    {
                        throw new Exception("Tutor not found.");
                    }
                    tutor.IsVerified = isVerified;
                    if (isVerified)
                    {
                        tutor.Status = "Đã xác thực";
                    }
                    else
                    {
                        tutor.Status = "Từ chối";
                        tutor.reasonDesc = reason;

                    }
                    _unitOfWork.Tutors.Update(tutor);

                    var notificationDto = new NotificationRequestDto
                    {
                        UserId = guidId,
                        Title = isVerified ? "Hồ sơ gia sư của bạn đã được kiểm duyệt" : "Hồ sơ gia sư của bạn bị từ chối",
                        Description = isVerified ? "Hãy lên trang danh sách gia sư tìm kiếm bạn nhé" : reason,
                        NotificationType = NotificationType.TutorRegistrationResult,
                        Href = isVerified ? "/tutors" : "/user/tutor-profile"
                    };

                    await _notificationService.SendNotificationAsync(notificationDto, false);
                    break;

                case EntityTypeName.TutorSubject:
                    var tutorSubject = _unitOfWork.TutorSubjects.GetSingleById(id);
                    if (tutorSubject == null)
                    {
                        throw new Exception($"Tutor subject with ID {id} not found.");
                    }
                    tutorSubject.IsVerified = isVerified;
                    if (isVerified)
                    {
                        tutorSubject.Status = "Đã xác thực";
                    }
                    else
                    {
                        tutorSubject.Status = "Từ chối";
                        tutorSubject.reasonDesc = reason;
                    }
                    _unitOfWork.TutorSubjects.Update(tutorSubject);
                    break;

                case EntityTypeName.TutorRequest:
                    var tutorRequest = _unitOfWork.TutorRequest.GetSingleById(id);
                    if (tutorRequest == null)
                    {
                        throw new Exception($"Tutor request with ID {id} not found.");
                    }
                    tutorRequest.IsVerified = isVerified;
                    tutorRequest.reasonDesc = reason;
                    if (isVerified)
                    {
                        tutorRequest.Status = "Đã xác thực";
                    }
                    else
                    {
                        tutorRequest.Status = "Từ chối";
                        tutorRequest.reasonDesc = reason;

                    }
                    _unitOfWork.TutorRequest.Update(tutorRequest);
                    break;

                case EntityTypeName.TutorLearnerSubject:
                    // Retrieve TutorLearnerSubject by ID
                    var tutorLearnerSubject = _unitOfWork.TutorLearnerSubject.GetSingleById(id);
                    if (tutorLearnerSubject == null)
                    {
                        throw new Exception($"Tutor learner subject with ID {id} not found.");
                    }
                    // Fetch freetime schedule of the tutor
                    var freetimeOfTutor = _unitOfWork.schedule
                        .GetMultiAsQueryable(s => s.TutorId == currentUser.Id)
                        .Select(s => new UpdateScheduleDTO
                        {
                            DayOfWeek = s.DayOfWeek ?? 0, // Default to 0 if NULL
                            FreeTimes = new List<FreeTimeDTO>
                            {
                                new FreeTimeDTO
                                {
                                    StartTime = s.StartTime ?? TimeSpan.Zero, // Default to TimeSpan.Zero if NULL
                                    EndTime = s.EndTime ?? TimeSpan.Zero      // Default to TimeSpan.Zero if NULL
                                }
                            }
                        })
                        .ToList();

                    // Fetch schedule time registered by learners
                    var timeLearnerRegister = _unitOfWork.schedule
                        .GetMultiAsQueryable(s => s.TutorLearnerSubjectId == tutorLearnerSubject.TutorLearnerSubjectId)
                        .Select(s => new UpdateScheduleDTO
                        {
                            Id = s.ScheduleId,
                            tutorID = s.TutorId,
                            DayOfWeek = s.DayOfWeek,
                            FreeTimes = new List<FreeTimeDTO>
                            {
                                new FreeTimeDTO
                                {
                                    StartTime = s.StartTime,
                                    EndTime = s.EndTime
                                }
                            }
                        })
                        .ToList();

                    var a = await _scheduleService.AdjustTutorFreeTime(freetimeOfTutor, timeLearnerRegister);
                    // Adjust tutor's free time based on learner's registered time
                    if (timeLearnerRegister != null )
                    {
                        // Update the new schedule for the tutor
                        await _scheduleService.UpdateNewSchedule(currentUser.Id, a);

                        // Update `TutorId` for the learner's schedule records
                        foreach (var learnerSchedule in timeLearnerRegister)
                        {
                            // Lấy bản ghi Schedule hiện có từ database
                            var existingSchedule = _unitOfWork.schedule.GetSingleById(learnerSchedule.Id);

                            if (existingSchedule != null)
                            {
                                // Chỉ cập nhật TutorId
                                existingSchedule.TutorId = currentUser.Id;

                                // Gọi Update chỉ để cập nhật TutorId
                                _unitOfWork.schedule.Update(existingSchedule);
                            }
                        }

                        // Commit all changes to the database
                        await _unitOfWork.CommitAsync();
                    }
                    tutorLearnerSubject.IsVerified = isVerified;
                    tutorLearnerSubject.reasonDesc = reason;
                    if (isVerified)
                    {
                        tutorLearnerSubject.Status = "Đã xác thực";
                    }
                    else
                    {
                        tutorLearnerSubject.Status = "Từ chối";
                        tutorLearnerSubject.reasonDesc = reason;

                    }
                    _unitOfWork.TutorLearnerSubject.Update(tutorLearnerSubject);

                    var notificationToLearner = new NotificationRequestDto
                    {
                        UserId = tutorLearnerSubject.LearnerId ?? new Guid(),
                        Title = isVerified ? "Yêu cầu đăng ký môn học đã được xác nhận" : "Yêu cầu đăng ký môn học đã bị từ chối",
                        Description = isVerified ? "Hãy kiểm tra lớp học của bạn nhé!" : reason,
                        NotificationType = NotificationType.TutorLearnerSubjectResult,
                        Href = "/user/learning-classrooms"
                    };

                    await _notificationService.SendNotificationAsync(notificationToLearner, false);

                    if (isVerified)
                    {
                        var notificationToTutor = new NotificationRequestDto
                        {
                            UserId = currentUser.Id,
                            Title = "Đã tạo lớp học mới",
                            Description = "Vui lòng liên lạc với học viên để tạo hợp đồng",
                            NotificationType = NotificationType.TutorLearnerSubjectResult,
                            Href = "/user/teaching-classrooms"
                        };

                        await _notificationService.SendNotificationAsync(notificationToTutor, false);
                    }
                    break;

                case EntityTypeName.Certificate:
                    var certificate = _unitOfWork.Certificates.GetSingleById(id);
                    if (certificate == null)
                    {
                        throw new Exception($"Certificate with ID {id} not found.");
                    }
                    certificate.IsVerified = isVerified;
                    certificate.reasonDesc = reason;
                    if (isVerified)
                    {
                        certificate.Status = "Đã xác thực";
                    }
                    else
                    {
                        certificate.Status = "Từ chối";
                        certificate.reasonDesc = reason;

                    }
                    _unitOfWork.Certificates.Update(certificate);
                    break;

                case EntityTypeName.Contract:
                    var tutorLearnerSubjectContract = _unitOfWork.TutorLearnerSubject.GetSingleById(id);
                    if (tutorLearnerSubjectContract == null)
                    {
                        throw new Exception($"Tutor Learner Subject with ID {id} not found.");
                    }
                    tutorLearnerSubjectContract.IsContractVerified = isVerified;
                    tutorLearnerSubjectContract.ContractNote = reason;
                    _unitOfWork.TutorLearnerSubject.Update(tutorLearnerSubjectContract);
                    break;

                default:
                    throw new ArgumentException("Invalid entity type specified.");
            }

            await _unitOfWork.CommitAsync();
        }


        public async Task<AdminHomePageDTO> GetAllTutors(
            ClaimsPrincipal user,
            FilterDto filterDto,
            int index = 0,
            int size = 20)
        {
            // Kiểm tra quyền Admin
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
            {
                throw new UnauthorizedAccessException("You do not have permission to access this resource.");
            }

            // Filter logic
            int total;
            var tutors = _unitOfWork.Tutors.GetMultiPaging(
                filter: request =>
                    (string.IsNullOrEmpty(filterDto.Search) ||
                        request.TutorNavigation.Fullname.Contains(filterDto.Search) ||
                        request.TutorNavigation.Email.Contains(filterDto.Search)) &&
                    (string.IsNullOrEmpty(filterDto.Status) ||
                        request.Status == filterDto.Status) &&
                    (!filterDto.StartDate.HasValue ||
                        request.CreatedDate >= filterDto.StartDate) &&
                    (!filterDto.EndDate.HasValue ||
                        request.CreatedDate <= filterDto.EndDate),
                total: out total,
                index: index,
                size: size,
                includes: new[] { "TutorNavigation", "TutorTeachingLocations", "Schedules", "TutorTeachingLocations.TeachingLocation" }
            );

            // Kiểm tra kết quả
            if (tutors == null || !tutors.Any())
            {
                throw new KeyNotFoundException("No tutor requests found.");
            }

            // Map kết quả sang DTO
            var tutorsDto = _mapper.Map<List<AdminTutorDto>>(tutors);

            return new AdminHomePageDTO
            {
                Tutors = tutorsDto,
                TotalRecordCount = total
            };
        }


        public async Task<List<AdminMenuAction>> GetAdminMenuActionAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
            {
                throw new UnauthorizedAccessException("You do not have permission to access this resource.");
            }

            var tutorCount = _unitOfWork.Tutors.Count(t => t.IsVerified == null);
            var paymentRequestCount = _unitOfWork.PaymentRequest.Count(p => p.Status == "Pending" && p.VerificationStatus == "Confirmed");
            var tutorRequestCount = _unitOfWork.TutorRequest.Count(t => t.IsVerified == null);
            var contractCount = _unitOfWork.TutorLearnerSubject.Count(t => t.IsContractVerified == null);

            var adminMenuActions = new List<AdminMenuAction>
            {
                new() { Href= "/admin/tutors", NumberOfAction= tutorCount },
                new() { Href= "/admin/payment-requests", NumberOfAction= paymentRequestCount },
                new() { Href= "/admin/tutor-requests", NumberOfAction= tutorRequestCount },
                new() { Href= "/admin/contracts", NumberOfAction= contractCount },

            };

            return adminMenuActions;
        }
    }
}


