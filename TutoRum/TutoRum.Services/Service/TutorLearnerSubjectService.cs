using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Helper;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;


namespace TutoRum.Services.Service
{
    public class TutorLearnerSubjectService : ITutorLearnerSubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IScheduleService _scheduleService;
        private readonly APIAddress _apiAddress;
        private readonly INotificationService _notificationService;



        public TutorLearnerSubjectService(IUnitOfWork unitOfWork
            , UserManager<AspNetUser> userManager
            , IScheduleService scheduleService,
                APIAddress apiAddress,
                INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _scheduleService = scheduleService;
            _apiAddress = apiAddress;
            _notificationService = notificationService;
        }


        public async Task RegisterLearnerForTutorAsync(RegisterLearnerDTO learnerDto, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            // Kiểm tra nếu người dùng không tồn tại
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Kiểm tra nếu user đã là học sinh
            if (!await _userManager.IsInRoleAsync(currentUser, AccountRoles.Learner))
            {
                throw new UnauthorizedAccessException("User is not a learner!");
            }

            // Lấy ExecutionStrategy từ DbContext hoặc UnitOfWork
            var strategy = _unitOfWork.TutorLearnerSubject.GetExecutionStrategy(); // Adjust this to your specific UnitOfWork or DbContext implementation

            // Thực hiện các thao tác trong phạm vi của ExecutionStrategy để có thể thử lại nếu có lỗi tạm thời
            await strategy.ExecuteAsync(async () =>
            {
                // Bắt đầu transaction
                using (var transaction = await _unitOfWork.TutorLearnerSubject.BeginTransactionAsync())
                {
                    try
                    {
                        // Tạo thực thể TutorLearnerSubject từ thông tin DTO
                        var newTutorLearnerSubject = new TutorLearnerSubject
                        {
                            TutorSubjectId = learnerDto.TutorSubjectId,
                            LearnerId = currentUser.Id,
                            Location = learnerDto.CityId,
                            DistrictId = learnerDto.DistrictId,
                            WardId = learnerDto.WardId,
                            ContractUrl = learnerDto.ContractUrl,
                            PricePerHour = learnerDto.PricePerHour,
                            Notes = learnerDto.Notes,
                            LocationDetail = learnerDto.LocationDetail,
                            SessionsPerWeek = learnerDto.SessionsPerWeek,
                            HoursPerSession = learnerDto.HoursPerSession,
                            PreferredScheduleType = learnerDto.PreferredScheduleType,
                            ExpectedStartDate = learnerDto.ExpectedStartDate,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = currentUser.Id,
                            Status = "Chờ xác thực"
                        };

                        // Thêm bản ghi vào hệ thống
                        _unitOfWork.TutorLearnerSubject.Add(newTutorLearnerSubject);
                        await _unitOfWork.CommitAsync();

                        // Lấy ID sau khi bản ghi được thêm
                        var tutorLearnerSubjectId = newTutorLearnerSubject.TutorLearnerSubjectId;

                        // Sử dụng ID để truyền vào phương thức RegisterSchedulesForClass
                        await _scheduleService.RegisterSchedulesForClass(learnerDto.Schedules, currentUser.Id, tutorLearnerSubjectId);

                        // Commit transaction nếu mọi thứ đều thành công
                        await transaction.CommitAsync();

                        var subject = await _unitOfWork.TutorSubjects.FindAsync(ts => ts.TutorSubjectId == learnerDto.TutorSubjectId);

                        var notificationToLearner = new NotificationRequestDto
                        {
                            UserId = currentUser.Id,
                            Title = "Yêu cầu đăng ký môn học đã được gửi",
                            Description = "Yêu cầu đăng ký của bạn đã được gửi đến gia sư. Vui lòng chờ gia sư xác nhận.",
                            NotificationType = NotificationType.TutorLearnerSubjectPending,
                            Href = "/user/registered-tutors"
                        };

                        await _notificationService.SendNotificationAsync(notificationToLearner, false);

                        var notificationToTutor = new NotificationRequestDto
                        {
                            UserId = subject.TutorId ?? new Guid(),
                            Title = "Yêu cầu đăng ký môn học mới từ học viên",
                            Description = "Vui lòng xem và phản hồi yêu cầu từ học viên nhé!",
                            NotificationType = NotificationType.TutorLearnerSubjectPending,
                            Href = "/user/registered-learners"
                        };

                        await _notificationService.SendNotificationAsync(notificationToTutor, false);
                    }
                    catch (Exception ex)
                    {
                        // Rollback nếu có lỗi xảy ra
                        await transaction.RollbackAsync();
                        throw new Exception("Error during learner registration process: " + ex.Message, ex);
                    }
                }
            });
        }

