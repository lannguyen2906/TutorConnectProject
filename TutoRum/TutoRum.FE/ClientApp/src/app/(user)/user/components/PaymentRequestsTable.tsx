"use client";
import {
  useDeletePaymentRequest,
  usePaymentRequestsByTutor,
} from "@/hooks/use-payment-request";
import { formatNumber } from "@/utils/other/formatter";
import { PaymentRequestDTO } from "@/utils/services/Api";
import { Button, Empty, Input, Popconfirm, Tag, Tooltip } from "antd";
import Table, { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import { Search, Trash } from "lucide-react";
import React, { useState } from "react";
import { toast } from "react-toastify";

const PaymentRequestsTable = () => {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading } = usePaymentRequestsByTutor(pageNumber, pageSize);
  const [search, setSearch] = useState("");
  const { mutateAsync: deletePaymentRequest, isLoading: isLoadingDelete } =
    useDeletePaymentRequest();

  const handleDelete = async (paymentRequestId: number) => {
    try {
      const result = await deletePaymentRequest(paymentRequestId);
      if (result.status == 200) {
        toast.success("Xóa thành công");
      }
    } catch (error) {
      console.log(error);
      toast.error("Xóa không thành công");
    }
  };

  const columns: ColumnsType<PaymentRequestDTO> = [
    {
      title: "#",
      dataIndex: "paymentRequestId",
      key: "paymentRequestId",
    },
    {
      title: "Ngân hàng",
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
      render: (status: string) => {
        const color = status === "Pending" ? "red" : "green";
        const text = status === "Pending" ? "Chưa xác thực" : "Đã xác thực";
        return <Tag color={color}>{text}</Tag>;
      },
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      key: "status",
      render: (status: string, record) => {
        let color = "orange";
        let text = "Chờ xử lý";
        if (status === "Rejected") {
          color = "red";
          text = "Bị từ chối";
        } else if (status === "Approved" && record.isPaid) {
          color = "green";
          text = "Đã chuyển tiền";
        }
        return (
          <Tooltip title={status == "Rejected" ? record.adminNote : ""}>
            <Tag color={color}>{text}</Tag>
          </Tooltip>
        );
      },
    },
    {
      title: "Hành động",
      dataIndex: "action",
      key: "action",
      render: (_, record) => (
        <Popconfirm
          title="Xóa yêu cầu"
          description="Bạn có muốn chắc muốn xóa yêu cầu?"
          onConfirm={() => handleDelete(record.paymentRequestId!)}
          okText="Có"
          cancelText="Không"
          okType="danger"
        >
          <Button
            danger
            icon={<Trash size={16} />}
            disabled={record.status !== "Pending"}
          />
        </Popconfirm>
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

export default PaymentRequestsTable;
