"use client";
import React, { useEffect, useMemo, useRef, useState } from "react";
import { Button, Card, DatePicker, InputNumber, Rate, Select } from "antd";
import PriceTag from "../../components/PriceTag";
import { useForm } from "react-hook-form";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import PriceInput from "@/components/ui/price-input";
import { Textarea } from "@/components/ui/textarea";
import AddressAutocomplete from "@/app/(user)/user/settings/user-profile/components/AddressAutocomplete";
import ClassroomLearningSchedule from "./ClassroomLearningSchedule";
import { useRegisterTutorSubject, useTutorDetail } from "@/hooks/use-tutor";
import {
  RegisterLearnerDTO,
  TutorLearnerSubjectSummaryDetailDto,
} from "@/utils/services/Api";
import {
  RegisterLearnerDTOSchema,
  RegisterLearnerDTOType,
} from "@/utils/schemaValidations/tutor.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import dayjs, { Dayjs } from "dayjs";
import { VerifyButton } from "@/app/(admin)/admin/tutors/[id]/components/verify-buttons";
import { useRouter } from "next/navigation";
import { useAppContext } from "@/components/provider/app-provider";
import AcceptTermsCondition from "@/app/(user)/user/settings/user-profile/components/AcceptTermsCondition";
import { toast } from "react-toastify";
import { useUpdateClassroom } from "@/hooks/use-tutor-learner-subject";
import ScheduleTable from "../../components/ScheduleTable";

