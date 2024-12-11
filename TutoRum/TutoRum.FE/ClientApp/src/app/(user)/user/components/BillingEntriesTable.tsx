"use client";
import { formatNumber } from "@/utils/other/formatter";
import { Button, Input, Table, Tag, Tooltip } from "antd";
import dayjs from "dayjs";
import { Search } from "lucide-react";
import React, { useState } from "react";
import { FaFileInvoice } from "react-icons/fa";
import AddClassRoomButton from "./AddClassRoomButton";
import AddBillingEntryButton from "./AddBillingEntryButton";
import { useAppContext } from "@/components/provider/app-provider";
import { useClassroomContext } from "@/components/provider/classroom-provider";
import { ColumnsType, TableProps } from "antd/es/table";
import { BillingEntryDTO } from "@/utils/services/Api";
import {
  useBillingEntriesByTutorLearnerSubject,
  useGenBillFromBillingEntries,
} from "@/hooks/use-billing-entry";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import UpdateBillingEntryButton, {
  DeleteBillingEntryButton,
} from "./UpdateBillingEntryButton";
import { AddBillingEntryDTO } from "@/utils/schemaValidations/billing-entry.schema";
import GenBillButton from "./GenBillButton";
// Hàm tính toán thống kê
const calculateStatistics = (sessions: BillingEntryDTO[]) => {
  // const confirmedSessions = sessions.filter(
  //   (session) => session.confirmationStatus === "Đã xác nhận"
  // );

  const totalClasses = sessions.length; // Tổng số buổi học đã xác nhận
  let totalHours = 0; // Tổng số tiếng
  let totalMoney = 0; // Tổng số tiền

  sessions.forEach((session) => {
    const start = dayjs(session.startDateTime); // Sử dụng dayjs để xử lý ngày giờ
    const end = dayjs(session.endDateTime);
    const hours = end.diff(start, "hour"); // Tính số giờ
    totalHours += hours;
    totalMoney += session.totalAmount ?? 0;
  });

  const averageHourlyRate =
    totalHours > 0 ? (totalMoney / totalHours).toFixed(0) : 0; // Số tiền trung bình 1 giờ

  return {
    totalClasses,
    totalHours,
    totalMoney,
    averageHourlyRate,
  };
};

// Sử dụng `rowClassName` trong bảng để thay đổi màu nền hàng
const rowClassName = (record: any) => {
  switch (record.confirmationStatus) {
    // case "Đã xác nhận":
    //   return "bg-[#dff0d8]"; // Thêm class cho đã xác nhận
    // case "Chờ xác nhận":
    //   return "bg-[#fcf8e3]"; // Thêm class cho chờ xác nhận
    case "Từ chối":
      return "bg-[#f2dede]"; // Thêm class cho từ chối
    default:
      return "";
  }
};

