import z from "zod";

export const AccountRes = z
  .object({
    data: z.object({
      id: z.string(),
      name: z.string(),
      email: z.string(),
    }),
    message: z.string(),
  })
  .strict();

export type AccountResType = z.TypeOf<typeof AccountRes>;

export const UpdateMeBody = z.object({
  name: z.string().trim().min(2).max(256),
});

export type UpdateMeBodyType = z.TypeOf<typeof UpdateMeBody>;

export const AssignRoleSchema = z.object({
  position: z
    .string()
    .min(1, "Chức danh không được để trống.")
    .max(100, "Chức danh không được vượt quá 100 ký tự."),
  hireDate: z.string(),
  salary: z
    .string()
    .min(0, "Lương phải là một số dương.")
    .max(1000000, "Lương không được vượt quá 1.000.000."),
  supervisorId: z.string().optional(),
});

export type AssignRoleType = z.TypeOf<typeof AssignRoleSchema>;

export interface FullProfile {
  id: string;
  fullName: string;
  dateOfBirth: string;
  address: string;
  city: string;
  district: string;
  ward: string;
  email: string;
  gender: string;
  phoneNumber: string;
  avatarUrl: string;
  roles: string[];
  balance: number;
}
