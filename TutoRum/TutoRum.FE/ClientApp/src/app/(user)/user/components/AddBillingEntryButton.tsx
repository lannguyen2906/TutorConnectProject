import { Button, Modal, Tooltip } from "antd";
import React, { useState } from "react";
import BillingEntryForm from "./BillingEntryForm";

const AddBillingEntryButton = ({
  disabled,
  tutorLearnerSubjectId,
}: {
  disabled?: boolean;
  tutorLearnerSubjectId: number;
}) => {
  const [open, setOpen] = useState(false);
  return (
    <div>
      <Tooltip
        title={
          disabled
            ? "Hợp đồng cần được xác thực để dùng được tính năng này"
            : ""
        }
      >
        <Button
          disabled={disabled}
          onClick={() => setOpen(true)}
          type="primary"
        >
          Thêm buổi học
        </Button>
      </Tooltip>

      <Modal
        title="Thêm buổi học"
        open={open}
        onCancel={() => setOpen(false)}
        footer={null}
        width={800}
      >
        <BillingEntryForm
          setOpen={setOpen}
          tutorLearnerSubjectId={tutorLearnerSubjectId}
        />
      </Modal>
    </div>
  );
};

export default AddBillingEntryButton;
