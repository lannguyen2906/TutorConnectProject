using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class TeachingLocationsService : ITeachingLocationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;


        public TeachingLocationsService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task DeleteTeachingLocationAsync(Guid tutorId, int[] locationIds)
        {
            foreach (var locationId in locationIds)
            {
                // Kiểm tra xem địa điểm dạy học có tồn tại không
                var location = _unitOfWork.tutorTeachingLocations.GetSingleById(locationId);

                if (location == null)
                {
                    throw new KeyNotFoundException("Teaching location not found.");
                }

                // Kiểm tra nếu địa điểm dạy học không thuộc về gia sư này
                if (location.TutorId != tutorId)
                {
                    throw new UnauthorizedAccessException("You do not have permission to delete this teaching location.");
                }

                // Xóa địa điểm dạy học
                _unitOfWork.tutorTeachingLocations.Delete(location.TeachingLocationId);

                // Commit transaction
                await _unitOfWork.CommitAsync();
            }
        }


        public async Task AddTeachingLocationsAsync(IEnumerable<AddTeachingLocationViewDTO> teachingLocations, Guid tutorId)
        {
            if (teachingLocations != null && teachingLocations.Any())
            {
                foreach (var locationDto in teachingLocations)
                {

                    if (locationDto.Districts != null && locationDto.Districts.Any())
                    {
                        foreach (var district in locationDto.Districts)
                        {

                            var location = await _unitOfWork.teachingLocation.FindAsync(locationDto.CityId, district.DistrictId);

                            if (location == null)
                            {

                                location = new TeachingLocation
                                {
                                    CityId = locationDto.CityId,
                                    DistrictId = district.DistrictId,
                                    CreatedDate = DateTime.UtcNow
                                };

                                _unitOfWork.teachingLocation.Add(location);
                                await _unitOfWork.CommitAsync();
                            }


                            var existingTutorLocation = await _unitOfWork.tutorTeachingLocations
                               .ExistsAsync(tutorId, location.TeachingLocationId);

                            if (existingTutorLocation == false)
                            {

                                var tutorLocation = new TutorTeachingLocations
                                {
                                    TutorId = tutorId,
                                    TeachingLocationId = location.TeachingLocationId
                                };

                                _unitOfWork.tutorTeachingLocations.Add(tutorLocation);
                            }
                        }
                    }
                }


                await _unitOfWork.CommitAsync();
            }
        }

    }
}
