"use client";
import {
  useAllPaymentRequests,
  useDeletePaymentRequest,
  usePaymentRequestsByTutor,
} from "@/hooks/use-payment-request";
import { formatNumber } from "@/utils/other/formatter";
import { PaymentRequestDTO } from "@/utils/services/Api";
import { Button, Empty, Input, Popconfirm, Tag } from "antd";
import Table, { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import { Search, Trash } from "lucide-react";
import React, { useState } from "react";
import { toast } from "react-toastify";
import {
  ApprovePaymentRequest,
  RejectPaymentRequest,
} from "./PaymentRequestButtonAdmin";

const PaymentRequestsTableAdmin = () => {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading } = useAllPaymentRequests(pageNumber, pageSize);
  const [search, setSearch] = useState("");

  const columns: ColumnsType<PaymentRequestDTO> = [
    {
      title: "#",
      dataIndex: "paymentRequestId",
      key: "paymentRequestId",
    },
    {
      title: "Mã ngân hàng",
      dataIndex: "bankCode",
      key: "bankCode",
    },
    {
      title: "Số tài khoản",
      dataIndex: "accountNumber",
      key: "accountNumber",
    },
    {
      title: "Tên tài khoản",
      dataIndex: "tutorName",
      key: "tutorName",
    },
    {
      title: "Số tiền",
      dataIndex: "amount",
      key: "amount",
      render: (number: number) => `${formatNumber(number.toString())} VND`,
    },
    {
      title: "Ngày tạo",
      dataIndex: "createdDate",
      key: "createdDate",
      render: (date: string) => dayjs(date).format("HH:mm DD/MM/YYYY"),
    },
    {
      title: "Xác thực",
      dataIndex: "verificationStatus",
      key: "verificationStatus",
      render: (status: string) =>
        status == "Pending" ? (
          <Tag color="red">Chưa xác thực</Tag>
        ) : (
          <Tag color="green">Đã xác thực</Tag>
        ),
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      key: "status",
      render: (status: string) => {
        switch (status) {
          case "Pending":
            return <Tag color="orange">Chờ xử lý</Tag>;
          case "Approved":
            return <Tag color="green">Đã chuyển tiền</Tag>;
          case "Rejected":
            return <Tag color="red">Bị từ chối</Tag>;
        }
      },
    },
    {
      title: "Hành động",
      dataIndex: "action",
      key: "action",
      render: (_, record) => (
        <div className="flex gap-2">
          <ApprovePaymentRequest
            paymentRequestId={record.paymentRequestId!}
            disabled={
              record.status !== "Pending" ||
              record.verificationStatus == "Pending"
            }
          />
          <RejectPaymentRequest
            paymentRequestId={record.paymentRequestId!}
            disabled={
              record.status !== "Pending" ||
              record.verificationStatus == "Pending"
            }
          />
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <Input
        addonBefore={<Search size={16} className="text-muted-foreground" />}
        placeholder="Tìm kiếm ..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />
      <Table
        pagination={{
          position: ["bottomCenter"],
          pageSize: pageSize,
          onChange(page, pageSize) {
            setPageNumber(page);
            setPageSize(pageSize);
          },
          pageSizeOptions: [20, 50, 100],
          showSizeChanger: true,
        }}
        columns={columns}
        dataSource={data?.items ?? []}
        rowKey={"billId"}
        scroll={{ x: "max-content" }}
        locale={{ emptyText: <Empty description={"Không có dữ liệu"} /> }}
      />
    </div>
  );
};

export default PaymentRequestsTableAdmin;
