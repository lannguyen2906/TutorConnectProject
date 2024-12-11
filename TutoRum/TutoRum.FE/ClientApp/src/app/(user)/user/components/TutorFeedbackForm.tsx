"use client";
import { useAppContext } from "@/components/provider/app-provider";
import {
  Form,
  FormField,
  FormLabel,
  FormMessage,
  FormItem,
} from "@/components/ui/form";
import Logo from "@/components/ui/logo";
import { Separator } from "@/components/ui/separator";
import { useBillsByTutorLearnerSubject } from "@/hooks/use-billing-entry";
import {
  useCreateFeedback,
  useFeedbackDetailByTutorLearnerSubjectId,
  useUpdateFeedback,
} from "@/hooks/use-feedback";
import { useTutorDetail } from "@/hooks/use-tutor";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import { CreateFeedbackDto, FeedbackDto } from "@/utils/services/Api";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Radio, Space, Spin } from "antd";
import TextArea from "antd/es/input/TextArea";
import dayjs from "dayjs";
import { MessageSquare } from "lucide-react";
import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { z } from "zod";

// Định nghĩa lại schema với các giá trị giảm dần
const tutorFeedbackSchema = z.object({
  punctuality: z
    .number({ required_error: "Vui lòng chọn mức độ đúng giờ của gia sư" })
    .min(1, "Vui lòng chọn mức độ đúng giờ của gia sư"),
  teachingSkills: z
    .number({
      required_error: "Vui lòng chọn mức kỹ năng sư phạm của gia sư",
    })
    .min(1, "Vui lòng chọn mức kỹ năng sư phạm của gia sư"),
  supportQuality: z
    .number({ required_error: "Vui lòng chọn mức độ hỗ trợ của gia sư" })
    .min(1, "Vui lòng chọn mức độ hỗ trợ của gia sư"),
  responseToQuestions: z
    .number({ required_error: "Vui nhập mức đáp ứng thắc mắc của gia sư" })
    .min(1, "Vui lòng chọn mức đáp ứng thắc mắc của gia sư"),
  satisfaction: z
    .number({ required_error: "Vui nhập mức độ hài lòng của bạn" })
    .min(1, "Vui lòng chọn mức độ hài lòng của bạn"),
  note: z.string().min(1, "Vui lòng nhập ghi chú"),
});

type TutorFeedbackType = z.infer<typeof tutorFeedbackSchema>;

const mapTutorFeedbackToCreateFeedbackDto = (
  data: FeedbackDto
): TutorFeedbackType => {
  return {
    punctuality: data.punctuality || 0,
    teachingSkills: data.teachingSkills || 0,
    supportQuality: data.supportQuality || 0,
    responseToQuestions: data.responseToQuestions || 0,
    satisfaction: data.satisfaction || 0,
    note: data.comments || "",
  };
};

