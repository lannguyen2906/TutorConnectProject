using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class BillingEntryService : IBillingEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;


        public BillingEntryService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        public async Task AddBillingEntryAsync(AdddBillingEntryDTO billingEntryDTO, ClaimsPrincipal user = null)
        {
            Guid currentUserId = Guid.Empty;

            // Lấy thông tin người dùng nếu user không null
            if (user != null)
            {
                var currentUser = await _userManager.GetUserAsync(user);
                if (currentUser != null)
                {
                    currentUserId = currentUser.Id;
                }
            }

            // Kiểm tra tính hợp lệ của BillingEntry bằng IsValidBillingEntry
            if (!await IsValidBillingEntry(billingEntryDTO))
            {
                throw new InvalidOperationException("A billing entry with overlapping time exists for this subject.");
            }

            var existingEntries = await _unitOfWork.BillingEntry.GetBillingEntriesByTutorLearnerSubjectIdAsync(billingEntryDTO.TutorLearnerSubjectId);

                bool isOverlap = existingEntries.Any(entry =>
                    (billingEntryDTO.StartDateTime < entry.EndDateTime && billingEntryDTO.EndDateTime > entry.StartDateTime));

                if (isOverlap)
                {
                    throw new InvalidOperationException("A billing entry with overlapping time exists for this subject.");
                }

                // Tạo một đối tượng BillingEntry mới từ DTO
                var newBillingEntry = new BillingEntry
                {
                    TutorLearnerSubjectId = billingEntryDTO.TutorLearnerSubjectId,
                    Rate = billingEntryDTO.Rate,
                    StartDateTime = billingEntryDTO.StartDateTime,
                    EndDateTime = billingEntryDTO.EndDateTime,
                    Description = billingEntryDTO.Description,
                    TotalAmount = billingEntryDTO.TotalAmount,
                    IsDraft = false,
                    CreatedBy = currentUserId,
                    CreatedDate = DateTime.UtcNow,
                };

                // Thêm billing entry vào UnitOfWork
                _unitOfWork.BillingEntry.Add(newBillingEntry);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.CommitAsync();
            //}
        }


        public async Task AddDraftBillingEntryAsync(AdddBillingEntryDTO billingEntryDTO, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }
            else
            {
                // Tạo một đối tượng BillingEntry mới từ DTO
                var newBillingEntry = new BillingEntry
                {
                    TutorLearnerSubjectId = billingEntryDTO.TutorLearnerSubjectId,
                    Rate = billingEntryDTO.Rate,
                    StartDateTime = billingEntryDTO.StartDateTime,
                    EndDateTime = billingEntryDTO.EndDateTime,
                    Description = billingEntryDTO.Description,
                    TotalAmount = billingEntryDTO.TotalAmount,
                    IsDraft = true,
                    CreatedBy = currentUser.Id,
                    CreatedDate = DateTime.UtcNow,
                };

                // Thêm billing entry vào UnitOfWork
                _unitOfWork.BillingEntry.Add(newBillingEntry);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task UpdateBillingEntryAsync(int billingEntryId, UpdateBillingEntryDTO billingEntryDTO, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }
            else
            {
                // Lấy billing entry hiện tại từ cơ sở dữ liệu
                var existingBillingEntry = _unitOfWork.BillingEntry.GetSingleById(billingEntryId);

                if (existingBillingEntry == null)
                {
                    throw new Exception("Billing entry not found.");
                }

                // Cập nhật các thuộc tính của billing entry từ DTO
                existingBillingEntry.Rate = billingEntryDTO.Rate ?? existingBillingEntry.Rate;
                existingBillingEntry.StartDateTime = billingEntryDTO.StartDateTime ?? existingBillingEntry.StartDateTime;
                existingBillingEntry.EndDateTime = billingEntryDTO.EndDateTime ?? existingBillingEntry.EndDateTime;
                existingBillingEntry.Description = billingEntryDTO.Description ?? existingBillingEntry.Description;
                existingBillingEntry.TotalAmount = billingEntryDTO.TotalAmount ?? existingBillingEntry.TotalAmount;
                existingBillingEntry.UpdatedBy = currentUser.Id;
                existingBillingEntry.UpdatedDate = DateTime.UtcNow;

                // Lưu thay đổi vào cơ sở dữ liệu
                _unitOfWork.BillingEntry.Update(existingBillingEntry);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task DeleteBillingEntriesAsync(List<int> billingEntryIds, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy danh sách BillingEntry từ database
            var billingEntriesToDelete = _unitOfWork.BillingEntry
                .GetMulti(be => billingEntryIds.Contains(be.BillingEntryId));

            if (billingEntriesToDelete == null || !billingEntriesToDelete.Any())
            {
                throw new Exception("No billing entries found to delete.");
            }

            // Xóa từng BillingEntry trong danh sách
            foreach (var billingEntry in billingEntriesToDelete)
            {
                _unitOfWork.BillingEntry.Delete(billingEntry.BillingEntryId);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();
        }

        public async Task<BillingEntryDTOS> GetAllBillingEntriesAsync(ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy tất cả BillingEntry từ cơ sở dữ liệu với phân trang
            int totalRecords;
            var billingEntries = _unitOfWork.BillingEntry.GetMultiPaging(
                null, // Không có bộ lọc, lấy tất cả các BillingEntry
                out totalRecords,
                index: pageIndex,
                size: pageSize);

            if (billingEntries == null || !billingEntries.Any())
            {
                throw new Exception("No billing entries found.");
            }

            // Chuyển đổi các BillingEntry sang BillingEntryDTO để trả về
            var billingEntryDTOs = billingEntries.Select(billingEntry => new BillingEntryDTO
            {
                BillingEntryID = billingEntry.BillingEntryId,
                BillId = billingEntry.BillId,
                TutorLearnerSubjectId = billingEntry.TutorLearnerSubjectId,
                Rate = billingEntry.Rate,
                StartDateTime = billingEntry.StartDateTime,
                EndDateTime = billingEntry.EndDateTime,
                Description = billingEntry.Description,
                TotalAmount = billingEntry.TotalAmount,
            }).ToList();

            // Trả về đối tượng BillingEntryDTOS chứa totalRecords và danh sách BillingEntryDTO
            return new BillingEntryDTOS
            {
                totalRecords = totalRecords,
                BillingEntries = billingEntryDTOs
            };
        }



        public async Task<BillingEntryDTO> GetBillingEntryByIdAsync(int billingEntryId, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy BillingEntry từ cơ sở dữ liệu dựa trên ID
            var billingEntry = _unitOfWork.BillingEntry.GetSingleById(billingEntryId);

            if (billingEntry == null)
            {
                throw new Exception("Billing entry not found.");
            }

            // Chuyển đổi BillingEntry sang BillingEntryDTO
            var billingEntryDTO = new BillingEntryDTO
            {
                BillingEntryID = billingEntry.BillingEntryId,
                TutorLearnerSubjectId = billingEntry.TutorLearnerSubjectId,
                Rate = billingEntry.Rate,
                StartDateTime = billingEntry.StartDateTime,
                EndDateTime = billingEntry.EndDateTime,
                Description = billingEntry.Description,
                TotalAmount = billingEntry.TotalAmount,
            };

            return billingEntryDTO;
        }

        public async Task<BillingEntryDTOS> GetAllBillingEntriesByTutorLearnerSubjectIdAsync(
         int tutorLearnerSubjectId,
         ClaimsPrincipal user,
         int pageIndex = 0,
         int pageSize = 20)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy tất cả BillingEntry từ cơ sở dữ liệu dựa trên TutorLearnerSubjectId với phân trang
            int totalRecords;
            var billingEntries = _unitOfWork.BillingEntry.GetMultiPaging(
                be => be.TutorLearnerSubjectId == tutorLearnerSubjectId,
                out totalRecords,
                pageIndex,
                pageSize);

            if (billingEntries == null || !billingEntries.Any())
            {
                return new BillingEntryDTOS
                {
                    totalRecords = 0,
                    BillingEntries = null
                };
            }

            // Chuyển đổi các BillingEntry sang BillingEntryDTO
            var billingEntryDTOs = billingEntries.Select(billingEntry => new BillingEntryDTO
            {   BillingEntryID = billingEntry.BillingEntryId,
                BillId = billingEntry.BillId,
                TutorLearnerSubjectId = billingEntry.TutorLearnerSubjectId,
                Rate = billingEntry.Rate,
                StartDateTime = billingEntry.StartDateTime,
                EndDateTime = billingEntry.EndDateTime,
                Description = billingEntry.Description,
                TotalAmount = billingEntry.TotalAmount,
            }).ToList();

            // Trả về đối tượng BillingEntryDTOS chứa totalRecords và danh sách BillingEntryDTO
            return new BillingEntryDTOS
            {
                totalRecords = totalRecords,
                BillingEntries = billingEntryDTOs
            };
        }


        public async Task<BillingEntryDetailsDTO> GetBillingEntryDetailsAsync(int tutorLearnerSubjectId)
        {
            // Lấy danh sách Schedule dựa trên TutorLearnerSubjectId
            var schedules = _unitOfWork.schedule.GetMulti(s => s.TutorLearnerSubjectId == tutorLearnerSubjectId);

            if (schedules == null || !schedules.Any())
            {
                throw new Exception("Schedule not found for the specified TutorLearnerSubjectId.");
            }

            // Lấy Rate từ bảng TutorLearnerSubject dựa trên TutorLearnerSubjectId
            var tutorLearnerSubject = await _unitOfWork.TutorLearnerSubject.FindAsync(tls => tls.TutorLearnerSubjectId == tutorLearnerSubjectId);

            if (tutorLearnerSubject == null || tutorLearnerSubject.PricePerHour == null)
            {
                throw new Exception("Rate not found for the specified TutorLearnerSubjectId.");
            }

            DateTime today = DateTime.Today;
            DateTime currentTime = DateTime.Now; // Lấy thời gian hiện tại
            DayOfWeek dayOfWeek = today.DayOfWeek;
            int todayDayOfWeekInt = ((int)dayOfWeek + 1);

            // Tìm Schedule phù hợp cho ngày hôm nay và chưa kết thúc
            var todaySchedule = schedules.FirstOrDefault(item =>
                item.DayOfWeek == todayDayOfWeekInt &&
                today.Add(item.EndTime ?? TimeSpan.Zero) > currentTime // Kiểm tra EndTime lớn hơn thời gian hiện tại
            );

            // Tạo đối tượng BillingEntryDetailsDTO và gán giá trị
            return new BillingEntryDetailsDTO
            {
                StartDateTime = todaySchedule != null ? today.Add(todaySchedule.StartTime ?? TimeSpan.Zero) : DateTime.MinValue,
                EndDateTime = todaySchedule != null ? today.Add(todaySchedule.EndTime ?? TimeSpan.Zero) : DateTime.MinValue,
                Rate = tutorLearnerSubject.PricePerHour.Value
            };
        }

        public async Task<bool> IsValidBillingEntry(AdddBillingEntryDTO billingEntryDTO)
        {
            var existingEntries = await _unitOfWork.BillingEntry.GetBillingEntriesByTutorLearnerSubjectIdAsync(billingEntryDTO.TutorLearnerSubjectId);

            // Kiểm tra trùng lặp ngày và thời gian chính xác
            bool isOverlap = existingEntries.Any(entry =>
                entry.StartDateTime == billingEntryDTO.StartDateTime &&
                entry.EndDateTime == billingEntryDTO.EndDateTime);

            return !isOverlap; // Trả về true nếu không trùng lặp
        }




        public decimal CalculateTotalAmount(DateTime startDateTime, DateTime endDateTime, decimal rate)
        {
            // Tính số giờ từ khoảng thời gian
            var durationInHours = (decimal)(endDateTime - startDateTime).TotalHours;

            // Tính totalAmount
            var totalAmount = durationInHours * rate;
            return totalAmount;
        }

    }
}
