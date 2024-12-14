"use client";
import { useDeleteBill } from "@/hooks/use-billing-entry";
import { Modal } from "antd";
import { Delete, Trash } from "lucide-react";
import React from "react";
import { toast } from "react-toastify";

const BillDeleteConfirmButton = ({
  billId,
  tutorLearnerSubjectId,
}: {
  billId: number;
  tutorLearnerSubjectId: number;
}) => {
  const [open, setOpen] = React.useState(false);
  const { mutateAsync, isLoading } = useDeleteBill(
    billId,
    tutorLearnerSubjectId
  );
  const handleDelete = async () => {
    try {
      const data = await mutateAsync();
      if (data.status == 200) {
        toast.success("Xóa hóa đơn thành công");
        setOpen(false);
      }
    } catch (error) {
      console.log(error);
      toast.error("Xóa hóa đơn không thành công");
    }
  };
  return (
    <>
      <button title="Xóa" className="text-red" onClick={() => setOpen(true)}>
        <Trash size={16} />
      </button>
      <Modal
        open={open}
        onCancel={() => setOpen(false)}
        okText="Xóa"
        cancelText="Hủy"
        title="Xóa hóa đơn"
        onOk={handleDelete}
        confirmLoading={isLoading}
      >
        Bạn có đồng ý xóa hóa đơn này chứ?
      </Modal>
    </>
  );
};

export default BillDeleteConfirmButton;
