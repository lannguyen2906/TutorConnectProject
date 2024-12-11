"use client";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import PriceInput from "@/components/ui/price-input";
import {
  useAddBillingEntry,
  useUpdateBillingEntry,
} from "@/hooks/use-billing-entry";
import {
  AddBillingEntryDTO,
  AddBillingEntrySchema,
} from "@/utils/schemaValidations/billing-entry.schema";
import { AdddBillingEntryDTO } from "@/utils/services/Api";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, DatePicker, TimePicker } from "antd";
import TextArea from "antd/es/input/TextArea";
import dayjs from "dayjs";
import { Calendar } from "lucide-react";
import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

const BillingEntryForm = ({
  tutorLearnerSubjectId,
  setOpen,
  billingEntryUpdate,
  billingEntryId,
}: {
  tutorLearnerSubjectId?: number;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  billingEntryUpdate?: AddBillingEntryDTO;
  billingEntryId?: number;
}) => {
  const form = useForm<AddBillingEntryDTO>({
    resolver: zodResolver(AddBillingEntrySchema),
    defaultValues: billingEntryUpdate,
  });

  const { mutateAsync: addBillingEntry, isLoading } = useAddBillingEntry();
  const { mutateAsync: updateBillingEntry, isLoading: isLoadingUpdate } =
    useUpdateBillingEntry(billingEntryId || 0);

  const onSubmit = async (data: AddBillingEntryDTO) => {
    try {
      const request: AdddBillingEntryDTO = {
        description: data.description,
        rate: data.rate,
        startDateTime: data.date + " " + data.startTime,
        endDateTime: data.date + " " + data.endTime,
        totalAmount: data.totalAmount,
        tutorLearnerSubjectId: tutorLearnerSubjectId,
      };
      const response = billingEntryId
        ? await updateBillingEntry(request)
        : await addBillingEntry(request);
      if (response.status === 201) {
        setOpen(false);
        toast.success(
          billingEntryId
            ? "Cập nhật buổi học thành công"
            : "Thêm buổi học thành công"
        );
      }
    } catch (error) {
      toast.error(
        billingEntryId
          ? "Cập nhật buổi học không thành công"
          : "Thêm buổi học không thành công"
      );
      console.log(error);
    }
  };

  useEffect(() => {
    const totalHour = dayjs(form.watch("endTime"), "HH:mm").diff(
      dayjs(form.watch("startTime"), "HH:mm"),
      "hour"
    );

    const rate = form.watch("rate");

    form.setValue(
      "totalAmount",
      totalHour && rate ? totalHour * rate : undefined
    );
  }, [
    form,
    form.watch("rate"),
    form.watch("startTime"),
    form.watch("endTime"),
  ]);

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <FormField
            control={form.control}
            name="date"
            render={({ field }) => (
              <FormItem className="flex flex-col gap-2">
                <FormLabel required>Ngày học</FormLabel>
                <DatePicker
                  {...field}
                  onChange={(date, dateString) => field.onChange(dateString)}
                  value={
                    field.value ? dayjs(field.value, "YYYY-MM-DD") : undefined
                  }
                  placeholder="Ngày"
                  className="w-full"
                />
                <FormMessage />
              </FormItem>
            )}
          />
          <FormItem className="flex flex-col gap-2">
            <FormLabel required>Giờ học</FormLabel>
            <TimePicker.RangePicker
              format="HH:mm"
              onChange={(_, dateStrings) => {
                form.setValue("startTime", dateStrings[0]);
                form.setValue("endTime", dateStrings[1]);
              }}
              defaultValue={[
                billingEntryUpdate
                  ? dayjs(billingEntryUpdate.startTime, "HH:mm")
                  : null,
                billingEntryUpdate
                  ? dayjs(billingEntryUpdate.endTime, "HH:mm")
                  : null,
              ]}
              minuteStep={10}
              needConfirm={false}
              onBlur={() => form.trigger(["startTime", "endTime"])}
              className="w-full"
            />
            <FormMessage />
          </FormItem>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <FormField
            control={form.control}
            name="rate"
            render={({ field }) => (
              <FormItem className="flex flex-col gap-2">
                <FormLabel required>Giá tiền 1 giờ</FormLabel>
                <PriceInput field={field} />
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="totalAmount"
            render={({ field }) => (
              <FormItem className="flex flex-col gap-2">
                <FormLabel required>Tổng tiền</FormLabel>
                <PriceInput readonly field={field} />
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <FormField
          control={form.control}
          name="description"
          render={({ field }) => (
            <FormItem className="flex flex-col gap-2">
              <FormLabel>Nhận xét</FormLabel>
              <TextArea
                {...field}
                placeholder="Ghi chú"
                rows={4}
                className="w-full"
              />
              <FormMessage />
            </FormItem>
          )}
        />
        <Button
          icon={<Calendar size={16} />}
          type="primary"
          htmlType="submit"
          loading={isLoading || isLoadingUpdate}
        >
          {billingEntryId ? "Cập nhật buổi học" : "Thêm buổi học"}
        </Button>
      </form>
    </Form>
  );
};

export default BillingEntryForm;
