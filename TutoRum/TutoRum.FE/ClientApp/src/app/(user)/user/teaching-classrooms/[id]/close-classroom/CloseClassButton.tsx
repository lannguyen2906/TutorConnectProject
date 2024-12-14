"use client";
import {
  useCloseClassroom,
  useTutorLearnerSubjectDetail,
} from "@/hooks/use-tutor-learner-subject";
import { Button, Modal } from "antd";
import { LogOut } from "lucide-react";
import { useRouter } from "next/navigation";
import React, { useState } from "react";
import { toast } from "react-toastify";

const CloseClassButton = ({ id }: { id: number }) => {
  const { mutateAsync, isLoading } = useCloseClassroom();
  const { data } = useTutorLearnerSubjectDetail(id);
  const router = useRouter();
  const [isOpen, setIsOpen] = useState(false);

  const handleClose = async () => {
    try {
      const result = await mutateAsync(id);
      if (result.status === 201) {
        toast.success("Kết thúc thành công");
        router.push("/user/teaching-classrooms");
      }
    } catch (error) {
      toast.error("Kết thúc thất bại");
    }
  };

  return (
    <div>
      <Button
        type="primary"
        onClick={() => setIsOpen(true)}
        icon={<LogOut size={16} />}
        disabled={data?.isClosed == true}
      >
        Kết thúc lớp học
      </Button>
      <Modal
        title="Kết thúc lớp học"
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        okText="Kết thúc"
        cancelText="Hủy"
        confirmLoading={isLoading}
        onOk={handleClose}
      >
        <p>
          Sau khi kết thúc lớp học bạn không thể tạo thêm bất kì buổi học cũng
          như hóa đơn cho lớp học này?
        </p>
      </Modal>
    </div>
  );
};

export default CloseClassButton;
