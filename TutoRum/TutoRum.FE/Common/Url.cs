namespace TutoRum.FE.Common
{
    public static class Url
    {
        public static class User
        {
            public const string UpdateProfile = "UpdateProfile";

            public static class Identity
            {

                public const string SignIn = "SignIn";
                public const string SignOut = "SignOut";
                public const string SignUp = "SignUp";
                public const string SendEmailConfirmation = "SendEmailConfirmation";
                public const string ConfirmEmail = "ConfirmEmail";
                public const string ForgotPassword = "ForgotPassword";
                public const string ResetPassword = "ResetPassword";
                public const string ShowResetPasswordForm = "ShowResetPasswordForm";
                public const string ViewAllAccounts = "ViewAllAccounts";
                public const string BlockUser = "BlockUser";
                public const string UnblockUser = "UnblockUser";
                public const string ChangePassword = "ChangePassword";
            }


            public static class Tutor  
            {
                public const string AddSubjects = "AddSubjects";
                public const string UpdateSubjects = "UpdateSubjects";
                public const string GetSubjects = "GetSubjects";
                public const string DeleteSubjects = "DeleteSubjects";

                public const string RegisterTutor = "RegisterTutor";
                public const string DeleteTutor = "DeleteTutor";
                public const string AddTeachingLocations = "AddTeachingLocations";
                public const string GetTeachingLocations = "GetTeachingLocations";
                public const string DeleteTeachingLocation = "DeleteTeachingLocation";
                public const string GetTutorById = "GetTutorById";
                public const string TutorHomePage = "TutorHomePage";
                public const string UpdateTutor = "UpdateTutor";
                public const string  MajorsWithMinor = "MajorsWithMinor";
                public const string VerifyCertificate = "VerifyCertificate";
                
            }

            public static class RateRange
            {
                public const string Create = "CreateRateRange";
                public const string GetAll = "GetAllRateRanges";
                public const string GetById = "GetRateRangeById";
                public const string Update = "UpdateRateRange";
                public const string Delete = "DeleteRateRange";
            }

            public static class LearnerSubject
            {
                public const string AddLearnerSubject = "AddLearnerSubject";

            }

            public static class Admin 
            {
                public const string AssignRoleAdmin = "AssignRoleAdmin";
                public const string GetAdminListByTutor = "GetAdminListByTutor";
                public const string VerificationStatus = "VerificationStatus";
                public const string GetAdminMenuAction = "GetAdminMenuAction";

                
            }

            public static class Post
            {
                public const string AddPost = "AddPost";
                public const string UpdatePost = "UpdatePost";
                public const string DeletePost = "DeletePost";
                public const string GetAllPost = "GetAllPost";
                public const string GetPostById = "GetPostById";
                public const string GetAllPostCategories = "GetAllPostCategories";
                public const string GetPostsHomePage = "GetPostsHomePage";



            }
            public static class TutorRequest
            {
                public const string GetAllTutorRequests = "GetAllTutorRequests";
                public const string GetTutorRequestById = "GetTutorRequestById";
                public const string CreateTutorRequest = "CreateTutorRequest";
                public const string UpdateTutorRequest = "UpdateTutorRequest";
                public const string ChooseTutorForTutorRequestAsync = "ChooseTutorForTutorRequestAsync";
                public const string AddTutorToRequest = "RegisterTutorRequest";
                public const string GetListTutorsByTutorRequest = "GetListTutorRequestRegistered";
                public const string GetListTutorRequestsByLeanrerID = "GetListTutorRequestsByLeanrerID";
                public const string GetListTutorRequestsByTutorID = "GetListTutorRequestsByTutorID";
                public const string GetTutorRequestsAdmin = "GetTutorRequestsAdmin";
                public const string GetTutorLearnerSubjectInfoByTutorRequestId = "GetTutorLearnerSubjectInfoByTutorRequestId";
                public const string GetListTutorsByLeanrerID = "GetListTutorsByLeanrerID";
                public const string CloseTutorRequest = "CloseTutorRequest";
                

            }

            public static class BillingEntry
            {
                public const string AddBillingEntry = "AddBillingEntry";
                public const string AddDraftBillingEntry = "AddDraftBillingEntry";
                public const string UpdateBillingEntry = "UpdateBillingEntry";
                public const string DeleteBillingEntry = "DeleteBillingEntry";
                public const string GetAllBillingEntries = "GetAllBillingEntries";
                public const string GetBillingEntryById = "GetBillingEntryById";
                public const string GetAllBillingEntriesByTutorLearnerSubjectId = "GetAllBillingEntriesByTutorLearnerSubjectId";
                public const string CalculateTotalAmount = "CalculateTotalAmount";
                public const string GetBillingEntryDetails = "GetBillingEntryDetails";
                public const string ApproveBill = "ApproveBill";


                

            }

            public static class Bill
            {
                public const string GenerateBillFromBillingEntries = "GenerateBillFromBillingEntries";
                public const string GenerateBillPdf = "GenerateBillPdf";
                public const string DeleteBill = "DeleteBill";
                public const string GetAllBills = "GetAllBills";
                public const string ViewBillHtml = "ViewBillHtml";
                public const string SendBillEmail = "SendBillEmail";
                public const string ApproveBill = "ApproveBill";

                

            }

            public static class Faq
            {
                public const string GetAllFAQs = "GetAllFAQs";
                public const string GetFAQById = "GetFAQById";
                public const string CreateFAQ = "CreateFAQ";
                public const string UpdateFAQ = "UpdateFAQ";
                public const string DeleteFAQ = "DeleteFAQ";
                public const string GetHomepageFAQs = "GetHomepageFAQs";
            }

            public static class Subject
            {
                public const string GetAllSubjects = "GetAllSubjects";
                public const string GetSubjectById = "GetSubjectById";
                public const string CreateSubject = "CreateSubject";
                public const string UpdateSubject = "UpdateSubject";
                public const string DeleteSubject = "DeleteSubject";
                public const string GetSubjectsHomePage = "GetSubjectsHomePage";



                public const string CreateSubjectForSuperAdmin = "CreateSubjectForSuperAdmin";
                public const string GetSubjectByIdForSuperAdmin = "GetSubjectByIdForSuperAdmin";
                public const string UpdateSubjectForSuperAdmin = "UpdateSubjectForSuperAdmin";
                public const string DeleteSubjectForSuperAdmin = "DeleteSubjectForSuperAdmin";

            }
            public static class Schedule
            {
                public const string GetTutorAvailableTimes = "GetTutorAvailableTimes";
            }
            public static class QualificationLevel
            {
                public const string GetAllQualificationLevels = "GetAllQualificationLevels";
            }

            public static class Feedback  
            {
                public const string GetAllFeedbacks = "GetAllFeedbacks";
                public const string CreateFeedback = "CreateFeedback";
                public const string UpdateFeedback = "UpdateFeedback";
                public const string GetFeedbacksByLearnerId = "GetFeedbacksByLearnerId";
                public const string GetStatistics = "GetStatistics";


                
            }

            public static class Notification
            {
                public const string GetAllNotifications = "GetAllNotifications";
                public const string SendNotification = "SendNotification";
                public const string SaveFCMToken = "SaveFCMToken";
                public const string MarkNotificationsAsRead = "MarkNotificationsAsRead";

            }
        }
    }
}