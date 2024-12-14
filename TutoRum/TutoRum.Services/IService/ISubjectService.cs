using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectFilterDTO>> GetAllSubjectAsync();

        Task AddSubjectsWithRateAsync(IEnumerable<AddSubjectDTO>? subjects, Guid tutorId);

        // Create Subject
        Task<Subject> CreateSubjectAsync(SubjectDTO subjectDto, ClaimsPrincipal user);

        // Get Subject by ID
        Task<Subject> GetSubjectByIdAsync(int subjectId);

        // Get all Subjects with pagination
        Task<(IEnumerable<SubjectFilterDTO>, int)> GetAllSubjectsAsync(int pageNumber, int pageSize);

        // Update Subject
        Task UpdateSubjectAsync(int subjectId, SubjectDTO subjectDto, ClaimsPrincipal user);

        // Delete Subject
        Task DeleteSubjectAsync(int subjectId, ClaimsPrincipal user);

        Task<List<SubjectFilterDTO>> GetTopSubjectsAsync(int size);

        Task DeleteTutorSubjectAsync(Guid tutorId, int subjectId);
    }
}
