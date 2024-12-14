import { differenceInYears } from "date-fns";
import { z } from "zod";

export const UserProfileSchema = z
  .object({
    id: z.string().optional(),
    fullName: z
      .string({ required_error: "Họ và tên không được để trống" })
      .min(1, "Họ và tên không được để trống")
      .regex(/^[\p{L} ]+$/u, "Họ và tên chỉ được chứa chữ cái và khoảng trắng"),
    phoneNumber: z
      .string()
      .optional()
      .refine(
        (phoneNumber) => !phoneNumber || /^\d+$/.test(phoneNumber), // Chỉ kiểm tra regex khi có giá trị
        {
          message: "Số điện thoại chỉ được chứa số",
        }
      )
      .refine(
        (phoneNumber) => !phoneNumber || phoneNumber.length === 10, // Chỉ kiểm tra độ dài khi có giá trị
        {
          message: "Số điện thoại phải có đủ 10 ký tự",
        }
      ),
    email: z
      .string({ required_error: "Email không được để trống" })
      .email("Email không hợp lệ"),
    gender: z
      .string({ required_error: "Giới tính không được để trống" })
      .refine((value) => ["male", "female"].includes(value), {
        message: "Giới tính phải là male hoặc female",
      })
      .optional(),
    dateOfBirth: z
      .string()
      .optional()
      .refine(
        (value) =>
          !value || // Không kiểm tra nếu không có giá trị
          (value && differenceInYears(new Date(), new Date(value)) >= 16),
        {
          message: "Bạn phải đủ 16 tuổi trở lên",
        }
      ),
    city: z.string().optional(),
    district: z.string().optional(),
    ward: z.string().optional(),
    address: z.string().optional(),
    avatarUrl: z.string().optional(),
    roles: z.array(z.string()).optional(),
    balance: z.number().optional(),
  })
  .strict();

export type UserProfileBodyType = z.TypeOf<typeof UserProfileSchema>;
