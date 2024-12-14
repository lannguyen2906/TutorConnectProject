"use client";
import { Subject, SubjectDTO } from "@/utils/services/Api";
import { Button, Modal, Popconfirm } from "antd";
import { Edit, Trash } from "lucide-react";
import React from "react";
import SubjectForm from "./SubjectForm";
import { toast } from "react-toastify";
import { useDeleteSubject } from "@/hooks/use-subject";

export const UpdateSubjectButton = ({
  subject,
  disabled,
}: {
  subject: SubjectDTO;
  disabled: boolean;
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  return (
    <div>
      <Button
        disabled={disabled}
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
        <SubjectForm subject={subject} />
      </Modal>
    </div>
  );
};

export const AddSubjectButton = ({}: {}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  return (
    <div>
      <Button onClick={() => setIsOpen(true)} type="primary" className="mb-3">
        Thêm môn học mới
      </Button>
      <Modal
        title="Chi tiết"
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        footer={null}
      >
        <SubjectForm />
      </Modal>
    </div>
  );
};

export const DeleteSubjectButton = ({
  SubjectId,
  disabled,
}: {
  SubjectId: number;
  disabled: boolean;
}) => {
  const { mutateAsync, isLoading } = useDeleteSubject();

  const handleDelete = async () => {
    try {
      const result = await mutateAsync(SubjectId);
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
          disabled={disabled}
        />
      </Popconfirm>
    </div>
  );
};
