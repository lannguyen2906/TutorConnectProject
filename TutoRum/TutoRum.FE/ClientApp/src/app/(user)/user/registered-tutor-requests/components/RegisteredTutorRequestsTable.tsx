"use client";
import React from "react";
import { Button, Table, Tag, Tooltip } from "antd";
import type { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import CreateTutorLearnerSubjectButton from "./CreateTutorLearnerSubjectButton";
import { ListTutorRequestForTutorDto } from "@/utils/services/Api";
import { useAllTutorRequestsByTutor } from "@/hooks/use-tutor-request";
import { useAppContext } from "@/components/provider/app-provider";
import Link from "next/link";
import { LogIn } from "lucide-react";

// Định nghĩa cột cho bảng
const columns: ColumnsType<ListTutorRequestForTutorDto> = [
  {
    title: "ID",
    dataIndex: "id",
    key: "id",
    render: (id: number) => `#${id}`,
  },
  {
    title: "Môn học",
    dataIndex: "subject",
    key: "subject",
  },
  {
    title: "Địa điểm",
    dataIndex: "teachingLocation",
    key: "teachingLocation",
  },
  {
    title: "Số học sinh",
    dataIndex: "numberOfStudents",
    key: "numberOfStudents",
    render: (num: number) => `${num} học sinh`,
  },
  {
    title: "Học phí",
    dataIndex: "fee",
    key: "fee",
    render: (fee: number) => `${fee.toLocaleString()} VNĐ/buổi`,
  },
  {
    title: "Thời gian bắt đầu",
    dataIndex: "startDate",
    key: "startDate",
    render: (date: string) => dayjs(date).format("DD/MM/YYYY"),
  },
  {
    title: "Trạng thái",
    key: "status",
    render: (_, record) => {
      if (record.tutorLearnerSubjectId !== null) {
        return (
          <Tooltip title="Bạn đã tạo lớp">
            <Tag color="green">Đã tạo lớp</Tag>
          </Tooltip>
        );
      }
      if (record.isChosen) {
        return (
          <Tooltip title="Vui lòng tạo lớp mới">
            <Tag color="green">Đã chọn</Tag>
          </Tooltip>
        );
      }
      if (record.isInterested) {
        return (
          <Tooltip title="Vui lòng kiểm tra email">
            <Tag color="blue">Mong muốn kết nối</Tag>
          </Tooltip>
        );
      }
      if (record.status === "Pending") {
        return (
          <Tooltip title="Học viên sẽ liên lạc với bạn qua email">
            <Tag color="orange">Đang chờ kết quả</Tag>
          </Tooltip>
        );
      } else {
        return (
          <Tooltip title="Vui lòng tạo lớp mới">
            <Tag color="red">Bị từ chối </Tag>
          </Tooltip>
        );
      }
    },
  },
  {
    title: "Thao tác",
    key: "actions",
    render: (_, record) => (
      <div>
        {record.tutorLearnerSubjectId === null ? (
          <CreateTutorLearnerSubjectButton
            tutorRequestId={record.id!}
            disabled={!record.isChosen}
          />
        ) : (
          <Link
            href={`/user/teaching-classrooms/${record.tutorLearnerSubjectId}`}
          >
            <Button type="primary" icon={<LogIn size={16} />}>
              Đến lớp
            </Button>
          </Link>
        )}
      </div>
    ),
  },
];

// Component chính hiển thị bảng
const RegisteredTutorRequestsTable = () => {
  const [pageIndex, setPageIndex] = React.useState(1);
  const [pageSize, setPageSize] = React.useState(5);
  const { user } = useAppContext();
  const { data, isLoading } = useAllTutorRequestsByTutor(
    user?.id!,
    pageIndex,
    pageSize
  );
  return (
    <div style={{ padding: "16px" }}>
      <Table
        columns={columns}
        loading={isLoading}
        dataSource={data?.items || []}
        rowKey={"id"}
        bordered
        pagination={{
          pageSize: pageSize,
          current: pageIndex,
          total: data?.totalRecords,
          onChange: setPageIndex,
        }}
        title={() => "Danh sách yêu cầu đã nhận"}
      />
    </div>
  );
};

export default RegisteredTutorRequestsTable;