        public async Task UpdateClassroom(int tutorLearnerSubjectId, RegisterLearnerDTO learnerDto, ClaimsPrincipal user)
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(user);

            // Check if user exists
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the user has the Learner role
            if (!await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor))
            {
                throw new UnauthorizedAccessException("User is not a tutor!");
            }

            // Retrieve ExecutionStrategy from DbContext or UnitOfWork
            var strategy = _unitOfWork.TutorLearnerSubject.GetExecutionStrategy();

            // Use ExecutionStrategy to allow retries for transient errors
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.TutorLearnerSubject.BeginTransactionAsync())
                {
                    try
                    {
                        // Check if the existing TutorLearnerSubject record exists
                        var existingTutorLearnerSubject = await _unitOfWork.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(tutorLearnerSubjectId);
                        if (existingTutorLearnerSubject == null)
                        {
                            throw new Exception("Record not found or unauthorized access.");
                        }

                        // Conditionally update fields based on non-null DTO properties
                        if (learnerDto.TutorSubjectId != null)
                            existingTutorLearnerSubject.TutorSubjectId = learnerDto.TutorSubjectId;

                        if (!string.IsNullOrEmpty(learnerDto.CityId))
                            existingTutorLearnerSubject.Location = learnerDto.CityId;

                        if (!string.IsNullOrEmpty(learnerDto.DistrictId))
                            existingTutorLearnerSubject.DistrictId = learnerDto.DistrictId;

                        if (!string.IsNullOrEmpty(learnerDto.WardId))
                            existingTutorLearnerSubject.WardId = learnerDto.WardId;

                        if (!string.IsNullOrEmpty(learnerDto.ContractUrl))
                            existingTutorLearnerSubject.ContractUrl = learnerDto.ContractUrl;

                        if (learnerDto.PricePerHour.HasValue)
                        {
                            if (learnerDto.PricePerHour.Value < 0)
                            {
                                throw new ArgumentException("Price per hour cannot be negative.");
                            }
                            existingTutorLearnerSubject.PricePerHour = learnerDto.PricePerHour.Value;
                        }

                        if (learnerDto.PricePerHour.HasValue)
                            existingTutorLearnerSubject.PricePerHour = learnerDto.PricePerHour.Value;

                        if (!string.IsNullOrEmpty(learnerDto.Notes))
                            existingTutorLearnerSubject.Notes = learnerDto.Notes;

                        if (!string.IsNullOrEmpty(learnerDto.LocationDetail))
                            existingTutorLearnerSubject.LocationDetail = learnerDto.LocationDetail;

                        if (learnerDto.SessionsPerWeek.HasValue)
                            existingTutorLearnerSubject.SessionsPerWeek = learnerDto.SessionsPerWeek.Value;

                        if (learnerDto.HoursPerSession.HasValue)
                            existingTutorLearnerSubject.HoursPerSession = learnerDto.HoursPerSession.Value;

                        if (!string.IsNullOrEmpty(learnerDto.PreferredScheduleType))
                            existingTutorLearnerSubject.PreferredScheduleType = learnerDto.PreferredScheduleType;

                        if (learnerDto.ExpectedStartDate.HasValue)
                            existingTutorLearnerSubject.ExpectedStartDate = learnerDto.ExpectedStartDate.Value;

                        existingTutorLearnerSubject.Status = "Updated"; // Or keep the current status if appropriate
                        existingTutorLearnerSubject.UpdatedDate = DateTime.UtcNow;
                        existingTutorLearnerSubject.UpdatedBy = currentUser.Id;

                        // Update the record in the system
                        _unitOfWork.TutorLearnerSubject.Update(existingTutorLearnerSubject);
                        await _unitOfWork.CommitAsync();

                        if (learnerDto.Schedules.Count > 0)
                            // Update schedules for the class
                            await _scheduleService.RegisterSchedulesForClass(learnerDto.Schedules, currentUser.Id, tutorLearnerSubjectId);

                        // Commit transaction if everything is successful
                        await transaction.CommitAsync();

                        // Handle contract URL upload and notify tutor & admin
                        await HandleContractUploadAndNotifyAsync(currentUser.Id, tutorLearnerSubjectId, learnerDto.ContractUrl);
                    }
                    catch (Exception ex)
                    {
                        // Rollback if an error occurs
                        await transaction.RollbackAsync();
                        throw new Exception("Error during learner update process: " + ex.Message, ex);
                    }
                }
            });
        }


        public async Task HandleContractUploadAndNotifyAsync(Guid tutorId, int tutorLearnerSubjectId, string contractUrl)
        {
            try
            {
                // Kiểm tra nếu có contractUrl, tìm lớp học và cập nhật contractUrl
                if (!string.IsNullOrEmpty(contractUrl))
                {
                    // Tìm lớp học dựa trên tutorLearnerSubjectId
                    var existingTutorLearnerSubject = await _unitOfWork.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(tutorLearnerSubjectId);

                    // Kiểm tra nếu lớp học tồn tại
                    if (existingTutorLearnerSubject == null)
                    {
                        throw new Exception("Lớp học không tồn tại hoặc bạn không có quyền chỉnh sửa lớp học này.");
                    }

                    // Cập nhật contractUrl và ngày cập nhật
                    existingTutorLearnerSubject.ContractUrl = contractUrl;
                    existingTutorLearnerSubject.IsContractVerified = false; // Thêm ngày cập nhật hợp đồng
                    existingTutorLearnerSubject.Status = "Chờ xác thực";
                    existingTutorLearnerSubject.UpdatedDate = DateTime.UtcNow;
                    existingTutorLearnerSubject.UpdatedBy = tutorId;    

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _unitOfWork.TutorLearnerSubject.Update(existingTutorLearnerSubject);
                    await _unitOfWork.CommitAsync();

                    // Gửi thông báo cho tutor về hợp đồng mới
                    var notificationToTutor = new NotificationRequestDto
                    {
                        UserId = tutorId,
                        Title = "Lớp học đã được cập nhật hợp đồng",
                        Description = "Quản trị viên sẽ xét duyệt hợp đồng sau 1 ngày",
                        NotificationType = NotificationType.TutorLearnerSubjectContractPending,
                        Href = $"/user/teaching-classrooms/{tutorLearnerSubjectId}/contract/"
                    };

                    // Gửi thông báo tới Tutor
                    await _notificationService.SendNotificationAsync(notificationToTutor, false);

                    // Gửi thông báo cho Admin về hợp đồng chờ duyệt
                    await NotifyAdminForPendingTutorLearnerSubjectContract();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during contract upload and notification process: " + ex.Message, ex);
            }
        }



        public async Task NotifyAdminForPendingTutorLearnerSubjectContract()
        {
            // Đếm số lượng gia sư chờ phê duyệt
            var pendingCount = _unitOfWork.TutorLearnerSubject
                .Count(t => t.IsContractVerified == null);

            if (pendingCount > 0)
            {
                var notification = new NotificationRequestDto
                {
                    Title = "Kiểm duyệt hợp đồng",
                    Description = $"Hiện có {pendingCount} hợp đồng cần được kiểm duyệt.",
                    NotificationType = NotificationType.AdminContractApproval,
                    Href = "/admin/contracts", // Đường dẫn đến danh sách chờ
                };

                await _notificationService.SendNotificationAsync(notification, true);
            }
        }


        public async Task<List<SubjectDetailDto>> GetSubjectDetailsByUserIdAsync(Guid userId, string viewType)
        {
            // Validate the user and determine their role
            var currentUser = await _userManager.FindByIdAsync(userId.ToString());

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            bool isLearner = await _userManager.IsInRoleAsync(currentUser, AccountRoles.Learner);
            bool isTutor = await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor);

            // Ensure user has a valid role before proceeding
            if (!isLearner && !isTutor)
            {
                throw new UnauthorizedAccessException("User is neither a learner nor a tutor!");
            }

            // Determine the list of TutorLearnerSubjects based on role input
            List<TutorLearnerSubject> tutorLearnerSubjects;
            try
            {
                if (viewType == "viewLearners" && isTutor)
                {
                    // Get requests made by learners to the tutor
                    tutorLearnerSubjects = await _unitOfWork.TutorLearnerSubject
                        .GetSubjectsByUserIdAndRoleAsync(userId, false, true, false);
                }
                else if (viewType == "viewTutors" && isLearner)
                {
                    // Get requests made by the learner to tutors
                    tutorLearnerSubjects = await _unitOfWork.TutorLearnerSubject
                        .GetSubjectsByUserIdAndRoleAsync(userId, true, false, false);
                }
                else
                {
                    throw new UnauthorizedAccessException("Role mismatch with user's actual role.");
                }

                // Prepare the subject details list
                var subjectDetails = new List<SubjectDetailDto>();

                // Fetch location names asynchronously for each subject
                foreach (var tls in tutorLearnerSubjects)
                {
                    string fullLocation = "No Address";

                    if (tls.Location != null)
                    {
                        // Attempt to fetch the full address as a single string
                        try
                        {
                            // Fetch full address as a string directly
                            fullLocation = await _apiAddress.GetFullAddressByAddressesIdAsync(tls.Location, tls.DistrictId, tls.WardId);
                        }
                        catch (Exception addressEx)
                        {
                            // Log or handle any errors from GetFullAddressByAddressIdAsync
                            fullLocation = "Address fetch error";
                        }
                    }

                    // Concatenate full address with LocationDetail
                    string fullLocationDetail = $"{tls.LocationDetail}, {fullLocation}";

                    // Add the result to the subjectDetails list
                    subjectDetails.Add(new SubjectDetailDto
                    {
                        TutorLearnerSubjectId = tls.TutorLearnerSubjectId,
                        SubjectName = tls.TutorSubject?.Subject?.SubjectName ?? "N/A",
                        Rate = tls.TutorSubject?.Rate ?? 0m,
                        Location = fullLocationDetail, // Updated Location with full address and detail
                        ExpectedStartDate = tls.ExpectedStartDate,
                        HoursPerSession = tls.HoursPerSession ?? 0,
                        LocationDetail = fullLocationDetail, // Full location detail
                        PricePerHour = tls.TutorSubject?.Rate ?? 0m,
                        SessionsPerWeek = tls.SessionsPerWeek ?? 0,
                        IsVerify = tls.IsVerified,
                        LearnerId = tls.LearnerId ?? Guid.Empty,
                        TutorId = tls.TutorSubject.TutorId ?? Guid.Empty,
                        IsClosed = tls.IsCloseClass
                    });
                }

                return subjectDetails;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching subject details: " + ex.Message, ex);
            }
        }

        

        public async Task<List<SubjectDetailDto>> GetClassroomsByUserIdAsync(Guid userId, string viewType)
        {
            // Validate the user and determine their role
            var currentUser = await _userManager.FindByIdAsync(userId.ToString());

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            bool isLearner = await _userManager.IsInRoleAsync(currentUser, AccountRoles.Learner);
            bool isTutor = await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor);

            // Ensure user has a valid role before proceeding
            if (!isLearner && !isTutor)
            {
                throw new UnauthorizedAccessException("User is neither a learner nor a tutor!");
            }

            // Determine the list of TutorLearnerSubjects based on role input
            List<TutorLearnerSubject> tutorLearnerSubjects;
            try
            {
                if (viewType == "viewLearners" && isTutor)
                {
                    // Get requests made by learners to the tutor
                    tutorLearnerSubjects = await _unitOfWork.TutorLearnerSubject
                        .GetSubjectsByUserIdAndRoleAsync(userId, false, true, true);
                }
                else if (viewType == "viewTutors" && isLearner)
                {
                    // Get requests made by the learner to tutors
                    tutorLearnerSubjects = await _unitOfWork.TutorLearnerSubject
                        .GetSubjectsByUserIdAndRoleAsync(userId, true, false, true);
                }
                else
                {
                    throw new UnauthorizedAccessException("Role mismatch with user's actual role.");
                }

                // Prepare the subject details list
                var subjectDetails = new List<SubjectDetailDto>();

                // Fetch location names asynchronously for each subject
                foreach (var tls in tutorLearnerSubjects)
                {
                    string fullLocation = "No Address";

                    if (tls.Location != null)
                    {
                        // Attempt to fetch the full address as a single string
                        try
                        {
                            // Fetch full address as a string directly
                            fullLocation = await _apiAddress.GetFullAddressByAddressesIdAsync(tls.Location, tls.DistrictId, tls.WardId);
                        }
                        catch (Exception addressEx)
                        {
                            // Log or handle any errors from GetFullAddressByAddressIdAsync
                            fullLocation = "Address fetch error";
                        }
                    }

                    // Concatenate full address with LocationDetail
                    string fullLocationDetail = $"{tls.LocationDetail}, {fullLocation}";

                    // Add the result to the subjectDetails list
                    subjectDetails.Add(new SubjectDetailDto
                    {
                        TutorLearnerSubjectId = tls.TutorLearnerSubjectId,
                        SubjectName = tls.TutorSubject?.Subject?.SubjectName ?? "N/A",
                        Rate = tls.TutorSubject?.Rate ?? 0m,
                        Location = fullLocationDetail, // Updated Location with full address and detail
                        ExpectedStartDate = tls.ExpectedStartDate,
                        HoursPerSession = tls.HoursPerSession ?? 0,
                        LocationDetail = fullLocationDetail, // Full location detail
                        PricePerHour = tls.TutorSubject?.Rate ?? 0m,
                        SessionsPerWeek = tls.SessionsPerWeek ?? 0,
                        IsVerify = tls.IsVerified,
                        LearnerId = tls.LearnerId ?? Guid.Empty,
                        TutorId = tls.TutorSubject.TutorId ?? Guid.Empty,
                        ContractUrl = tls.ContractUrl,
                        IsClosed = tls.IsCloseClass
                    });
                }

                return subjectDetails;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching subject details: " + ex.Message, ex);
            }
        }


        public async Task<bool> VerifyTutorLearnerContractAsync(int tutorLearnerSubjectId, ClaimsPrincipal user, bool isVerified, string? reason)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            // Kiểm tra nếu người dùng không tồn tại
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User not found or not authorized.");
            }

            // Lấy đối tượng từ database
            var tutorLearnerSubject = await _unitOfWork.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(tutorLearnerSubjectId) ;

            if (tutorLearnerSubject == null)
            {
                throw new Exception("TutorLearnerSubject not found.");
            }

            // Cập nhật trạng thái xác minh hợp đồng
            tutorLearnerSubject.IsContractVerified = isVerified;
            tutorLearnerSubject.Status = isVerified ? "Verified" : "Unverified"; // Trạng thái mới
            tutorLearnerSubject.ContractNote = reason ?? "";
            tutorLearnerSubject.UpdatedDate = DateTime.UtcNow;
            tutorLearnerSubject.UpdatedBy = currentUser.Id;

            // Cập nhật đối tượng trong database
            _unitOfWork.TutorLearnerSubject.Update(tutorLearnerSubject);

            // Lưu các thay đổi vào database
            await _unitOfWork.CommitAsync();

            var notificationToTutor = new NotificationRequestDto
            {
                UserId = tutorLearnerSubject.TutorSubject.TutorId ?? new Guid(),
                Title = isVerified ? $"Hợp đồng của lớp số {tutorLearnerSubjectId} đã được kiểm duyệt" : $"Hợp đồng của lớp số {tutorLearnerSubjectId} đã bị từ chối",
                Description = isVerified ? "Từ giờ bạn có thể lưu số buổi học" : reason,
                NotificationType = NotificationType.TutorLearnerSubjectContractResult,
                Href = $"/user/teaching-classrooms/{tutorLearnerSubjectId}/contract/"
            };

            await _notificationService.SendNotificationAsync(notificationToTutor, false);

            return true; // Trả về kết quả xác minh thành công
        }



        public async Task<PagedResult<ContractDto>> GetListContractAsync(
            ContractFilterDto filter,
            int pageIndex = 0,
            int pageSize = 20)
        {
            Expression<Func<TutorLearnerSubject, bool>> predicate = request =>
               // Điều kiện lọc theo tên gia sư (TutorName)
               (string.IsNullOrEmpty(filter.Search)
                   || request.TutorSubject.Subject.SubjectName.ToLower().Contains(filter.Search.ToLower())
                   || request.TutorLearnerSubjectId.ToString().ToLower().Contains(filter.Search.ToLower())
                   || request.TutorSubject.Tutor.TutorNavigation.Fullname.ToLower().Contains(filter.Search.ToLower()) &&

               // Điều kiện lọc theo ID yêu cầu thanh toán (PaymentRequestId)
               (!filter.IsVerified.HasValue || request.IsVerified == filter.IsVerified)) ;

            // Bao gồm các bảng liên quan
            var includes = new[] { "TutorSubject.Subject", "TutorSubject.Tutor.TutorNavigation" };

            // Lấy danh sách hợp đồng với phân trang
            int totalRecords;
            var contracts = _unitOfWork.TutorLearnerSubject.GetMultiPaging(
                filter: predicate,
                out totalRecords,
                pageIndex,
                pageSize,
                includes
            );

            // Ánh xạ danh sách hợp đồng sang DTO
            var contractDtos = contracts.Select(contract => new ContractDto
            {
                ContractId = contract.TutorLearnerSubjectId,
                ClassName = contract.TutorSubject.Subject.SubjectName ?? "N/A",
                TutorName = contract.TutorSubject?.Tutor?.TutorNavigation.Fullname ?? "N/A",
                ContractImg = contract.ContractUrl,
                Rate = contract.TutorSubject?.Rate ?? 0,
                StartDate = contract.ExpectedStartDate,
                IsVerified = contract.IsContractVerified,
            }).ToList();

            // Trả về dữ liệu phân trang
            return await Task.FromResult(new PagedResult<ContractDto>
            {
                Items = contractDtos,
                TotalRecords = totalRecords
            });
        }

        public async Task<TutorLearnerSubjectSummaryDetailDto> GetTutorLearnerSubjectSummaryDetailByIdAsync(
    int tutorLearnerSubjectId,
    ClaimsPrincipal user)
        {
            // Validate the user
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy thông tin chi tiết của TutorLearnerSubject
            TutorLearnerSubject tls =  _unitOfWork.TutorLearnerSubject.GetSingleByCondition(
                tls => tls.TutorLearnerSubjectId == tutorLearnerSubjectId,
                includes: new[]
                {
            "TutorSubject.Subject",
            "TutorSubject.Tutor.TutorNavigation",
            "BillingEntries.Bill",
            "Learner"
                }
            );

            if (tls == null)
            {
                throw new Exception("Tutor Learner Subject not found");
            }

            // Lấy lịch trình đã đăng ký từ bảng Schedule
            var schedules = _unitOfWork.schedule
                .GetMulti(s => s.TutorLearnerSubjectId == tutorLearnerSubjectId)
                .Select(s => new ScheduleDTO
                {
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

            // Tính tổng số buổi đã học từ bảng BillingEntry
            var totalSessionsCompleted = _unitOfWork.BillingEntry
                .GetMulti(be => be.TutorLearnerSubjectId == tutorLearnerSubjectId && !be.IsDraft)
                .Count();

            // Tính tổng tiền đã thanh toán từ bảng Payment
            var totalPaidAmount = _unitOfWork.Payment
                .GetTotalPaidAmountByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId);

            // Map dữ liệu vào DTO tổng hợp
            var result = new TutorLearnerSubjectSummaryDetailDto
            {
                TutorLearnerSubjectId = tutorLearnerSubjectId,
                TutorSubjectId = tls.TutorSubjectId,
                LearnerId = tls.LearnerId ?? Guid.Empty,
                TutorId = tls.TutorSubject?.TutorId ?? Guid.Empty,
                CityId = tls.Location,
                DistrictId = tls.DistrictId,
                WardId = tls.WardId,
                LocationDetail = tls.LocationDetail,
                PricePerHour = tls.TutorSubject?.Rate ?? 0m,
                Notes = tls.Notes,
                SessionsPerWeek = tls.SessionsPerWeek ?? 0,
                HoursPerSession = tls.HoursPerSession ?? 0,
                PreferredScheduleType = tls.PreferredScheduleType?.Trim(),
                ExpectedStartDate = tls.ExpectedStartDate,
                IsVerified = tls.IsVerified,
                Schedules = schedules,
                ContractUrl = tls.ContractUrl,
                IsContractVerified = tls.IsContractVerified,
                IsClosed = tls.IsCloseClass,

                // Thông tin bổ sung từ ClassSummary
                ClassType = tls.PreferredScheduleType,
                SubjectName = tls.TutorSubject?.Subject?.SubjectName ?? "N/A",
                TotalSessionsCompleted = totalSessionsCompleted,
                LearnerEmail = tls.Learner.Email,
                TotalPaidAmount = totalPaidAmount,
            };

            return result;
        }



        public async Task<bool> CreateClassForLearnerAsync(CreateClassDTO classDto, int tutorRequestId, ClaimsPrincipal user)
        {
            // Get the current logged-in user
            var currentUser = await _userManager.GetUserAsync(user);

            // Check if the user is a tutor
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor))
            {
                throw new UnauthorizedAccessException("User is not a tutor!");
            }

            // Check if the learner exists using their ID
            var learner = await _userManager.FindByIdAsync(classDto.LearnerId.ToString());
            if (learner == null)
            {
                throw new Exception("Learner not found.");
            }

            // Retrieve ExecutionStrategy from DbContext or UnitOfWork
            var strategy = _unitOfWork.TutorLearnerSubject.GetExecutionStrategy(); // Adjust based on your implementation

            // Execute operations within the ExecutionStrategy to handle transient errors
            await strategy.ExecuteAsync(async () =>
            {
                // Start a transaction
                using (var transaction = await _unitOfWork.TutorLearnerSubject.BeginTransactionAsync())
                {
                    try
                    {
                        // Create a new TutorLearnerSubject entity from the DTO
                        var newTutorLearnerSubject = new TutorLearnerSubject
                        {
                            TutorSubjectId = classDto.TutorSubjectId, // ID of the subject the tutor teaches
                            LearnerId = learner.Id, // ID of the learner
                            Location = classDto.CityId, // City or location details
                            DistrictId = classDto.DistrictId, // District ID
                            WardId = classDto.WardId, // Ward ID
                            LocationDetail = classDto.LocationDetail,
                            ContractUrl = classDto.ContractUrl, // URL for contract details
                            PricePerHour = classDto.PricePerHour, // Price per hour
                            Notes = classDto.Notes, // Additional notes
                            SessionsPerWeek = classDto.SessionsPerWeek, // Number of sessions per week
                            HoursPerSession = classDto.HoursPerSession, // Hours per session
                            PreferredScheduleType = classDto.PreferredScheduleType, // Preferred schedule type
                            ExpectedStartDate = classDto.ExpectedStartDate, // Expected start date of classes
                            CreatedDate = DateTime.UtcNow, // Record creation date
                            CreatedBy = currentUser.Id, // ID of the tutor who created this record
                            Status = "Đã xác thực", // Initial status of the class request
                            IsVerified = true,
                        };

                        // Add the new record to the database
                        _unitOfWork.TutorLearnerSubject.Add(newTutorLearnerSubject);
                        await _unitOfWork.CommitAsync(); // Commit the changes
                        var tutorLearnerSubjectId = newTutorLearnerSubject.TutorLearnerSubjectId;

                        // Adjust tutor's free time based on the learner's schedules
                        var adjustedFreeTimes = await _scheduleService.AdjustTutorSchedulesAsync(currentUser.Id, tutorLearnerSubjectId, classDto.Schedules);

                        var tutorRequestForClass = await _unitOfWork.TutorRequest.FindAsync(tr => tr.Id == tutorRequestId);

                        tutorRequestForClass.TutorLearnerSubjectId = tutorLearnerSubjectId;

                        _unitOfWork.TutorRequest.Update(tutorRequestForClass);

                        await transaction.CommitAsync();

                        // Send notification to the learner about the class creation
                        var notificationToLearner = new NotificationRequestDto
                        {
                            UserId = learner.Id,
                            Title = "Đã tạo lớp học mới",
                            Description = "Yêu cầu của bạn đã được tạo lớp mới thành công",
                            NotificationType = NotificationType.TutorLearnerSubjectResult,
                            Href = "/user/learning-classrooms"
                        };

                        await _notificationService.SendNotificationAsync(notificationToLearner, false);

                        // Optionally, send notification to the tutor about the class creation
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
                    catch (Exception ex)
                    {
                        // Rollback if an error occurs
                        await transaction.RollbackAsync();
                        throw new Exception("Error during class creation process: " + ex.Message, ex);
                    }
                }
            });

            return true; // Indicate success
        }

        public async Task<bool> CloseClassAsync(int tutorLearnerSubjectId, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User not found or not authorized.");
            }

            // Lấy đối tượng TutorLearnerSubject từ database
            var tutorLearnerSubject = await _unitOfWork.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(tutorLearnerSubjectId);
            if (tutorLearnerSubject == null)
            {
                throw new Exception("TutorLearnerSubject not found.");
            }

            // Kiểm tra xem có hóa đơn chưa thanh toán không
            var unpaidBills = _unitOfWork.BillingEntry.GetMulti(be =>
                be.TutorLearnerSubjectId == tutorLearnerSubjectId && be.BillId == null).ToList();

            // Nếu có hóa đơn chưa thanh toán, không cho phép kết thúc lớp
            if (unpaidBills.Any())
            {
                var notificationToTutorClose = new NotificationRequestDto
                {
                    UserId = tutorLearnerSubject.TutorSubject.TutorId ?? Guid.Empty,
                    Title = "Lớp chưa thể kết thúc",
                    Description = "Lớp không thể kết thúc vì còn hóa đơn chưa thanh toán. Vui lòng kiểm tra và thanh toán các hóa đơn.",
                    NotificationType = NotificationType.TutorLearnerSubjectContractResult,
                    Href = $"/user/teaching-classrooms/{tutorLearnerSubjectId}/billing/"
                };
                await _notificationService.SendNotificationAsync(notificationToTutorClose, false);

                return false; // Trả về false nếu không thể kết thúc lớp
            }

            // Cập nhật trạng thái lớp học
            tutorLearnerSubject.IsCloseClass = true;
            tutorLearnerSubject.UpdatedDate = DateTime.UtcNow;
            tutorLearnerSubject.UpdatedBy = currentUser.Id;

            // Lưu cập nhật vào cơ sở dữ liệu
            _unitOfWork.TutorLearnerSubject.Update(tutorLearnerSubject);
            await _unitOfWork.CommitAsync();

            // Gọi phương thức MergeScheduleWithFreeTime để cập nhật lịch cho gia sư
            await _scheduleService.MergeScheduleWithFreeTime(tutorLearnerSubject.TutorSubject.TutorId ?? Guid.Empty, new List<TutorRequestSchedulesDTO>());

            // Gửi thông báo cho gia sư và học viên về việc kết thúc lớp
            var notificationToLearner = new NotificationRequestDto
            {
                UserId = tutorLearnerSubject.LearnerId ?? Guid.Empty,
                Title = "Lớp học đã kết thúc",
                Description = "Lớp học của bạn với gia sư đã kết thúc. Cảm ơn bạn đã tham gia.",
                NotificationType = NotificationType.TutorLearnerSubjectContractResult,
                Href = $"/user/registered-classes"
            };
            await _notificationService.SendNotificationAsync(notificationToLearner, false);

            var notificationToTutor = new NotificationRequestDto
            {
                UserId = tutorLearnerSubject.TutorSubject.TutorId ?? Guid.Empty,
                Title = "Lớp học đã kết thúc",
                Description = "Lớp học của bạn với học viên đã kết thúc.",
                NotificationType = NotificationType.TutorLearnerSubjectContractResult,
                Href = $"/user/teaching-classrooms"
            };
            await _notificationService.SendNotificationAsync(notificationToTutor, false);

            return true; // Trả về true nếu lớp đã được kết thúc thành công
        }


    }

    public class HandleContractUploadDTO
    {
        public Guid TutorId { get; set; }
        public int TutorLearnerSubjectId { get; set; }
        public string ContractUrl { get; set; }
    }


    public class CreateClassDTO
    {
        public Guid LearnerId { get; set; } // ID of the learner
        public int TutorSubjectId { get; set; } // ID of the subject
        public string? CityId { get; set; }
        public string DistrictId { get; set; } // District ID
        public string WardId { get; set; } // Ward ID
        public string? LocationDetail { get; set; } // Địa chỉ chi tiết
        public string ContractUrl { get; set; } // URL for contract details
        public decimal PricePerHour { get; set; } // Price per hour
        public string Notes { get; set; } // Additional notes
        public int SessionsPerWeek { get; set; } // Number of sessions per week
        public int HoursPerSession { get; set; } // Hours per session
        public string PreferredScheduleType { get; set; } // Preferred schedule type
        public DateTime ExpectedStartDate { get; set; } // Expected start date of classes
        public List<TutorRequestSchedulesDTO> Schedules { get; set; } = new List<TutorRequestSchedulesDTO>();// List of schedules for the class
    }
}