const TutorFeedbackForm = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const { data: tutorLearnerSubjectDetail } = useTutorLearnerSubjectDetail(
    tutorLearnerSubjectId
  );
  const { data: currentFeedback } = useFeedbackDetailByTutorLearnerSubjectId(
    tutorLearnerSubjectId
  );
  const { data: tutor, isLoading } = useTutorDetail(
    tutorLearnerSubjectDetail?.tutorId!
  );
  const { user } = useAppContext();
  const { data } = useBillsByTutorLearnerSubject(1, 10, tutorLearnerSubjectId);
  const { mutateAsync: createFeedback, isLoading: isLoadingCreate } =
    useCreateFeedback();
  const { mutateAsync: updateFeedback, isLoading: isLoadingUpdate } =
    useUpdateFeedback();

  const form = useForm<TutorFeedbackType>({
    resolver: zodResolver(tutorFeedbackSchema),
  });

  useEffect(() => {
    if (currentFeedback) {
      form.reset(mapTutorFeedbackToCreateFeedbackDto(currentFeedback));
    }
  }, [currentFeedback, form]);

  const onSubmit = async (formData: TutorFeedbackType) => {
    try {
      const addRequest = {
        tutorLearnerSubjectId: tutorLearnerSubjectId,
        comments: formData.note,
        rating:
          (formData.punctuality / 4 +
            formData.teachingSkills / 4 +
            formData.supportQuality / 4 +
            formData.responseToQuestions / 4 +
            formData.satisfaction / 5) /
          5,
        punctuality: formData.punctuality,
        teachingSkills: formData.teachingSkills,
        supportQuality: formData.supportQuality,
        responseToQuestions: formData.responseToQuestions,
        satisfaction: formData.satisfaction,
      };
      const updateRequest: FeedbackDto = {
        ...addRequest,
        tutorLearnerSubjectId: tutorLearnerSubjectId,
        feedbackId: currentFeedback?.feedbackId!,
      };
      const data = currentFeedback
        ? await updateFeedback(updateRequest)
        : await createFeedback(addRequest);
      if (data.status === 201) {
        toast.success("Gửi feedback thành công");
      }
    } catch (err: any) {
      toast.error(err);
    }
  };

  if (data?.items?.filter((b) => b.isPaid).length == 0) {
    return (
      <div className="w-full flex flex-col items-center justify-center gap-4 text-Blueviolet text-lg font-bold">
        <Logo hasText={false} size={200} />
        Bạn cần thanh toán hóa đơn đầu tiên để có thể mở khóa chức năng này
      </div>
    );
  }

  if (tutorLearnerSubjectDetail?.tutorId == user?.id) {
    return (
      <div className="w-full flex flex-col items-center justify-center gap-4 text-Blueviolet text-lg font-bold">
        <Logo hasText={false} size={200} />
        Bạn là gia sư không thể xem chức năng này
      </div>
    );
  }

  return (
    <div className="p-6 space-y-4 bg-white rounded-lg shadow-md">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <span className="font-semibold">Mã lớp:</span>
          {tutorLearnerSubjectDetail?.tutorLearnerSubjectId}
        </div>
        <div>
          <span className="font-semibold">Ngày bắt đầu:</span>{" "}
          {dayjs(tutorLearnerSubjectDetail?.expectedStartDate).format(
            "DD/MM/YYYY"
          )}
        </div>
        <div>
          <span className="font-semibold">Môn học:</span>{" "}
          {
            tutor?.tutorSubjects?.find(
              (ts) =>
                ts.tutorSubjectId == tutorLearnerSubjectDetail?.tutorSubjectId
            )?.subject?.subjectName
          }
        </div>
        <div>
          <span className="font-semibold">Gia sư: </span>
          <span className="text-Blueviolet font-bold">{tutor?.fullName}</span>
        </div>
        <div>
          <span className="font-semibold">Được tạo vào: </span>
          <span>
            {currentFeedback?.createdDate &&
              dayjs(currentFeedback?.createdDate).format("HH:mm DD/MM/YYYY")}
          </span>
        </div>
        <div>
          <span className="font-semibold">Cập nhật lần cuối: </span>
          <span>
            {currentFeedback?.updatedDate &&
              dayjs(currentFeedback?.updatedDate).format("HH:mm DD/MM/YYYY")}
          </span>
        </div>
      </div>

      <Separator className="w-full" />

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
          <div className="flex flex-col lg:flex-row gap-5">
            <FormField
              name="punctuality"
              control={form.control}
              render={({ field }) => (
                <FormItem className="flex flex-col gap-2 flex-1">
                  <FormLabel required className="leading-5">
                    Sự đúng giờ của gia sư trong giờ học
                  </FormLabel>
                  <Radio.Group {...field}>
                    <Space direction="vertical">
                      <Radio value={4}>Luôn đúng giờ</Radio>
                      <Radio value={3}>Phần lớn đúng giờ</Radio>
                      <Radio value={2}>Ít khi đúng giờ</Radio>
                      <Radio value={1}>Không bao giờ đúng giờ</Radio>
                    </Space>
                  </Radio.Group>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              name="teachingSkills"
              control={form.control}
              render={({ field }) => (
                <FormItem className="flex flex-col gap-2 flex-1">
                  <FormLabel required className="leading-5">
                    Kỹ năng sư phạm của gia sư
                  </FormLabel>
                  <Radio.Group {...field}>
                    <Space direction="vertical">
                      <Radio value={4}>Xuất sắc</Radio>
                      <Radio value={3}>Tốt</Radio>
                      <Radio value={2}>Khá</Radio>
                      <Radio value={1}>Yếu</Radio>
                    </Space>
                  </Radio.Group>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="flex flex-col lg:flex-row gap-5">
            <FormField
              name="supportQuality"
              control={form.control}
              render={({ field }) => (
                <FormItem className="flex flex-col gap-2 flex-1">
                  <FormLabel required className="leading-5">
                    Mức độ hỗ trợ trong học tập của gia sư
                  </FormLabel>
                  <Radio.Group {...field}>
                    <Space direction="vertical">
                      <Radio value={4}>Rất tốt</Radio>
                      <Radio value={3}>Tốt</Radio>
                      <Radio value={2}>Khá</Radio>
                      <Radio value={1}>Cần cải thiện</Radio>
                    </Space>
                  </Radio.Group>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              name="responseToQuestions"
              control={form.control}
              render={({ field }) => (
                <FormItem className="flex flex-col gap-2 flex-1">
                  <FormLabel required className="leading-5">
                    Đáp ứng yêu cầu của học viên
                  </FormLabel>
                  <Radio.Group {...field}>
                    <Space direction="vertical">
                      <Radio value={4}>Rất nhanh</Radio>
                      <Radio value={3}>Nhanh</Radio>
                      <Radio value={2}>Chậm</Radio>
                      <Radio value={1}>Rất chậm</Radio>
                    </Space>
                  </Radio.Group>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="flex flex-col lg:flex-row gap-5">
            <FormField
              name="satisfaction"
              control={form.control}
              render={({ field }) => (
                <FormItem className="flex flex-col gap-2 flex-1">
                  <FormLabel required className="leading-5">
                    Mức độ hài lòng của học viên với khóa học
                  </FormLabel>
                  <Radio.Group {...field}>
                    <Space direction="vertical">
                      <Radio value={5}>Rất hài lòng</Radio>
                      <Radio value={4}>Hài lòng</Radio>
                      <Radio value={3}>Bình thường</Radio>
                      <Radio value={2}>Chưa hài lòng</Radio>
                      <Radio value={1}>Không hài lòng</Radio>
                    </Space>
                  </Radio.Group>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              name="note"
              control={form.control}
              render={({ field }) => (
                <FormItem className="flex flex-col gap-2 flex-1">
                  <FormLabel required className="leading-5">
                    Nhận xét khác
                  </FormLabel>
                  <TextArea {...field} rows={4} />
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="text-center">
            <Button
              type="primary"
              size="large"
              icon={<MessageSquare />}
              htmlType="submit"
              disabled={
                isLoadingCreate ||
                isLoadingUpdate ||
                tutorLearnerSubjectDetail?.isClosed === true
              }
              loading={isLoadingCreate || isLoadingUpdate}
            >
              {currentFeedback ? "Cập nhật đánh giá" : "Gửi Đánh Giá"}
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
};

export default TutorFeedbackForm;
