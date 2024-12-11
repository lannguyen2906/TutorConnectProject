import { Button, Modal } from "antd";
import React, { useState } from "react";
import ClassroomForm from "./ClassroomForm";

const AddClassRoomButton = () => {
  const [open, setOpen] = useState(false);
  return (
    <div>
      <Button onClick={() => setOpen(true)} type="primary">
        Thêm lớp học
      </Button>
      <Modal
        title="Thêm lớp"
        open={open}
        onCancel={() => setOpen(false)}
        footer={null}
        width={800}
      >
        <ClassroomForm />
      </Modal>
    </div>
  );
};

export default AddClassRoomButton;
