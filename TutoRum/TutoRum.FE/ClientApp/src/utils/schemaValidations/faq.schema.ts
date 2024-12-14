import z from "zod";
export const FaqSchema = z.object({
  question: z
    .string()
    .trim()
    .min(10, { message: "Câu hỏi phải dài hơn 10 ký tự" }),
  answer: z
    .string()
    .trim()
    .min(10, { message: "Câu trả lời phải dài hơn 10 ký tự" }),
});

export type FaqSchemaType = z.TypeOf<typeof FaqSchema>;
