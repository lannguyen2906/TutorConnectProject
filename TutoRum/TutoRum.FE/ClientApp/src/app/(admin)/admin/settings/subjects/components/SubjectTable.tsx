"use client";
import { Button, Modal } from "antd";
import React, { useState } from "react";
import RateRangeForm from "./SubjectForm";
import Table, { ColumnsType } from "antd/es/table";
import { RateRange, SubjectFilterDTO } from "@/utils/services/Api";
import { formatNumber } from "@/utils/other/formatter";
import { useAllRateRanges } from "@/hooks/use-rate-range";
import {
  AddSubjectButton,
  DeleteSubjectButton,
  UpdateSubjectButton,
} from "./SubjectButtons";
import { useSubjectList } from "@/hooks/use-subject";

const RateRangeTable = () => {
  const [open, setOpen] = useState(false);
  const { data, isLoading } = useSubjectList();

  const columns: ColumnsType<SubjectFilterDTO> = [
    {
      title: "ID",
      dataIndex: "subjectId",
      key: "subjectId",
    },
    {
      title: "Tên môn học",
      dataIndex: "subjectName",
      key: "subjectName",
    },
    {
      title: "Số sử dụng",
      dataIndex: "numberOfUsages",
      key: "numberOfUsages",
    },
    {
      title: "Thao tác",
      key: "action",
      render: (_, record) => (
        <div className="flex gap-2">
          <UpdateSubjectButton
            disabled={record.numberOfUsages! > 0}
            subject={record}
          />
          <DeleteSubjectButton
            disabled={record.numberOfUsages! > 0}
            SubjectId={record.subjectId!}
          />
        </div>
      ),
    },
  ];

  return (
    <div>
      <AddSubjectButton />
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