const TutorLearnerSubjectDetailForm = ({
  tutorLearnerSubjectDetail,
}: {
  tutorLearnerSubjectDetail?: TutorLearnerSubjectSummaryDetailDto;
}) => {
  const form = useForm<RegisterLearnerDTOType>({
    resolver: zodResolver(RegisterLearnerDTOSchema),
    defaultValues: {
      address: tutorLearnerSubjectDetail?.locationDetail ?? undefined,
      expectedStartDate:
        tutorLearnerSubjectDetail?.expectedStartDate ?? undefined,
      hoursPerSession: tutorLearnerSubjectDetail?.hoursPerSession ?? undefined,
      notes: tutorLearnerSubjectDetail?.notes ?? undefined,
      preferredScheduleType:
        tutorLearnerSubjectDetail?.preferredScheduleType ?? undefined,
      pricePerHour: tutorLearnerSubjectDetail?.pricePerHour ?? undefined,
      sessionsPerWeek: tutorLearnerSubjectDetail?.sessionsPerWeek ?? undefined,
      ward: tutorLearnerSubjectDetail?.wardId ?? undefined,
      district: tutorLearnerSubjectDetail?.districtId ?? undefined,
      city: tutorLearnerSubjectDetail?.cityId ?? undefined,
      schedules: tutorLearnerSubjectDetail?.schedules?.map((s) => ({
        dayOfWeek: s.dayOfWeek ?? undefined,
        learningTimes: s.freeTimes?.map((lt, i) => ({
          startTime: lt.startTime ?? undefined,
          endTime: lt.endTime ?? undefined,
          availabilityIndex: i ?? undefined,
        })),
      })),
      tutorSubjectId: tutorLearnerSubjectDetail?.tutorSubjectId ?? undefined,
    },
  });
  const { user } = useAppContext();
  const subjectId = form.watch("tutorSubjectId");
  const selectedClassType = form.watch("preferredScheduleType");
  const { data: tutor } = useTutorDetail(user?.id!);
  const selectedSubject = tutor?.tutorSubjects?.find(
    (subject: any) => subject.tutorSubjectId == subjectId
  );
  const { mutateAsync: update, isLoading } = useUpdateClassroom(
    tutorLearnerSubjectDetail?.tutorLearnerSubjectId!
  );
  const route = useRouter();

  useEffect(() => {
    if (selectedSubject) {
      form.setValue("pricePerHour", selectedSubject.rate);
      !tutorLearnerSubjectDetail?.preferredScheduleType &&
        form.setValue("preferredScheduleType", selectedSubject.subjectType);
    }
  }, [
    form,
    selectedSubject,
    subjectId,
    tutorLearnerSubjectDetail?.preferredScheduleType,
  ]);

  async function onSubmit(values: RegisterLearnerDTOType) {
    try {
      const data: RegisterLearnerDTO = {
        ...values,
        schedules: values.schedules
          ?.filter((s) => !!s)
          .map((s) => ({
            dayOfWeek: s?.dayOfWeek,
            freeTimes: s?.learningTimes?.flatMap((lt) => ({
              startTime: lt.startTime,
              endTime: lt.endTime,
            })),
          })),
        cityId: values.city,
        districtId: values.district,
        wardId: values.ward,
        locationDetail: values.address,
      };
      const res = await update(data);
      if (res.status == 201) {
        toast.success("Cập nhật thành công");
      }
    } catch (err) {
      toast.error("Cập nhật không thành công");
      console.log(err);
    }
  }

  return (
    <div className="w-full">
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="flex flex-col xl:flex-row gap-10">
            {/* Subject and Price Fields */}
            <div className="flex flex-col xl:flex-row gap-5 w-full xl:w-1/2">
              <FormField
                control={form.control}
                name="tutorSubjectId"
                render={({ field }) => (
                  <FormItem className="w-full xl:w-1/2">
                    <FormLabel required>Môn học</FormLabel>
                    <Select
                      options={tutor?.tutorSubjects?.map((subject) => ({
                        value: subject.tutorSubjectId,
                        label: `${subject.subject?.subjectName} - ${
                          subject.subjectType === "FIXED"
                            ? "Cố định"
                            : "Linh hoạt"
                        }`,
                      }))}
                      placeholder="Chọn môn bạn muốn học"
                      className="w-full"
                      {...field}
                      onChange={(value) =>
                        form.setValue("tutorSubjectId", value)
                      }
                    />
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="pricePerHour"
                render={({ field }) => (
                  <FormItem className="w-full xl:w-1/2">
                    <FormLabel required>Giá tiền</FormLabel>
                    <PriceInput field={field} />
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            {/* Description Field */}
            <FormField
              control={form.control}
              name="notes"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel>Ghi chú</FormLabel>
                  <Textarea
                    placeholder="Ghi chú"
                    className="w-full"
                    {...field}
                    rows={2}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          {/* Address Autocomplete */}
          <div className="my-5">
            <AddressAutocomplete form={form} />
          </div>

          {/* Session and Class Type Fields */}
          <div className="flex flex-col xl:flex-row gap-10">
            <FormField
              control={form.control}
              name="sessionsPerWeek"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel required>Số buổi/tuần</FormLabel>
                  <div>
                    <InputNumber
                      style={{ width: "100%" }}
                      placeholder="3"
                      {...field}
                    />
                  </div>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="hoursPerSession"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel required>Số giờ/buổi</FormLabel>
                  <div>
                    <InputNumber
                      style={{ width: "100%" }}
                      placeholder="3"
                      {...field}
                    />
                  </div>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="preferredScheduleType"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel required>Kiểu thời gian</FormLabel>
                  <Select
                    className="w-full"
                    placeholder="Chọn kiểu thời gian"
                    disabled={selectedSubject?.subjectType !== "FLEXIBLE"}
                    {...field}
                    value={field.value?.trim()}
                    options={[
                      { value: "FIXED", label: "Cố định" },
                      { value: "FLEXIBLE", label: "Linh hoạt" },
                    ]}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="expectedStartDate"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel required>Thời gian dự kiến</FormLabel>
                  <DatePicker
                    placeholder="Chọn thời gian"
                    className="w-full"
                    {...field}
                    value={
                      field.value ? dayjs(field.value, "YYYY-MM-DD") : null
                    } // Kiểm tra giá trị đầu vào
                    onChange={(date) => {
                      field.onChange(dayjs(date).format("YYYY-MM-DD"));
                    }}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          <ScheduleTable
            schedules={tutorLearnerSubjectDetail?.schedules || []}
            title="Lịch học"
          />
          <div className="text-end">
            <Button
              className="mt-5"
              type="primary"
              htmlType="submit"
              loading={isLoading}
              disabled={
                !form.formState.isDirty ||
                tutorLearnerSubjectDetail?.isContractVerified == true ||
                tutorLearnerSubjectDetail?.isClosed == true
              }
            >
              Cập nhật
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
};

export default TutorLearnerSubjectDetailForm;
