import { z } from "zod";

const FreeScheduleSchema = z.array(
  z.object({
    daysOfWeek: z.array(
      z
        .number()
        .min(1, "Ngày phải nằm trong khoảng 1 đến 7")
        .max(7, "Ngày phải nằm trong khoảng 1 đến 7")
    ),
    freeTimes: z.array(
      z.object({
        startTime: z.string().min(1, "Giờ bắt đầu không được thiếu").optional(),
        endTime: z.string().min(1, "Giờ kết thúc không được thiếu").optional(),
      })
    ),
  })
);

export const TutorRequestSchema = z.object({
  learnerType: z.number().min(1, "Loại học sinh không được để trống"),
  minRate: z.number().min(0, "Giá phải là số không âm").optional(),
  maxRate: z.number().min(0, "Giá phải là số không âm").optional(),
  phoneNumber: z
    .string({ required_error: "Số điện thoại không được để trống" })
    .min(1, "Số điện thoại không hợp lệ"),
  requestSummary: z
    .string({ required_error: "Mô tả ngắn gọn không được để trống" })
    .min(1, "Mô tả ngắn gọn không được để trống"),
  city: z
    .string({ required_error: "Thành phố không được để trống" })
    .min(1, "Vui lòng chọn thành phố"),
  district: z
    .string({ required_error: "Quận huyện không được để trống" })
    .min(1, "Vui lòng chọn quận"),
  ward: z
    .string({ required_error: "Phường xã không được để trống" })
    .min(1, "Vui lòng chọn xã"),
  address: z
    .string({ required_error: "Địa chỉ chi tiết không được để trống" })
    .min(1, "Vui lòng chọn địa chỉ chi tiết"),
  numberOfStudents: z
    .number({ required_error: "Số lượng học sinh ít nhất là 1" })
    .min(1, "Số lượng học sinh phải lớn hơn 1"),
  startDate: z
    .string({ required_error: "Ngày bắt đầu không được để trống" })
    .refine((val) => !isNaN(Date.parse(val)), "Ngày không hợp lệ"),
  preferedScheduleType: z
    .string({ required_error: "Vui lòng chọn loại thời gian" })
    .min(1, "Vui lòng chọn loại thời gian"),
  timePerSession: z
    .number({ required_error: "Số giờ mỗi buổi nên lớn hơn hoặc bằng 1" })
    .min(1, "Time per session must be positive"),
  subject: z
    .string({ required_error: "Môn học không được để trống" })
    .min(1, "Môn học không được để trống"),
  studentGender: z
    .string({ required_error: "Giới tính không được để trống" })
    .refine((value) => ["male", "female"].includes(value), {
      message: "Giới tính phải là male hoặc female",
    }),
  tutorGender: z
    .string({ required_error: "Giới tính không được để trống" })
    .refine((value) => ["male", "female"].includes(value), {
      message: "Giới tính phải là male hoặc female",
    }),
  fee: z
    .number({ required_error: "Giá tiền không được để trống" })
    .min(0, "Giá phải là số không âm"),
  sessionsPerWeek: z
    .number({ required_error: "Phải có ít nhất 1 buổi học 1 tuần" })
    .min(1, "Sessions per week must be at least 1"),
  detailedDescription: z.string().optional(),
  tutorQualificationId: z.string({
    required_error: "Vui lòng chọn trình độ của gia sư",
  }),
  freeSchedules: FreeScheduleSchema,
});
export type TutorRequestType = z.TypeOf<typeof TutorRequestSchema>;

export type FreeScheduleType = z.TypeOf<typeof FreeScheduleSchema>;
