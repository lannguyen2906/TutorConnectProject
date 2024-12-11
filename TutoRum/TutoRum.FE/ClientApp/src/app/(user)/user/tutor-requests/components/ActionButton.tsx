"use client";
import { Button, Drawer, Popconfirm, Tabs } from "antd";
import { Delete, Eye, GraduationCap, Trash, X } from "lucide-react";
import React from "react";
import RegisteredTutorsTable from "./RegisteredTutorsTable";
import TutorRequestDetail from "./TutorRequestDetail";
import { useCloseTutorRequest } from "@/hooks/use-tutor-request";
import { toast } from "react-toastify";

export const TutorRequestDetailButton = ({
  tutorRequestId,
}: {
  tutorRequestId: number;
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  return (
    <>
      <Button
        type="text"
        icon={<Eye size={16} />}
        onClick={() => setIsOpen(true)}
      />
      <Drawer
        title="Chi tiết yêu cầu"
        placement="right"
        size="large"
        open={isOpen}
        onClose={() => setIsOpen(false)}
        width={1000}
      >
        <Tabs
          defaultActiveKey="1"
          type="card"
          size="middle"
          items={[
            {
              key: "1",
              label: `Chi tiết yêu cầu`,
              icon: <Eye size={16} />,
              children: <TutorRequestDetail tutorRequestId={tutorRequestId} />,
            },
            {
              key: "2",
              label: `Gia sư đăng ký`,
              icon: <GraduationCap size={16} />,
              children: (
                <RegisteredTutorsTable tutorRequestId={tutorRequestId} />
              ),
            },
          ]}
        />
      </Drawer>
    </>
  );
};

export const CloseTutorRequestButton = ({
  tutorRequestId,
}: {
  tutorRequestId: number;
}) => {
  const { mutateAsync, isLoading } = useCloseTutorRequest();

  const handleClose = async () => {
    try {
      const response = await mutateAsync(tutorRequestId);
      if (response.status === 201) {
        toast.success("Đóng yêu cầu thành công");
      }
    } catch (error) {
      toast.error("Lỗi khi đóng yêu cầu");
    }
  };

  return (
    <Popconfirm
      title="Đóng yêu cầu"
      onConfirm={handleClose}
      okText="Đóng"
      cancelText="Hủy"
      okType="danger"
    >
      <Button type="text" danger icon={<X size={16} />} loading={isLoading} />
    </Popconfirm>
  );
};
