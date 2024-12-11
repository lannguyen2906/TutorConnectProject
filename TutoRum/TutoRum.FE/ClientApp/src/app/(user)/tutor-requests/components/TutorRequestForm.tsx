"use client";

import React, { useEffect, useState } from "react";
import { useAppContext } from "@/components/provider/app-provider";
import { useRouter } from "next/navigation";
import {
  Button,
  DatePicker,
  Input,
  InputNumber,
  Select,
  TimePicker,
} from "antd";
import { LogIn, Save, Send } from "lucide-react";
import { useForm, Controller, useFormContext } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  FormMessage,
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
} from "@/components/ui/form";
import TextArea from "antd/es/input/TextArea";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { FaMale, FaFemale } from "react-icons/fa";
import dayjs from "dayjs";
import {
  TutorRequestSchema,
  TutorRequestType,
} from "@/utils/schemaValidations/tutor.request.schema";
import { cn } from "@/lib/utils";
import AddressAutocomplete from "../../user/settings/user-profile/components/AddressAutocomplete";
import SubjectSelect from "./SubjectSelect";
import PriceInput from "@/components/ui/price-input";
import QualificationLevelSelect from "../../tutors/[id]/components/QualificationLevelSelect";
import { toast } from "react-toastify";
import {
  useCreateTutorRequest,
  useUpdateTutorRequest,
} from "@/hooks/use-tutor-request";
import { TutorRequestDTO } from "@/utils/services/Api";
import SelectFreeSchedules from "./SelectFreeSchedules";
import {
  formatFreeSchedules,
  mapTutorRequestToForm,
} from "@/utils/other/mapper";
import { formatNumber } from "@/utils/other/formatter";
import { useAllRateRanges } from "@/hooks/use-rate-range";

