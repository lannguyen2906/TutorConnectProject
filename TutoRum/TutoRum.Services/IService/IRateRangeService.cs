using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Services.IService
{


    public interface IRateRangeService
    {
        /// <summary>
        /// Tạo một RateRange mới.
        /// </summary>
        /// <param name="rateRange">Đối tượng RateRange cần tạo.</param>
        /// <returns>RateRange được tạo.</returns>
        Task<RateRange> CreateRateRangeAsync(RateRange rateRange);

        /// <summary>
        /// Lấy danh sách tất cả các RateRange.
        /// </summary>
        /// <returns>Danh sách các RateRange.</returns>
        Task<IEnumerable<RateRange>> GetAllRateRangesAsync();

        /// <summary>
        /// Lấy thông tin chi tiết của một RateRange theo Id.
        /// </summary>
        /// <param name="id">Id của RateRange.</param>
        /// <returns>RateRange tương ứng với Id.</returns>
        Task<RateRange> GetRateRangeByIdAsync(int id);

        /// <summary>
        /// Cập nhật thông tin của một RateRange.
        /// </summary>
        /// <param name="id">Id của RateRange cần cập nhật.</param>
        /// <param name="updatedRateRange">Đối tượng RateRange chứa thông tin cập nhật.</param>
        /// <returns>RateRange sau khi được cập nhật.</returns>
        Task<RateRange> UpdateRateRangeAsync(int id, RateRange updatedRateRange);

        /// <summary>
        /// Xóa một RateRange theo Id.
        /// </summary>
        /// <param name="id">Id của RateRange cần xóa.</param>
        Task DeleteRateRangeAsync(int id);
    }

    
}
