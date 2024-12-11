using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Data.Repositories;
using TutoRum.Services.Helper;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using ZXing.QrCode.Internal;

namespace TutoRum.Services.Service
{
    public class TutorService : ITutorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IScheduleService _scheduleService;
        private readonly ITeachingLocationsService _TeachingLocationsService;
        private readonly ISubjectService _subjectService;
        private readonly ICertificatesSevice _CertificatesSevice;
        private readonly HttpClient _httpClient;
        private readonly APIAddress _apiAddress;
        private readonly INotificationService _notificationService;


        public TutorService(IUnitOfWork unitOfWork,
            UserManager<AspNetUser> userManager,
            IMapper mapper,
            IScheduleService scheduleService,
            ITeachingLocationsService teachingLocationsService,
            ICertificatesSevice certificatesSevice,
            ISubjectService subjectService,
            HttpClient httpClient,
            APIAddress apiAddress,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _scheduleService = scheduleService;
            _TeachingLocationsService = teachingLocationsService;
            _CertificatesSevice = certificatesSevice;
            _subjectService = subjectService;
            _httpClient = httpClient;
            _apiAddress = apiAddress;
            _notificationService = notificationService;
        }


        public async Task RegisterTutorAsync(AddTutorDTO tutorDto, ClaimsPrincipal user)
        {
            // Lấy thông tin user hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            // Kiểm tra nếu user không tồn tại
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Kiểm tra nếu user đã là gia sư
            if (await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor))
            {
                throw new UnauthorizedAccessException("User is already a tutor!");
            }

            //// Lấy ExecutionStrategy từ UnitOfWork (hoặc từ DbContext nếu UnitOfWork của bạn không có phương thức này)
            //var strategy = _unitOfWork.Tutors.GetExecutionStrategy();

