import { z } from "zod";

export const TutorProfileSchema = z
  .object({
    job: z.string().trim().min(2).max(256),
    
  })
  .strict();

export type TutorProfileBodyType = z.TypeOf<typeof TutorProfileSchema>;
