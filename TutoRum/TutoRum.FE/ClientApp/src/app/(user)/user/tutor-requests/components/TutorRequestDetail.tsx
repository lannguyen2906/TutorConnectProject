import React from "react";
import { Button, Descriptions, Spin } from "antd";
import type { DescriptionsProps } from "antd";
import dayjs from "dayjs";
import { TutorRequestDTO } from "@/utils/services/Api";
import { useTutorRequestDetail } from "@/hooks/use-tutor-request";
import { Edit } from "lucide-react";
import TutorRequestForm from "@/app/(user)/tutor-requests/components/TutorRequestForm";

const mapTutorRequestDTOToItems = (
  data: TutorRequestDTO
): DescriptionsProps["items"] => {
  return [
    { key: "1", label: "ID", children: data?.id },
    { key: "2", label: "Số điện thoại", children: data?.phoneNumber },
    { key: "3", label: "Tóm tắt yêu cầu", children: data?.requestSummary },
    { key: "7", label: "Địa điểm dạy", children: data?.teachingLocation },
    { key: "8", label: "Số học sinh", children: data?.numberOfStudents },
    {
      key: "9",
      label: "Ngày bắt đầu",
      children: dayjs(data?.startDate).format("YYYY-MM-DD HH:mm"),
    },
    {
      key: "10",
      label: "Thời gian ưu tiên",
      children:
        data?.preferredScheduleType == "FIXED" ? "Cố định" : "Linh hoạt",
    },
    { key: "11", label: "Thời lượng mỗi buổi", children: data?.timePerSession },
    { key: "12", label: "Môn học", children: data?.subject },
    {
      key: "13",
      label: "Giới tính học sinh",
      children: data?.studentGender == "male" ? "Nam" : "Nữ",
    },
    {
      key: "14",
      label: "Giới tính gia sư",
      children: data?.tutorGender == "male" ? "Nam" : "Nữ",
    },
    {
      key: "15",
      label: "Học phí",
      children: `${data?.fee?.toLocaleString()} VNĐ`,
    },
    { key: "16", label: "Số buổi/tuần", children: data?.sessionsPerWeek },
    {
      key: "17",
      label: "Mô tả chi tiết",
      span: 2,
      children: data?.detailedDescription,
    },
    {
      key: "18",
      label: "Yêu cầu trình độ gia sư",
      children: data?.tutorQualificationName,
    },
    { key: "19", label: "Trạng thái", children: data?.status },
    { key: "20", label: "Lịch rảnh", children: data?.freeSchedules },
  ];
};

const TutorRequestDetail = ({ tutorRequestId }: { tutorRequestId: number }) => {
  const { data, isLoading } = useTutorRequestDetail(tutorRequestId);
  const [editMode, setEditMode] = React.useState(false);

  if (isLoading) {
    return <Spin size="large" />;
  }

  return (
    <>
      <div className="flex items-center justify-between mb-3">
        <p className="font-bold">Yêu cầu gia sư</p>
        <Button
          type={editMode ? "primary" : "default"}
          onClick={() => {
            setEditMode(!editMode);
          }}
          icon={<Edit size={16} />}
        />
      </div>
      {editMode ? (
        <TutorRequestForm tutorRequestDto={data!} />
      ) : (
        <Descriptions
          layout="vertical"
          items={mapTutorRequestDTOToItems(data!)}
        />
      )}
    </>
  );
};

export default TutorRequestDetail;
