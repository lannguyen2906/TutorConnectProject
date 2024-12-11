using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;

namespace TutoRum.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        ITutorRepository Tutors { get; }
        ISubjectRepository Subjects { get; }
        ITutorSubjectRepository TutorSubjects { get; }
        IAdminRepository Admins { get; }
        IPostRepository Posts { get; }
        IAccountRepository Accounts { get; }
        ITutorTeachingLocationsRepository tutorTeachingLocations{ get; }
        ITeachingLocationRepository teachingLocation { get; }
        ICertificatesRepository Certificates { get; }
        IScheduleRepository schedule { get; }
        IPostCategoryRepository PostCategories { get; }
        ITutorRequestRepository TutorRequest { get; }
        ITutorLearnerSubjectRepository TutorLearnerSubject { get; }
        IFAQRepository Faq { get; }
        IQualificationLevelRepository QualificationLevel { get; }
        IBillingEntryRepository BillingEntry { get; }
        IFeedbackRepository feedback { get; }
        IBillRepository Bill { get; }
        IPaymentRepository Payment { get; }
        INotificationRepository Notifications { get; }
        IUserTokenRepository UserTokens { get; }
        IPaymentRequestRepository PaymentRequest { get; }
        ITutorRequestTutorRepository TutorRequestTutor { get; }
        IRateRangeRepository RateRange { get; }

        void Commit();
        Task CommitAsync();
    }
}
