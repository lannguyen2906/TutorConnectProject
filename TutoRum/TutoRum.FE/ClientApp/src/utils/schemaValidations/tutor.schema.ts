import { z } from "zod";

const CertificateSchema = z.object({
  uid: z.string(),
  certificateId: z.number().optional(),
  imgUrl: z.string().optional(),
  description: z.string().min(1, "Mô tả không được để trống"),
  issueDate: z.string().min(1, "Ngày cấp khóa học không được ü bao"),
  expiryDate: z.string().optional(),
  isVerified: z.boolean().optional(),
});

const TeachingLocationSchema = z.object({
  cityId: z
    .string({ required_error: "Vui lòng chọn thành phố" })
    .min(1, "Vui lòng chọn thành phố"),
  districtIds: z
    .array(z.string().min(1, "Vui lòng chọn quận"))
    .min(1, "Phải chọn ít nhất một quận"),
});

const FreeTimeSchema = z.object({
  startTime: z.string().min(1, "Giờ bắt đầu không được để trống"),
  endTime: z.string().min(1, "Giờ kết thúc không được để trống"),
});

const ScheduleSchema = z.object({
  dayOfWeek: z
    .number()
    .min(1, "Ngày phải nằm trong khoảng 1 đến 7")
    .max(7, "Ngày phải nằm trong khoảng 1 đến 7"),
  freeTimes: z.array(FreeTimeSchema),
});

const SubjectSchema = z.object({
  tutorSubjectId: z.number().optional(),
  subjectType: z
    .string({ required_error: "Kiểu môn học không được để trống" })
    .min(1, "Kiểu môn học không được để trống"),
  learnerType: z
    .number({ required_error: "Loại học sinh không được để trống" })
    .min(1, "Loại học sinh không được để trống"),
  minRate: z.number().min(0, "Giá phải là số không âm").optional(),
  maxRate: z.number().min(0, "Giá phải là số không âm").optional(),
  subjectName: z.string().min(1, "Tên môn học không được để trống"),
  rate: z.number().min(0, "Giá phải là số không âm"),
  description: z.string().optional(),
});

export const TutorSchema = z.object({
  experience: z
    .string({ required_error: "Kinh nghiệm không được để trống" })
    .min(0, "Kinh nghiệm phải là số không âm"),
  major: z
    .string({ required_error: "Chuyên ngành không được để trống" })
    .min(1, "Chuyên ngành không được để trống"),
  specialization: z
    .string({ required_error: "Chuyên ngành hẹp không được để trống" })
    .min(1, "Chuyên ngành hẹp không được để trống"),
  briefIntroduction: z
    .string({ required_error: "Giới thiệu ngắn gọn không nên để trống" })
    .min(1, "Giới thiệu ngắn gọn không nên để trống"),
  educationalLevel: z
    .string({ required_error: "Trình độ không được để trống" })
    .min(1, "Trình độ không được để trống"),
  profileDescription: z
    .string({ required_error: "Giới thiệu chi tiết không được để trống" })
    .min(1, "Mô tả hồ sơ không được để trống"),
  certificates: z.array(CertificateSchema).optional(),
  teachingLocations: z.array(TeachingLocationSchema, {
    required_error: "Vui lòng",
  }),
  schedule: z.array(ScheduleSchema),
  subjects: z.array(SubjectSchema),
  videoUrl: z.string().optional(),
});

export type TutorType = z.TypeOf<typeof TutorSchema>;

export const TutorFilterSchema = z.object({
  subjects: z.array(z.string()),
  minPrice: z
    .string()
    .transform((val) => Number(val))
    .refine((val) => val >= 0, "Giá tiền tối thiểu không hợp lệ")
    .optional(),
  maxPrice: z
    .string()
    .transform((val) => Number(val))
    .refine((val) => val >= 0, "Giá tiền tối đa không hợp lệ")
    .optional(),
  city: z.string().optional(),
  district: z.string().optional(),
  searchingQuery: z.string().optional(),
});
export type TutorFilterType = z.infer<typeof TutorFilterSchema>;

const learningTime = z.object({
  availabilityIndex: z.number(),
  startTime: z.string().min(1, "Giờ bắt đầu không được để trống"),
  endTime: z.string().min(1, "Giờ kết thúc không được để trống"),
});

const LearningSchema = z.object({
  dayOfWeek: z
    .number()
    .min(1, "Ngày phải nằm trong khoảng 1 đến 7")
    .max(7, "Ngày phải nằm trong khoảng 1 đến 7"),
  learningTimes: z.array(learningTime).nullable(), // Cho phép null ở đây
});

// Tạo schema cho RegisterLearnerDTO
export const RegisterLearnerDTOSchema = z.object({
  tutorSubjectId: z.number(),
  location: z.string().nullable().optional(),
  pricePerHour: z.number().nullable().optional(),
  notes: z.string().optional(),
  city: z.string(),
  district: z.string(),
  ward: z.string(),
  address: z.string(),
  sessionsPerWeek: z.number().int(),
  hoursPerSession: z.number().int(),
  preferredScheduleType: z.string().nullable().optional(),
  expectedStartDate: z.string(),
  schedules: z.array(LearningSchema.optional()).nullable(),
});

export type RegisterLearnerDTOType = z.infer<typeof RegisterLearnerDTOSchema>;
