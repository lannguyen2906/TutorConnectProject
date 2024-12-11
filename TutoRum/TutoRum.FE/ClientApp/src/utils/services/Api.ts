/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface AddDistrictDTO {
  districtId?: string | null;
}

export interface AddPostsDTO {
  title?: string | null;
  thumbnail?: string | null;
  content?: string | null;
  subContent?: string | null;
  status?: string | null;
  /** @format int32 */
  postType?: number | null;
}

export interface AddPostsDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: AddPostsDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface AddScheduleDTO {
  /** @format int32 */
  dayOfWeek?: number | null;
  freeTimes?: FreeTimeDTO[] | null;
}

export interface AddSubjectDTO {
  subjectName?: string | null;
  /** @format double */
  rate?: number | null;
  description?: string | null;
  subjectType?: string | null;
  /** @format int32 */
  rateRangeId?: number | null;
}

export interface AddTeachingLocationViewDTO {
  cityId?: string | null;
  districts?: AddDistrictDTO[] | null;
}

export interface AddTutorDTO {
  experience?: string | null;
  specialization?: string | null;
  status?: string | null;
  profileDescription?: string | null;
  major?: string | null;
  briefIntroduction?: string | null;
  /** @format int32 */
  educationalLevelID?: number | null;
  shortDescription?: string | null;
  videoUrl?: string | null;
  addressID?: string | null;
  isAccepted?: boolean;
  certificates?: CertificateDTO[] | null;
  schedule?: ScheduleDTO[] | null;
  subjects?: AddSubjectDTO[] | null;
  teachingLocation?: AddTeachingLocationViewDTO[] | null;
}

export interface AddTutorDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: AddTutorDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface AdddBillingEntryDTO {
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  /** @format double */
  rate?: number | null;
  /** @format date-time */
  startDateTime?: string | null;
  /** @format date-time */
  endDateTime?: string | null;
  description?: string | null;
  /** @format double */
  totalAmount?: number | null;
}

export interface Admin {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format uuid */
  adminId?: string;
  position?: string | null;
  /** @format date-time */
  hireDate?: string | null;
  /** @format double */
  salary?: number | null;
  /** @format int32 */
  supervisorId?: number | null;
  aspNetUser?: AspNetUser;
  posts?: Post[] | null;
  faqs?: Faq[] | null;
}

export interface AdminHomePageDTO {
  tutors?: AdminTutorDto[] | null;
  /** @format int32 */
  totalRecordCount?: number;
}

