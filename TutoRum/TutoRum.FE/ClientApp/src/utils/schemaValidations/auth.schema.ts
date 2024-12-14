import z from "zod";

const passwordSchema = z
  .string({ required_error: "Mật khẩu không được để trống" })
  .min(8, { message: "Mật khẩu phải có ít nhất 8 ký tự" })
  .regex(/[A-Z]/, {
    message: "Mật khẩu phải có ít nhất một chữ cái viết hoa",
  })
  .regex(/[^A-Za-z0-9]/, {
    message: "Mật khẩu phải có ít nhất một ký tự đặc biệt",
  });

export const SignupBodySchema = z
  .object({
    email: z
      .string({ required_error: "Email không được để trống" })
      .email("Email phải là một email hợp lệ"),
    password: passwordSchema,
    confirmPassword: passwordSchema,
    fullName: z
      .string({ required_error: "Họ và tên không được để trống" })
      .min(1, "Họ và tên không được để trống")
      .regex(/^[\p{L} ]+$/u, "Họ và tên chỉ được chứa chữ cái và khoảng trắng"),
  })
  .superRefine(({ password, confirmPassword }, ctx) => {
    if (password !== confirmPassword) {
      ctx.addIssue({
        code: "custom",
        message: "Mật khẩu không khớp",
        path: ["confirmPassword"],
      });
    }
  });

export type SignupBodyType = z.TypeOf<typeof SignupBodySchema>;

export const LoginBodySchema = z.object({
  email: z
    .string({ required_error: "Email không được để trống" })
    .email("Email phải là một email hợp lệ"),
  password: passwordSchema,
});

export type LoginBodyType = z.TypeOf<typeof LoginBodySchema>;

export const ForgotPasswordSchema = z.object({
  email: z.string().email("Email phải là một email hợp lệ"),
});

export type ForgotPasswordType = z.TypeOf<typeof ForgotPasswordSchema>;

export const ResetPasswordBodySchema = z
  .object({
    email: z.string().email("Email phải là một email hợp lệ"),
    token: z.string(),
    newPassword: passwordSchema,
    confirmPassword: passwordSchema,
  })
  .superRefine(({ newPassword, confirmPassword }, ctx) => {
    if (newPassword !== confirmPassword) {
      ctx.addIssue({
        code: "custom",
        message: "Mật khẩu không khớp",
        path: ["confirmPassword"],
      });
    }
  });
export type ResetPasswordType = z.TypeOf<typeof ResetPasswordBodySchema>;

export const ChangePasswordBodySchema = z
  .object({
    currentPassword: passwordSchema,
    newPassword: passwordSchema,
    confirmPassword: passwordSchema,
  })
  .superRefine(({ newPassword, confirmPassword }, ctx) => {
    if (newPassword !== confirmPassword) {
      ctx.addIssue({
        code: "custom",
        message: "Mật khẩu không khớp",
        path: ["confirmPassword"],
      });
    }
  });
export type ChangePasswordType = z.TypeOf<typeof ChangePasswordBodySchema>;
