"use client";

import { useAdminTutorList } from "@/hooks/admin/use-tutors";
import {
  DatePicker,
  Input,
  Select,
  Table,
  Tag,
  Button,
  Spin,
  Tooltip,
} from "antd";
import { Eye } from "lucide-react";
import Link from "next/link";
import React, { useState } from "react";
import dayjs from "dayjs";
import { useForm } from "react-hook-form";
import { Form, FormField } from "@/components/ui/form";
import { ColumnsType } from "antd/es/table";
import { AdminTutorDto } from "@/utils/services/Api";

const columns: ColumnsType<AdminTutorDto> = [
  {
    title: "ID",
    dataIndex: "tutorId",
    key: "tutorId",
    hidden: true,
  },
  {
    title: "Tên",
    dataIndex: "fullName",
    key: "fullName",
    render: (name) => <Tooltip title={name}>{name}</Tooltip>,
  },
  {
    title: "Chuyên ngành",
    dataIndex: "specialization",
    key: "specialization",
    render: (spec) => <Tooltip title={spec}>{spec}</Tooltip>,
  },
  {
    title: "Đánh giá",
    dataIndex: "rating",
    key: "rating",
    render: (rating: number) => (rating ? `${rating} / 5` : "Chưa có"),
  },
  {
    title: "Kiểm duyệt",
    dataIndex: "isVerified",
    key: "isVerified",
    render: (isVerified: boolean, record) => {
      if (isVerified == null) {
        return <Tag color="yellow">{record.status}</Tag>;
      } else if (isVerified) {
        return <Tag color="green">{record.status}</Tag>;
      }
      return <Tag color="red">{record.status}</Tag>;
    },
  },
  {
    title: "Ngày tạo",
    dataIndex: "createdDate",
    key: "createdDate",
    render: (createdDate: string) => (
      <span>{dayjs(createdDate).format("DD/MM/YYYY")}</span>
    ),
  },
  {
    title: "Hành động",
    dataIndex: "action",
    key: "action",
    render: (_: any, record: any) => (
      <div className="flex gap-3" key={record.tutorId}>
        <Link title="Chi tiết" href={`/admin/tutors/${record.tutorId}`}>
          <Eye />
        </Link>
      </div>
    ),
  },
];

const TutorsTable = () => {
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);

  const form = useForm();
  const { data, isLoading } = useAdminTutorList(
    currentPage,
    pageSize,
    form.watch("search"),
    form.watch("status"),
    form.watch("startTime"),
    form.watch("endTime")
  );

  const handleReset = () => {
    form.reset({
      search: "",
      status: null,
      startTime: null,
      endTime: null,
    });
    setCurrentPage(1);
  };

  return (
    <div className="p-5 bg-white rounded-lg shadow">
      <h2 className="text-xl font-bold mb-4">Quản lý Gia sư</h2>

      {/* Form lọc */}
      <Form {...form}>
        <form className="flex gap-4 mb-5">
          <FormField
            control={form.control}
            name="search"
            render={({ field }) => (
              <Input
                {...field}
                placeholder="Tìm kiếm theo tên, ID..."
                className="flex-1"
              />
            )}
          />
          <FormField
            control={form.control}
            name="status"
            render={({ field }) => (
              <Select
                {...field}
                className="flex-1"
                placeholder="Chọn trạng thái"
                allowClear
                options={[
                  { label: "Chờ xác thực", value: "Chờ xác thực" },
                  { label: "Đã xác thực", value: "Đã xác thực" },
                  { label: "Từ chối", value: "Từ chối" },
                ]}
              />
            )}
          />
          <FormField
            control={form.control}
            name="startTime"
            render={({ field }) => (
              <DatePicker
                {...field}
                placeholder="Ngày bắt đầu"
                className="flex-1"
              />
            )}
          />
          <FormField
            control={form.control}
            name="endTime"
            render={({ field }) => (
              <DatePicker
                {...field}
                placeholder="Ngày kết thúc"
                className="flex-1"
              />
            )}
          />
        </form>
      </Form>

      {/* Bảng */}
      <Table
        loading={isLoading}
        className="mt-5"
        sortDirections={["descend", "ascend"]}
        columns={columns}
        dataSource={data?.tutors || []}
        rowKey="tutorId"
        pagination={{
          pageSizeOptions: [5, 10, 20],
          defaultPageSize: 5,
          current: currentPage,
          onChange: (page, pageSize) => {
            setCurrentPage(page);
            setPageSize(pageSize);
          },
          pageSize: pageSize,
          showSizeChanger: true,
          total: data?.totalRecordCount || 0,
          showTotal: (total) => `Tổng cộng ${total} gia sư`,
        }}
      />
    </div>
  );
};

export default TutorsTable;
