import z from "zod";

export const PostSchema = z
  .object({
    postId: z.number().optional(),
    title: z.string().trim().min(16, { message: "Tên phải lớn hơn 16 ký tự" }),
    subContent: z
      .string()
      .trim()
      .min(16, { message: "Mô tả phải lớn hơn 16 ký tự" }),
    postType: z.string(),
    content: z.string().trim().min(2),
  })
  .strict();

export type PostType = z.TypeOf<typeof PostSchema>;

export const DisplayPostSchema = z
  .object({
    title: z.string().trim().min(2).max(256),
    categoryName: z.string().trim(),
    writer: z.object({
      name: z.string().trim().min(2).max(256),
      avatarUrl: z.string().trim(),
    }),
    thumbnailUrl: z.string().trim(),
    content: z.string().trim().min(2),
    createdDate: z.date(),
    updatedDate: z.date().optional(),
  })
  .strict();

export type DisplayPostType = z.TypeOf<typeof DisplayPostSchema>;
