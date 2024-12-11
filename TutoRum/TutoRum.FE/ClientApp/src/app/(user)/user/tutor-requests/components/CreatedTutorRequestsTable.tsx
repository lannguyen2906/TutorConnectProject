"use client";
import { Table, Tag } from "antd";
import React, { useState } from "react";
import { ColumnsType } from "antd/es/table";
import { ListTutorRequestDTO } from "@/utils/services/Api";
import {
  CloseTutorRequestButton,
  TutorRequestDetailButton,
} from "./ActionButton";
import { useAllTutorRequestsByLearner } from "@/hooks/use-tutor-request";
import { useAppContext } from "@/components/provider/app-provider";

const columnsType: ColumnsType<ListTutorRequestDTO> = [
  {
    title: "#",
    dataIndex: "tutorRequestId",
    key: "id",
  },
  {
    title: "Môn học",
    dataIndex: "subject",
    key: "subject",
  },
  {
    title: "Giá tiền/1h",
    dataIndex: "fee",
    key: "fee",
    render: (fee) => `${fee?.toLocaleString()} VND`, // Hiển thị dạng tiền tệ
  },
  {
    title: "Ngày bắt đầu",
    dataIndex: "startDate",
    key: "startDate",
    render: (startDate) => new Date(startDate!).toLocaleDateString(), // Format ngày
  },
  {
    title: "Trạng thái",
    key: "status",
    render: (status, record) => {
      if (!record.isVerified) {
        return <Tag color="orange">Chưa xác nhận</Tag>;
      } else {
        if (record.isDelete) return <Tag color="black">Đã đóng</Tag>;
        return (
          <div>
            <Tag color="blue">Đang tìm kiếm</Tag>
            <Tag color="pink">{record.numberOfRegisteredTutor} đăng ký</Tag>
          </div>
        );
      }
    },
  },
  {
    title: "Hành động",
    render: (_, record) => (
      <div className="flex items-center gap-2">
        <TutorRequestDetailButton tutorRequestId={record.tutorRequestId!} />
        <CloseTutorRequestButton tutorRequestId={record.tutorRequestId!} />
      </div>
    ),
  },
];

const CreatedTutorRequestsTable: React.FC = () => {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const { user } = useAppContext();
  const { data, isLoading } = useAllTutorRequestsByLearner(
    user?.id!,
    pageNumber,
    pageSize
  );
  return (
    <div style={{ padding: "16px" }}>
      <Table
        loading={isLoading}
        columns={columnsType}
        dataSource={data?.items || []}
        rowKey={"id"} // Đặt key cho từng hàng
        pagination={{
          pageSize: pageSize,
          onChange: setPageNumber,
          current: pageNumber,
        }} // Phân trang mỗi 5 dòng
        bordered
        title={() => "Danh sách yêu cầu gia sư đã tạo"}
      />
    </div>
  );
};

export default CreatedTutorRequestsTable;
