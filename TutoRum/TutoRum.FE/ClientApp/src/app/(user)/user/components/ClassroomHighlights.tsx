import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import React from "react";

// Mock Data
const mockData = {
  classType: "Cố định", // or "Linh hoạt"
  subject: "Tin học",
  totalSessions: 12,
  totalEarnings: 5000000, // in VND
  rate: 100000,
};

// Highlight Card Component
const HighlightCard = ({
  title,
  value,
}: {
  title: string;
  value: string | number;
}) => (
  <div className="flex flex-col gap-2 justify-center items-center rounded shadow-md p-4 flex-1 bg-white">
    <div className="text-Blueviolet font-semibold text-xl">{value}</div>
    <div className="text-sm font-medium">{title}</div>
  </div>
);

const ClassroomHighlights = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const { data } = useTutorLearnerSubjectDetail(tutorLearnerSubjectId);

  if (data?.isClosed == true) {
    return (
      <div className="w-full flex justify-center">
        <HighlightCard
          title="Trạng thái lớp học"
          value={"Lớp học đã kết thúc"}
        />
      </div>
    );
  }

  return (
    <div className="flex gap-6">
      <HighlightCard
        title="Loại lớp học"
        value={data?.classType == "FLEXIBLE" ? "Linh hoạt" : "Cố định"}
      />
      <HighlightCard title="Môn học" value={data?.subjectName ?? ""} />
      <HighlightCard
        title="Tổng số buổi đã học"
        value={data?.totalSessionsCompleted ?? 0}
      />
      <HighlightCard
        title="Tổng tiền đã thanh toán"
        value={`${data?.totalPaidAmount?.toLocaleString()} VND`}
      />
      <HighlightCard
        title="Số tiền theo giờ"
        value={`${data?.pricePerHour?.toLocaleString()} VND/giờ`}
      />
    </div>
  );
};

export default ClassroomHighlights;
