import { z } from "zod";

export const AddBillingEntrySchema = z.object({
  rate: z.number(),
  date: z.string(),
  startTime: z.string(),
  endTime: z.string(),
  description: z.string().optional(),
  totalAmount: z.number().nullable().optional(),
});

export type AddBillingEntryDTO = z.infer<typeof AddBillingEntrySchema>;
