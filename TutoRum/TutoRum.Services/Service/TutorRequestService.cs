using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

namespace TutoRum.Services.Service
{
    public class TutorRequestService : ITutorRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly APIAddress _ApiAddress;
        private readonly INotificationService _notificationService;
        private readonly ISubjectService _subjectService;


        public TutorRequestService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager, IEmailService emailService, APIAddress aPIAddress, INotificationService notificationService, ISubjectService subjectService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _ApiAddress = aPIAddress;
            _notificationService = notificationService;
            _subjectService = subjectService;
        }

        public async Task<int> CreateTutorRequestAsync(TutorRequestDTO tutorRequestDto, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }
            if (tutorRequestDto.Fee < 0)
            {
                throw new ArgumentException("Fee must not be negative.");
            }

            // Normalize the subject name to a standard form for consistent comparison
            string normalizedSubjectName = tutorRequestDto.Subject
                .ToUpperInvariant() // Make case-insensitive
                .Normalize(NormalizationForm.FormD); // Normalize Unicode characters

            var subjects = _unitOfWork.Subjects.GetAll();
            var existingSubject = subjects.FirstOrDefault(s =>
                s.SubjectName.ToUpperInvariant().Normalize(NormalizationForm.FormD) == normalizedSubjectName);

            if (existingSubject == null)
            {
                // If subject does not exist, create a new subject
                var newSubject = new Subject
                {
                    SubjectName = tutorRequestDto.Subject,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = currentUser.Id
                };

                _unitOfWork.Subjects.Add(newSubject);
                await _unitOfWork.CommitAsync();

                // Set the new subject as the existing subject
                existingSubject = newSubject;
            }


            var tutorRequest = new TutorRequest
            {
                PhoneNumber = tutorRequestDto.PhoneNumber,
                RequestSummary = tutorRequestDto.RequestSummary,
                CityId = tutorRequestDto.CityId,
                DistrictId = tutorRequestDto.DistrictId,
                WardId = tutorRequestDto.WardId,
                TeachingLocation = tutorRequestDto.TeachingLocation,
                NumberOfStudents = tutorRequestDto.NumberOfStudents,
                StartDate = tutorRequestDto.StartDate,
                PreferredScheduleType = tutorRequestDto.PreferredScheduleType,
                TimePerSession = tutorRequestDto.TimePerSession,
                Subject = tutorRequestDto.Subject,
                StudentGender = tutorRequestDto.StudentGender,
                TutorGender = tutorRequestDto.TutorGender,
                Fee = tutorRequestDto.Fee,
                SessionsPerWeek = tutorRequestDto.SessionsPerWeek,
                DetailedDescription = tutorRequestDto.DetailedDescription,
                TutorQualificationId = tutorRequestDto.TutorQualificationId,
                AspNetUserId = currentUser.Id,
                Status = "Chưa có gia sư", // Trạng thái mặc định khi tạo mới
                IsVerified = null, // Có thể thêm logic xác minh
                CreatedBy = currentUser.Id,
                CreatedDate = DateTime.UtcNow,
                FreeSchedules = tutorRequestDto.FreeSchedules,
                RateRangeId = tutorRequestDto.rateRangeId,
            };

            // Thêm TutorRequest vào UnitOfWork
            _unitOfWork.TutorRequest.Add(tutorRequest);
            await _unitOfWork.CommitAsync();

            return tutorRequest.Id;
        }

        public async Task<TutorRequestDTO> GetTutorRequestByIdAsync(int tutorRequestId)
        {
            // Lấy thông tin TutorRequest từ cơ sở dữ liệu
            var tutorRequest =  _unitOfWork.TutorRequest.GetSingleById(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }

            // Lấy thông tin người đăng ký (AspNetUser)
            var user =  _unitOfWork.Accounts
                .GetMultiAsQueryable(u => u.Id == tutorRequest.AspNetUserId)
                .FirstOrDefault();

            // Kiểm tra xem người dùng có tồn tại không
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var teachingLocationCity = await _ApiAddress.GetCityNameByIdAsync(tutorRequest.CityId);
            var teachingLocationDistrict = await _ApiAddress.GetDistrictNameByIdAsync(tutorRequest.DistrictId);
            var teachingLocationWard = await _ApiAddress.GetWardNameByIdAsync(tutorRequest.WardId);
            var qualificationLevel = await _unitOfWork.QualificationLevel.FindAsync(q => q.Id == tutorRequest.TutorQualificationId);

            // Trả về DTO với tên người đăng ký được lấy từ AspNetUser
            return new TutorRequestDTO
            {
                TutorQualificationName = qualificationLevel.Level,
                Id = tutorRequest.Id,
                PhoneNumber = tutorRequest.PhoneNumber,
                RequestSummary = tutorRequest.RequestSummary,
                CityId = tutorRequest.CityId,
                DistrictId = tutorRequest.DistrictId,
                WardId = tutorRequest.WardId,
                TeachingLocation = $"{tutorRequest.TeachingLocation}, {teachingLocationWard}, {teachingLocationDistrict}, {teachingLocationCity}" ,
                NumberOfStudents = tutorRequest.NumberOfStudents,
                StartDate = tutorRequest.StartDate,
                PreferredScheduleType = tutorRequest.PreferredScheduleType,
                TimePerSession = tutorRequest.TimePerSession,
                Subject = tutorRequest.Subject,
                StudentGender = tutorRequest.StudentGender,
                TutorGender = tutorRequest.TutorGender,
                Fee = tutorRequest.Fee,
                SessionsPerWeek = tutorRequest.SessionsPerWeek,
                DetailedDescription = tutorRequest.DetailedDescription,
                TutorQualificationId = tutorRequest.TutorQualificationId,
                Status = tutorRequest.Status,
                LearnerName = user.Fullname,
                FreeSchedules = tutorRequest.FreeSchedules,
                rateRangeId = tutorRequest.RateRangeId
            };
        }


        public async Task<PagedResult<TutorRequestDTO>> GetAllTutorRequestsAsync(TutorRequestHomepageFilterDto filter, int pageIndex = 0, int pageSize = 20)
        {
            // Kiểm tra và chuyển đổi giá trị CityId và DistrictId từ string sang int
            bool isCityIdValid = int.TryParse(filter.CityId, out int cityIdValue);
            bool isDistrictIdValid = int.TryParse(filter.DistrictId, out int districtIdValue);

            Expression<Func<TutorRequest, bool>> predicate = request =>
                request.IsVerified == true && request.IsDelete == false &&

                // Điều kiện lọc theo từ khóa tìm kiếm (Search)
                (string.IsNullOrEmpty(filter.Search) ||
                    request.RequestSummary.ToLower().Contains(filter.Search.ToLower()) ||
                    request.DetailedDescription.ToLower().Contains(filter.Search.ToLower())) &&

                // Điều kiện lọc theo khoảng phí (MinFee và MaxFee)
                (!filter.MinFee.HasValue || request.Fee >= filter.MinFee) &&
                (!filter.MaxFee.HasValue || request.Fee <= filter.MaxFee) &&

                // Điều kiện lọc theo RateRangeId
                (!filter.RateRangeId.HasValue || request.RateRangeId == filter.RateRangeId) &&

                // Điều kiện lọc theo TutorQualificationId
                (!filter.TutorQualificationId.HasValue || request.TutorQualificationId == filter.TutorQualificationId) &&

                // Điều kiện lọc theo giới tính gia sư (TutorGender)
                (string.IsNullOrEmpty(filter.TutorGender) || request.TutorGender == filter.TutorGender) &&

                // Điều kiện lọc theo CityId
                (!isCityIdValid || request.CityId == cityIdValue.ToString()) &&

                // Điều kiện lọc theo DistrictId
                (!isDistrictIdValid || request.DistrictId == districtIdValue.ToString()) &&

                // Điều kiện lọc theo môn học (Subject)
                (string.IsNullOrEmpty(filter.Subject) || request.Subject == filter.Subject);


            int totalRecords;
            var tutorRequests = _unitOfWork.TutorRequest.GetMultiPaging(
                filter: predicate, 
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] {"TutorQualification", "TutorRequestTutors" });

            if (tutorRequests == null || !tutorRequests.Any())
            {
                return new PagedResult<TutorRequestDTO>
                {
                    Items = new List<TutorRequestDTO>(),  // Danh sách yêu cầu gia sư đã phân trang
                    TotalRecords = totalRecords // Tổng số yêu cầu gia sư
                };
            }

            var tutorRequestDtos = await Task.WhenAll( tutorRequests.Select(async tr => {
                var teachingLocationCity = await _ApiAddress.GetCityNameByIdAsync(tr.CityId);
                var teachingLocationDistrict = await _ApiAddress.GetDistrictNameByIdAsync(tr.DistrictId);
                var teachingLocationWard = await _ApiAddress.GetWardNameByIdAsync(tr.WardId);

                return new TutorRequestDTO
                {
                    TutorQualificationName = tr.TutorQualification.Level,
                    Id = tr.Id,
                    RequestSummary = tr.RequestSummary,
                    CityId = tr.CityId,
                    DistrictId = tr.DistrictId,
                    WardId = tr.WardId,
                    TeachingLocation = tr.TeachingLocation + $", {teachingLocationWard}, {teachingLocationDistrict}, {teachingLocationCity}",
                    NumberOfStudents = tr.NumberOfStudents,
                    StartDate = tr.StartDate,
                    PreferredScheduleType = tr.PreferredScheduleType,
                    TimePerSession = tr.TimePerSession,
                    Subject = tr.Subject,
                    StudentGender = tr.StudentGender,
                    TutorGender = tr.TutorGender,
                    Fee = tr.Fee,
                    SessionsPerWeek = tr.SessionsPerWeek,
                    DetailedDescription = tr.DetailedDescription,
                    TutorQualificationId = tr.TutorQualificationId,
                    Status = tr.Status,
                    FreeSchedules = tr.FreeSchedules,
                    CreatedUserId = tr.AspNetUserId,
                    rateRangeId = tr.RateRangeId,
                    RegisteredTutorIds = tr.TutorRequestTutors.Select(trt => trt.TutorId).ToList(),
                };
            }).ToList());

            // Tạo đối tượng PagedResult để trả về kết quả phân trang
            var result = new PagedResult<TutorRequestDTO>
            {
                Items = tutorRequestDtos.ToList(),  // Danh sách yêu cầu gia sư đã phân trang
                TotalRecords = totalRecords // Tổng số yêu cầu gia sư
            };

            return result;
        }

        public async Task<bool> UpdateTutorRequestAsync(int tutorRequestId, TutorRequestDTO tutorRequestDto, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            var tutorRequest = _unitOfWork.TutorRequest.GetSingleById(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }
            if (tutorRequestDto.Fee < 0)
            {
                throw new ArgumentException("Fee must not be negative.");
            }

            tutorRequest.PhoneNumber = tutorRequestDto.PhoneNumber;
            tutorRequest.RequestSummary = tutorRequestDto.RequestSummary;
            tutorRequest.CityId = tutorRequestDto.CityId;
            tutorRequest.DistrictId = tutorRequestDto.DistrictId;
            tutorRequest.WardId = tutorRequestDto.WardId;
            tutorRequest.TeachingLocation = tutorRequestDto.TeachingLocation;
            tutorRequest.NumberOfStudents = tutorRequestDto.NumberOfStudents;
            tutorRequest.StartDate = tutorRequestDto.StartDate;
            tutorRequest.PreferredScheduleType = tutorRequestDto.PreferredScheduleType;
            tutorRequest.TimePerSession = tutorRequestDto.TimePerSession;
            tutorRequest.Subject = tutorRequestDto.Subject;
            tutorRequest.StudentGender = tutorRequestDto.StudentGender;
            tutorRequest.TutorGender = tutorRequestDto.TutorGender;
            tutorRequest.Fee = tutorRequestDto.Fee;
            tutorRequest.SessionsPerWeek = tutorRequestDto.SessionsPerWeek;
            tutorRequest.DetailedDescription = tutorRequestDto.DetailedDescription;
            tutorRequest.TutorQualificationId = tutorRequestDto.TutorQualificationId;
            tutorRequest.Status = tutorRequestDto.Status;
            tutorRequest.UpdatedBy = currentUser.Id;
            tutorRequest.UpdatedDate = DateTime.UtcNow;
            tutorRequest.FreeSchedules = tutorRequestDto.FreeSchedules;
            tutorRequest.RateRangeId = tutorRequestDto.rateRangeId;
            tutorRequest.IsVerified = false;
            tutorRequest.Status = "Chưa xác minh";
            // Cập nhật lại đối tượng trong UnitOfWork
            _unitOfWork.TutorRequest.Update(tutorRequest);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> ChooseTutorForTutorRequestAsync(int tutorRequestId, Guid tutorId, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy thông tin yêu cầu gia sư từ cơ sở dữ liệu
            var tutorRequest = _unitOfWork.TutorRequest.GetSingleById(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }

            // Kiểm tra xem gia sư có đăng ký vào yêu cầu này không
            var tutorRequestTutor =  _unitOfWork.TutorRequestTutor
                .GetSingleByCondition(trt => trt.TutorRequestId == tutorRequestId && trt.TutorId == tutorId);
                

            if (tutorRequestTutor == null)
            {
                throw new Exception("Tutor not found for this tutor request.");
            }


            var subject = new List<AddSubjectDTO>(); // List là triển khai của ICollection
            subject.Add(new AddSubjectDTO
            {
                SubjectName = tutorRequest.Subject,
                Rate = tutorRequest.Fee,
            });

            // Thêm môn học của TutorRequest vào Tutor
            await _subjectService.AddSubjectsWithRateAsync(subject, tutorId);

            // Cập nhật trường IsDeleted trong bảng TutorRequest
            tutorRequest.IsDelete = true; // Đánh dấu yêu cầu gia sư đã xóa
            tutorRequest.Status = "Đã đóng"; // Thay đổi trạng thái thành "Deleted"
            tutorRequest.UpdatedBy = currentUser.Id;
            tutorRequest.UpdatedDate = DateTime.UtcNow;

            tutorRequestTutor.Ischoose = true;

            // Cập nhật lại đối tượng trong UnitOfWork
            _unitOfWork.TutorRequest.Update(tutorRequest);
            _unitOfWork.TutorRequestTutor.Update(tutorRequestTutor);
            await _unitOfWork.CommitAsync();

            return true;
        }


        public async Task<bool> AddTutorToRequestAsync(int tutorRequestId, Guid tutorId)
        {
            // Lấy thông tin TutorRequest từ cơ sở dữ liệu
            var tutorRequest =  _unitOfWork.TutorRequest.GetSingleById(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }

            if(tutorRequest.AspNetUserId == tutorId)
            {
                throw new Exception("You cannot register your tutor request");
            }


            // Lấy thông tin Tutor từ AspNetUser
            var tutor = await _userManager.FindByIdAsync(tutorId.ToString());
            if (tutor == null)
            {
                throw new Exception("Tutor not found.");
            }

            if(!await _userManager.IsInRoleAsync(tutor, AccountRoles.Tutor))
            {
                throw new Exception("User does not have Tutor role");
            }

            // Kiểm tra xem gia sư đã đăng ký vào yêu cầu này chưa
            var existingRegistration =  _unitOfWork.TutorRequestTutor
                .GetSingleByCondition(trt => trt.TutorRequestId == tutorRequestId && trt.TutorId == tutorId);

            if (existingRegistration != null)
            {
                throw new Exception("This tutor has already registered for this request.");
            }

            // Tạo đối tượng TutorRequestTutor mới và thêm vào database
            var tutorRequestTutor = new TutorRequestTutor
            {
                TutorRequestId = tutorRequestId,
                TutorId = tutorId,
                DateJoined = DateTime.UtcNow,
                Status = "Pending" // Trạng thái đăng ký ban đầu
            };

            // Thêm bản ghi vào cơ sở dữ liệu
            _unitOfWork.TutorRequestTutor.Add(tutorRequestTutor);

            // Commit các thay đổi
            await _unitOfWork.CommitAsync();

            var notificationDto = new NotificationRequestDto
            {
                UserId = tutorId,
                Title = "Bạn đã đăng ký yêu cầu thành công",
                Description = "Vui lòng chờ học viên phản hồi qua email",
                Href = "/user/registered-tutor-requests/",
                NotificationType = NotificationType.GeneralUser,
            };

            await _notificationService.SendNotificationAsync(notificationDto, false);

            return true;
        }

        public async Task<TutorRequestWithTutorsDTO> GetListTutorsByTutorRequestAsync(int tutorRequestId)
        {
            // Lấy thông tin TutorRequest từ cơ sở dữ liệu
            var tutorRequest = _unitOfWork.TutorRequest.GetSingleById(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }

            // Kiểm tra điều kiện IsVerified của TutorRequest
            if (tutorRequest.IsVerified != true)
            {
                throw new Exception("The tutor request is not verified.");
            }

            // Lấy danh sách TutorIds và IsVerified đã đăng ký vào TutorRequest này
            var tutorRequestTutors =  _unitOfWork.TutorRequestTutor
                .GetMultiAsQueryable(trt => trt.TutorRequestId == tutorRequestId)
                .Select(trt => new
                {
                    trt.TutorId,
                    trt.IsVerified  // Lấy thêm IsVerified cho mỗi gia sư
                })
                .ToList(); // Lấy danh sách tutorRequestTutors về bộ nhớ

            if (tutorRequestTutors == null || !tutorRequestTutors.Any())
            {
                throw new Exception("No tutors found for this tutor request.");
            }

            // Lấy thông tin chi tiết các gia sư dựa trên tutorIds, bao gồm email
            var tutorIds = tutorRequestTutors.Select(trt => trt.TutorId).ToList();

            var tutors =  _unitOfWork.Tutors
                .GetMultiAsQueryable(t => tutorIds.Contains(t.TutorId))
                .Include(t => t.TutorNavigation) // Bao gồm TutorNavigation để lấy thông tin fullname
                .ToList(); // Lấy tất cả gia sư vào bộ nhớ

            // Kết hợp thông tin từ tutorRequestTutors và tutors
            var tutorsWithDetails = tutors
                .Select(t => new TutorInTutorRequestDTO
                {
                    TutorId = t.TutorId,
                    Name = t.TutorNavigation.Fullname,
                    Specialization = t.Specialization,
                    Email = t.TutorNavigation.Email, // Lấy email của tutor từ AspNetUser
                    IsVerified = tutorRequestTutors.FirstOrDefault(trt => trt.TutorId == t.TutorId)?.IsVerified ?? false // Lấy IsVerified từ tutorRequestTutors
                })
                .ToList();

            // Tạo đối tượng DTO để trả về thông tin yêu cầu gia sư và danh sách gia sư
            var tutorRequestWithTutors = new TutorRequestWithTutorsDTO
            {
                TutorRequestId = tutorRequest.Id,
                Subject = tutorRequest.Subject,
                StartDate = tutorRequest.StartDate,
                Tutors = tutorsWithDetails
            };

            return tutorRequestWithTutors;
        }

        public async Task<PagedResult<ListTutorRequestDTO>> GetTutorRequestsAdmin(TutorRequestFilterDto filter, ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20)
        {
            Expression<Func<TutorRequest, bool>> predicate = request =>
                // Điều kiện lọc theo tên gia sư (TutorName)
                (string.IsNullOrEmpty(filter.Search)
                    || request.RequestSummary.ToLower().Contains(filter.Search.ToLower())
                    || request.Id.ToString().ToLower().Contains(filter.Search.ToLower())
                    || request.DetailedDescription.ToLower().Contains(filter.Search.ToLower()) &&

                // Điều kiện lọc theo ID yêu cầu thanh toán (PaymentRequestId)
                (!filter.IsVerified.HasValue || request.IsVerified == filter.IsVerified) &&

                // Điều kiện lọc theo trạng thái thanh toán (IsPaid)
                (!filter.StartDate.HasValue || request.StartDate == filter.StartDate) &&

                // Điều kiện lọc theo trạng thái xác thực (VerificationStatus)
                (string.IsNullOrEmpty(filter.Subject) || request.Subject == filter.Subject));

            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
            {
                throw new Exception("User does not have role for this");
            }

            // Lấy danh sách TutorRequests của Learner theo AspNetUserId và kiểm tra không bị xóa
            int totalRecords;
            var tutorRequests = _unitOfWork.TutorRequest.GetMultiPaging(
                filter: predicate,
                out totalRecords,
                index: pageIndex,
                size: pageSize
            );

            if (tutorRequests == null || !tutorRequests.Any())
            {
                throw new Exception("No tutor requests found for the given learner.");
            }

            // Lấy thông tin xem yêu cầu đã tìm được gia sư chưa
            var tutorRequestDtos = tutorRequests.Select(tr => new ListTutorRequestDTO
            {
                Fee = tr.Fee,
                RequestSummary = tr.RequestSummary,
                SessionsPerWeek = tr.SessionsPerWeek,
                TimePerSession = tr.TimePerSession,
                PhoneNumber = tr.PhoneNumber,
                TutorRequestId = tr.Id,
                Subject = tr.Subject, // Tên môn học
                StartDate = tr.StartDate, // Ngày đăng
                DetailedDescription = tr.DetailedDescription, // Mô tả chi tiết
                Status = tr.Status, // Trạng thái yêu cầu
                IsVerified = tr.IsVerified,
                IsTutorAssigned = _unitOfWork.TutorRequestTutor
                    .GetMultiAsQueryable(trt => trt.TutorRequestId == tr.Id)
                    .Any() // Kiểm tra đã có gia sư chưa
            }).ToList();

            // Tạo đối tượng PagedResult để trả về kết quả phân trang
            var result = new PagedResult<ListTutorRequestDTO>
            {
                Items = tutorRequestDtos,  // Danh sách yêu cầu gia sư đã phân trang
                TotalRecords = totalRecords // Tổng số yêu cầu gia sư
            };

            return result;
        }

        public async Task<PagedResult<ListTutorRequestDTO>> GetTutorRequestsByLearnerIdAsync(Guid learnerId, int pageIndex = 0, int pageSize = 20)
        {
            // Lấy danh sách TutorRequests của Learner theo AspNetUserId và kiểm tra không bị xóa
            int totalRecords;
            var tutorRequests = _unitOfWork.TutorRequest.GetMultiPaging(
                tr => tr.AspNetUserId == learnerId, // Lọc theo LearnerId và kiểm tra không bị xóa
                out totalRecords,
                index: pageIndex,
                size: pageSize
            );

            if (tutorRequests == null || !tutorRequests.Any())
            {
                throw new Exception("No tutor requests found for the given learner.");
            }

            // Tải trước dữ liệu cần thiết từ TutorRequestTutor để tránh truy vấn nhiều lần
            var tutorRequestIds = tutorRequests.Select(tr => tr.Id).ToList();
            var tutorRequestTutors =  _unitOfWork.TutorRequestTutor
                .GetMultiAsQueryable(trt => tutorRequestIds.Contains(trt.TutorRequestId))
                .ToList();

            var tutorRequestDtos = tutorRequests.Select(tr =>
            {
                var relatedTutors = tutorRequestTutors.Where(trt => trt.TutorRequestId == tr.Id).ToList();
                var chosenTutor = relatedTutors.FirstOrDefault(trt => trt.Ischoose == true);

                return new ListTutorRequestDTO
                {
                    Fee = tr.Fee,
                    IsDelete = tr.IsDelete,
                    TutorRequestId = tr.Id,
                    Subject = tr.Subject, // Tên môn học
                    StartDate = tr.StartDate, // Ngày đăng
                    DetailedDescription = tr.DetailedDescription, // Mô tả chi tiết
                    Status = tr.Status, // Trạng thái yêu cầu
                    IsVerified = tr.IsVerified,
                    NumberOfRegisteredTutor = relatedTutors.Count,
                    ChosenTutorId = chosenTutor?.TutorId,
                    IsTutorAssigned = relatedTutors.Any(trt => trt.Ischoose == true) // Kiểm tra đã có gia sư chưa
                };
            }).ToList();

            // Tạo đối tượng PagedResult để trả về kết quả phân trang
            var result = new PagedResult<ListTutorRequestDTO>
            {
                Items = tutorRequestDtos, // Danh sách yêu cầu gia sư đã phân trang
                TotalRecords = totalRecords // Tổng số yêu cầu gia sư
            };

            return result;
        }


        public async Task<PagedResult<ListTutorRequestForTutorDto>> GetTutorRequestsByTutorIdAsync(Guid tutorId, int pageIndex = 0, int pageSize = 20)
        {

            // Lấy danh sách TutorRequests của Learner theo AspNetUserId và kiểm tra không bị xóa
            int totalRecords;
            var tutorRequestTutors = _unitOfWork.TutorRequestTutor.GetMultiPaging(
                tr => tr.TutorId == tutorId, // Lọc theo LearnerId và kiểm tra không bị xóa
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] { "TutorRequest" }
            );

            if (tutorRequestTutors == null || !tutorRequestTutors.Any())
            {
                throw new Exception("No tutor requests found for the given learner.");
            }

            

            // Lấy thông tin xem yêu cầu đã tìm được gia sư chưa
            var tutorRequestDtos = await Task.WhenAll( tutorRequestTutors.Select(async tr => {
                var teachingLocationCity = await _ApiAddress.GetCityNameByIdAsync(tr.TutorRequest.CityId);
                var teachingLocationDistrict = await _ApiAddress.GetDistrictNameByIdAsync(tr.TutorRequest.DistrictId);
                var teachingLocationWard = await _ApiAddress.GetWardNameByIdAsync(tr.TutorRequest.WardId);

                return new ListTutorRequestForTutorDto
                {
                    Id = tr.TutorRequestId,
                    Fee = tr.TutorRequest.Fee,
                    Subject = tr.TutorRequest.Subject, // Tên môn học
                    StartDate = tr.TutorRequest.StartDate, // Ngày đăng
                    TeachingLocation = tr.TutorRequest.TeachingLocation + $", {teachingLocationWard}, {teachingLocationDistrict}, {teachingLocationCity}",
                    NumberOfStudents = tr.TutorRequest.NumberOfStudents,
                    Status = tr.Status, // Trạng thái yêu cầu
                    IsChosen = tr.Ischoose,
                    IsInterested = tr.IsVerified,
                    TutorLearnerSubjectId = tr.TutorRequest.TutorLearnerSubjectId,
                };
            } ).ToList());

            // Tạo đối tượng PagedResult để trả về kết quả phân trang
            var result = new PagedResult<ListTutorRequestForTutorDto>
            {
                Items = tutorRequestDtos.ToList(),  // Danh sách yêu cầu gia sư đã phân trang
                TotalRecords = totalRecords // Tổng số yêu cầu gia sư
            };

            return result;
        }


        public async Task SendTutorRequestEmailAsync(int tutorRequestId, Guid tutorID)
        {
            // Lấy thông tin TutorRequest từ cơ sở dữ liệu
            var tutorRequest = await GetTutorRequestByIdAsync(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }

            // Lấy danh sách các gia sư đã đăng ký vào yêu cầu này
            var tutorRequestTutors = await _unitOfWork.TutorRequestTutor
                .GetMultiAsQueryable(trt => trt.TutorRequestId == tutorRequestId && trt.Tutor.TutorId == tutorID)
                .FirstOrDefaultAsync();
            var tutor =  await _userManager.FindByIdAsync(tutorID.ToString());
            if (tutorRequestTutors == null)
            {
                throw new Exception("Tutor not found for this request.");
            }

            // Cập nhật trường IsVerified của TutorRequestTutor thành true cho gia sư này
            tutorRequestTutors.IsVerified = true;

            // Lưu thay đổi vào cơ sở dữ liệu
            _unitOfWork.TutorRequestTutor.Update(tutorRequestTutors);
            await _unitOfWork.CommitAsync();  // Lưu các thay đổi vào cơ sở dữ liệu

            // Tạo nội dung HTML của email
            string htmlContent = GenerateTutorRequestHtmlContent(tutorRequest);

            // Thêm nút "Xem chi tiết yêu cầu" vào nội dung email
            htmlContent += $@"
    <div style='text-align: center; margin-top: 20px;'>
        <a href='https://tutor-rum-project.vercel.app/tutor-requests/{tutorRequest.Id}' 
           style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 5px;'>
            Xem chi tiết yêu cầu
        </a>
    </div>";

            // Chủ đề email
            var emailSubject = $"Thông báo yêu cầu gia sư từ {tutorRequest.LearnerName}";

            // Gửi email cho gia sư
            await _emailService.SendEmailAsync(tutor.Email, emailSubject, htmlContent);
        }

        private string GenerateTutorRequestHtmlContent(TutorRequestDTO tutorRequest)
        {
            // Tạo nội dung HTML để gửi email
            return $@"
    <html>
        <body>
            <h2>Thông tin yêu cầu gia sư</h2>
            <p><strong>Người đăng ký:</strong> {tutorRequest.LearnerName}</p>
            <p><strong>Môn học:</strong> {tutorRequest.Subject}</p>
            <p><strong>Số điện thoại:</strong> {tutorRequest.PhoneNumber}</p>
            <p><strong>Địa chỉ yêu cầu:</strong> {tutorRequest.TeachingLocation}</p>
            <p><strong>Thời gian bắt đầu:</strong> {tutorRequest.StartDate:dd/MM/yyyy}</p>
            <p><strong>Mô tả chi tiết:</strong> {tutorRequest.DetailedDescription}</p>
        </body>
    </html>";
        }

        public async Task<TutorLearnerSubjectDetailDto> GetTutorLearnerSubjectInfoByTutorRequestId (ClaimsPrincipal user, int tutorRequestId)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy thông tin TutorRequest từ cơ sở dữ liệu
            var tutorRequest = _unitOfWork.TutorRequest.GetSingleById(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }

            var tutorSubject = await _unitOfWork.TutorSubjects.FindAsync(ts => ts.TutorId == currentUser.Id && ts.Subject.SubjectName == tutorRequest.Subject);

            var result = new TutorLearnerSubjectDetailDto
            {
                LearnerId = tutorRequest.AspNetUserId ?? new Guid(),
                CityId = tutorRequest.CityId,
                DistrictId = tutorRequest.DistrictId,
                WardId = tutorRequest.WardId,
                LocationDetail = tutorRequest.TeachingLocation,
                PricePerHour = tutorRequest.Fee,
                Notes = tutorRequest.DetailedDescription,
                SessionsPerWeek = tutorRequest.SessionsPerWeek,
                PreferredScheduleType = tutorRequest.PreferredScheduleType,
                ExpectedStartDate = tutorRequest.StartDate,
                HoursPerSession = (int)Math.Round(tutorRequest.TimePerSession.TotalHours),
                TutorSubjectId = tutorSubject.TutorSubjectId,
            };
            return result;
        }


        public async Task<bool> CloseTutorRequestAsync(int tutorRequestId, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy thông tin yêu cầu gia sư từ cơ sở dữ liệu
            var tutorRequest = _unitOfWork.TutorRequest.GetSingleById(tutorRequestId);
            if (tutorRequest == null)
            {
                throw new Exception("Tutor request not found.");
            }

            // Kiểm tra xem người dùng hiện tại có quyền xóa yêu cầu này không
            if (tutorRequest.AspNetUserId != currentUser.Id)
            {
                throw new Exception("You do not have permission to delete this tutor request.");
            }

            // Đánh dấu yêu cầu gia sư là đã xóa
            tutorRequest.IsDelete = true; // Đánh dấu yêu cầu gia sư đã xóa
            tutorRequest.UpdatedBy = currentUser.Id;
            tutorRequest.UpdatedDate = DateTime.UtcNow;

            // Cập nhật lại đối tượng trong UnitOfWork
            _unitOfWork.TutorRequest.Update(tutorRequest);
            await _unitOfWork.CommitAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return true;
        }


    }

}