const TutorRequestForm = ({
  tutorRequestDto,
}: {
  tutorRequestDto?: TutorRequestDTO;
}) => {
  const { user } = useAppContext();
  const router = useRouter();
  const form = useForm<TutorRequestType>({
    resolver: zodResolver(TutorRequestSchema),
    defaultValues: tutorRequestDto
      ? mapTutorRequestToForm(tutorRequestDto)
      : {
          freeSchedules: [
            { daysOfWeek: [], freeTimes: [{ startTime: "", endTime: "" }] },
          ],
        },
  });
  const selectedLearnerGender = form.watch("studentGender");
  const selectedTutorGender = form.watch("tutorGender");
  const { mutateAsync, isLoading } = useCreateTutorRequest();
  const { mutateAsync: updateTutorRequest, isLoading: updateLoading } =
    useUpdateTutorRequest();
  const selectedSubject = form.watch("subject");
  const { data: rateRanges } = useAllRateRanges();

  useEffect(() => {
    form.setValue(
      "requestSummary",
      `Tìm gia sư ${
        selectedTutorGender == "female" ? "nữ" : "nam"
      } dạy ${selectedSubject}`
    );
  }, [form, selectedSubject, selectedTutorGender]);

  async function onSubmit(values: TutorRequestType) {
    try {
      const request: TutorRequestDTO = {
        rateRangeId: values.learnerType,
        requestSummary: values.requestSummary,
        subject: values.subject,
        numberOfStudents: values.numberOfStudents,
        studentGender: values.studentGender,
        tutorGender: values.tutorGender,
        startDate: dayjs(values.startDate).format("YYYY-MM-DD"),
        tutorQualificationId: Number.parseInt(values.tutorQualificationId),
        cityId: values.city,
        districtId: values.district,
        wardId: values.ward,
        teachingLocation: values.address,
        phoneNumber: values.phoneNumber,
        preferredScheduleType: values.preferedScheduleType,
        detailedDescription: values.detailedDescription || "",
        status: "",
        sessionsPerWeek: values.sessionsPerWeek,
        fee: values.fee,
        freeSchedules: formatFreeSchedules(values.freeSchedules),
        id: tutorRequestDto?.id,
        timePerSession: dayjs()
          .startOf("day") // Bắt đầu từ 00:00:00
          .add(values.timePerSession, "hour") // Thêm số giờ từ timePerSession
          .format("HH:mm:ss"), // Định dạng thành HH:mm:ss
      };
      const data = tutorRequestDto
        ? await updateTutorRequest(request)
        : await mutateAsync(request);

      if (data.status == 201) {
        toast.success(`${tutorRequestDto ? "Sửa" : "Tạo"} đăng ký thành công`);
        router.push("/tutor-requests");
      }
    } catch (err) {
      toast.error(
        `${tutorRequestDto ? "Sửa" : "Tạo"} yêu cầu không thành công`
      );
      console.log(err);
    } finally {
    }
  }
  return (
    <div>
      <Form {...form}>
        <div className="flex flex-col gap-4">
          {/* Left Side */}
          <div className="flex flex-col gap-4">
            <div className="flex gap-4">
              <div className="flex-1">
                <h3 className="text-lg font-semibold my-2 border-b-2 border-muted">
                  Yêu cầu về môn học
                </h3>
                <FormItem className="flex-1">
                  <FormLabel required>Đối tượng học</FormLabel>
                  <FormControl>
                    <Controller
                      name="learnerType"
                      control={form.control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          options={rateRanges?.map((item) => ({
                            value: item.id,
                            label: `${item.level} - ${formatNumber(
                              item.minRate?.toString() || "0"
                            )} đến ${formatNumber(
                              item.maxRate?.toString() || "0"
                            )} VND`,
                          }))}
                          onChange={(v) => {
                            const rateRange = rateRanges?.find(
                              (r) => r.id == v
                            );
                            field.onChange(v);
                            form.setValue(`minRate`, rateRange?.minRate || 0);
                            form.setValue(`maxRate`, rateRange?.maxRate || 0);
                          }}
                          placeholder="Đối tượng dạy học"
                          className="w-full"
                        />
                      )}
                    />
                  </FormControl>
                  <FormMessage>
                    {form.formState.errors.fee?.message}
                  </FormMessage>
                </FormItem>
                <div className="flex-1 flex gap-4">
                  {/* Row 1: Yêu cầu về môn học */}
                  <FormItem className="flex-1">
                    <FormLabel required>Môn học</FormLabel>
                    <FormControl>
                      <Controller
                        name="subject"
                        control={form.control}
                        render={({ field }) => <SubjectSelect field={field} />}
                      />
                    </FormControl>
                    <FormMessage>
                      {form.formState.errors.subject?.message}
                    </FormMessage>
                  </FormItem>

                  <FormItem className="flex-1">
                    <FormLabel required>Giá tiền (vnd)</FormLabel>
                    <FormControl>
                      <Controller
                        name="fee"
                        control={form.control}
                        render={({ field }) => (
                          <PriceInput
                            field={field}
                            disabled={form.watch("learnerType") == null}
                            minRate={form.watch("minRate")}
                            maxRate={form.watch("maxRate")}
                          />
                        )}
                      />
                    </FormControl>
                    <FormMessage>
                      {form.formState.errors.fee?.message}
                    </FormMessage>
                  </FormItem>
                </div>
              </div>

              <div className="flex-1">
                <h3 className="text-lg font-semibold my-2 border-b-2 border-muted">
                  Yêu cầu về gia sư
                </h3>
                <div className="flex-1 flex gap-4">
                  <FormField
                    control={form.control}
                    name="tutorGender"
                    render={({ field }) => (
                      <FormItem className="w-full xl:w-1/2">
                        <FormLabel>Giới tính gia sư</FormLabel>
                        <FormControl>
                          <RadioGroup
                            onValueChange={field.onChange}
                            defaultValue={field.value}
                            className="flex"
                          >
                            <FormItem>
                              <FormLabel
                                className={cn(
                                  "font-normal border-2 rounded-lg p-2 flex gap-1 cursor-pointer",
                                  {
                                    "bg-black text-white":
                                      selectedTutorGender === "male",
                                  }
                                )}
                              >
                                <span>Nam</span>
                                <FaMale
                                  color={
                                    selectedTutorGender === "male"
                                      ? "white"
                                      : "black"
                                  }
                                />
                              </FormLabel>
                              <FormControl>
                                <RadioGroupItem value="male" hidden={true} />
                              </FormControl>
                            </FormItem>

                            <FormItem>
                              <FormLabel
                                className={cn(
                                  "font-normal border-2 rounded-lg p-2 flex gap-1 cursor-pointer",
                                  {
                                    "bg-black text-white":
                                      selectedTutorGender === "female",
                                  }
                                )}
                              >
                                <span>Nữ</span>
                                <FaFemale
                                  color={
                                    selectedTutorGender === "female"
                                      ? "white"
                                      : "black"
                                  }
                                />
                              </FormLabel>
                              <FormControl>
                                <RadioGroupItem value="female" hidden={true} />
                              </FormControl>
                            </FormItem>
                          </RadioGroup>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormItem className="flex-1">
                    <FormLabel>Trình độ</FormLabel>
                    <FormControl>
                      <Controller
                        name="tutorQualificationId"
                        control={form.control}
                        render={({ field }) => (
                          <QualificationLevelSelect field={field} />
                        )}
                      />
                    </FormControl>
                    <FormMessage>
                      {form.formState.errors.tutorQualificationId?.message}
                    </FormMessage>
                  </FormItem>
                </div>
                {/* Row 2: Yêu cầu về gia sư */}
              </div>
            </div>
            <h3 className="text-lg font-semibold my-2 border-b-2 border-muted">
              Yêu cầu về lớp học
            </h3>
            {/* Row 2: Yêu cầu về lớp học */}
            <div className="flex gap-4">
              <FormItem className="flex-1">
                <FormLabel required>Mô tả ngắn</FormLabel>
                <FormControl>
                  <Controller
                    name="requestSummary"
                    control={form.control}
                    render={({ field }) => (
                      <TextArea {...field} placeholder="Mô tả ngắn" rows={3} />
                    )}
                  />
                </FormControl>
                <FormMessage>
                  {form.formState.errors.requestSummary?.message}
                </FormMessage>
              </FormItem>

              <FormItem className="flex-1">
                <FormLabel>Mô tả chi tiết</FormLabel>
                <FormControl>
                  <Controller
                    name="detailedDescription"
                    control={form.control}
                    render={({ field }) => (
                      <TextArea
                        {...field}
                        placeholder="Mô tả chi tiết"
                        rows={3}
                      />
                    )}
                  />
                </FormControl>
                <FormMessage>
                  {form.formState.errors.detailedDescription?.message}
                </FormMessage>
              </FormItem>
            </div>
            <AddressAutocomplete form={form} />
            <div className="flex-1 flex gap-2">
              <FormField
                control={form.control}
                name="numberOfStudents"
                render={({ field }) => (
                  <FormItem className="flex-1">
                    <FormLabel required>Số lượng học viên</FormLabel>
                    <div className="w-full">
                      <InputNumber
                        style={{ width: "100%" }}
                        placeholder="Chọn số lượng học sinh"
                        {...field}
                      />
                    </div>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormItem className="flex-1">
                <FormLabel required>Số điện thoại liên lạc</FormLabel>
                <FormControl>
                  <Controller
                    name="phoneNumber"
                    control={form.control}
                    render={({ field }) => (
                      <div className="w-full">
                        <Input
                          style={{ width: "100%" }}
                          {...field}
                          placeholder="0987654321"
                        />
                      </div>
                    )}
                  />
                </FormControl>
                <FormMessage>
                  {form.formState.errors.timePerSession?.message}
                </FormMessage>
              </FormItem>
              <FormField
                control={form.control}
                name="studentGender"
                render={({ field }) => (
                  <FormItem className="flex-1">
                    <FormLabel>Giới tính học viên</FormLabel>
                    <FormControl>
                      <RadioGroup
                        onValueChange={field.onChange}
                        defaultValue={field.value}
                        className="flex"
                      >
                        <FormItem>
                          <FormLabel
                            className={cn(
                              "font-normal border-2 rounded-lg p-2 flex gap-1 cursor-pointer",
                              {
                                "bg-black text-white":
                                  selectedLearnerGender === "male",
                              }
                            )}
                          >
                            <span>Nam</span>
                            <FaMale
                              color={
                                selectedLearnerGender === "male"
                                  ? "white"
                                  : "black"
                              }
                            />
                          </FormLabel>
                          <FormControl>
                            <RadioGroupItem value="male" hidden={true} />
                          </FormControl>
                        </FormItem>

                        <FormItem>
                          <FormLabel
                            className={cn(
                              "font-normal border-2 rounded-lg p-2 flex gap-1 cursor-pointer",
                              {
                                "bg-black text-white":
                                  selectedLearnerGender === "female",
                              }
                            )}
                          >
                            <span>Nữ</span>
                            <FaFemale
                              color={
                                selectedLearnerGender === "female"
                                  ? "white"
                                  : "black"
                              }
                            />
                          </FormLabel>
                          <FormControl>
                            <RadioGroupItem value="female" hidden={true} />
                          </FormControl>
                        </FormItem>
                      </RadioGroup>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <div className="flex gap-4">
              <div className="flex-1 flex gap-2">
                <FormField
                  control={form.control}
                  name="preferedScheduleType"
                  render={({ field }) => (
                    <FormItem className="flex-1">
                      <FormLabel required>Kiểu thời gian</FormLabel>
                      <Select
                        className="w-full"
                        placeholder="Chọn kiểu thời gian"
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
                <FormItem className="flex-1">
                  <FormLabel required>Số giờ mỗi buổi</FormLabel>
                  <FormControl>
                    <Controller
                      name="timePerSession"
                      control={form.control}
                      render={({ field }) => (
                        <div className="w-full">
                          <InputNumber
                            style={{ width: "100%" }}
                            {...field}
                            placeholder="Giờ"
                            min={1}
                            max={10}
                            onChange={(value) =>
                              field.onChange(Math.round(value || 0))
                            }
                          />
                        </div>
                      )}
                    />
                  </FormControl>
                  <FormMessage>
                    {form.formState.errors.timePerSession?.message}
                  </FormMessage>
                </FormItem>
              </div>
              <div className="flex-1 flex gap-2">
                <FormItem className="flex-1">
                  <FormLabel required>Số buổi mỗi tuần</FormLabel>
                  <FormControl>
                    <Controller
                      name="sessionsPerWeek"
                      control={form.control}
                      render={({ field }) => (
                        <div className="w-full">
                          <InputNumber
                            style={{ width: "100%" }}
                            {...field}
                            placeholder="Buổi"
                          />
                        </div>
                      )}
                    />
                  </FormControl>
                  <FormMessage>
                    {form.formState.errors.sessionsPerWeek?.message}
                  </FormMessage>
                </FormItem>

                <FormItem className="flex-1">
                  <FormLabel required>Ngày bắt đầu</FormLabel>
                  <FormControl>
                    <Controller
                      name="startDate"
                      control={form.control}
                      render={({ field }) => (
                        <div className="w-full">
                          <DatePicker
                            className="w-full"
                            {...field}
                            placeholder="Ngày bắt đầu"
                            format="YYYY-MM-DD"
                            onChange={(date, dateString) =>
                              field.onChange(dateString)
                            }
                            value={
                              field.value
                                ? dayjs(field.value, "YYYY-MM-DD")
                                : null
                            }
                          />
                        </div>
                      )}
                    />
                  </FormControl>
                  <FormMessage>
                    {form.formState.errors.startDate?.message}
                  </FormMessage>
                </FormItem>
              </div>
            </div>
          </div>
        </div>

        <div className="space-y-2 my-2">
          <FormLabel required>Thời gian rảnh</FormLabel>
          <SelectFreeSchedules />
        </div>
      </Form>
      <div className="col-span-2 flex justify-end mt-4">
        <Button
          onClick={form.handleSubmit(onSubmit)}
          htmlType="submit"
          type="primary"
          icon={<Send size={16} />}
          loading={isLoading || updateLoading}
          disabled={isLoading || updateLoading}
        >
          {tutorRequestDto ? "Sửa" : "Tạo"}
        </Button>
      </div>
    </div>
  );
};

export default TutorRequestForm;