const BillingEntriesTable = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const [search, setSearch] = useState("");
  const { user } = useAppContext();
  const { data: tutorLearnerSubjectDetail } = useTutorLearnerSubjectDetail(
    tutorLearnerSubjectId
  );
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [selectedRowKeys, setSelectedRowKeys] = useState<number[]>([]);
  const { data, isLoading: isLoadingBillingEntries } =
    useBillingEntriesByTutorLearnerSubject(
      pageIndex,
      pageSize,
      tutorLearnerSubjectId
    );

  const columns: ColumnsType<BillingEntryDTO> = [
    {
      title: "#",
      dataIndex: "billingEntryID",
      key: "billingEntryID",
    },
    {
      title: "Mã hóa đơn",
      dataIndex: "billId",
      key: "billId",
      render: (text: string) => text ?? "Chưa xuất",
    },
    {
      title: "Ngày học",
      dataIndex: "startDateTime",
      key: "date",
      render: (text: string) => dayjs(text).format("DD/MM/YYYY"),
    },
    {
      title: "Thời gian học",
      key: "time",
      render: (text, record) => (
        <Tooltip
          title={`${dayjs(record.endDateTime).diff(
            dayjs(record.startDateTime),
            "hour",
            true
          )} giờ`}
        >
          {`${dayjs(record.startDateTime).format("HH:mm")} - ${dayjs(
            record.endDateTime
          ).format("HH:mm")}`}
        </Tooltip>
      ),
    },
    {
      title: "Tiền / 1 giờ",
      dataIndex: "rate",
      key: "rate",
      render: (number: number) => `${formatNumber(number.toString())} VND`, // Định dạng số với dấu phẩy
    },
    {
      title: "Tổng tiền của buổi học",
      dataIndex: "totalAmount",
      key: "totalAmount",
      render: (number: number) => `${formatNumber(number.toString())} VND`, // Định dạng số với dấu phẩy
    },
    {
      title: "Nhận xét",
      dataIndex: "description",
      key: "description",
      render: (text: string) => (
        <Tooltip title={text}>
          {text?.length > 20 ? `${text.slice(0, 20)}...` : text}
        </Tooltip>
      ),
    },
    {
      title: "Hành động",
      dataIndex: "action",
      key: "action",
      hidden: user?.id! != tutorLearnerSubjectDetail?.tutorId,
      render: (_, record) => {
        const billingEntry: AddBillingEntryDTO = {
          date: dayjs(record.startDateTime).format("YYYY-MM-DD"),
          startTime: dayjs(record.startDateTime).format("HH:mm"),
          endTime: dayjs(record.endDateTime).format("HH:mm"),
          rate: record.rate ?? 0,
          description: record.description ?? "",
          totalAmount: record.totalAmount,
        };

        return (
          <div className="flex items-center gap-2">
            <UpdateBillingEntryButton
              disabled={
                tutorLearnerSubjectDetail?.isClosed === true ||
                record.billId !== null
              }
              billingEntryId={record.billingEntryID || 0}
              billingEntry={billingEntry}
              tutorLearnerSubjectId={record.tutorLearnerSubjectId || 0}
            />
            <DeleteBillingEntryButton
              billingEntryId={record.billingEntryID!}
              tutorLearnerSubjectDetailId={record.tutorLearnerSubjectId!}
            />
          </div>
        );
      },
    },
  ];

  const statistics =
    selectedRowKeys.length > 0
      ? calculateStatistics(
          data?.billingEntries?.filter((be) =>
            selectedRowKeys.includes(be.billingEntryID!)
          ) || []
        )
      : calculateStatistics(data?.billingEntries || []);

  const rowSelection: TableProps<BillingEntryDTO>["rowSelection"] = {
    onChange: (selectedRowKeys: React.Key[]) => {
      setSelectedRowKeys(selectedRowKeys as number[]);
    },
    getCheckboxProps: (record: BillingEntryDTO) => ({
      name: record.billingEntryID?.toString(),
      disabled: record.billId !== null,
    }),
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between gap-10">
        <Input
          addonBefore={<Search size={16} className="text-muted-foreground" />}
          placeholder="Tìm kiếm ..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        {user?.id! == tutorLearnerSubjectDetail?.tutorId && (
          <AddBillingEntryButton
            disabled={
              !tutorLearnerSubjectDetail?.contractUrl ||
              !tutorLearnerSubjectDetail?.isContractVerified
            }
            tutorLearnerSubjectId={tutorLearnerSubjectId}
          />
        )}
      </div>
      <Table
        loading={isLoadingBillingEntries}
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
        dataSource={data?.billingEntries || []}
        rowClassName={rowClassName}
        rowKey={"billingEntryID"}
        scroll={{ x: "max-content" }}
        rowSelection={{ type: "checkbox", ...rowSelection }}
        summary={(pageData) => {
          return (
            <>
              <Table.Summary.Row className="bg-muted text-Blueviolet font-bold">
                <Table.Summary.Cell index={0} colSpan={3}>
                  Tổng
                </Table.Summary.Cell>
                <Table.Summary.Cell index={1}>
                  {statistics.totalClasses} buổi
                </Table.Summary.Cell>
                <Table.Summary.Cell index={2}>
                  {statistics.totalHours} giờ
                </Table.Summary.Cell>
                <Table.Summary.Cell index={3}>
                  ~ {formatNumber(statistics.averageHourlyRate.toString())} VND
                </Table.Summary.Cell>
                <Table.Summary.Cell index={3}>
                  {formatNumber(statistics.totalMoney.toString())} VND
                </Table.Summary.Cell>
                <Table.Summary.Cell index={4} colSpan={2}>
                  <GenBillButton
                    disabled={tutorLearnerSubjectDetail?.isClosed === true}
                    learnerMail={tutorLearnerSubjectDetail?.learnerEmail! || ""}
                    selectedRowKeys={selectedRowKeys}
                    tutorLearnerSubjectDetail={tutorLearnerSubjectDetail}
                  />
                </Table.Summary.Cell>
              </Table.Summary.Row>
            </>
          );
        }}
      />
    </div>
  );
};

export default BillingEntriesTable;
