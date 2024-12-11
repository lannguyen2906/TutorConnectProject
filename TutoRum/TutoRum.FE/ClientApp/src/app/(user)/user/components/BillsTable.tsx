"use client";
import { formatNumber } from "@/utils/other/formatter";
import { Input, Table, Tag } from "antd";
import dayjs from "dayjs";
import { Eye, Search } from "lucide-react";
import { useEffect, useState } from "react";
import { useAppContext } from "@/components/provider/app-provider";
import { ColumnsType } from "antd/es/table";
import { BillDetailsDTO } from "@/utils/services/Api";
import {
  useBillsByTutor,
  useBillsByTutorLearnerSubject,
} from "@/hooks/use-billing-entry";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import { useRouter } from "next/navigation";
import EmailBillButton from "./EmailBillButton";
import BillDeleteConfirmButton from "./BillDeleteConfirmButton";

const BillsTable = ({
  tutorLearnerSubjectId,
  type,
}: {
  tutorLearnerSubjectId?: number;
  type: string;
}) => {
  const [search, setSearch] = useState("");
  const { user } = useAppContext();
  const { data: tutorLearnerSubjectDetail } = useTutorLearnerSubjectDetail(
    tutorLearnerSubjectId!
  );
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const { data: billsByTutorLearnerSubject } = useBillsByTutorLearnerSubject(
    pageIndex,
    pageSize,
    tutorLearnerSubjectId!
  );
  const { data: billsByTutor } = useBillsByTutor(pageIndex, pageSize);
  const route = useRouter();

  const data = tutorLearnerSubjectId
    ? billsByTutorLearnerSubject?.items
    : billsByTutor?.items;

  const columns: ColumnsType<BillDetailsDTO> = [
    {
      title: "#",
      dataIndex: "billId",
      key: "billId",
    },
    {
      title: "Tổng tiền hóa đơn",
      dataIndex: "totalBill",
      key: "totalBill",
      render: (text: string) => (
        <span className="text-kellygreen font-bold">
          {formatNumber(text)} VND
        </span>
      ),
    },
    {
      title: "Giảm giá",
      dataIndex: "discount",
      key: "discount",
      render: (text: string) => (
        <span className="text-gold font-bold">{formatNumber(text)} VND</span>
      ),
    },
    {
      title: "Khấu trừ",
      dataIndex: "deduction",
      key: "deduction",
      render: (text: string) => (
        <span className="text-red font-bold">{formatNumber(text)} VND</span>
      ),
    },
    {
      title: "Ngày xuất đơn",
      dataIndex: "createdDate",
      key: "createdDate",
      render: (text: string) => `${dayjs(text).format("HH:mm DD/MM/YYYY")}`,
    },
    {
      title: "Ghi chú",
      dataIndex: "description",
      key: "description",
    },
    {
      title: "Trạng thái",
      key: "status",
      render: (_, record) => renderStatusTag(record),
    },
    {
      title: "Hành động",
      dataIndex: "action",
      key: "action",
      render: (_, record) => {
        return (
          <div className="flex gap-2">
            <button
              className="hover:text-Blueviolet transition-all"
              title="Chi tiết"
              onClick={() =>
                route.push(
                  `/user/${type}/${tutorLearnerSubjectId}/bills/${record.billId}`
                )
              }
            >
              <Eye size={16} />
            </button>
            {user?.id == tutorLearnerSubjectDetail?.tutorId && (
              <>
                <EmailBillButton
                  learnerMail={record.learnerEmail!}
                  billId={record.billId!}
                  disabled={record.isApprove!}
                  type="icon"
                />
                <BillDeleteConfirmButton
                  billId={record.billId!}
                  tutorLearnerSubjectId={
                    record.billingEntries?.at(0)?.tutorLearnerSubjectId!
                  }
                />
              </>
            )}
          </div>
        );
      },
    },
  ];

  const renderStatusTag = (record: BillDetailsDTO) => {
    if (!record.isApprove) {
      return <Tag color="red">Chưa xác nhận</Tag>;
    } else if (record.isPaid) {
      return <Tag color="green">Đã thanh toán</Tag>;
    } else {
      return <Tag color="orange">Chưa thanh toán</Tag>;
    }
  };

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
            setPageIndex(page);
            setPageSize(pageSize);
          },
          pageSizeOptions: [20, 50, 100],
          showSizeChanger: true,
        }}
        columns={columns}
        dataSource={data ?? []}
        rowKey={"billId"}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default BillsTable;