            //// Thực hiện các thao tác trong phạm vi của ExecutionStrategy để có thể thử lại nếu có lỗi tạm thời
            //await strategy.ExecuteAsync(async () =>
            //{
            //    // Bắt đầu transaction
            //    using (var transaction = await _unitOfWork.Tutors.BeginTransactionAsync())
            //    {
            try
            {
                // Thêm vai trò gia sư cho user
                var roleResult = await _userManager.AddToRoleAsync(currentUser, AccountRoles.Tutor);

                if (!roleResult.Succeeded)
                {
                    throw new UnauthorizedAccessException("Failed to assign Tutor role to the user.");
                }

                // Tạo thực thể Tutor từ thông tin DTO
                var newTutor = new Tutor
                {
                    TutorId = currentUser.Id,
                    Experience = tutorDto.Experience,
                    Specialization = tutorDto.Specialization,
                    ProfileDescription = tutorDto.ProfileDescription,
                    BriefIntroduction = tutorDto.BriefIntroduction,
                    EducationalLevel = tutorDto.EducationalLevelID,
                    ShortDescription = tutorDto.ShortDescription,
                    Major = tutorDto.Major,
                    videoUrl = tutorDto.videoUrl,
                    CreatedBy = currentUser.Id,
                    IsVerified = null,
                    CreatedDate = DateTime.UtcNow,
                    IsAccepted = tutorDto.IsAccepted,
                    Status = "Chờ xác thực",
                };

                // Cập nhật địa chỉ cho user
                currentUser.AddressId = tutorDto.AddressID;
                _unitOfWork.Accounts.Update(currentUser);

                // Thêm gia sư mới vào hệ thống
                _unitOfWork.Tutors.Add(newTutor);

                // Thêm các địa điểm dạy học
                await _TeachingLocationsService.AddTeachingLocationsAsync(tutorDto.TeachingLocation, newTutor.TutorId);

                // Thêm chứng chỉ
                await _CertificatesSevice.AddCertificatesAsync(tutorDto.Certificates, newTutor.TutorId);

                // Thêm lịch dạy học
                await _scheduleService.AddSchedulesAsync(tutorDto.Schedule, newTutor.TutorId);

                // Thêm các môn học đã có và giá trị tương ứng
                await _subjectService.AddSubjectsWithRateAsync(tutorDto.Subjects, newTutor.TutorId);

                // Lưu các thay đổi và commit transaction nếu mọi thứ đều thành công
                await _unitOfWork.CommitAsync();
                //await transaction.CommitAsync(); // Commit transaction

                var notificationDto = new NotificationRequestDto
                {
                    UserId = currentUser.Id,
                    Title = "Yêu cầu đăng ký gia sư đã được ghi nhận!",
                    Description = "Yêu cầu sẽ được xét duyệt trong vòng 1 ngày. Chúng tôi sẽ thông báo khi có kết quả.",
                    NotificationType = NotificationType.TutorRegistrationPending,
                    Href = "/user/tutor-profile"
                };

                await _notificationService.SendNotificationAsync(notificationDto, false);

                // Notify admins for pending tutors
                await NotifyAdminForPendingTutors();

            }
            catch (Exception ex)
            {
                // Rollback nếu có lỗi xảy ra
                //await transaction.RollbackAsync();
                throw new Exception("Error during tutor registration process: " + ex.Message, ex);
            }
            //    }
            //});
        }

        public async Task UpdateTutorInfoAsync(UpdateTutorInforDTO tutorDto, ClaimsPrincipal user)
        {
            // Lấy thông tin user hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            // Kiểm tra nếu user không tồn tại
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Kiểm tra nếu user không phải là gia sư
            if (!await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor))
            {
                throw new UnauthorizedAccessException("User is not a tutor!");
            }

            // Lấy ExecutionStrategy từ UnitOfWork (hoặc từ DbContext nếu UnitOfWork của bạn không có phương thức này)
            var strategy = _unitOfWork.Tutors.GetExecutionStrategy();

            // Thực hiện các thao tác trong phạm vi của ExecutionStrategy để có thể thử lại nếu có lỗi tạm thời
            await strategy.ExecuteAsync(async () =>
            {
                // Bắt đầu transaction
                using (var transaction = await _unitOfWork.Tutors.BeginTransactionAsync())
                {

                    // Tìm gia sư trong cơ sở dữ liệu
                    var existingTutor = _unitOfWork.Tutors.GetSingleByGuId(currentUser.Id);

                    // Cập nhật thông tin gia sư từ DTO
                    existingTutor.Experience = tutorDto.Experience;
                    existingTutor.Specialization = tutorDto.Specialization;
                    existingTutor.ProfileDescription = tutorDto.ProfileDescription;
                    existingTutor.BriefIntroduction = tutorDto.BriefIntroduction;
                    existingTutor.EducationalLevel = tutorDto.EducationalLevelID;
                    existingTutor.ShortDescription = tutorDto.ShortDescription;
                    existingTutor.Major = tutorDto.Major;
                    existingTutor.videoUrl = tutorDto.VideoUrl;
                    existingTutor.IsAccepted = tutorDto.IsAccepted;

                    // Cập nhật thông tin địa chỉ của người dùng
                    currentUser.AddressId = tutorDto.AddressID;
                    _unitOfWork.Accounts.Update(currentUser);

                    // Cập nhật thông tin gia sư trong cơ sở dữ liệu
                    _unitOfWork.Tutors.Update(existingTutor);

                    // Thêm các địa điểm dạy học
                    await _TeachingLocationsService.AddTeachingLocationsAsync(tutorDto.TeachingLocation, existingTutor.TutorId);

                    // Thêm chứng chỉ
                    await _CertificatesSevice.AddCertificatesAsync(tutorDto.Certificates, existingTutor.TutorId);


                    // Thêm các môn học đã có và giá trị tương ứng
                    await _subjectService.AddSubjectsWithRateAsync(tutorDto.Subjects, existingTutor.TutorId);


                    // Lưu các thay đổi và commit transaction nếu mọi thứ đều thành công
                    await _unitOfWork.CommitAsync();
                    await transaction.CommitAsync(); // Commit transaction

                    // Gửi thông báo cho gia sư
                    var notificationDto = new NotificationRequestDto
                    {
                        UserId = currentUser.Id,
                        Title = "Thông tin gia sư của bạn đã được cập nhật!",
                        Description = "Thông tin gia sư của bạn đã được cập nhật và đang chờ duyệt.",
                        NotificationType = NotificationType.TutorInfoUpdated,
                        Href = "/user/tutor-profile"
                    };

                    await _notificationService.SendNotificationAsync(notificationDto, false);

                    // Notify admins về gia sư đã được cập nhật
                    await NotifyAdminForUpdatedTutor(existingTutor.TutorId);

                }
            });
        }




        public async Task<TutorDto> GetTutorByIdAsync(Guid id)
        {
            var tutor = await _unitOfWork.Tutors.GetByIdAsync(id);
            if (tutor == null)
            {
                throw new KeyNotFoundException("Tutor not found.");
            }

            var tutorDto = _mapper.Map<TutorDto>(tutor);

            //// Chuyển đổi TeachingLocations
            foreach (var location in tutorDto.TeachingLocations)
            {

                foreach (var district in location.Districts)
                {
                    district.DistrictName = await _apiAddress.GetDistrictNameByIdAsync(district.DistrictId);
                }
                location.CityName = await _apiAddress.GetCityNameByIdAsync(location.CityId);

            }

            tutorDto.AddressDetail = tutorDto.AddressID != null ? await _apiAddress.GetCityNameByIdAsync(tutorDto.AddressID) : "Chưa cập nhật";
            var qualificationLevel = _unitOfWork.QualificationLevel.GetSingleById(tutorDto.EducationalLevelID);
            tutorDto.EducationalLevelName = qualificationLevel.Level;
            return tutorDto;

        }
        public async Task NotifyAdminForPendingTutors()
        {
            // Đếm số lượng gia sư chờ phê duyệt
            var pendingCount = _unitOfWork.Tutors
                .Count(t => t.IsVerified == null);

            if (pendingCount > 0)
            {
                var notification = new NotificationRequestDto
                {
                    Title = "Kiểm duyệt gia sư",
                    Description = $"Hiện có {pendingCount} gia sư cần được kiểm duyệt.",
                    NotificationType = NotificationType.AdminTutorApproval,
                    Href = "/admin/tutors", // Đường dẫn đến danh sách chờ
                };

                await _notificationService.SendNotificationAsync(notification, true);
            }
        }

        private async Task NotifyAdminForUpdatedTutor(Guid tutorId)
        {
            var notificationToAdmin = new NotificationRequestDto
            {
                UserId = Guid.Empty, // Gửi thông báo đến tất cả admin
                Title = "Gia sư đã cập nhật thông tin",
                Description = "Một gia sư đã cập nhật thông tin, vui lòng xét duyệt.",
                NotificationType = NotificationType.AdminTutorInfoUpdated,
                Href = $"/admin/tutors/{tutorId}"
            };

            await _notificationService.SendNotificationAsync(notificationToAdmin, true);
        }
        public async Task<TutorHomePageDTO> GetTutorHomePage(TutorFilterDto? tutorFilterDto, int index = 0, int size = 20)
        {
            int total;

            // Tạo bộ lọc dựa trên các điều kiện từ TutorFilterDto
            Expression<Func<Tutor, bool>> filter = tutor =>
                tutor.IsVerified.HasValue && tutor.IsVerified.Value == true && tutor.IsDelete == false &&
                (tutorFilterDto.Subjects.Count == 0 || tutor.TutorSubjects.Any(ts => tutorFilterDto.Subjects.Contains(ts.Subject.SubjectId))) &&
                (!tutorFilterDto.MinPrice.HasValue || tutor.TutorSubjects.FirstOrDefault(ts => ts.Rate >= tutorFilterDto.MinPrice) != null) &&
                (!tutorFilterDto.MaxPrice.HasValue || tutor.TutorSubjects.FirstOrDefault(ts => ts.Rate <= tutorFilterDto.MaxPrice) != null) &&
                (string.IsNullOrEmpty(tutorFilterDto.City) || tutor.TutorTeachingLocations.Any(tl => tl.TeachingLocation.CityId == tutorFilterDto.City)) &&
                (string.IsNullOrEmpty(tutorFilterDto.District) || tutor.TutorTeachingLocations.Any(tl => tl.TeachingLocation.DistrictId == tutorFilterDto.District)) &&
                (string.IsNullOrEmpty(tutorFilterDto.SearchingQuery) ||
                tutor.TutorNavigation.Fullname.Contains(tutorFilterDto.SearchingQuery) ||
                tutor.BriefIntroduction.Contains(tutorFilterDto.SearchingQuery) ||
                tutor.ProfileDescription.Contains(tutorFilterDto.SearchingQuery));

            var tutors = _unitOfWork.Tutors.GetMultiPaging(
               filter: filter,
                total: out total,
                index: index,
                size: size,
                includes: new[] { "TutorNavigation", "TutorTeachingLocations", "Schedules", "TutorTeachingLocations.TeachingLocation", "TutorSubjects", "TutorSubjects.Subject" }
            );


            if (tutors == null || !tutors.Any())
            {
                return new TutorHomePageDTO
                {
                    Tutors = new List<TutorSummaryDto>(),
                    TotalRecordCount = total
                };
            }

            var tutorsDto = _mapper.Map<List<TutorSummaryDto>>(tutors);
            foreach (var tutorDto in tutorsDto)
            {
                tutorDto.NumberOfStudents = _unitOfWork.TutorLearnerSubject.Count(t => t.TutorSubject.TutorId == tutorDto.TutorId);
                tutorDto.Rating = await _unitOfWork.Tutors.GetAverageRatingForTutorAsync(tutorDto.TutorId);

                foreach (var location in tutorDto.TeachingLocations)
                {

                    foreach (var district in location.Districts)
                    {
                        district.DistrictName = await _apiAddress.GetDistrictNameByIdAsync(district.DistrictId);
                    }
                    location.CityName = await _apiAddress.GetCityNameByIdAsync(location.CityId);

                }
            }

            return new TutorHomePageDTO
            {
                Tutors = tutorsDto,
                TotalRecordCount = total
            };
        }

        public async Task DeleteTutorAsync(Guid tutorId, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                throw new UnauthorizedAccessException("User does not have the Admin role.");

            var tutor = await _unitOfWork.Tutors.GetByIdAsync(tutorId);
            if (tutor == null)
            {
                throw new KeyNotFoundException("Tutor not found.");
            }
            else
            {
                tutor.IsDelete = true;
                tutor.Status = TutorStatus.Inactive.ToString();
                tutor.UpdatedDate = DateTime.UtcNow;
                tutor.UpdatedBy = currentUser.Id;
            }

            // Chuyển gia sư về vai trò Learner
            var currentRoles = await _userManager.GetRolesAsync(tutor.TutorNavigation);
            if (currentRoles.Contains(AccountRoles.Tutor))
            {
                await _userManager.RemoveFromRoleAsync(tutor.TutorNavigation, AccountRoles.Tutor);
                await _userManager.AddToRoleAsync(tutor.TutorNavigation, AccountRoles.Learner);
            }

            _unitOfWork.Tutors.Update(tutor);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<TutorMajorDto>> GetAllTutorsWithMajorsAndMinorsAsync()
        {
            var majorMinors = MajorMinorData.FieldsList;


            var tutorsWithMajors = majorMinors.Select(m => new TutorMajorDto
            {
                Major = m.Major,
                Minors = m.Minors
            }).ToList();

            return await Task.FromResult(tutorsWithMajors);
        }
        public async Task<List<TutorRatingDto>> GetAllTutorsWithFeedbackAsync()
        {
            var tutors = await _unitOfWork.Tutors.GetAllTutorsAsync();
            var tutorRatingDtos = new List<TutorRatingDto>();

            foreach (var tutor in tutors)
            {

                var averageRating = await _unitOfWork.Tutors.GetAverageRatingForTutorAsync(tutor.TutorId);
                var feedbacks = await _unitOfWork.feedback.GetFeedbacksForTutorAsync(tutor.TutorId);


                var tutorRatingDto = _mapper.Map<TutorRatingDto>(tutor);
                tutorRatingDto.AverageRating = averageRating;
                tutorRatingDto.Feedbacks = _mapper.Map<List<FeedbackDto>>(feedbacks);

                tutorRatingDtos.Add(tutorRatingDto);
            }

            return tutorRatingDtos;
        }

        public async Task<WalletOverviewDto> GetWalletOverviewDtoAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor))
                throw new UnauthorizedAccessException("User does not have the Tutor role.");

            var tutor = await _unitOfWork.Tutors.FindAsync(t => t.TutorId == currentUser.Id);

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Ngày đầu tháng
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Ngày cuối tháng

            var totalEarningsThisMonth = _unitOfWork.Payment
                .GetTotalEarningsThisMonth(currentUser.Id, startOfMonth, endOfMonth);

            var pendingWithDrawals = _unitOfWork.PaymentRequest.Count(pr => pr.TutorId == currentUser.Id && pr.Status == "Pending");

            return new WalletOverviewDto
            {
                CurrentBalance = tutor.Balance,
                PendingWithdrawals = pendingWithDrawals,
                TotalEarningsThisMonth = totalEarningsThisMonth,
            };
        }

        public async Task<List<TutorSummaryDto>> GetTopTutorAsync(int size)
        {
            var tutors = await _unitOfWork.Tutors.GetAllTutorsAsync();

            if (tutors == null || !tutors.Any())
            {
                return new List<TutorSummaryDto>();
            }

            // Tính toán điểm tổng và sắp xếp
            var sortedTutors = tutors
                .Select(t => new
                {
                    Tutor = t,
                    NumberOfLearners = _unitOfWork.TutorLearnerSubject
                        .Count(tls => tls.TutorSubject.TutorId == t.TutorId && !tls.IsDelete), // Thêm điều kiện isdelete = false
                    TotalPoints = t.Rating * 3 + _unitOfWork.TutorLearnerSubject
                        .Count(tls => tls.TutorSubject.TutorId == t.TutorId && !tls.IsDelete) * 1 // Thêm điều kiện isdelete = false
                })
                .OrderByDescending(t => t.TotalPoints) // Sắp xếp theo điểm tổng giảm dần
                .ThenByDescending(t => t.NumberOfLearners) // Nếu điểm tổng bằng nhau, ưu tiên theo số lượng học sinh
                .Take(size) // Chỉ lấy số lượng gia sư theo yêu cầu
                .ToList();

            // Map sang DTO
            var topTutorsDto = _mapper.Map<List<TutorSummaryDto>>(sortedTutors.Select(t => t.Tutor).ToList());

            // Xử lý thông tin bổ sung (số lượng học sinh và địa chỉ)
            foreach (var tutorDto in topTutorsDto)
            {
                tutorDto.NumberOfStudents = _unitOfWork.TutorLearnerSubject.Count(tls => tls.TutorSubject.TutorId == tutorDto.TutorId);

                // Xử lý thông tin địa chỉ
                foreach (var location in tutorDto.TeachingLocations)
                {
                    // Lấy tên quận/huyện
                    var districtTasks = location.Districts.Select(async district =>
                    {
                        district.DistrictName = await _apiAddress.GetDistrictNameByIdAsync(district.DistrictId);
                    }).ToList();

                    // Lấy tên thành phố
                    location.CityName = await _apiAddress.GetCityNameByIdAsync(location.CityId);

                    // Chờ tất cả các tác vụ hoàn thành
                    await Task.WhenAll(districtTasks);
                }
            }

            return topTutorsDto;
        }

    }


}


