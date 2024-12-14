using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Data.Repositories;

namespace TutoRum.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private ApplicationDbContext dbContext;
        public ITutorRepository Tutors { get; private set; }
        public ISubjectRepository Subjects { get; private set; }
        public ITutorSubjectRepository TutorSubjects { get; private set; }
        public IPostRepository Posts { get; private set; }
        public IAccountRepository Accounts { get; private set; }
        public IAdminRepository Admins { get; private set; }
        public ITutorTeachingLocationsRepository tutorTeachingLocations { get; private set; }
        public ITeachingLocationRepository teachingLocation { get; private set; }
        public ICertificatesRepository Certificates { get; private set; }
        public IScheduleRepository schedule { get; private set; }
        public IPostCategoryRepository PostCategories { get; private set; }
        public ITutorRequestRepository TutorRequest { get; private set; }
        public IQualificationLevelRepository QualificationLevel { get; private set; }
        public ITutorLearnerSubjectRepository TutorLearnerSubject { get; private set; }
        public IFAQRepository Faq { get; private set; }
        public IFeedbackRepository feedback { get; private set; }
        public IBillingEntryRepository BillingEntry { get; private set; }   
        public IBillRepository Bill { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public INotificationRepository Notifications { get; private set; }
        public IUserTokenRepository UserTokens { get; private set; }
        public IPaymentRequestRepository PaymentRequest { get; private set; }
        public ITutorRequestTutorRepository TutorRequestTutor { get; private set; }
        public IRateRangeRepository RateRange { get; private set; }
        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
            Tutors = new TutorRepository(dbFactory);
            Subjects = new SubjectRepository(dbFactory);
            TutorSubjects = new TutorSubjectRepository(dbFactory);
            Posts = new PostRepository(dbFactory);
            Accounts = new AccountRepository(dbFactory);
            Admins = new AdminRepository(dbFactory);
            teachingLocation = new TeachingLocationRepository(dbFactory);
            tutorTeachingLocations = new TutorTeachingLocationsRepository(dbFactory);
            Certificates = new CertificatesRepository(dbFactory);
            schedule = new ScheduleRepository(dbFactory);
            PostCategories = new PostCategoryRepository(dbFactory);
            TutorRequest = new TutorRequestRepository(dbFactory);
            TutorLearnerSubject = new TutorLearnerSubjectRepository(dbFactory);
            Faq = new FAQRepository(dbFactory);
            QualificationLevel = new QualificationLevelRepository(dbFactory);
            feedback = new FeedbackRepository(dbFactory);
            BillingEntry = new BillingEntryRepository(dbFactory);   
            Bill = new BillRepository(dbFactory);
            Payment = new PaymentRepository(dbFactory);
            Notifications = new NotificationRepository(dbFactory);
            UserTokens = new UserTokenRepository(dbFactory);
            PaymentRequest = new PaymentRequestRepository(dbFactory);
            TutorRequestTutor = new TutorRequestTutorRepository(dbFactory); 
            RateRange = new RateRangeRepository(dbFactory);
        }

        public ApplicationDbContext DbContext
        {
            get { return dbContext ?? (dbContext = dbFactory.Init()); }
        }


        public void Commit()
        {
            DbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }
    }
}