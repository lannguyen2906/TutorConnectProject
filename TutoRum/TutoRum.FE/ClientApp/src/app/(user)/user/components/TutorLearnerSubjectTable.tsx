"use client";
import { useAppContext } from "@/components/provider/app-provider";
import { formatNumber } from "@/utils/other/formatter";
import { Button, Input, Table, Tag, Tooltip } from "antd";
import { Eye, Search } from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import React, { useState } from "react";
import AddClassRoomButton from "./AddClassRoomButton";
import { ColumnsType } from "antd/es/table";
import { SubjectDetailDto } from "@/utils/services/Api";
import { useGetSubjectDetailsList } from "@/hooks/use-tutor-learner-subject";

const TutorLearnerSubjectTable = ({ viewType }: { viewType: string }) => {
  const [search, setSearch] = useState("");
  const pathname = usePathname();
  const { user } = useAppContext();
  const { data: subjectDetailList } = useGetSubjectDetailsList(
    user?.id!,
    viewType
  );

  const columns: ColumnsType<SubjectDetailDto> = [
    {
      title: "id",
      dataIndex: "tutorLearnerSubjectId",
      key: "tutorLearnerSubjectId",
      hidden: true,
    },
    {
      title: "Môn học",
      dataIndex: "subjectName",
      key: "subjectName",
    },
    {
      title: "Giá tiền/ 1h",
      dataIndex: "rate",
      key: "rate",
      render: (number: number) => `${formatNumber(number.toString())} VND`,
    },
    {
      title: "Địa chỉ",
      dataIndex: "location",
      key: "location",
      render: (text: string) => (
        <Tooltip title={text}>
          <span>{text.length > 20 ? `${text.slice(0, 20)}...` : text}</span>
        </Tooltip>
      ),
    },
    {
      title: "Thời gian học trong tuần",
      key: "time",
      render: (_, record) =>
        `${record.sessionsPerWeek} buổi (${record.hoursPerSession}h/ buổi) `,
    },
    {
      title: "Ngày bắt đầu dự kiến",
      dataIndex: "expectedStartDate",
      key: "expectedStartDate",
      render: (text: string) => new Date(text).toLocaleDateString("vi-VN"),
    },

    {
      title: "Trạng thái",
      dataIndex: "isVerify",
      key: "isVerify",
      render: (isVerified: boolean | null) => {
        if (isVerified == null) {
          return <Tag color="gold">Chờ xác nhận</Tag>;
        } else if (isVerified) {
          return <Tag color="green">Đã xác nhận</Tag>;
        } else {
          return <Tag color="red">Bị từ chối</Tag>;
        }
      },
    },
    {
      title: "Hành động",
      key: "action",
      render: (_: any, record) => (
        <Link
          title="Chi tiết"
          href={`${pathname}/${record.tutorLearnerSubjectId}`}
        >
          <Eye size={16} />
        </Link>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <div className="flex justify-between gap-10">
        <Input
          addonBefore={<Search size={16} className="text-muted-foreground" />}
          placeholder="Tìm kiếm ..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>
      <Table
        pagination={false}
        columns={columns}
        dataSource={subjectDetailList}
        rowKey={"tutorLearnerSubjectId"}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TutorLearnerSubjectTable;
