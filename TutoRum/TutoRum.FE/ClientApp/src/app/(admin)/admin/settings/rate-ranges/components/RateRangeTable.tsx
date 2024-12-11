"use client";
import { Button, Modal } from "antd";
import React, { useState } from "react";
import RateRangeForm from "./RateRangeForm";
import Table, { ColumnsType } from "antd/es/table";
import { RateRange } from "@/utils/services/Api";
import { formatNumber } from "@/utils/other/formatter";
import { useAllRateRanges } from "@/hooks/use-rate-range";
import {
  AddRateRangeButton,
  DeleteRateRangeButton,
  UpdateRateRangeButton,
} from "./RateRangeButtons";

const RateRangeTable = () => {
  const [open, setOpen] = useState(false);
  const { data, isLoading } = useAllRateRanges();

  const columns: ColumnsType<RateRange> = [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
    },
    {
      title: "Cấp độ",
      dataIndex: "level",
      key: "level",
      width: 200,
    },
    {
      title: "Khoảng giá",
      key: "rateRange",
      render: (_, record) =>
        `${formatNumber(record.minRate?.toString() || "")} - ${formatNumber(
          record.maxRate?.toString() || ""
        )} VND`,
      width: 200,
    },
    {
      title: "Mô tả",
      dataIndex: "description",
      key: "description",
    },
    {
      title: "Thao tác",
      key: "action",
      render: (_, record) => (
        <div className="flex gap-2">
          <UpdateRateRangeButton dataRateRange={record} />
          <DeleteRateRangeButton rateRangeId={record.id!} />
        </div>
      ),
    },
  ];

  return (
    <div>
      <AddRateRangeButton />
      <Table
        loading={isLoading}
        columns={columns}
        dataSource={data}
        pagination={false}
      />
      <Modal open={open} onCancel={() => setOpen(false)} footer={null}>
        <RateRangeForm />
      </Modal>
    </div>
  );
};

export default RateRangeTable;
