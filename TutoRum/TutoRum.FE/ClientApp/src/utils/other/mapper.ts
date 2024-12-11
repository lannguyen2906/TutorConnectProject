import { ITutor } from "@/app/(user)/tutors/[id]/components/mockData";
import { TutorType } from "../schemaValidations/tutor.schema";
import { TutorDto, TutorRequestDTO, TutorSubjectDto } from "../services/Api";
import {
  FreeScheduleType,
  TutorRequestType,
} from "../schemaValidations/tutor.request.schema";
import dayjs from "dayjs";

export const scheduleColumns: Record<number, string> = {
  1: "Chủ nhật",
  2: "Thứ hai",
  3: "Thứ ba",
  4: "Thứ tư",
  5: "Thứ năm",
  6: "Thứ sáu",
  7: "Thứ bảy",
};
export interface AddressResponse {
  error: number;
  error_text: string;
  data: {
    name: string;
    name_en: string;
    full_name: string;
    full_name_en: string;
    latitude: string;
    longitude: string;
    tinh: string; // City
    quan: string; // District
    phuong: string; // Ward
  };
}

const routeTranslations: { [key: string]: string } = {
  home: "Trang chủ",
  posts: "Bài Viết",
  contact: "Liên hệ",
  "user-profile": "Thông tin cá nhân",
  // Thêm các route khác ở đây
  admin: "Dashboard",
  accounts: "Tài khoản",
  tutors: "Danh sách gia sư",
  learners: "Danh sách học viên",
  settings: "Cài đặt chung",
  notifications: "Thông báo",
  password: "Mật khẩu",
  user: "Nguời dùng",
  "learning-classrooms": "Lớp đang học",
  "teaching-classrooms": "Lớp đang dạy",
  "registered-learners": "Học viên đăng ký",
  "registered-tutors": "Gia sư đăng ký",
  "tutor-requests": "Yêu cầu tìm gia sư",
  "billing-entries": "Buổi học",
  contract: "Hợp đồng",
  feedback: "Nhận xét",
  bills: "Hóa đơn",
  "close-classroom": "Đóng lớp",
};

export const mapRouteToName = (route: string) => routeTranslations[route];

export const mapTutorToForm = (data: TutorDto): TutorType => {
  return {
    experience: data.experience?.toString()!,
    major: data?.major!,
    specialization: data?.specialization!,
    briefIntroduction: data?.briefIntroduction!,
    educationalLevel: data.educationalLevelID?.toString()!, // Giá trị mặc định
    profileDescription: data?.profileDescription!,
    certificates: data.certificates?.map((cert: any, index: number) => ({
      uid: `${data.tutorId}-cert-${index}`, // Tạo UID duy nhất
      certificateId: cert?.certificateId!,
      imgUrl: cert?.imgUrl!,
      description: cert?.description!,
      issueDate: cert?.issueDate!,
      expiryDate: cert?.expiryDate!,
      isVerified: false, // Mặc định là chưa xác minh
    })),
    teachingLocations: data.teachingLocations?.map((location: any) => ({
      cityId: location.cityId,
      districtIds: location.districts.map(
        (district: any) => district.districtId
      ),
    }))!,
    schedule: data.schedules?.map((schedule: any) => ({
      dayOfWeek: schedule.dayOfWeek,
      freeTimes: schedule.freeTimes.map((time: any) => ({
        startTime: time.startTime,
        endTime: time.endTime,
      })),
    }))!,
    subjects: data.tutorSubjects?.map((subject: TutorSubjectDto) => ({
      tutorSubjectId: subject?.tutorSubjectId!,
      learnerType: subject?.rateRangeId!,
      subjectType: subject?.subjectType!,
      subjectName: subject.subject?.subjectName!,
      rate: subject?.rate!,
      description: subject?.description!,
    }))!,
    videoUrl: data?.videoUrl!,
  };
};

const daysOfWeekMap: Record<number, string> = {
  1: "Chủ nhật",
  2: "Thứ 2",
  3: "Thứ 3",
  4: "Thứ 4",
  5: "Thứ 5",
  6: "Thứ 6",
  7: "Thứ 7",
};

export const formatFreeSchedules = (schedules: FreeScheduleType): string => {
  return schedules
    .map((schedule) => {
      const dayNames = schedule.daysOfWeek
        .map((day) => daysOfWeekMap[day])
        .join(", ");

      const timeRanges = schedule.freeTimes
        .map((time) => `${time.startTime} đến ${time.endTime}`)
        .join(" | ");

      return `${dayNames}: ${timeRanges}`;
    })
    .join(" | ");
};

export const parseFormattedFreeSchedules = (
  formattedSchedules: string
): FreeScheduleType => {
  const schedules: FreeScheduleType = formattedSchedules
    .split(" | ")
    .map((schedule) => {
      const [daysPart, timesPart] = schedule.split(": ");

      const daysOfWeek = daysPart
        ?.split(", ")
        .map((day) => {
          const entry = Object.entries(daysOfWeekMap).find(
            ([_, value]) => value === day
          );
          return entry ? parseInt(entry[0]) : null;
        })
        .filter((day) => day !== null) as number[];

      console.log(schedule, timesPart);

      const freeTimes =
        timesPart?.split(" | ").map((timeRange) => {
          const [startTime, endTime] = timeRange.split(" đến ");
          return { startTime, endTime };
        }) || [];

      return { daysOfWeek, freeTimes };
    });

  return schedules;
};

export const mapTutorRequestToForm = (
  tutorRequest: TutorRequestDTO
): TutorRequestType => ({
  address: tutorRequest.teachingLocation?.split(",")[0] || "",
  city: tutorRequest.cityId || "",
  district: tutorRequest.districtId || "",
  ward: tutorRequest.wardId || "",
  preferedScheduleType: tutorRequest.preferredScheduleType || "FLEXIBLE",
  sessionsPerWeek: tutorRequest.sessionsPerWeek || 0,
  fee: tutorRequest.fee || 0,
  freeSchedules: parseFormattedFreeSchedules(tutorRequest.freeSchedules || ""),
  numberOfStudents: tutorRequest.numberOfStudents || 0,
  subject: tutorRequest.subject || "",
  studentGender: tutorRequest.studentGender || "",
  tutorGender: tutorRequest.tutorGender || "",
  timePerSession: dayjs(tutorRequest.timePerSession || "", "HH:mm:ss").hour(),
  startDate: tutorRequest.startDate || "",
  requestSummary: tutorRequest.requestSummary || "",
  phoneNumber: tutorRequest.phoneNumber || "",
  tutorQualificationId: tutorRequest.tutorQualificationId?.toString() || "",
  detailedDescription: tutorRequest.detailedDescription || "",
  learnerType: tutorRequest.rateRangeId || 0,
});
