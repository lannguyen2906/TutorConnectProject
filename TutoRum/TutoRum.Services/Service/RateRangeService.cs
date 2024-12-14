using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;

namespace TutoRum.Services.Service
{
    public class RateRangeService : IRateRangeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RateRangeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // CREATE: Thêm mới một RateRange
        public async Task<RateRange> CreateRateRangeAsync(RateRange rateRange)
        {
            if (rateRange.MinRate < 0 || rateRange.MaxRate <= rateRange.MinRate)
            {
                throw new Exception("Invalid rate range values.");
            }

            _unitOfWork.RateRange.Add(rateRange);
            await _unitOfWork.CommitAsync();
            return rateRange;
        }

        // READ: Lấy danh sách tất cả RateRange
        public async Task<IEnumerable<RateRange>> GetAllRateRangesAsync()
        {
            return  _unitOfWork.RateRange.GetAll();
        }

        // READ: Lấy thông tin một RateRange theo Id
        public async Task<RateRange> GetRateRangeByIdAsync(int id)
        {
            var rateRange =  _unitOfWork.RateRange.GetSingleById(id);
            if (rateRange == null)
            {
                throw new Exception("Rate range not found.");
            }
            return rateRange;
        }

        // UPDATE: Cập nhật thông tin một RateRange
        public async Task<RateRange> UpdateRateRangeAsync(int id, RateRange updatedRateRange)
        {
            var existingRateRange =  _unitOfWork.RateRange.GetSingleById(id);
            if (existingRateRange == null)
            {
                throw new Exception("Rate range not found.");
            }

            if (updatedRateRange.MinRate < 0 || updatedRateRange.MaxRate <= updatedRateRange.MinRate)
            {
                throw new Exception("Invalid rate range values.");
            }

            existingRateRange.Level = updatedRateRange.Level;
            existingRateRange.MinRate = updatedRateRange.MinRate;
            existingRateRange.MaxRate = updatedRateRange.MaxRate;
            existingRateRange.Description = updatedRateRange.Description;

            _unitOfWork.RateRange.Update(existingRateRange);
            await _unitOfWork.CommitAsync();
            return existingRateRange;
        }

        // DELETE: Xóa một RateRange theo Id
        public async Task DeleteRateRangeAsync(int id)
        {
            var rateRange =  _unitOfWork.RateRange.GetSingleById(id);
            if (rateRange == null)
            {
                throw new Exception("Rate range not found.");
            }

            _unitOfWork.RateRange.Delete(rateRange.Id);
            await _unitOfWork.CommitAsync();
        }
    }

}
