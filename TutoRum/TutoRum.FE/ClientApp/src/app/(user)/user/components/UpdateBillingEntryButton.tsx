import { Button, Modal, Popconfirm, Tooltip } from "antd";
import React, { useState } from "react";
import BillingEntryForm from "./BillingEntryForm";
import { AddBillingEntryDTO } from "@/utils/schemaValidations/billing-entry.schema";
import { Edit, Trash } from "lucide-react";
import { useDeleteBillingEntry } from "@/hooks/use-billing-entry";
import { toast } from "react-toastify";

export const DeleteBillingEntryButton = ({
  billingEntryId,
  tutorLearnerSubjectDetailId,
}: {
  billingEntryId: number;
  tutorLearnerSubjectDetailId: number;
}) => {
  const { mutateAsync: deleteBillingEntry } = useDeleteBillingEntry(
    tutorLearnerSubjectDetailId
  );

  const handleDelete = async () => {
    try {
      const result = await deleteBillingEntry([billingEntryId]);
      if (result.status === 201) {
        toast.success("Xóa thành cong");
      }
    } catch (error) {
      console.log(error);
      toast.error("Xóa không thành công");
    }
  };

  return (
    <Popconfirm
      title="Xoá"
      okText="Xoá"
      cancelText="Hủy"
      onConfirm={handleDelete}
    >
      <Button type="primary" danger icon={<Trash size={16} />} />
    </Popconfirm>
  );
};

const UpdateBillingEntryButton = ({
  disabled,
  billingEntry,
  billingEntryId,
  tutorLearnerSubjectId,
}: {
  disabled?: boolean;
  billingEntry: AddBillingEntryDTO;
  billingEntryId?: number;
  tutorLearnerSubjectId?: number;
}) => {
  const [open, setOpen] = useState(false);
  return (
    <div>
      <button disabled={disabled} onClick={() => setOpen(true)}>
        <Edit size={16} />
      </button>

      <Modal
        title="Thêm buổi học"
        open={open}
        onCancel={() => setOpen(false)}
        footer={null}
        width={800}
      >
        <BillingEntryForm
          setOpen={setOpen}
          billingEntryUpdate={billingEntry}
          billingEntryId={billingEntryId}
          tutorLearnerSubjectId={tutorLearnerSubjectId}
        />
      </Modal>
    </div>
  );
};

export default UpdateBillingEntryButton;
