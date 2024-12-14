using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class SubjectService : ISubjectService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;

        public SubjectService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<IEnumerable<SubjectFilterDTO>> GetAllSubjectAsync()
        {
            // Retrieve all post Subjects from the database
            var postSubjects = _unitOfWork.Subjects.GetAll();
            var verifiedSubjects = postSubjects
         .Where(subject => subject.TutorSubjects
           .Any(tutorSubject => tutorSubject.Tutor.IsVerified == true));

            if (postSubjects == null || !postSubjects.Any())
            {
                throw new Exception("No post categories found.");
            }

            var postSubjectsDTO = new List<SubjectFilterDTO>();

            foreach (var postSubject in postSubjects)
            {
                postSubjectsDTO.Add(new SubjectFilterDTO
                {
                    SubjectId = postSubject.SubjectId,
                    SubjectName = postSubject.SubjectName,
                    NumberOfUsages = _unitOfWork.TutorSubjects.Count(t => t.Subject.SubjectId == postSubject.SubjectId),
                });
            }

            return postSubjectsDTO;
        }


        public async Task AddSubjectsWithRateAsync(IEnumerable<AddSubjectDTO>? subjects, Guid tutorId)
        {
            if (subjects != null && subjects.Any())
            {
                foreach (var subjectDto in subjects)
                {
                    Subject? existingSubject = null;

                    if (!string.IsNullOrEmpty(subjectDto.SubjectName))
                    {
                        // Normalize the subject name to a standard form for consistent comparison
                        string normalizedSubjectName = subjectDto.SubjectName
                            .ToUpperInvariant() // Make case-insensitive
                            .Normalize(NormalizationForm.FormD); // Normalize Unicode characters

                        // Load all subjects into memory and perform comparison
                        var allSubjects = _unitOfWork.Subjects.GetAll();

                        existingSubject = allSubjects.FirstOrDefault(s =>
                            s.SubjectName.ToUpperInvariant().Normalize(NormalizationForm.FormD) == normalizedSubjectName
                        );
                    }

                    if (existingSubject == null && !string.IsNullOrEmpty(subjectDto.SubjectName))
                    {
                        // If subject does not exist, create a new subject
                        var newSubject = new Subject
                        {
                            SubjectName = subjectDto.SubjectName,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = tutorId
                        };

                        _unitOfWork.Subjects.Add(newSubject);
                        await _unitOfWork.CommitAsync();

                        // Set the new subject as the existing subject
                        existingSubject = newSubject;
                    }

                    // Link subject with tutor and add Rate + Description
                    if (existingSubject != null)
                    {
                        var tutorSubject = new TutorSubject
                        {
                            TutorId = tutorId,
                            SubjectId = existingSubject.SubjectId,
                            Rate = subjectDto.Rate ?? 0,
                            Description = subjectDto.Description,
                            SubjectType = subjectDto.SubjectType,
                            CreatedDate = DateTime.UtcNow,
                            RateRangeId = subjectDto.RateRangeId,
                            Status = "Chờ xác thực"
                        };

                        _unitOfWork.TutorSubjects.Add(tutorSubject);
                    }
                }

                await _unitOfWork.CommitAsync();
            }
        }
        public async Task DeleteTutorSubjectAsync(Guid tutorId, int subjectId)
        {
            // Kiểm tra xem subject có tồn tại không
            var subject = _unitOfWork.Subjects.GetSingleById(subjectId);
            if (subject == null)
            {
                throw new KeyNotFoundException("Subject not found.");
            }


            _unitOfWork.TutorSubjects.Delete(subject.SubjectId);

            // Lưu thay đổi
            await _unitOfWork.CommitAsync();
        }


        public async Task UpdateSubjectsAsync(IEnumerable<UpdateSubjectDTO> subjects, Guid tutorId)
        {
            if (subjects != null && subjects.Any())
            {
                foreach (var subjectDto in subjects)
                {

                    var existingSubject = await _unitOfWork.Subjects.FindAsync(s => s.SubjectId == subjectDto.SubjectId);

                    if (existingSubject != null)
                    {

                        if (!string.IsNullOrEmpty(subjectDto.SubjectName) && existingSubject.SubjectName != subjectDto.SubjectName)
                        {
                            existingSubject.SubjectName = subjectDto.SubjectName;
                        }


                        if (subjectDto.Rate.HasValue)
                        {
                            var tutorSubject = await _unitOfWork.TutorSubjects.FindAsync(ts => ts.TutorId == tutorId && ts.SubjectId == existingSubject.SubjectId);

                            if (tutorSubject != null)
                            {
                                tutorSubject.RateRangeId = subjectDto.RateRangeId;
                                tutorSubject.Rate = subjectDto.Rate.Value;
                            }
                        }


                        if (!string.IsNullOrEmpty(subjectDto.Description))
                        {
                            var tutorSubject = await _unitOfWork.TutorSubjects.FindAsync(ts => ts.TutorId == tutorId && ts.SubjectId == existingSubject.SubjectId);

                            if (tutorSubject != null)
                            {
                                tutorSubject.Description = subjectDto.Description;
                            }
                        }


                        existingSubject.UpdatedDate = DateTime.UtcNow;
                        existingSubject.UpdatedBy = tutorId;


                        _unitOfWork.Subjects.Update(existingSubject);
                    }
                    else
                    {
                        throw new Exception($"Subject with ID {subjectDto.SubjectId} not found.");
                    }
                }

                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<List<SubjectFilterDTO>> GetTopSubjectsAsync(int size)
        {
            // Lấy tất cả lớp học và nhóm theo môn học
            var classrooms = await _unitOfWork.TutorLearnerSubject.GetAllTutorLearnerSubjectAsync();

            var subjects = classrooms
                .GroupBy(c => c.TutorSubject.SubjectId) // Nhóm theo SubjectId
                .Select(group => new
                {
                    SubjectId = group.Key,
                    ClassCount = group.Count() // Đếm số lớp học cho mỗi môn
                })
                .OrderByDescending(s => s.ClassCount) // Sắp xếp giảm dần theo số lượng lớp học
                .Take(size) // Lấy top N môn học
                .ToList();

            // Map kết quả sang DTO
            var topSubjects = subjects.Select(subject => new SubjectFilterDTO
            {
                SubjectId = subject.SubjectId,
                SubjectName = _unitOfWork.Subjects.GetSingleById(subject.SubjectId).SubjectName,
            }).ToList();

            return topSubjects;
        }

        // CRUD Operation for Subject
        public async Task<Subject> CreateSubjectAsync(SubjectDTO subjectDto, ClaimsPrincipal user)
        {
            if (subjectDto == null || string.IsNullOrEmpty(subjectDto.SubjectName))
            {
                throw new ArgumentException("Subject name is required.");
            }

            var currentUser = await _userManager.GetUserAsync(user);

            // Check if the subject already exists
            var existingSubject = await _unitOfWork.Subjects.FindAsync(s => s.SubjectName.ToUpper() == subjectDto.SubjectName.ToUpper());

            if (existingSubject != null)
            {
                throw new Exception("Subject already exists.");
            }

            // Create a new subject
            var newSubject = new Subject
            {
                SubjectName = subjectDto.SubjectName,
                IsVerified = true,
                CreatedBy = currentUser.Id,
                CreatedDate = DateTime.UtcNow
            };

            _unitOfWork.Subjects.Add(newSubject);
            await _unitOfWork.CommitAsync();

            return newSubject;
        }

        public async Task<Subject> GetSubjectByIdAsync(int subjectId)
        {
            var subject = await _unitOfWork.Subjects.FindAsync(s => s.SubjectId == subjectId);
            if (subject == null)
            {
                throw new Exception($"Subject with ID {subjectId} not found.");
            }
            return subject;
        }

        public async Task<(IEnumerable<SubjectFilterDTO>, int)> GetAllSubjectsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page number and page size must be greater than zero.");
            }

            // Define the filter expression (can be adjusted as needed)
            Expression<Func<Subject, bool>> filter = subject => true;  // No filter by default, retrieve all

            // Get the paginated data using GetMultiPaging
            int total = 0;
            var subjects = _unitOfWork.Subjects.GetMultiPaging(
                filter,
                out total,
                pageNumber - 1, // Adjust for zero-based index
                pageSize
            ).ToList();

            // Map subjects to DTOs
            var subjectDTOs = subjects.Select(subject => new SubjectFilterDTO
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName
            }).ToList();

            // Return both the paginated data and the total count
            return (subjectDTOs, total);
        }

        public async Task UpdateSubjectAsync(int subjectId, SubjectDTO subjectDto, ClaimsPrincipal user)
        {
            if (subjectDto == null)
            {
                throw new ArgumentException("Subject data is required.");
            }

            // Find the existing subject
            var existingSubject = await _unitOfWork.Subjects.FindAsync(s => s.SubjectId == subjectId);
            if (existingSubject == null)
            {
                throw new Exception($"Subject with ID {subjectId} not found.");
            }
            var currentUser = await _userManager.GetUserAsync(user);

            // Update properties
            existingSubject.SubjectName = subjectDto.SubjectName ?? existingSubject.SubjectName;
            existingSubject.IsVerified = true;
            existingSubject.UpdatedBy = currentUser.Id;
            existingSubject.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Subjects.Update(existingSubject);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteSubjectAsync(int subjectId, ClaimsPrincipal user)
        {
            var subject = await _unitOfWork.Subjects.FindAsync(s => s.SubjectId == subjectId);
            if (subject == null)
            {
                throw new Exception($"Subject with ID {subjectId} not found.");
            }
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }
            subject.IsDelete = true;
            subject.UpdatedBy = currentUser.Id;
            subject.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.CommitAsync();
        }


    }
}

