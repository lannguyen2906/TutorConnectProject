"use client";
import { Table, Tag, Button } from "antd";
import { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import React from "react";
import { VerifyButton } from "../../tutors/[id]/components/verify-buttons";
import { useTutorRequestsAdmin } from "@/hooks/use-tutor-request";
import { ListTutorRequestDTO } from "@/utils/services/Api";

const mockTutorRequests = [
  {
    id: 1,
    phoneNumber: "0987654321",
    requestSummary: "Cần gia sư Toán lớp 10 tại quận 1",
    cityId: "HCM",
    districtId: "Q1",
    wardId: "P5",
    teachingLocation: "Tại nhà",
    numberOfStudents: 2,
    startDate: "2024-12-01T10:00:00.000Z",
    preferredScheduleType: "Buổi tối",
    timePerSession: "2 giờ",
    subject: "Toán",
    studentGender: "Nam",
    tutorGender: "Nữ",
    fee: 500000,
    sessionsPerWeek: 3,
    detailedDescription: "Học sinh cần nâng cao kiến thức để thi học kỳ.",
    tutorQualificationId: 3,
    schedule: [
      { day: "Thứ Hai", time: "18:00 - 20:00" },
      { day: "Thứ Tư", time: "18:00 - 20:00" },
    ],
  },
  {
    id: 2,
    phoneNumber: "0901234567",
    requestSummary: "Cần gia sư Lý lớp 12 tại quận 3",
    cityId: "HCM",
    districtId: "Q3",
    wardId: "P7",
    teachingLocation: "Trực tuyến",
    numberOfStudents: 1,
    startDate: "2024-11-25T14:00:00.000Z",
    preferredScheduleType: "Buổi chiều",
    timePerSession: "1.5 giờ",
    subject: "Vật Lý",
    studentGender: "Nữ",
    tutorGender: "Nam",
    fee: 300000,
    sessionsPerWeek: 2,
    detailedDescription: "Ôn tập để thi đại học.",
    tutorQualificationId: 4,
    schedule: [
      { day: "Thứ Ba", time: "14:00 - 15:30" },
      { day: "Thứ Sáu", time: "14:00 - 15:30" },
    ],
  },
];

const tutorRequestColumns: ColumnsType<ListTutorRequestDTO> = [
  {
    title: "Tóm tắt",
    dataIndex: "requestSummary",
    key: "requestSummary",
  },
  {
    title: "Môn học",
    dataIndex: "subject",
    key: "subject",
  },
  {
    title: "Số buổi/tuần",
    dataIndex: "sessionsPerWeek",
    key: "sessionsPerWeek",
    render: (value: number) => `${value} buổi`,
  },
  {
    title: "Học phí",
    dataIndex: "fee",
    key: "fee",
    render: (value: number) => `${value.toLocaleString()} VND`,
  },
  {
    title: "Thời gian bắt đầu",
    dataIndex: "startDate",
    key: "startDate",
    render: (value: string) => dayjs(value).format("YYYY-MM-DD"),
  },
  {
    title: "Thời gian mỗi buổi",
    dataIndex: "timePerSession",
    key: "timePerSession",
  },
  {
    title: "Số điện thoại",
    dataIndex: "phoneNumber",
    key: "phoneNumber",
  },
  {
    title: "Chi tiết",
    dataIndex: "detailedDescription",
    key: "detailedDescription",
  },
  {
    title: "Trạng thái",
    key: "status",
    render: (status, record) => {
      if (record.isVerified == null) {
        return <Tag color="yellow">Chưa xác nhận</Tag>;
      } else if (record.isVerified == false) {
        return <Tag color="red">Bị từ chối</Tag>;
      } else {
        return <Tag color="green">Đã xác nhận</Tag>;
      }
    },
  },
  {
    title: "Hành động",
    dataIndex: "action",
    key: "action",
    fixed: "right",
    render: (_, record) => (
      <div>
        <VerifyButton
          id={record.tutorRequestId!}
          guid={null}
          entityType={2}
          isVerified={record.isVerified!}
        />
      </div>
    ),
  },
];

const handleRequestDetails = (id: number) => {
  console.log("Xem chi tiết yêu cầu ID:", id);
  // Logic xử lý khi bấm nút "Chi tiết"
};

const TutorRequestTable = () => {
  const [pageIndex, setPageIndex] = React.useState(1);
  const [pageSize, setPageSize] = React.useState(5);
  const { data, isLoading } = useTutorRequestsAdmin(pageIndex, pageSize);
  return (
    <Table
      loading={isLoading}
      columns={tutorRequestColumns}
      dataSource={data?.items || []}
      rowKey={"id"}
      pagination={{
        pageSize: 5,
        onChange(page, pageSize) {
          setPageIndex(page);
          setPageSize(pageSize);
        },
        showSizeChanger: true,
        total: data?.totalRecords,
      }}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TutorRequestTable;
