"use-client";
import SelectPriceRange from "@/app/(user)/tutor-requests/components/SelectPriceRange";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import PriceInput from "@/components/ui/price-input";
import { useCreateRateRange, useUpdateRateRange } from "@/hooks/use-rate-range";
import { RateRange } from "@/utils/services/Api";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input } from "antd";
import TextArea from "antd/es/input/TextArea";
import React from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

import { z } from "zod";

export const rateRangeSchema = z.object({
  id: z.number().int().optional(), // Số nguyên 32-bit, tùy chọn
  level: z.string().nullable().optional(), // Chuỗi hoặc null, tùy chọn
  minRate: z.number().optional(), // Số thực, tùy chọn
  maxRate: z.number().optional(), // Số thực, tùy chọn
  description: z.string().nullable().optional(), // Chuỗi hoặc null, tùy chọn
});

const RateRangeForm = ({ dataRateRange }: { dataRateRange?: RateRange }) => {
  const { mutateAsync: createRateRange, isLoading: isCreating } =
    useCreateRateRange();
  const { mutateAsync: updateRateRange, isLoading: isUpdating } =
    useUpdateRateRange();

  const form = useForm<RateRange>({
    resolver: zodResolver(rateRangeSchema),
    defaultValues: dataRateRange ? dataRateRange : undefined,
  });

  const onSubmit = async (data: RateRange) => {
    try {
      const response = dataRateRange
        ? await updateRateRange(data)
        : await createRateRange(data);
      if (response.status === 201) {
        form.reset();
        toast.success(`${dataRateRange ? "Cập nhật" : "Thêm mới"} thành công`);
      }
    } catch (error) {
      toast.error(
        `${dataRateRange ? "Cập nhật" : "Thêm mới"} không thành công`
      );
      console.log(error);
    }
  };

  return (
    <div>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-2">
          <FormField
            control={form.control}
            name="level"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Cấp độ</FormLabel>
                <FormControl>
                  <Input
                    {...field}
                    value={dataRateRange?.level || field.value || ""}
                    placeholder="Cấp độ"
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="minRate"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Từ</FormLabel>
                <FormControl>
                  <PriceInput field={field} addonAfter="VND" />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="maxRate"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Đến</FormLabel>
                <FormControl>
                  <PriceInput field={field} addonAfter="VND" />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="description"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Mô tả</FormLabel>
                <FormControl>
                  <TextArea
                    {...field}
                    value={dataRateRange?.description || field.value || ""}
                    rows={4}
                    placeholder="Mô tả"
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
              {dataRateRange ? "Cập nhật" : "Thêm mới"}
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
};

export default RateRangeForm;
