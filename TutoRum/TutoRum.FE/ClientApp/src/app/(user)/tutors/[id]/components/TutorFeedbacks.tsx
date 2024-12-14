"use client";
import { Avatar, Button, Card, List, Modal, Rate, Slider, Spin } from "antd";
import { IFeedback, ITutor } from "./mockData";
import { formatDateVi } from "@/utils/other/formatter";
import { useState } from "react";
import { cn } from "@/lib/utils";
import { ScrollArea } from "@/components/ui/scroll-area";
import { useFeedbackDetailByTutorId } from "@/hooks/use-feedback";
import {
  FeedbackDetail,
  FeedbackDto,
  UserFeedbackDto,
} from "@/utils/services/Api";
import dayjs from "dayjs";

interface TutorFeedbacksProps {
  feedbackDetail: FeedbackDetail;
  size?: "md" | "lg";
  column?: number;
}

const TutorFeedbacks = ({ tutorId }: { tutorId: string }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const { data: tutorFeedbacks, isLoading } = useFeedbackDetailByTutorId(
    tutorId,
    false
  );

  if (isLoading) return <Spin />;

  if (!tutorFeedbacks?.avarageRating) return <></>;

  return (
    <div className="my-8">
      <div className="text-2xl font-bold mb-5 mt-10 border-b-2 border-gray-200 pb-2">
        Học viên đánh giá tôi
      </div>
      <FeedbackBreakdowns feedbackDetail={tutorFeedbacks!} size="lg" />

      <ListFeedbacks key="list" feedbacks={tutorFeedbacks?.feedbacks || []} />
      <div className="flex justify-center">
        <Button onClick={() => setIsModalOpen(true)}>
          Xem tất cả đánh giá
        </Button>
        <AllFeedbacks
          isModalOpen={isModalOpen}
          setIsModalOpen={setIsModalOpen}
          tutorId={tutorId}
        />
      </div>
    </div>
  );
};

const AllFeedbacks = ({
  isModalOpen,
  setIsModalOpen,
  tutorId,
}: {
  isModalOpen: boolean;
  setIsModalOpen: (value: boolean) => void;
  tutorId: string;
}) => {
  // Truyền isModalOpen vào hook useFeedbackDetailByTutorId
  const { data: tutorFeedbacks, isLoading } = useFeedbackDetailByTutorId(
    tutorId,
    true
  );
  return (
    <Modal
      title="Các đánh giá về tôi"
      open={isModalOpen}
      onCancel={() => setIsModalOpen(false)}
      footer={null}
      style={{ top: 20 }}
      loading={isLoading}
    >
      <ScrollArea className="h-[600px] px-5">
        <FeedbackBreakdowns feedbackDetail={tutorFeedbacks!} size="md" />
        <ListFeedbacks
          key="modal"
          feedbacks={tutorFeedbacks?.feedbacks || []}
          column={1}
        />
      </ScrollArea>
    </Modal>
  );
};

const ListFeedbacks = ({
  feedbacks,
  column = 2,
}: {
  feedbacks: UserFeedbackDto[];
  column?: number;
}) => {
  return (
    <List
      key={feedbacks.length}
      dataSource={feedbacks}
      grid={{
        gutter: 16,
        column,
      }}
      renderItem={(feedback) => (
        <List.Item>
          <div className="mb-10 hover:bg-muted transition-all p-4 rounded-sm">
            <div className="flex flex-col items-center lg:flex-row  gap-2">
              <Avatar
                size={40}
                src={feedback.avatarUrl}
                alt={feedback.fullName || "avatar"}
                className="rounded-full"
              />
              <div className="flex flex-col justify-between gap-2 w-full">
                <div className="flex flex-col items-center md:flex-row w-full justify-between">
                  <p className="font-bold">{feedback.fullName}</p>
                  <p className="text-Blueviolet font-semibold">
                    Lớp {feedback.tutorLearnerSubjectName}
                  </p>
                </div>
                <div className="flex flex-col items-center md:flex-row justify-between">
                  <Rate
                    style={{ fontSize: 16 }}
                    disabled
                    defaultValue={feedback.rating || 0}
                  />
                  <p className="italic text-xs">
                    {feedback.createdDate
                      ? dayjs(feedback.createdDate).format("DD/MM/YYYY")
                      : ""}
                  </p>
                </div>
              </div>
            </div>
            <p className="mt-4 text-center md:text-start">
              {feedback.comments ?? "Không có nhận xét chi tiết"}
            </p>
          </div>
        </List.Item>
      )}
    />
  );
};

// Hàm để lấy phần note từ chuỗi comments
const extractNote = (comments: string) => {
  const regex = /note:\s*([^#]*)/i; // Biểu thức chính quy tìm phần 'note'
  const match = comments.match(regex);
  return match ? match[1]?.trim() : ""; // Nếu tìm thấy thì trả về note, nếu không trả về chuỗi rỗng
};

const FeedbackBreakdowns = ({ feedbackDetail, size }: TutorFeedbacksProps) => (
  <div className="flex flex-col lg:flex-row items-center gap-5 mb-10">
    <div className="flex flex-col items-center gap-4 w-full lg:w-1/3">
      <div
        className={cn(
          "font-bold text-blueviolet",
          size === "md" ? "text-3xl" : "text-7xl"
        )}
      >
        {Math.round((feedbackDetail.avarageRating || 0) * 100) / 100}
      </div>
      <Rate
        disabled
        allowHalf
        defaultValue={feedbackDetail.avarageRating || 4.9}
      />
      <span
        className={cn(
          "tracking-wide text-gray-600",
          size === "lg" && "text-lg"
        )}
      >
        {feedbackDetail.totalFeedbacks} đánh giá
      </span>
    </div>
    <div className={cn("w-2/3 flex flex-col", size == "lg" && "gap-2")}>
      {feedbackDetail.ratingsBreakdown &&
        Object.keys(feedbackDetail.ratingsBreakdown)
          .sort((a, b) => Number(b) - Number(a))
          .map((key) => (
            <div
              key={key}
              className={cn("flex justify-between items-center gap-4")}
            >
              <span className="w-10 text-right text-gray-500">{key}</span>
              <Slider
                className="w-full"
                max={feedbackDetail.totalFeedbacks!}
                value={
                  feedbackDetail.ratingsBreakdown &&
                  feedbackDetail.ratingsBreakdown[key]
                }
                defaultValue={
                  feedbackDetail.ratingsBreakdown &&
                  feedbackDetail.ratingsBreakdown[key]
                }
              />
              <span className="w-12 text-right text-gray-500">
                (
                {feedbackDetail.ratingsBreakdown &&
                  feedbackDetail.ratingsBreakdown[key]}
                )
              </span>
            </div>
          ))}
    </div>
  </div>
);

export default TutorFeedbacks;
