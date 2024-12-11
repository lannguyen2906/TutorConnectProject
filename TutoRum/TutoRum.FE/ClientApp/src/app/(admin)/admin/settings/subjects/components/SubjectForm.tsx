"use-client";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { useCreateSubject, useUpdateSubject } from "@/hooks/use-subject";
import { RateRange, SubjectDTO } from "@/utils/services/Api";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input } from "antd";
import React from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

import { z } from "zod";

export const subjectSchema = z.object({
  subjectName: z.string(), // Chuỗi hoặc null, tùy chọn
});

const SubjectForm = ({ subject }: { subject?: SubjectDTO }) => {
  const { mutateAsync: createSubject, isLoading: isCreating } =
    useCreateSubject();
  const { mutateAsync: updateSubject, isLoading: isUpdating } =
    useUpdateSubject();

  const form = useForm<SubjectDTO>({
    resolver: zodResolver(subjectSchema),
    defaultValues: subject ? subject : undefined,
  });

  const onSubmit = async (data: SubjectDTO) => {
    try {
      const request = subject
        ? {
            subjectId: subject.subjectId,
            subjectName: data.subjectName,
          }
        : data;
      const response = subject
        ? await updateSubject(request)
        : await createSubject(request);
      if (response.status === 201) {
        form.reset();
        toast.success(`${subject ? "Cập nhật" : "Thêm mới"} thành công`);
      }
    } catch (error) {
      toast.error(`${subject ? "Cập nhật" : "Thêm mới"} không thành công`);
      console.log(error);
    }
  };

  return (
    <div>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-2">
          <FormField
            control={form.control}
            name="subjectName"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Tên môn học</FormLabel>
                <FormControl>
                  <Input
                    {...field}
                    value={field.value || ""}
                    placeholder="Tên môn học"
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <div className="flex justify-end mt-2">
            <Button
              htmlType="submit"
              type="primary"
              loading={isCreating || isUpdating}
            >
              {subject ? "Cập nhật" : "Thêm mới"}
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
};

export default SubjectForm;
