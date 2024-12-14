"use client";
import { RateRange } from "@/utils/services/Api";
import { Button, Modal, Popconfirm } from "antd";
import { Edit, Trash } from "lucide-react";
import React from "react";
import RateRangeForm from "./RateRangeForm";
import { useDeleteRateRange } from "@/hooks/use-rate-range";
import { toast } from "react-toastify";

export const UpdateRateRangeButton = ({
  dataRateRange,
}: {
  dataRateRange: RateRange;
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  return (
    <div>
      <Button
        onClick={() => setIsOpen(true)}
        type="text"
        icon={<Edit size={16} />}
      />
      <Modal
        title="Chi tiết"
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        footer={null}
      >
        <RateRangeForm dataRateRange={dataRateRange} />
      </Modal>
    </div>
  );
};

export const AddRateRangeButton = ({}: {}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  return (
    <div>
      <Button onClick={() => setIsOpen(true)} type="primary" className="mb-3">
        Thêm khoảng giá mới
      </Button>
      <Modal
        title="Chi tiết"
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        footer={null}
      >
        <RateRangeForm />
      </Modal>
    </div>
  );
};

export const DeleteRateRangeButton = ({
  rateRangeId,
}: {
  rateRangeId: number;
}) => {
  const { mutateAsync, isLoading } = useDeleteRateRange();

  const handleDelete = async () => {
    try {
      const result = await mutateAsync(rateRangeId);
      if (result.status === 201) {
        toast.success("Xóa thành công");
      }
    } catch (error) {
      toast.error("Xóa thất bại");
      console.log(error);
    }
  };

  return (
    <div>
      <Popconfirm
        title="Xoá"
        onConfirm={handleDelete}
        okText="Xoá"
        cancelText="Hủy"
        okType="danger"
      >
        <Button
          type="text"
          danger
          icon={<Trash size={16} />}
          loading={isLoading}
        />
      </Popconfirm>
    </div>
  );
};