export interface AdminHomePageDTOListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: AdminHomePageDTO[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface AdminMenuAction {
  href?: string | null;
  /** @format int32 */
  numberOfAction?: number | null;
}

export interface AdminMenuActionListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: AdminMenuAction[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface AdminTutorDto {
  /** @format uuid */
  tutorId?: string;
  experience?: string | null;
  specialization?: string | null;
  major?: string | null;
  briefIntroduction?: string | null;
  educationalLevel?: string | null;
  /** @format double */
  rating?: number | null;
  status?: string | null;
  profileDescription?: string | null;
  isVerified?: boolean | null;
  /** @format int32 */
  tutorQualificationId?: number | null;
  fullName?: string | null;
  avatarUrl?: string | null;
  /** @format int32 */
  addressId?: number;
  isAccepted?: boolean;
  /** @format date-time */
  createdDate?: string | null;
  certificates?: CertificateDTO[] | null;
  tutorSubjects?: TutorSubjectDto[] | null;
  teachingLocations?: TeachingLocationViewDTO[] | null;
  schedules?: ScheduleDTO[] | null;
}

export interface AnswerBreakdown {
  /** @format int32 */
  value?: number;
  answerCount?: string | null;
}

export interface ApiError {
  code?: string | null;
  detail?: string | null;
}

export interface AspNetUser {
  /** @format uuid */
  id?: string;
  userName?: string | null;
  normalizedUserName?: string | null;
  email?: string | null;
  normalizedEmail?: string | null;
  emailConfirmed?: boolean;
  passwordHash?: string | null;
  securityStamp?: string | null;
  concurrencyStamp?: string | null;
  phoneNumber?: string | null;
  phoneNumberConfirmed?: boolean;
  twoFactorEnabled?: boolean;
  /** @format date-time */
  lockoutEnd?: string | null;
  lockoutEnabled?: boolean;
  /** @format int32 */
  accessFailedCount?: number;
  fullname?: string | null;
  /** @format date-time */
  dob?: string | null;
  gender?: boolean | null;
  avatarUrl?: string | null;
  addressId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  addressDetail?: string | null;
  status?: string | null;
  admin?: Admin;
  tutor?: Tutor;
  tutorRequest?: TutorRequest[] | null;
  notifications?: Notification[] | null;
  userTokens?: UserToken[] | null;
}

export interface AssignRoleAdminDto {
  /** @format uuid */
  userId?: string;
  position?: string | null;
  /** @format date-time */
  hireDate?: string | null;
  /** @format double */
  salary?: number | null;
  /** @format int32 */
  supervisorId?: number | null;
}

export interface AssignRoleAdminDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: AssignRoleAdminDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface AuthorDTO {
  authorName?: string | null;
  avatar?: string | null;
}

export interface Bill {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  billId?: number;
  /** @format double */
  discount?: number | null;
  /** @format double */
  deduction?: number | null;
  /** @format date-time */
  startDate?: string | null;
  description?: string | null;
  /** @format double */
  totalBill?: number | null;
  status?: string | null;
  isApprove?: boolean | null;
  isPaid?: boolean;
  /** @format date-time */
  paymentDate?: string | null;
  billingEntries?: BillingEntry[] | null;
  payments?: Payment[] | null;
}

export interface BillDTO {
  /** @format int32 */
  billId?: number;
  /** @format double */
  discount?: number | null;
  /** @format date-time */
  startDate?: string | null;
  description?: string | null;
  /** @format double */
  totalBill?: number | null;
  status?: string | null;
  /** @format date-time */
  createdDate?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
}

export interface BillDTOS {
  /** @format int32 */
  totalRecords?: number;
  bills?: BillDTO[] | null;
}

export interface BillDTOSApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: BillDTOS;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface BillDetailsDTO {
  /** @format int32 */
  billId?: number;
  /** @format double */
  discount?: number | null;
  /** @format double */
  deduction?: number | null;
  /** @format date-time */
  startDate?: string | null;
  description?: string | null;
  /** @format double */
  totalBill?: number | null;
  isApprove?: boolean | null;
  isPaid?: boolean | null;
  status?: string | null;
  /** @format date-time */
  createdDate?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  learnerEmail?: string | null;
  billingEntries?: BillingEntryDTO[] | null;
}

export interface BillDetailsDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: BillDetailsDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface BillDetailsDTOPagedResult {
  items?: BillDetailsDTO[] | null;
  /** @format int32 */
  totalRecords?: number;
}

export interface BillDetailsDTOPagedResultApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: BillDetailsDTOPagedResult;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface BillingEntry {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  billingEntryId?: number;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  /** @format double */
  rate?: number | null;
  /** @format date-time */
  startDateTime?: string | null;
  /** @format date-time */
  endDateTime?: string | null;
  description?: string | null;
  /** @format double */
  totalAmount?: number | null;
  isDraft?: boolean;
  /** @format int32 */
  billId?: number | null;
  bill?: Bill;
  tutorLearnerSubject?: TutorLearnerSubject;
}

export interface BillingEntryDTO {
  /** @format int32 */
  billingEntryID?: number | null;
  /** @format int32 */
  billId?: number | null;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  /** @format double */
  rate?: number | null;
  /** @format date-time */
  startDateTime?: string | null;
  /** @format date-time */
  endDateTime?: string | null;
  description?: string | null;
  /** @format double */
  totalAmount?: number | null;
}

export interface BillingEntryDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: BillingEntryDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface BillingEntryDTOS {
  /** @format int32 */
  totalRecords?: number;
  billingEntries?: BillingEntryDTO[] | null;
}

export interface BillingEntryDTOSApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: BillingEntryDTOS;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface BillingEntryDetailsDTO {
  /** @format date-time */
  startDateTime?: string;
  /** @format date-time */
  endDateTime?: string;
  /** @format double */
  rate?: number;
}

export interface BillingEntryDetailsDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: BillingEntryDetailsDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface BooleanApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: boolean;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface CalculateTotalAmountRequest {
  /** @format date-time */
  startDateTime?: string;
  /** @format date-time */
  endDateTime?: string;
  /** @format double */
  rate?: number;
}

export interface Certificate {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  certificateId?: number;
  /** @format uuid */
  tutorId?: string | null;
  imgUrl?: string | null;
  description?: string | null;
  status?: string | null;
  /** @format date-time */
  issueDate?: string | null;
  /** @format date-time */
  expiryDate?: string | null;
  isVerified?: boolean | null;
  entityType?: string | null;
  tutor?: Tutor;
}

export interface CertificateDTO {
  /** @format int32 */
  certificateId?: number | null;
  imgUrl?: string | null;
  description?: string | null;
  isVerified?: boolean | null;
  entityType?: string | null;
  status?: string | null;
  /** @format date-time */
  issueDate?: string | null;
  /** @format date-time */
  expiryDate?: string | null;
}

export interface ChangePasswordDTO {
  /** @minLength 1 */
  currentPassword: string;
  /**
   * @minLength 6
   * @maxLength 100
   */
  newPassword: string;
  confirmPassword?: string | null;
}

export interface ContractDto {
  /** @format int32 */
  contractId?: number;
  className?: string | null;
  tutorName?: string | null;
  contractImg?: string | null;
  /** @format double */
  rate?: number;
  /** @format date-time */
  startDate?: string | null;
  isVerified?: boolean | null;
}

export interface ContractDtoPagedResult {
  items?: ContractDto[] | null;
  /** @format int32 */
  totalRecords?: number;
}

export interface ContractDtoPagedResultApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: ContractDtoPagedResult;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface CreateClassDTO {
  /** @format uuid */
  learnerId?: string;
  /** @format int32 */
  tutorSubjectId?: number;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  locationDetail?: string | null;
  contractUrl?: string | null;
  /** @format double */
  pricePerHour?: number;
  notes?: string | null;
  /** @format int32 */
  sessionsPerWeek?: number;
  /** @format int32 */
  hoursPerSession?: number;
  preferredScheduleType?: string | null;
  /** @format date-time */
  expectedStartDate?: string;
  schedules?: TutorRequestSchedulesDTO[] | null;
}

export interface CreateFeedbackDto {
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  /** @format double */
  rating?: number | null;
  comments?: string | null;
  /** @format int32 */
  punctuality?: number | null;
  /** @format int32 */
  supportQuality?: number | null;
  /** @format int32 */
  teachingSkills?: number | null;
  /** @format int32 */
  responseToQuestions?: number | null;
  /** @format int32 */
  satisfaction?: number | null;
}

export interface CreatePaymentRequestDTO {
  bankCode?: string | null;
  accountNumber?: string | null;
  fullName?: string | null;
  /** @format double */
  amount?: number;
}

export interface DecimalApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  /** @format double */
  data?: number;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface DeleteScheduleDTO {
  /** @format int32 */
  scheduleId?: number;
  /** @format int32 */
  dayOfWeek?: number | null;
  freeTimes?: FreeTimeDTO[] | null;
}

export interface DistrictDTO {
  /** @format int32 */
  teachingLocationId?: number | null;
  districtId?: string | null;
  districtName?: string | null;
}

/** @format int32 */
export enum EntityTypeName {
  Value0 = 0,
  Value1 = 1,
  Value2 = 2,
  Value3 = 3,
  Value4 = 4,
  Value5 = 5,
}

export interface Faq {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  id?: number;
  /** @format uuid */
  adminId?: string;
  question?: string | null;
  answer?: string | null;
  isActive?: boolean;
  admin?: Admin;
}

export interface FaqCreateDto {
  question?: string | null;
  answer?: string | null;
  isActive?: boolean;
}

export interface FaqDto {
  /** @format int32 */
  id?: number;
  question?: string | null;
  answer?: string | null;
  isActive?: boolean;
  /** @format date-time */
  createdDate?: string;
  /** @format date-time */
  updatedDate?: string | null;
  adminPosition?: string | null;
  adminFullname?: string | null;
}

export interface FaqDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: FaqDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface FaqDtoIEnumerableApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: FaqDto[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface FaqUpdateDto {
  /** @format int32 */
  id?: number;
  question?: string | null;
  answer?: string | null;
  isActive?: boolean;
}

export interface Feedback {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  feedbackId?: number;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  /** @format double */
  rating?: number | null;
  comments?: string | null;
  /** @format int32 */
  punctuality?: number | null;
  /** @format int32 */
  supportQuality?: number | null;
  /** @format int32 */
  teachingSkills?: number | null;
  /** @format int32 */
  responseToQuestions?: number | null;
  /** @format int32 */
  satisfaction?: number | null;
  tutorLearnerSubject?: TutorLearnerSubject;
}

export interface FeedbackDetail {
  /** @format double */
  avarageRating?: number | null;
  /** @format int32 */
  totalFeedbacks?: number | null;
  ratingsBreakdown?: Record<string, number>;
  feedbacks?: UserFeedbackDto[] | null;
}

export interface FeedbackDetailApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: FeedbackDetail;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface FeedbackDto {
  /** @format int32 */
  feedbackId?: number;
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  /** @format double */
  rating?: number | null;
  comments?: string | null;
  /** @format date-time */
  createdDate?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format int32 */
  punctuality?: number | null;
  /** @format int32 */
  supportQuality?: number | null;
  /** @format int32 */
  teachingSkills?: number | null;
  /** @format int32 */
  responseToQuestions?: number | null;
  /** @format int32 */
  satisfaction?: number | null;
}

export interface FeedbackDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: FeedbackDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface FeedbackStatisticsResponse {
  statistics?: QuestionStatistics[] | null;
  comments?: string[] | null;
}

export interface FeedbackStatisticsResponseApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: FeedbackStatisticsResponse;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface ForgotPasswordModel {
  email?: string | null;
}

export interface FreeTimeDTO {
  /** @format date-span */
  startTime?: string | null;
  /** @format date-span */
  endTime?: string | null;
}

export interface HandleContractUploadDTO {
  /** @format uuid */
  tutorId?: string;
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  contractUrl?: string | null;
}

export interface Int32ApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  /** @format int32 */
  data?: number;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface ListTutorRequestDTO {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  id?: number;
  phoneNumber?: string | null;
  requestSummary?: string | null;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  teachingLocation?: string | null;
  /** @format int32 */
  numberOfStudents?: number;
  preferredScheduleType?: string | null;
  /** @format date-span */
  timePerSession?: string;
  studentGender?: string | null;
  tutorGender?: string | null;
  /** @format double */
  fee?: number;
  /** @format int32 */
  sessionsPerWeek?: number;
  /** @format int32 */
  tutorQualificationId?: number | null;
  /** @format uuid */
  aspNetUserId?: string | null;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  tutorLearnerSubject?: TutorLearnerSubject;
  entityType?: string | null;
  aspNetUser?: AspNetUser;
  tutorQualification?: QualificationLevel;
  tutorRequestTutors?: TutorRequestTutor[] | null;
  schedules?: Schedule[] | null;
  /** @format int32 */
  rateRangeId?: number | null;
  rateRange?: RateRange;
  /** @format int32 */
  tutorRequestId?: number;
  subject?: string | null;
  /** @format date-time */
  startDate?: string;
  detailedDescription?: string | null;
  status?: string | null;
  isTutorAssigned?: boolean;
  isVerified?: boolean | null;
  freeSchedules?: string | null;
  /** @format uuid */
  chosenTutorId?: string | null;
  /** @format int32 */
  numberOfRegisteredTutor?: number;
}

export interface ListTutorRequestDTOPagedResult {
  items?: ListTutorRequestDTO[] | null;
  /** @format int32 */
  totalRecords?: number;
}

export interface ListTutorRequestForTutorDto {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  id?: number;
  phoneNumber?: string | null;
  requestSummary?: string | null;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  teachingLocation?: string | null;
  /** @format int32 */
  numberOfStudents?: number;
  /** @format date-time */
  startDate?: string;
  preferredScheduleType?: string | null;
  /** @format date-span */
  timePerSession?: string;
  subject?: string | null;
  studentGender?: string | null;
  tutorGender?: string | null;
  /** @format double */
  fee?: number;
  /** @format int32 */
  sessionsPerWeek?: number;
  detailedDescription?: string | null;
  /** @format int32 */
  tutorQualificationId?: number | null;
  /** @format uuid */
  aspNetUserId?: string | null;
  freeSchedules?: string | null;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  tutorLearnerSubject?: TutorLearnerSubject;
  isVerified?: boolean | null;
  entityType?: string | null;
  status?: string | null;
  aspNetUser?: AspNetUser;
  tutorQualification?: QualificationLevel;
  tutorRequestTutors?: TutorRequestTutor[] | null;
  schedules?: Schedule[] | null;
  /** @format int32 */
  rateRangeId?: number | null;
  rateRange?: RateRange;
  isInterested?: boolean | null;
  isChosen?: boolean | null;
}

export interface ListTutorRequestForTutorDtoPagedResult {
  items?: ListTutorRequestForTutorDto[] | null;
  /** @format int32 */
  totalRecords?: number;
}

export interface MajorMinorDto {
  major?: string | null;
  minors?: string[] | null;
}

export interface MajorMinorDtoListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: MajorMinorDto[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface Notification {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  notificationId?: number;
  /** @format uuid */
  userId?: string;
  title?: string | null;
  description?: string | null;
  href?: string | null;
  icon?: string | null;
  color?: string | null;
  isRead?: boolean;
  type?: NotificationType;
  user?: AspNetUser;
}

export interface NotificationDto {
  /** @format int32 */
  notificationId?: number;
  /** @format uuid */
  userId?: string;
  title?: string | null;
  description?: string | null;
  href?: string | null;
  icon?: string | null;
  color?: string | null;
  isRead?: boolean;
  /** @format date-time */
  createdDate?: string | null;
}

export interface NotificationDtos {
  /** @format int32 */
  totalRecords?: number;
  /** @format int32 */
  totalUnreadNotifications?: number;
  notifications?: NotificationDto[] | null;
}

export interface NotificationDtosApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: NotificationDtos;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface NotificationRequestDto {
  /** @format uuid */
  userId?: string;
  title?: string | null;
  description?: string | null;
  notificationType?: NotificationType;
  href?: string | null;
  icon?: string | null;
  color?: string | null;
}

export interface NotificationRequestDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: NotificationRequestDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

/** @format int32 */
export enum NotificationType {
  Value0 = 0,
  Value1 = 1,
  Value2 = 2,
  Value3 = 3,
  Value4 = 4,
  Value5 = 5,
  Value6 = 6,
  Value7 = 7,
  Value8 = 8,
  Value9 = 9,
  Value10 = 10,
  Value11 = 11,
  Value12 = 12,
  Value13 = 13,
  Value14 = 14,
}

export interface ObjectApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: any;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface Payment {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  paymentId?: number;
  /** @format int32 */
  billId?: number | null;
  /** @format double */
  amountPaid?: number | null;
  paymentMethod?: string | null;
  /** @format date-time */
  paymentDate?: string | null;
  currency?: string | null;
  paymentStatus?: string | null;
  transactionId?: string | null;
  responseCode?: string | null;
  orderId?: string | null;
  bill?: Bill;
}

export interface PaymentDetailsDTO {
  /** @format int32 */
  paymentId?: number;
  /** @format int32 */
  billId?: number | null;
  /** @format double */
  amountPaid?: number | null;
  paymentMethod?: string | null;
  /** @format date-time */
  paymentDate?: string | null;
  transactionId?: string | null;
  paymentStatus?: string | null;
  orderId?: string | null;
  currency?: string | null;
  billStatus?: string | null;
  /** @format double */
  billTotalAmount?: number | null;
}

export interface PaymentDetailsDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: PaymentDetailsDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface PaymentDetailsDTOPagedResult {
  items?: PaymentDetailsDTO[] | null;
  /** @format int32 */
  totalRecords?: number;
}

export interface PaymentDetailsDTOPagedResultApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: PaymentDetailsDTOPagedResult;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface PaymentRequest {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  paymentRequestId?: number;
  /** @format uuid */
  tutorId?: string;
  bankCode?: string | null;
  accountNumber?: string | null;
  fullName?: string | null;
  /** @format double */
  amount?: number;
  status?: string | null;
  verificationStatus?: string | null;
  /** @format uuid */
  token?: string;
  isPaid?: boolean;
  /** @format date-time */
  paidDate?: string | null;
  adminNote?: string | null;
  tutor?: Tutor;
}

export interface PaymentRequestDTO {
  /** @format int32 */
  paymentRequestId?: number;
  /** @format uuid */
  tutorId?: string;
  bankCode?: string | null;
  accountNumber?: string | null;
  isPaid?: boolean;
  /** @format double */
  amount?: number;
  status?: string | null;
  verificationStatus?: string | null;
  /** @format date-time */
  createdDate?: string | null;
  /** @format date-time */
  paidDate?: string | null;
  adminNote?: string | null;
  tutorName?: string | null;
}

export interface PaymentRequestDTOPagedResult {
  items?: PaymentRequestDTO[] | null;
  /** @format int32 */
  totalRecords?: number;
}

export interface PaymentRequestDTOPagedResultApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: PaymentRequestDTOPagedResult;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface PaymentResponseModel {
  orderDescription?: string | null;
  transactionId?: string | null;
  orderId?: string | null;
  paymentMethod?: string | null;
  paymentId?: string | null;
  success?: boolean;
  token?: string | null;
  vnPayResponseCode?: string | null;
}

export interface PaymentResponseModelApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: PaymentResponseModel;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface Post {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  postId?: number;
  /** @format uuid */
  adminId?: string | null;
  title?: string | null;
  content?: string | null;
  subcontent?: string | null;
  thumbnail?: string | null;
  status?: string | null;
  /** @format int32 */
  postType?: number | null;
  admin?: Admin;
  postCategory?: PostCategory;
}

export interface PostCategory {
  /** @format int32 */
  postType?: number;
  postName?: string | null;
}

export interface PostCategoryIEnumerableApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: PostCategory[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface PostsDTO {
  /** @format int32 */
  postId?: number;
  title?: string | null;
  thumbnail?: string | null;
  content?: string | null;
  subContent?: string | null;
  status?: string | null;
  postCategoryName?: string | null;
  /** @format date-time */
  createdDate?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  author?: AuthorDTO;
  /** @format int32 */
  postType?: number | null;
}

export interface PostsDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: PostsDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface PostsHomePageDTO {
  posts?: PostsDTO[] | null;
  /** @format int32 */
  totalRecordCount?: number | null;
  /** @format int32 */
  page_number?: number | null;
  /** @format int32 */
  page_size?: number | null;
}

export interface PostsHomePageDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: PostsHomePageDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface QualificationLevel {
  /** @format int32 */
  id?: number;
  level?: string | null;
  tutorRequests?: TutorRequest[] | null;
}

export interface QualificationLevelDto {
  /** @format int32 */
  id?: number;
  level?: string | null;
}

export interface QualificationLevelDtoIEnumerableApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: QualificationLevelDto[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface QuestionStatistics {
  questionType?: string | null;
  totalAnswerCount?: string | null;
  answerBreakdown?: AnswerBreakdown[] | null;
}

export interface RateRange {
  /** @format int32 */
  id?: number;
  level?: string | null;
  /** @format double */
  minRate?: number;
  /** @format double */
  maxRate?: number;
  description?: string | null;
}

export interface RateRangeApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: RateRange;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface RateRangeIEnumerableApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: RateRange[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface RegisterLearnerDTO {
  /** @format int32 */
  tutorSubjectId?: number | null;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  contractUrl?: string | null;
  /** @format double */
  pricePerHour?: number | null;
  notes?: string | null;
  locationDetail?: string | null;
  /** @format int32 */
  sessionsPerWeek?: number | null;
  /** @format int32 */
  hoursPerSession?: number | null;
  preferredScheduleType?: string | null;
  /** @format date-time */
  expectedStartDate?: string | null;
  schedules?: ScheduleDTO[] | null;
}

export interface RegisterLearnerDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: RegisterLearnerDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface RejectPaymentRequestDTO {
  rejectionReason?: string | null;
}

export interface ResetPasswordModel {
  /**
   * @format email
   * @minLength 1
   */
  email: string;
  /** @minLength 1 */
  token: string;
  /** @minLength 8 */
  newPassword: string;
  /** @minLength 1 */
  confirmPassword: string;
}

export interface Schedule {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  scheduleId?: number;
  /** @format uuid */
  tutorId?: string | null;
  /** @format int32 */
  tutorRequestID?: number | null;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  /** @format int32 */
  dayOfWeek?: number | null;
  /** @format date-span */
  startTime?: string | null;
  /** @format date-span */
  endTime?: string | null;
  tutor?: Tutor;
  tutorLearnerSubject?: TutorLearnerSubject;
  tutorRequest?: TutorRequest;
}

export interface ScheduleApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: Schedule;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface ScheduleDTO {
  /** @format int32 */
  dayOfWeek?: number | null;
  freeTimes?: FreeTimeDTO[] | null;
}

export interface ScheduleGroupDTO {
  /** @format int32 */
  dayOfWeek?: number | null;
  schedules?: ScheduleViewDTO[] | null;
}

export interface ScheduleGroupDTOListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: ScheduleGroupDTO[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface ScheduleViewDTO {
  /** @format int32 */
  id?: number;
  /** @format int32 */
  dayOfWeek?: number | null;
  freeTimes?: FreeTimeDTO[] | null;
  subjectNames?: string | null;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
}

export interface SignInModel {
  /**
   * @format email
   * @minLength 1
   */
  email: string;
  /** @minLength 1 */
  password: string;
}

export interface SignInResponseDto {
  /** @format uuid */
  id?: string | null;
  fullname?: string | null;
  /** @format date-time */
  dob?: string | null;
  gender?: boolean | null;
  avatarUrl?: string | null;
  email?: string | null;
  phoneNumber?: string | null;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  addressDetail?: string | null;
  /** @format double */
  balance?: number | null;
  roles?: string[] | null;
}

export interface SignInResponseDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: SignInResponseDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface SignUpModel {
  /**
   * @format email
   * @minLength 1
   */
  email: string;
  /** @minLength 1 */
  password: string;
  /** @minLength 1 */
  confirmPassword: string;
  fullname?: string | null;
}

export interface StringApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: string | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface Subject {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  subjectId?: number;
  subjectName?: string | null;
  isVerified?: boolean | null;
  tutorSubjects?: TutorSubject[] | null;
}

export interface SubjectApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: Subject;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface SubjectDTO {
  /** @format int32 */
  subjectId?: number | null;
  subjectName?: string | null;
}

export interface SubjectDetailDto {
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  /** @format uuid */
  learnerId?: string;
  /** @format uuid */
  tutorId?: string;
  subjectName?: string | null;
  /** @format double */
  rate?: number;
  location?: string | null;
  /** @format date-time */
  expectedStartDate?: string | null;
  /** @format int32 */
  hoursPerSession?: number;
  locationDetail?: string | null;
  /** @format double */
  pricePerHour?: number;
  /** @format int32 */
  sessionsPerWeek?: number | null;
  isVerify?: boolean | null;
  contractUrl?: string | null;
  isClosed?: boolean | null;
}

export interface SubjectDetailDtoListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: SubjectDetailDto[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface SubjectFilterDTO {
  /** @format int32 */
  subjectId?: number | null;
  subjectName?: string | null;
  /** @format int32 */
  rateRangeId?: number | null;
  /** @format int32 */
  numberOfUsages?: number | null;
}

export interface SubjectFilterDTOIEnumerableApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: SubjectFilterDTO[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface SubjectFilterDTOListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: SubjectFilterDTO[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TeachingLocation {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  teachingLocationId?: number;
  cityId?: string | null;
  districtId?: string | null;
  tutors?: TutorTeachingLocations[] | null;
}

export interface TeachingLocationViewDTO {
  cityId?: string | null;
  cityName?: string | null;
  districts?: DistrictDTO[] | null;
}

export interface Tutor {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format uuid */
  tutorId?: string;
  experience?: string | null;
  specialization?: string | null;
  /** @format double */
  rating?: number | null;
  status?: string | null;
  profileDescription?: string | null;
  briefIntroduction?: string | null;
  major?: string | null;
  shortDescription?: string | null;
  /** @format int32 */
  educationalLevel?: number | null;
  videoUrl?: string | null;
  isVerified?: boolean | null;
  isAccepted?: boolean;
  entityType?: string | null;
  tutorNavigation?: AspNetUser;
  certificates?: Certificate[] | null;
  tutorSubjects?: TutorSubject[] | null;
  /** @format double */
  balance?: number;
  tutorTeachingLocations?: TutorTeachingLocations[] | null;
  schedules?: Schedule[] | null;
  tutorRequestTutors?: TutorRequestTutor[] | null;
  paymentRequests?: PaymentRequest[] | null;
}

export interface TutorDto {
  /** @format uuid */
  tutorId?: string;
  experience?: string | null;
  specialization?: string | null;
  major?: string | null;
  briefIntroduction?: string | null;
  /** @format int32 */
  educationalLevelID?: number | null;
  educationalLevelName?: string | null;
  shortDescription?: string | null;
  /** @format double */
  rating?: number | null;
  status?: string | null;
  profileDescription?: string | null;
  videoUrl?: string | null;
  fullName?: string | null;
  avatarUrl?: string | null;
  addressID?: string | null;
  addressDetail?: string | null;
  entityType?: string | null;
  isVerified?: boolean | null;
  isAccepted?: boolean;
  certificates?: CertificateDTO[] | null;
  tutorSubjects?: TutorSubjectDto[] | null;
  teachingLocations?: TeachingLocationViewDTO[] | null;
  schedules?: ScheduleDTO[] | null;
}

export interface TutorDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorFilterDto {
  subjects?: number[] | null;
  /** @format double */
  minPrice?: number | null;
  /** @format double */
  maxPrice?: number | null;
  city?: string | null;
  district?: string | null;
  searchingQuery?: string | null;
}

export interface TutorHomePageDTO {
  tutors?: TutorSummaryDto[] | null;
  /** @format int32 */
  totalRecordCount?: number;
}

export interface TutorHomePageDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorHomePageDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorInTutorRequestDTO {
  /** @format uuid */
  tutorId?: string;
  name?: string | null;
  email?: string | null;
  specialization?: string | null;
  isVerified?: boolean | null;
}

export interface TutorLearnerSubject {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  /** @format int32 */
  tutorSubjectId?: number | null;
  /** @format uuid */
  learnerId?: string | null;
  /** @format double */
  pricePerHour?: number | null;
  notes?: string | null;
  location?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  locationDetail?: string | null;
  /** @format int32 */
  sessionsPerWeek?: number | null;
  /** @format int32 */
  hoursPerSession?: number | null;
  preferredScheduleType?: string | null;
  /** @format date-time */
  expectedStartDate?: string | null;
  contractUrl?: string | null;
  status?: string | null;
  isVerified?: boolean | null;
  isContractVerified?: boolean | null;
  contractNote?: string | null;
  entityType?: string | null;
  isCloseClass?: boolean;
  learner?: AspNetUser;
  tutorSubject?: TutorSubject;
  billingEntries?: BillingEntry[] | null;
  feedbacks?: Feedback[] | null;
}

export interface TutorLearnerSubjectDetailDto {
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  /** @format int32 */
  tutorSubjectId?: number | null;
  /** @format uuid */
  learnerId?: string;
  /** @format uuid */
  tutorId?: string;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  locationDetail?: string | null;
  contractUrl?: string | null;
  /** @format double */
  pricePerHour?: number | null;
  notes?: string | null;
  /** @format int32 */
  sessionsPerWeek?: number | null;
  /** @format int32 */
  hoursPerSession?: number | null;
  preferredScheduleType?: string | null;
  /** @format date-time */
  expectedStartDate?: string | null;
  isVerified?: boolean | null;
  schedules?: ScheduleDTO[] | null;
  isContractVerified?: boolean | null;
}

export interface TutorLearnerSubjectDetailDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorLearnerSubjectDetailDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorLearnerSubjectSummaryDetailDto {
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  /** @format int32 */
  tutorSubjectId?: number | null;
  /** @format uuid */
  learnerId?: string;
  /** @format uuid */
  tutorId?: string;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  locationDetail?: string | null;
  /** @format double */
  pricePerHour?: number;
  notes?: string | null;
  /** @format int32 */
  sessionsPerWeek?: number;
  /** @format int32 */
  hoursPerSession?: number;
  preferredScheduleType?: string | null;
  /** @format date-time */
  expectedStartDate?: string | null;
  isVerified?: boolean | null;
  schedules?: ScheduleDTO[] | null;
  contractUrl?: string | null;
  isContractVerified?: boolean | null;
  learnerEmail?: string | null;
  isClosed?: boolean | null;
  classType?: string | null;
  subjectName?: string | null;
  /** @format int32 */
  totalSessionsCompleted?: number;
  /** @format double */
  totalPaidAmount?: number;
}

export interface TutorLearnerSubjectSummaryDetailDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorLearnerSubjectSummaryDetailDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorRatingDto {
  /** @format uuid */
  tutorId?: string;
  /** @format double */
  averageRating?: number | null;
  feedbacks?: FeedbackDto[] | null;
}

export interface TutorRatingDtoListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorRatingDto[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorRequest {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  id?: number;
  phoneNumber?: string | null;
  requestSummary?: string | null;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  teachingLocation?: string | null;
  /** @format int32 */
  numberOfStudents?: number;
  /** @format date-time */
  startDate?: string;
  preferredScheduleType?: string | null;
  /** @format date-span */
  timePerSession?: string;
  subject?: string | null;
  studentGender?: string | null;
  tutorGender?: string | null;
  /** @format double */
  fee?: number;
  /** @format int32 */
  sessionsPerWeek?: number;
  detailedDescription?: string | null;
  /** @format int32 */
  tutorQualificationId?: number | null;
  /** @format uuid */
  aspNetUserId?: string | null;
  freeSchedules?: string | null;
  /** @format int32 */
  tutorLearnerSubjectId?: number | null;
  tutorLearnerSubject?: TutorLearnerSubject;
  isVerified?: boolean | null;
  entityType?: string | null;
  status?: string | null;
  aspNetUser?: AspNetUser;
  tutorQualification?: QualificationLevel;
  tutorRequestTutors?: TutorRequestTutor[] | null;
  schedules?: Schedule[] | null;
  /** @format int32 */
  rateRangeId?: number | null;
  rateRange?: RateRange;
}

export interface TutorRequestDTO {
  /** @format int32 */
  id?: number;
  phoneNumber?: string | null;
  requestSummary?: string | null;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  teachingLocation?: string | null;
  /** @format int32 */
  numberOfStudents?: number;
  /** @format date-time */
  startDate?: string;
  preferredScheduleType?: string | null;
  /** @format date-span */
  timePerSession?: string;
  subject?: string | null;
  studentGender?: string | null;
  tutorGender?: string | null;
  /** @format double */
  fee?: number;
  learnerName?: string | null;
  /** @format int32 */
  sessionsPerWeek?: number;
  detailedDescription?: string | null;
  /** @format int32 */
  tutorQualificationId?: number | null;
  tutorQualificationName?: string | null;
  status?: string | null;
  freeSchedules?: string | null;
  /** @format int32 */
  rateRangeId?: number | null;
  /** @format uuid */
  createdUserId?: string | null;
  registeredTutorIds?: string[] | null;
}

export interface TutorRequestDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorRequestDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorRequestDTOPagedResult {
  items?: TutorRequestDTO[] | null;
  /** @format int32 */
  totalRecords?: number;
}

export interface TutorRequestDTOPagedResultApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorRequestDTOPagedResult;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorRequestSchedulesDTO {
  /** @format int32 */
  dayOfWeek?: number | null;
  freeTimes?: FreeTimeDTO[] | null;
}

export interface TutorRequestTutor {
  /** @format int32 */
  tutorRequestId?: number;
  tutorRequest?: TutorRequest;
  /** @format uuid */
  tutorId?: string;
  tutor?: Tutor;
  isVerified?: boolean | null;
  ischoose?: boolean;
  /** @format date-time */
  dateJoined?: string;
  status?: string | null;
}

export interface TutorRequestWithTutorsDTO {
  /** @format int32 */
  tutorRequestId?: number;
  subject?: string | null;
  /** @format date-time */
  startDate?: string;
  tutors?: TutorInTutorRequestDTO[] | null;
}

export interface TutorRequestWithTutorsDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorRequestWithTutorsDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorSubject {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  tutorSubjectId?: number;
  /** @format uuid */
  tutorId?: string | null;
  /** @format int32 */
  subjectId?: number | null;
  /** @format double */
  rate?: number | null;
  description?: string | null;
  isVerified?: boolean | null;
  entityType?: string | null;
  subjectType?: string | null;
  /** @format int32 */
  rateRangeId?: number | null;
  rateRange?: RateRange;
  status?: string | null;
  subject?: Subject;
  tutor?: Tutor;
  tutorLearnerSubjects?: TutorLearnerSubject[] | null;
}

export interface TutorSubjectDto {
  /** @format int32 */
  tutorSubjectId?: number;
  /** @format uuid */
  tutorId?: string | null;
  /** @format int32 */
  subjectId?: number | null;
  /** @format double */
  rate?: number | null;
  description?: string | null;
  isVerified?: boolean | null;
  entityType?: string | null;
  subjectType?: string | null;
  status?: string | null;
  /** @format int32 */
  rateRangeId?: number | null;
  subject?: SubjectDTO;
}

export interface TutorSummaryDto {
  /** @format uuid */
  tutorId?: string;
  experience?: string | null;
  specialization?: string | null;
  major?: string | null;
  briefIntroduction?: string | null;
  educationalLevel?: string | null;
  /** @format double */
  rating?: number | null;
  status?: string | null;
  profileDescription?: string | null;
  isVerified?: boolean | null;
  isAccepted?: boolean;
  fullName?: string | null;
  avatarUrl?: string | null;
  addressId?: string | null;
  /** @format int32 */
  numberOfStudents?: number | null;
  certificates?: CertificateDTO[] | null;
  tutorSubjects?: TutorSubjectDto[] | null;
  teachingLocations?: TeachingLocationViewDTO[] | null;
  schedules?: ScheduleDTO[] | null;
}

export interface TutorSummaryDtoListApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: TutorSummaryDto[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface TutorTeachingLocations {
  /** @format uuid */
  tutorId?: string;
  tutor?: Tutor;
  /** @format int32 */
  teachingLocationId?: number;
  teachingLocation?: TeachingLocation;
}

export interface UpdateBillingEntryDTO {
  /** @format double */
  rate?: number | null;
  /** @format date-time */
  startDateTime?: string | null;
  /** @format date-time */
  endDateTime?: string | null;
  description?: string | null;
  /** @format double */
  totalAmount?: number | null;
}

export interface UpdatePaymentRequestDTO {
  bankCode?: string | null;
  accountNumber?: string | null;
  fullName?: string | null;
  /** @format double */
  amount?: number;
}

export interface UpdatePostDTO {
  /** @format int32 */
  postId?: number;
  title?: string | null;
  content?: string | null;
  subcontent?: string | null;
  thumbnail?: string | null;
  status?: string | null;
  /** @format int32 */
  postType?: number | null;
}

export interface UpdatePostDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: UpdatePostDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface UpdateScheduleDTO {
  /** @format int32 */
  id?: number;
  /** @format uuid */
  tutorID?: string | null;
  /** @format int32 */
  dayOfWeek?: number | null;
  /** @format int32 */
  tutorLearnerSubjectID?: number | null;
  freeTimes?: FreeTimeDTO[] | null;
}

export interface UpdateTutorInforDTO {
  experience?: string | null;
  specialization?: string | null;
  profileDescription?: string | null;
  briefIntroduction?: string | null;
  /** @format int32 */
  educationalLevelID?: number;
  shortDescription?: string | null;
  major?: string | null;
  videoUrl?: string | null;
  isAccepted?: boolean;
  addressID?: string | null;
  teachingLocation?: AddTeachingLocationViewDTO[] | null;
  certificates?: CertificateDTO[] | null;
  subjects?: AddSubjectDTO[] | null;
}

export interface UpdateTutorInforDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: UpdateTutorInforDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface UpdateUserDTO {
  fullname?: string | null;
  /** @format date-time */
  dob?: string | null;
  gender?: boolean | null;
  avatarUrl?: string | null;
  addressDetail?: string | null;
  cityId?: string | null;
  districtId?: string | null;
  wardId?: string | null;
  phoneNumber?: string | null;
}

export interface UpdateUserDTOApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: UpdateUserDTO;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface UserFeedbackDto {
  /** @format int32 */
  feedbackId?: number;
  /** @format int32 */
  tutorLearnerSubjectId?: number;
  tutorLearnerSubjectName?: string | null;
  fullName?: string | null;
  avatarUrl?: string | null;
  /** @format double */
  rating?: number | null;
  comments?: string | null;
  /** @format date-time */
  createdDate?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
}

export interface UserToken {
  /** @format date-time */
  createdDate?: string | null;
  /** @format uuid */
  createdBy?: string | null;
  /** @format date-time */
  updatedDate?: string | null;
  /** @format uuid */
  updatedBy?: string | null;
  reasonDesc?: string | null;
  isDelete?: boolean;
  /** @format int32 */
  userTokenId?: number;
  /** @format uuid */
  userId?: string;
  fcmToken?: string | null;
  deviceType?: string | null;
  user?: AspNetUser;
}

export interface UserTokenDto {
  /** @format uuid */
  userId?: string;
  token?: string | null;
  deviceType?: string | null;
}

export interface VerificationStatusDto {
  entityType?: EntityTypeName;
  /** @format uuid */
  guidId?: string;
  /** @format int32 */
  id?: number;
  isVerified?: boolean;
  reason?: string | null;
}

export interface ViewAccount {
  /** @format uuid */
  userId?: string;
  fullname?: string | null;
  /** @format date-time */
  dob?: string | null;
  gender?: boolean | null;
  addressId?: string | null;
  addressDetail?: string | null;
  status?: string | null;
  lockoutEnabled?: boolean | null;
  roles?: string[] | null;
}

export interface ViewAccountIEnumerableApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: ViewAccount[] | null;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

export interface WalletOverviewDto {
  /** @format double */
  currentBalance?: number;
  /** @format double */
  totalEarningsThisMonth?: number;
  /** @format int32 */
  pendingWithdrawals?: number;
}

export interface WalletOverviewDtoApiResponse {
  /** @format int32 */
  status?: number;
  success?: boolean;
  message?: string | null;
  data?: WalletOverviewDto;
  errors?: ApiError[] | null;
  timestamp?: string | null;
}

import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, HeadersDefaults, ResponseType } from "axios";
import axios from "axios";

export type QueryParamsType = Record<string | number, any>;

export interface FullRequestParams extends Omit<AxiosRequestConfig, "data" | "params" | "url" | "responseType"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseType;
  /** request body */
  body?: unknown;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> extends Omit<AxiosRequestConfig, "data" | "cancelToken"> {
  securityWorker?: (
    securityData: SecurityDataType | null,
  ) => Promise<AxiosRequestConfig | void> | AxiosRequestConfig | void;
  secure?: boolean;
  format?: ResponseType;
}

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public instance: AxiosInstance;
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private secure?: boolean;
  private format?: ResponseType;

  constructor({ securityWorker, secure, format, ...axiosConfig }: ApiConfig<SecurityDataType> = {}) {
    this.instance = axios.create({ ...axiosConfig, baseURL: axiosConfig.baseURL || "https://tutorconnectapi-d8gafsgrdka9gkbs.southafricanorth-01.azurewebsites.net" });
    this.secure = secure;
    this.format = format;
    this.securityWorker = securityWorker;
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected mergeRequestParams(params1: AxiosRequestConfig, params2?: AxiosRequestConfig): AxiosRequestConfig {
    const method = params1.method || (params2 && params2.method);

    return {
      ...this.instance.defaults,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...((method && this.instance.defaults.headers[method.toLowerCase() as keyof HeadersDefaults]) || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected stringifyFormItem(formItem: unknown) {
    if (typeof formItem === "object" && formItem !== null) {
      return JSON.stringify(formItem);
    } else {
      return `${formItem}`;
    }
  }

  protected createFormData(input: Record<string, unknown>): FormData {
    if (input instanceof FormData) {
      return input;
    }
    return Object.keys(input || {}).reduce((formData, key) => {
      const property = input[key];
      const propertyContent: any[] = property instanceof Array ? property : [property];

      for (const formItem of propertyContent) {
        const isFileType = formItem instanceof Blob || formItem instanceof File;
        formData.append(key, isFileType ? formItem : this.stringifyFormItem(formItem));
      }

      return formData;
    }, new FormData());
  }

  public request = async <T = any, _E = any>({
    secure,
    path,
    type,
    query,
    format,
    body,
    ...params
  }: FullRequestParams): Promise<AxiosResponse<T>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const responseFormat = format || this.format || undefined;

    if (type === ContentType.FormData && body && body !== null && typeof body === "object") {
      body = this.createFormData(body as Record<string, unknown>);
    }

    if (type === ContentType.Text && body && body !== null && typeof body !== "string") {
      body = JSON.stringify(body);
    }

    return this.instance.request({
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type ? { "Content-Type": type } : {}),
      },
      params: query,
      responseType: responseFormat,
      data: body,
      url: path,
    });
  };
}

/**
 * @title TutorConnect API
 * @version v1
 * @baseUrl http://localhost:7026
 */
export class Api<SecurityDataType extends unknown> {
  http: HttpClient<SecurityDataType>;

  constructor(http: HttpClient<SecurityDataType>) {
    this.http = http;
  }

  api = {
    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsCurrentList
     * @request GET:/api/Accounts/current
     * @secure
     */
    accountsCurrentList: (params: RequestParams = {}) =>
      this.http.request<SignInResponseDtoApiResponse, any>({
        path: `/api/Accounts/current`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsSignUpCreate
     * @request POST:/api/Accounts/SignUp
     * @secure
     */
    accountsSignUpCreate: (data: SignUpModel, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Accounts/SignUp`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsSendEmailConfirmationCreate
     * @request POST:/api/Accounts/SendEmailConfirmation
     * @secure
     */
    accountsSendEmailConfirmationCreate: (
      query?: {
        userId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<void, any>({
        path: `/api/Accounts/SendEmailConfirmation`,
        method: "POST",
        query: query,
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsConfirmEmailList
     * @request GET:/api/Accounts/ConfirmEmail
     * @secure
     */
    accountsConfirmEmailList: (
      query?: {
        userId?: string;
        token?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<void, any>({
        path: `/api/Accounts/ConfirmEmail`,
        method: "GET",
        query: query,
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsForgotPasswordCreate
     * @request POST:/api/Accounts/ForgotPassword
     * @secure
     */
    accountsForgotPasswordCreate: (data: ForgotPasswordModel, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Accounts/ForgotPassword`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsResetPasswordCreate
     * @request POST:/api/Accounts/ResetPassword
     * @secure
     */
    accountsResetPasswordCreate: (data: ResetPasswordModel, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Accounts/ResetPassword`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsSignInCreate
     * @request POST:/api/Accounts/SignIn
     * @secure
     */
    accountsSignInCreate: (data: SignInModel, params: RequestParams = {}) =>
      this.http.request<SignInResponseDtoApiResponse, any>({
        path: `/api/Accounts/SignIn`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsSignOutCreate
     * @request POST:/api/Accounts/SignOut
     * @secure
     */
    accountsSignOutCreate: (params: RequestParams = {}) =>
      this.http.request<void, any>({
        path: `/api/Accounts/SignOut`,
        method: "POST",
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsViewAllAccountsList
     * @request GET:/api/Accounts/ViewAllAccounts
     * @secure
     */
    accountsViewAllAccountsList: (params: RequestParams = {}) =>
      this.http.request<ViewAccountIEnumerableApiResponse, any>({
        path: `/api/Accounts/ViewAllAccounts`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsBlockUserCreate
     * @request POST:/api/Accounts/BlockUser
     * @secure
     */
    accountsBlockUserCreate: (data: string, params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/Accounts/BlockUser`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsUnblockUserCreate
     * @request POST:/api/Accounts/UnblockUser
     * @secure
     */
    accountsUnblockUserCreate: (data: string, params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/Accounts/UnblockUser`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Accounts
     * @name AccountsChangePasswordCreate
     * @request POST:/api/Accounts/ChangePassword
     * @secure
     */
    accountsChangePasswordCreate: (data: ChangePasswordDTO, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Accounts/ChangePassword`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Admin
     * @name AdminAssignRoleAdminCreate
     * @request POST:/api/Admin/AssignRoleAdmin
     * @secure
     */
    adminAssignRoleAdminCreate: (data: AssignRoleAdminDto, params: RequestParams = {}) =>
      this.http.request<AssignRoleAdminDtoApiResponse, any>({
        path: `/api/Admin/AssignRoleAdmin`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Admin
     * @name AdminGetAdminListByTutorList
     * @request GET:/api/Admin/GetAdminListByTutor
     * @secure
     */
    adminGetAdminListByTutorList: (
      query?: {
        Search?: string;
        Status?: string;
        /** @format date-time */
        StartDate?: string;
        /** @format date-time */
        EndDate?: string;
        /**
         * @format int32
         * @default 0
         */
        index?: number;
        /**
         * @format int32
         * @default 20
         */
        size?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<AdminHomePageDTOListApiResponse, any>({
        path: `/api/Admin/GetAdminListByTutor`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Admin
     * @name AdminGetAdminMenuActionList
     * @request GET:/api/Admin/GetAdminMenuAction
     * @secure
     */
    adminGetAdminMenuActionList: (params: RequestParams = {}) =>
      this.http.request<AdminMenuActionListApiResponse, any>({
        path: `/api/Admin/GetAdminMenuAction`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Admin
     * @name AdminVerificationStatusCreate
     * @request POST:/api/Admin/VerificationStatus
     * @secure
     */
    adminVerificationStatusCreate: (data: VerificationStatusDto, params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/Admin/VerificationStatus`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillGetAllBillsList
     * @request GET:/api/Bill/GetAllBills
     * @secure
     */
    billGetAllBillsList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillDTOSApiResponse, any>({
        path: `/api/Bill/GetAllBills`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillGenerateBillFromBillingEntriesCreate
     * @request POST:/api/Bill/GenerateBillFromBillingEntries
     * @secure
     */
    billGenerateBillFromBillingEntriesCreate: (data: number[], params: RequestParams = {}) =>
      this.http.request<Int32ApiResponse, any>({
        path: `/api/Bill/GenerateBillFromBillingEntries`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillDeleteBillDelete
     * @request DELETE:/api/Bill/DeleteBill
     * @secure
     */
    billDeleteBillDelete: (
      query?: {
        /** @format int32 */
        billId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/Bill/DeleteBill`,
        method: "DELETE",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillGenerateBillPdfList
     * @request GET:/api/Bill/GenerateBillPdf
     * @secure
     */
    billGenerateBillPdfList: (
      query?: {
        /** @format int32 */
        billId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<File, any>({
        path: `/api/Bill/GenerateBillPdf`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillApiBillViewBillHtmlList
     * @request GET:/api/Bill/api/bill/ViewBillHtml
     * @secure
     */
    billApiBillViewBillHtmlList: (
      query?: {
        /** @format int32 */
        billId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<string, any>({
        path: `/api/Bill/api/bill/ViewBillHtml`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillApproveBillCreate
     * @request POST:/api/Bill/ApproveBill
     * @secure
     */
    billApproveBillCreate: (
      query?: {
        /** @format int32 */
        billId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BooleanApiResponse, any>({
        path: `/api/Bill/ApproveBill`,
        method: "POST",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillSendBillEmailCreate
     * @request POST:/api/Bill/SendBillEmail
     * @secure
     */
    billSendBillEmailCreate: (
      query?: {
        /** @format int32 */
        billId?: number;
        parentEmail?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BooleanApiResponse, any>({
        path: `/api/Bill/SendBillEmail`,
        method: "POST",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillGetBillByTutorLearnerSubjectIdList
     * @request GET:/api/Bill/GetBillByTutorLearnerSubjectId
     * @secure
     */
    billGetBillByTutorLearnerSubjectIdList: (
      query?: {
        /** @format int32 */
        tutorLearnerSubjectId?: number;
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 10
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillDetailsDTOPagedResultApiResponse, any>({
        path: `/api/Bill/GetBillByTutorLearnerSubjectId`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillGetBillByTutorList
     * @request GET:/api/Bill/GetBillByTutor
     * @secure
     */
    billGetBillByTutorList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 10
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillDetailsDTOPagedResultApiResponse, any>({
        path: `/api/Bill/GetBillByTutor`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Bill
     * @name BillGetBillDetailByIdList
     * @request GET:/api/Bill/GetBillDetailById
     * @secure
     */
    billGetBillDetailByIdList: (
      query?: {
        /** @format int32 */
        billId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillDetailsDTOApiResponse, any>({
        path: `/api/Bill/GetBillDetailById`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryGetAllBillingEntriesList
     * @request GET:/api/BillingEntry/GetAllBillingEntries
     * @secure
     */
    billingEntryGetAllBillingEntriesList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillingEntryDTOSApiResponse, any>({
        path: `/api/BillingEntry/GetAllBillingEntries`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryGetBillingEntryByIdList
     * @request GET:/api/BillingEntry/GetBillingEntryById
     * @secure
     */
    billingEntryGetBillingEntryByIdList: (
      query?: {
        /** @format int32 */
        billingEntryId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillingEntryDTOApiResponse, any>({
        path: `/api/BillingEntry/GetBillingEntryById`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryGetAllBillingEntriesByTutorLearnerSubjectIdList
     * @request GET:/api/BillingEntry/GetAllBillingEntriesByTutorLearnerSubjectId
     * @secure
     */
    billingEntryGetAllBillingEntriesByTutorLearnerSubjectIdList: (
      query?: {
        /** @format int32 */
        tutorLearnerSubjectId?: number;
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillingEntryDTOSApiResponse, any>({
        path: `/api/BillingEntry/GetAllBillingEntriesByTutorLearnerSubjectId`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryAddBillingEntryCreate
     * @request POST:/api/BillingEntry/AddBillingEntry
     * @secure
     */
    billingEntryAddBillingEntryCreate: (data: AdddBillingEntryDTO, params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/BillingEntry/AddBillingEntry`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryAddDraftBillingEntryCreate
     * @request POST:/api/BillingEntry/AddDraftBillingEntry
     * @secure
     */
    billingEntryAddDraftBillingEntryCreate: (data: AdddBillingEntryDTO, params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/BillingEntry/AddDraftBillingEntry`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryUpdateBillingEntryUpdate
     * @request PUT:/api/BillingEntry/UpdateBillingEntry
     * @secure
     */
    billingEntryUpdateBillingEntryUpdate: (
      data: UpdateBillingEntryDTO,
      query?: {
        /** @format int32 */
        billingEntryId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/BillingEntry/UpdateBillingEntry`,
        method: "PUT",
        query: query,
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryDeleteBillingEntryDelete
     * @request DELETE:/api/BillingEntry/DeleteBillingEntry
     * @secure
     */
    billingEntryDeleteBillingEntryDelete: (data: number[], params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/BillingEntry/DeleteBillingEntry`,
        method: "DELETE",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryGetBillingEntryDetailsList
     * @request GET:/api/BillingEntry/GetBillingEntryDetails
     * @secure
     */
    billingEntryGetBillingEntryDetailsList: (
      query?: {
        /** @format int32 */
        tutorLearnerSubjectId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BillingEntryDetailsDTOApiResponse, any>({
        path: `/api/BillingEntry/GetBillingEntryDetails`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags BillingEntry
     * @name BillingEntryCalculateTotalAmountCreate
     * @request POST:/api/BillingEntry/CalculateTotalAmount
     * @secure
     */
    billingEntryCalculateTotalAmountCreate: (data: CalculateTotalAmountRequest, params: RequestParams = {}) =>
      this.http.request<DecimalApiResponse, any>({
        path: `/api/BillingEntry/CalculateTotalAmount`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Faq
     * @name FaqGetAllFaQsList
     * @request GET:/api/Faq/GetAllFAQs
     * @secure
     */
    faqGetAllFaQsList: (params: RequestParams = {}) =>
      this.http.request<FaqDtoIEnumerableApiResponse, any>({
        path: `/api/Faq/GetAllFAQs`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Faq
     * @name FaqGetFaqByIdDetail
     * @request GET:/api/Faq/GetFAQById/{id}
     * @secure
     */
    faqGetFaqByIdDetail: (id: number, params: RequestParams = {}) =>
      this.http.request<FaqDtoApiResponse, any>({
        path: `/api/Faq/GetFAQById/${id}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Faq
     * @name FaqCreateFaqCreate
     * @request POST:/api/Faq/CreateFAQ
     * @secure
     */
    faqCreateFaqCreate: (data: FaqCreateDto, params: RequestParams = {}) =>
      this.http.request<FaqDtoApiResponse, ObjectApiResponse>({
        path: `/api/Faq/CreateFAQ`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Faq
     * @name FaqUpdateFaqUpdate
     * @request PUT:/api/Faq/UpdateFAQ/{id}
     * @secure
     */
    faqUpdateFaqUpdate: (id: number, data: FaqUpdateDto, params: RequestParams = {}) =>
      this.http.request<FaqDtoApiResponse, ObjectApiResponse>({
        path: `/api/Faq/UpdateFAQ/${id}`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Faq
     * @name FaqDeleteFaqDelete
     * @request DELETE:/api/Faq/DeleteFAQ/{id}
     * @secure
     */
    faqDeleteFaqDelete: (id: number, params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, ObjectApiResponse>({
        path: `/api/Faq/DeleteFAQ/${id}`,
        method: "DELETE",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Faq
     * @name FaqGetHomepageFaQsList
     * @request GET:/api/Faq/GetHomepageFAQs
     * @secure
     */
    faqGetHomepageFaQsList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        index?: number;
        /**
         * @format int32
         * @default 20
         */
        size?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<FaqDtoIEnumerableApiResponse, any>({
        path: `/api/Faq/GetHomepageFAQs`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Feedback
     * @name FeedbackGetFeedbackDetailByTutorIdDetail
     * @request GET:/api/Feedback/GetFeedbackDetailByTutorId/{tutorId}
     * @secure
     */
    feedbackGetFeedbackDetailByTutorIdDetail: (
      tutorId: string,
      query?: {
        /** @default false */
        showAll?: boolean;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<FeedbackDetailApiResponse, any>({
        path: `/api/Feedback/GetFeedbackDetailByTutorId/${tutorId}`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Feedback
     * @name FeedbackGetByTutorLearnerSubjectIdDetail
     * @request GET:/api/Feedback/GetByTutorLearnerSubjectId/{tutorLearnerSubjectId}
     * @secure
     */
    feedbackGetByTutorLearnerSubjectIdDetail: (tutorLearnerSubjectId: number, params: RequestParams = {}) =>
      this.http.request<FeedbackDtoApiResponse, any>({
        path: `/api/Feedback/GetByTutorLearnerSubjectId/${tutorLearnerSubjectId}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Feedback
     * @name FeedbackCreateFeedbackCreate
     * @request POST:/api/Feedback/CreateFeedback
     * @secure
     */
    feedbackCreateFeedbackCreate: (data: CreateFeedbackDto, params: RequestParams = {}) =>
      this.http.request<FeedbackDtoApiResponse, any>({
        path: `/api/Feedback/CreateFeedback`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Feedback
     * @name FeedbackUpdateFeedbackUpdate
     * @request PUT:/api/Feedback/UpdateFeedback
     * @secure
     */
    feedbackUpdateFeedbackUpdate: (data: FeedbackDto, params: RequestParams = {}) =>
      this.http.request<FeedbackDtoApiResponse, any>({
        path: `/api/Feedback/UpdateFeedback`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Feedback
     * @name FeedbackGetStatisticsList
     * @request GET:/api/Feedback/GetStatistics
     * @secure
     */
    feedbackGetStatisticsList: (
      query?: {
        /** @format uuid */
        tutorId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<FeedbackStatisticsResponseApiResponse, any>({
        path: `/api/Feedback/GetStatistics`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Notification
     * @name NotificationGetAllNotificationsList
     * @request GET:/api/Notification/GetAllNotifications
     * @secure
     */
    notificationGetAllNotificationsList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 10
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<NotificationDtosApiResponse, any>({
        path: `/api/Notification/GetAllNotifications`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Notification
     * @name NotificationSendNotificationCreate
     * @request POST:/api/Notification/SendNotification
     * @secure
     */
    notificationSendNotificationCreate: (data: NotificationRequestDto, params: RequestParams = {}) =>
      this.http.request<NotificationRequestDtoApiResponse, any>({
        path: `/api/Notification/SendNotification`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Notification
     * @name NotificationSaveFcmTokenCreate
     * @request POST:/api/Notification/SaveFCMToken
     * @secure
     */
    notificationSaveFcmTokenCreate: (data: UserTokenDto, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Notification/SaveFCMToken`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Notification
     * @name NotificationMarkNotificationsAsReadUpdate
     * @request PUT:/api/Notification/MarkNotificationsAsRead
     * @secure
     */
    notificationMarkNotificationsAsReadUpdate: (data: number[], params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Notification/MarkNotificationsAsRead`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentCreatePaymentUrlCreate
     * @request POST:/api/Payment/CreatePaymentUrl/{billId}
     * @secure
     */
    paymentCreatePaymentUrlCreate: (billId: number, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Payment/CreatePaymentUrl/${billId}`,
        method: "POST",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentPaymentCallbackList
     * @request GET:/api/Payment/PaymentCallback
     * @secure
     */
    paymentPaymentCallbackList: (params: RequestParams = {}) =>
      this.http.request<PaymentResponseModelApiResponse, any>({
        path: `/api/Payment/PaymentCallback`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentGetPaymentByIdDetail
     * @request GET:/api/Payment/GetPaymentById/{paymentId}
     * @secure
     */
    paymentGetPaymentByIdDetail: (paymentId: number, params: RequestParams = {}) =>
      this.http.request<PaymentDetailsDTOApiResponse, any>({
        path: `/api/Payment/GetPaymentById/${paymentId}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentGetListPaymentsList
     * @request GET:/api/Payment/GetListPayments
     * @secure
     */
    paymentGetListPaymentsList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
        searchKeyword?: string;
        paymentStatus?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<PaymentDetailsDTOPagedResultApiResponse, any>({
        path: `/api/Payment/GetListPayments`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentGetPaymentsByTutorLearnerSubjectIdDetail
     * @request GET:/api/Payment/GetPaymentsByTutorLearnerSubjectId/{tutorLearnerSubjectId}
     * @secure
     */
    paymentGetPaymentsByTutorLearnerSubjectIdDetail: (
      tutorLearnerSubjectId: number,
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<PaymentDetailsDTOPagedResultApiResponse, any>({
        path: `/api/Payment/GetPaymentsByTutorLearnerSubjectId/${tutorLearnerSubjectId}`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestCreatePaymentRequestCreate
     * @request POST:/api/PaymentRequest/CreatePaymentRequest
     * @secure
     */
    paymentRequestCreatePaymentRequestCreate: (data: CreatePaymentRequestDTO, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/PaymentRequest/CreatePaymentRequest`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestGetListPaymentRequestsList
     * @request GET:/api/PaymentRequest/GetListPaymentRequests
     * @secure
     */
    paymentRequestGetListPaymentRequestsList: (
      query?: {
        /** @format int32 */
        PaymentRequestId?: number;
        TutorName?: string;
        IsPaid?: boolean;
        VerificationStatus?: string;
        /** @format date-time */
        FromDate?: string;
        /** @format date-time */
        ToDate?: string;
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<PaymentRequestDTOPagedResultApiResponse, any>({
        path: `/api/PaymentRequest/GetListPaymentRequests`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestGetListPaymentRequestsByTutorList
     * @request GET:/api/PaymentRequest/GetListPaymentRequestsByTutor
     * @secure
     */
    paymentRequestGetListPaymentRequestsByTutorList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<PaymentRequestDTOPagedResultApiResponse, any>({
        path: `/api/PaymentRequest/GetListPaymentRequestsByTutor`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestApprovePaymentRequestUpdate
     * @request PUT:/api/PaymentRequest/ApprovePaymentRequest/{paymentRequestId}
     * @secure
     */
    paymentRequestApprovePaymentRequestUpdate: (paymentRequestId: number, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/PaymentRequest/ApprovePaymentRequest/${paymentRequestId}`,
        method: "PUT",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestRejectPaymentRequestUpdate
     * @request PUT:/api/PaymentRequest/RejectPaymentRequest/{paymentRequestId}
     * @secure
     */
    paymentRequestRejectPaymentRequestUpdate: (
      paymentRequestId: number,
      data: RejectPaymentRequestDTO,
      params: RequestParams = {},
    ) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/PaymentRequest/RejectPaymentRequest/${paymentRequestId}`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestUpdatePaymentRequestUpdate
     * @request PUT:/api/PaymentRequest/UpdatePaymentRequest/{id}
     * @secure
     */
    paymentRequestUpdatePaymentRequestUpdate: (id: number, data: UpdatePaymentRequestDTO, params: RequestParams = {}) =>
      this.http.request<void, any>({
        path: `/api/PaymentRequest/UpdatePaymentRequest/${id}`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestConfirmPaymentRequestList
     * @request GET:/api/PaymentRequest/ConfirmPaymentRequest
     * @secure
     */
    paymentRequestConfirmPaymentRequestList: (
      query?: {
        /** @format int32 */
        requestId?: number;
        /** @format uuid */
        token?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<void, any>({
        path: `/api/PaymentRequest/ConfirmPaymentRequest`,
        method: "GET",
        query: query,
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags PaymentRequest
     * @name PaymentRequestDeletePaymentRequestByIdDelete
     * @request DELETE:/api/PaymentRequest/DeletePaymentRequestById/{id}
     * @secure
     */
    paymentRequestDeletePaymentRequestByIdDelete: (id: number, params: RequestParams = {}) =>
      this.http.request<void, any>({
        path: `/api/PaymentRequest/DeletePaymentRequestById/${id}`,
        method: "DELETE",
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Post
     * @name PostGetAllPostList
     * @request GET:/api/Post/GetAllPost
     * @secure
     */
    postGetAllPostList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        index?: number;
        /**
         * @format int32
         * @default 10
         */
        size?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<PostsHomePageDTOApiResponse, any>({
        path: `/api/Post/GetAllPost`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Post
     * @name PostGetPostsHomePageList
     * @request GET:/api/Post/GetPostsHomePage
     * @secure
     */
    postGetPostsHomePageList: (
      query?: {
        /**
         * @format int32
         * @default 0
         */
        index?: number;
        /**
         * @format int32
         * @default 10
         */
        size?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<PostsHomePageDTOApiResponse, any>({
        path: `/api/Post/GetPostsHomePage`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Post
     * @name PostGetAllPostCategoriesList
     * @request GET:/api/Post/GetAllPostCategories
     * @secure
     */
    postGetAllPostCategoriesList: (params: RequestParams = {}) =>
      this.http.request<PostCategoryIEnumerableApiResponse, any>({
        path: `/api/Post/GetAllPostCategories`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Post
     * @name PostGetPostByIdList
     * @request GET:/api/Post/GetPostById
     * @secure
     */
    postGetPostByIdList: (
      query?: {
        /** @format int32 */
        postId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<PostsDTOApiResponse, any>({
        path: `/api/Post/GetPostById`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Post
     * @name PostAddPostCreate
     * @request POST:/api/Post/AddPost
     * @secure
     */
    postAddPostCreate: (data: AddPostsDTO, params: RequestParams = {}) =>
      this.http.request<AddPostsDTOApiResponse, any>({
        path: `/api/Post/AddPost`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Post
     * @name PostUpdatePostCreate
     * @request POST:/api/Post/UpdatePost
     * @secure
     */
    postUpdatePostCreate: (data: UpdatePostDTO, params: RequestParams = {}) =>
      this.http.request<UpdatePostDTOApiResponse, any>({
        path: `/api/Post/UpdatePost`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Post
     * @name PostDeletePostCreate
     * @request POST:/api/Post/DeletePost
     * @secure
     */
    postDeletePostCreate: (
      query?: {
        /** @format int32 */
        postID?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<Int32ApiResponse, any>({
        path: `/api/Post/DeletePost`,
        method: "POST",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags QualificationLevel
     * @name QualificationLevelGetAllQualificationLevelsList
     * @request GET:/api/QualificationLevel/GetAllQualificationLevels
     * @secure
     */
    qualificationLevelGetAllQualificationLevelsList: (params: RequestParams = {}) =>
      this.http.request<QualificationLevelDtoIEnumerableApiResponse, any>({
        path: `/api/QualificationLevel/GetAllQualificationLevels`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags RateRange
     * @name RateRangeCreateRateRangeCreate
     * @request POST:/api/RateRange/CreateRateRange
     * @secure
     */
    rateRangeCreateRateRangeCreate: (data: RateRange, params: RequestParams = {}) =>
      this.http.request<RateRangeApiResponse, any>({
        path: `/api/RateRange/CreateRateRange`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags RateRange
     * @name RateRangeGetAllRateRangesList
     * @request GET:/api/RateRange/GetAllRateRanges
     * @secure
     */
    rateRangeGetAllRateRangesList: (params: RequestParams = {}) =>
      this.http.request<RateRangeIEnumerableApiResponse, any>({
        path: `/api/RateRange/GetAllRateRanges`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags RateRange
     * @name RateRangeGetRateRangeByIdList
     * @request GET:/api/RateRange/GetRateRangeById
     * @secure
     */
    rateRangeGetRateRangeByIdList: (
      query?: {
        /** @format int32 */
        id?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<RateRangeApiResponse, any>({
        path: `/api/RateRange/GetRateRangeById`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags RateRange
     * @name RateRangeUpdateRateRangeUpdate
     * @request PUT:/api/RateRange/UpdateRateRange
     * @secure
     */
    rateRangeUpdateRateRangeUpdate: (
      data: RateRange,
      query?: {
        /** @format int32 */
        id?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<RateRangeApiResponse, any>({
        path: `/api/RateRange/UpdateRateRange`,
        method: "PUT",
        query: query,
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags RateRange
     * @name RateRangeDeleteRateRangeDelete
     * @request DELETE:/api/RateRange/DeleteRateRange
     * @secure
     */
    rateRangeDeleteRateRangeDelete: (
      query?: {
        /** @format int32 */
        id?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/RateRange/DeleteRateRange`,
        method: "DELETE",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Schedule
     * @name ScheduleDetail
     * @request GET:/api/Schedule/{tutorId}
     * @secure
     */
    scheduleDetail: (tutorId: string, params: RequestParams = {}) =>
      this.http.request<ScheduleGroupDTOListApiResponse, any>({
        path: `/api/Schedule/${tutorId}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Schedule
     * @name ScheduleAddCreate
     * @request POST:/api/Schedule/add/{tutorId}
     * @secure
     */
    scheduleAddCreate: (tutorId: string, data: AddScheduleDTO, params: RequestParams = {}) =>
      this.http.request<ScheduleApiResponse, any>({
        path: `/api/Schedule/add/${tutorId}`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Schedule
     * @name ScheduleDeleteDelete
     * @request DELETE:/api/Schedule/delete/{tutorId}
     * @secure
     */
    scheduleDeleteDelete: (tutorId: string, data: DeleteScheduleDTO, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Schedule/delete/${tutorId}`,
        method: "DELETE",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Schedule
     * @name ScheduleUpdateUpdate
     * @request PUT:/api/Schedule/update/{tutorId}
     * @secure
     */
    scheduleUpdateUpdate: (tutorId: string, data: UpdateScheduleDTO, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Schedule/update/${tutorId}`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subject
     * @name SubjectGetAllSubjectsList
     * @request GET:/api/Subject/GetAllSubjects
     * @secure
     */
    subjectGetAllSubjectsList: (params: RequestParams = {}) =>
      this.http.request<SubjectFilterDTOIEnumerableApiResponse, any>({
        path: `/api/Subject/GetAllSubjects`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subject
     * @name SubjectCreateSubjectForSuperAdminCreate
     * @request POST:/api/Subject/CreateSubjectForSuperAdmin
     * @secure
     */
    subjectCreateSubjectForSuperAdminCreate: (data: SubjectDTO, params: RequestParams = {}) =>
      this.http.request<SubjectApiResponse, ObjectApiResponse>({
        path: `/api/Subject/CreateSubjectForSuperAdmin`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subject
     * @name SubjectGetTopSubjectDetail
     * @request GET:/api/Subject/get-top-subject/{size}
     * @secure
     */
    subjectGetTopSubjectDetail: (size: number, params: RequestParams = {}) =>
      this.http.request<SubjectFilterDTOListApiResponse, any>({
        path: `/api/Subject/get-top-subject/${size}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subject
     * @name SubjectGetSubjectByIdForSuperAdminList
     * @request GET:/api/Subject/GetSubjectByIdForSuperAdmin
     * @secure
     */
    subjectGetSubjectByIdForSuperAdminList: (subjectId: number, params: RequestParams = {}) =>
      this.http.request<SubjectApiResponse, ObjectApiResponse>({
        path: `/api/Subject/GetSubjectByIdForSuperAdmin`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subject
     * @name SubjectUpdateSubjectForSuperAdminUpdate
     * @request PUT:/api/Subject/UpdateSubjectForSuperAdmin{subjectId}
     * @secure
     */
    subjectUpdateSubjectForSuperAdminUpdate: (subjectId: number, data: SubjectDTO, params: RequestParams = {}) =>
      this.http.request<SubjectApiResponse, ObjectApiResponse>({
        path: `/api/Subject/UpdateSubjectForSuperAdmin${subjectId}`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subject
     * @name SubjectDeleteSubjectForSuperAdminDelete
     * @request DELETE:/api/Subject/DeleteSubjectForSuperAdmin/{subjectId}
     * @secure
     */
    subjectDeleteSubjectForSuperAdminDelete: (subjectId: number, params: RequestParams = {}) =>
      this.http.request<ObjectApiResponse, ObjectApiResponse>({
        path: `/api/Subject/DeleteSubjectForSuperAdmin/${subjectId}`,
        method: "DELETE",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorRegisterTutorCreate
     * @request POST:/api/Tutor/RegisterTutor
     * @secure
     */
    tutorRegisterTutorCreate: (data: AddTutorDTO, params: RequestParams = {}) =>
      this.http.request<AddTutorDTOApiResponse, any>({
        path: `/api/Tutor/RegisterTutor`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorMajorsWithMinorList
     * @request GET:/api/Tutor/MajorsWithMinor
     * @secure
     */
    tutorMajorsWithMinorList: (params: RequestParams = {}) =>
      this.http.request<MajorMinorDtoListApiResponse, any>({
        path: `/api/Tutor/MajorsWithMinor`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorGetTutorByIdDetail
     * @request GET:/api/Tutor/GetTutorById/{id}
     * @secure
     */
    tutorGetTutorByIdDetail: (id: string, params: RequestParams = {}) =>
      this.http.request<TutorDtoApiResponse, any>({
        path: `/api/Tutor/GetTutorById/${id}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorTutorHomePageCreate
     * @request POST:/api/Tutor/TutorHomePage
     * @secure
     */
    tutorTutorHomePageCreate: (
      data: TutorFilterDto,
      query?: {
        /**
         * @format int32
         * @default 0
         */
        index?: number;
        /**
         * @format int32
         * @default 20
         */
        size?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<TutorHomePageDTOApiResponse, any>({
        path: `/api/Tutor/TutorHomePage`,
        method: "POST",
        query: query,
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorDeleteTutorDelete
     * @request DELETE:/api/Tutor/DeleteTutor/{tutorId}
     * @secure
     */
    tutorDeleteTutorDelete: (tutorId: string, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Tutor/DeleteTutor/${tutorId}`,
        method: "DELETE",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorAllWithFeedbackList
     * @request GET:/api/Tutor/all-with-feedback
     * @secure
     */
    tutorAllWithFeedbackList: (params: RequestParams = {}) =>
      this.http.request<TutorRatingDtoListApiResponse, any>({
        path: `/api/Tutor/all-with-feedback`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorGetTopTutorDetail
     * @request GET:/api/Tutor/get-top-tutor/{size}
     * @secure
     */
    tutorGetTopTutorDetail: (size: number, params: RequestParams = {}) =>
      this.http.request<TutorSummaryDtoListApiResponse, any>({
        path: `/api/Tutor/get-top-tutor/${size}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorMyWalletOverviewList
     * @request GET:/api/Tutor/my-wallet-overview
     * @secure
     */
    tutorMyWalletOverviewList: (params: RequestParams = {}) =>
      this.http.request<WalletOverviewDtoApiResponse, any>({
        path: `/api/Tutor/my-wallet-overview`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorUpdateTutorInfoUpdate
     * @request PUT:/api/Tutor/update-tutor-info
     * @secure
     */
    tutorUpdateTutorInfoUpdate: (data: UpdateTutorInforDTO, params: RequestParams = {}) =>
      this.http.request<UpdateTutorInforDTOApiResponse, any>({
        path: `/api/Tutor/update-tutor-info`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorDeleteTeachingLocationDelete
     * @request DELETE:/api/Tutor/delete-teaching-location
     * @secure
     */
    tutorDeleteTeachingLocationDelete: (
      data: number[],
      query?: {
        /** @format uuid */
        tutorId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Tutor/delete-teaching-location`,
        method: "DELETE",
        query: query,
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorDeleteCertificateAsyncDelete
     * @request DELETE:/api/Tutor/DeleteCertificateAsync/{certiID}
     * @secure
     */
    tutorDeleteCertificateAsyncDelete: (
      certiId: number,
      query?: {
        /** @format uuid */
        tutorId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Tutor/DeleteCertificateAsync/${certiId}`,
        method: "DELETE",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tutor
     * @name TutorDeleteTutorSubjectAsyncDelete
     * @request DELETE:/api/Tutor/DeleteTutorSubjectAsync/{tutorSubjectID}
     * @secure
     */
    tutorDeleteTutorSubjectAsyncDelete: (
      tutorSubjectId: number,
      query?: {
        /** @format uuid */
        tutorId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<StringApiResponse, any>({
        path: `/api/Tutor/DeleteTutorSubjectAsync/${tutorSubjectId}`,
        method: "DELETE",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectRegisterLearnerCreate
     * @request POST:/api/TutorLearnerSubject/register-learner
     * @secure
     */
    tutorLearnerSubjectRegisterLearnerCreate: (
      data: RegisterLearnerDTO,
      query?: {
        /** @format uuid */
        tutorId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<RegisterLearnerDTOApiResponse, any>({
        path: `/api/TutorLearnerSubject/register-learner`,
        method: "POST",
        query: query,
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectDownloadContractDetail
     * @request GET:/api/TutorLearnerSubject/download-contract/{tutorLearnerSubjectID}
     * @secure
     */
    tutorLearnerSubjectDownloadContractDetail: (tutorLearnerSubjectId: number, params: RequestParams = {}) =>
      this.http.request<void, any>({
        path: `/api/TutorLearnerSubject/download-contract/${tutorLearnerSubjectId}`,
        method: "GET",
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectGetSubjectDetailsList
     * @request GET:/api/TutorLearnerSubject/get-subject-details
     * @secure
     */
    tutorLearnerSubjectGetSubjectDetailsList: (
      query?: {
        /** @format uuid */
        userId?: string;
        viewType?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<SubjectDetailDtoListApiResponse, any>({
        path: `/api/TutorLearnerSubject/get-subject-details`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectGetTutorLearnerSubjectDetailList
     * @request GET:/api/TutorLearnerSubject/get-tutor-learner-subject-detail
     * @secure
     */
    tutorLearnerSubjectGetTutorLearnerSubjectDetailList: (
      query?: {
        /** @format int32 */
        id?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<TutorLearnerSubjectSummaryDetailDtoApiResponse, any>({
        path: `/api/TutorLearnerSubject/get-tutor-learner-subject-detail`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectGetClassroomsList
     * @request GET:/api/TutorLearnerSubject/get-classrooms
     * @secure
     */
    tutorLearnerSubjectGetClassroomsList: (
      query?: {
        /** @format uuid */
        userId?: string;
        viewType?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<SubjectDetailDtoListApiResponse, any>({
        path: `/api/TutorLearnerSubject/get-classrooms`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectUpdateClassroomUpdate
     * @request PUT:/api/TutorLearnerSubject/update-classroom/{tutorLearnerSubjectID}
     * @secure
     */
    tutorLearnerSubjectUpdateClassroomUpdate: (
      tutorLearnerSubjectId: number,
      data: RegisterLearnerDTO,
      params: RequestParams = {},
    ) =>
      this.http.request<RegisterLearnerDTOApiResponse, any>({
        path: `/api/TutorLearnerSubject/update-classroom/${tutorLearnerSubjectId}`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectHandleContractUploadCreate
     * @request POST:/api/TutorLearnerSubject/handle-contract-upload
     * @secure
     */
    tutorLearnerSubjectHandleContractUploadCreate: (data: HandleContractUploadDTO, params: RequestParams = {}) =>
      this.http.request<StringApiResponse, StringApiResponse>({
        path: `/api/TutorLearnerSubject/handle-contract-upload`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectApiTutorlearnerVerifycontractCreate
     * @request POST:/api/TutorLearnerSubject/api/tutorlearner/verifycontract
     * @secure
     */
    tutorLearnerSubjectApiTutorlearnerVerifycontractCreate: (
      query?: {
        /** @format int32 */
        tutorLearnerSubjectId?: number;
        isVerified?: boolean;
        reason?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BooleanApiResponse, any>({
        path: `/api/TutorLearnerSubject/api/tutorlearner/verifycontract`,
        method: "POST",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectApiContractsList
     * @request GET:/api/TutorLearnerSubject/api/contracts
     * @secure
     */
    tutorLearnerSubjectApiContractsList: (
      query?: {
        Search?: string;
        IsVerified?: boolean;
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ContractDtoPagedResultApiResponse, any>({
        path: `/api/TutorLearnerSubject/api/contracts`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectCreateClassFromTutorRequestCreate
     * @request POST:/api/TutorLearnerSubject/createClassFromTutorRequest/{tutorRequestId}
     * @secure
     */
    tutorLearnerSubjectCreateClassFromTutorRequestCreate: (
      tutorRequestId: number,
      data: CreateClassDTO,
      params: RequestParams = {},
    ) =>
      this.http.request<void, any>({
        path: `/api/TutorLearnerSubject/createClassFromTutorRequest/${tutorRequestId}`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorLearnerSubject
     * @name TutorLearnerSubjectCloseClassUpdate
     * @request PUT:/api/TutorLearnerSubject/closeClass/{tutorLearnerSubjectId}
     * @secure
     */
    tutorLearnerSubjectCloseClassUpdate: (tutorLearnerSubjectId: number, params: RequestParams = {}) =>
      this.http.request<void, any>({
        path: `/api/TutorLearnerSubject/closeClass/${tutorLearnerSubjectId}`,
        method: "PUT",
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestGetAllTutorRequestsList
     * @request GET:/api/TutorRequest/GetAllTutorRequests
     * @secure
     */
    tutorRequestGetAllTutorRequestsList: (
      query?: {
        Search?: string;
        /** @format double */
        MinFee?: number;
        /** @format double */
        MaxFee?: number;
        CityId?: string;
        DistrictId?: string;
        Subject?: string;
        TutorGender?: string;
        /** @format int32 */
        TutorQualificationId?: number;
        /** @format int32 */
        RateRangeId?: number;
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<TutorRequestDTOPagedResultApiResponse, any>({
        path: `/api/TutorRequest/GetAllTutorRequests`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestGetTutorRequestByIdDetail
     * @request GET:/api/TutorRequest/GetTutorRequestById/{tutorRequestId}
     * @secure
     */
    tutorRequestGetTutorRequestByIdDetail: (tutorRequestId: number, params: RequestParams = {}) =>
      this.http.request<TutorRequestDTOApiResponse, any>({
        path: `/api/TutorRequest/GetTutorRequestById/${tutorRequestId}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestCreateTutorRequestCreate
     * @request POST:/api/TutorRequest/CreateTutorRequest
     * @secure
     */
    tutorRequestCreateTutorRequestCreate: (data: TutorRequestDTO, params: RequestParams = {}) =>
      this.http.request<TutorRequestDTOApiResponse, StringApiResponse>({
        path: `/api/TutorRequest/CreateTutorRequest`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestUpdateTutorRequestUpdate
     * @request PUT:/api/TutorRequest/UpdateTutorRequest/{tutorRequestId}
     * @secure
     */
    tutorRequestUpdateTutorRequestUpdate: (tutorRequestId: number, data: TutorRequestDTO, params: RequestParams = {}) =>
      this.http.request<void, any>({
        path: `/api/TutorRequest/UpdateTutorRequest/${tutorRequestId}`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestChooseTutorForTutorRequestAsyncUpdate
     * @request PUT:/api/TutorRequest/ChooseTutorForTutorRequestAsync/{tutorRequestId}
     * @secure
     */
    tutorRequestChooseTutorForTutorRequestAsyncUpdate: (
      tutorRequestId: number,
      query?: {
        /** @format uuid */
        tutorID?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ObjectApiResponse, any>({
        path: `/api/TutorRequest/ChooseTutorForTutorRequestAsync/${tutorRequestId}`,
        method: "PUT",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestRegisterTutorRequestCreate
     * @request POST:/api/TutorRequest/RegisterTutorRequest
     * @secure
     */
    tutorRequestRegisterTutorRequestCreate: (
      query?: {
        /** @format int32 */
        tutorRequestId?: number;
        /** @format uuid */
        tutorId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<BooleanApiResponse, any>({
        path: `/api/TutorRequest/RegisterTutorRequest`,
        method: "POST",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestGetListTutorRequestRegisteredDetail
     * @request GET:/api/TutorRequest/GetListTutorRequestRegistered/{tutorRequestId}
     * @secure
     */
    tutorRequestGetListTutorRequestRegisteredDetail: (tutorRequestId: number, params: RequestParams = {}) =>
      this.http.request<TutorRequestWithTutorsDTOApiResponse, any>({
        path: `/api/TutorRequest/GetListTutorRequestRegistered/${tutorRequestId}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestGetListTutorRequestsByLeanrerIdDetail
     * @request GET:/api/TutorRequest/GetListTutorRequestsByLeanrerID/{learnerId}
     * @secure
     */
    tutorRequestGetListTutorRequestsByLeanrerIdDetail: (
      learnerId: string,
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ListTutorRequestDTOPagedResult, any>({
        path: `/api/TutorRequest/GetListTutorRequestsByLeanrerID/${learnerId}`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestGetListTutorRequestsByTutorIdDetail
     * @request GET:/api/TutorRequest/GetListTutorRequestsByTutorID/{tutorId}
     * @secure
     */
    tutorRequestGetListTutorRequestsByTutorIdDetail: (
      tutorId: string,
      query?: {
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ListTutorRequestForTutorDtoPagedResult, any>({
        path: `/api/TutorRequest/GetListTutorRequestsByTutorID/${tutorId}`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestGetTutorRequestsAdminList
     * @request GET:/api/TutorRequest/GetTutorRequestsAdmin
     * @secure
     */
    tutorRequestGetTutorRequestsAdminList: (
      query?: {
        Search?: string;
        /** @format date-time */
        StartDate?: string;
        Subject?: string;
        IsVerified?: boolean;
        /**
         * @format int32
         * @default 0
         */
        pageIndex?: number;
        /**
         * @format int32
         * @default 20
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<ListTutorRequestDTOPagedResult, any>({
        path: `/api/TutorRequest/GetTutorRequestsAdmin`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestSendTutorRequestEmailCreate
     * @request POST:/api/TutorRequest/send-tutor-request-email/{tutorRequestId}
     * @secure
     */
    tutorRequestSendTutorRequestEmailCreate: (
      tutorRequestId: number,
      query?: {
        /** @format uuid */
        tutorID?: string;
      },
      params: RequestParams = {},
    ) =>
      this.http.request<void, any>({
        path: `/api/TutorRequest/send-tutor-request-email/${tutorRequestId}`,
        method: "POST",
        query: query,
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestGetTutorLearnerSubjectInfoByTutorRequestIdDetail
     * @request GET:/api/TutorRequest/GetTutorLearnerSubjectInfoByTutorRequestId/{tutorRequestId}
     * @secure
     */
    tutorRequestGetTutorLearnerSubjectInfoByTutorRequestIdDetail: (
      tutorRequestId: number,
      params: RequestParams = {},
    ) =>
      this.http.request<TutorLearnerSubjectDetailDtoApiResponse, any>({
        path: `/api/TutorRequest/GetTutorLearnerSubjectInfoByTutorRequestId/${tutorRequestId}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TutorRequest
     * @name TutorRequestCloseTutorRequestCreate
     * @request POST:/api/TutorRequest/CloseTutorRequest/{tutorRequestId}
     * @secure
     */
    tutorRequestCloseTutorRequestCreate: (tutorRequestId: number, params: RequestParams = {}) =>
      this.http.request<void, any>({
        path: `/api/TutorRequest/CloseTutorRequest/${tutorRequestId}`,
        method: "POST",
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name UserUpdateProfileCreate
     * @request POST:/api/User/UpdateProfile
     * @secure
     */
    userUpdateProfileCreate: (data: UpdateUserDTO, params: RequestParams = {}) =>
      this.http.request<UpdateUserDTOApiResponse, any>({
        path: `/api/User/UpdateProfile`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),
  };
}
