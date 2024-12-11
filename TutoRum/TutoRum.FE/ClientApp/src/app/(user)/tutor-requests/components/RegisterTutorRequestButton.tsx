"use client";
import { useAppContext } from "@/components/provider/app-provider";
import { tConnectService } from "@/utils/services/tConnectService";
import { Button, Modal } from "antd";
import React from "react";
import { toast } from "react-toastify";

const RegisterTutorRequestButton = ({
  title,
  disabled = false,
  tutorRequestId,
}: {
  title?: string;
  disabled?: boolean;
  tutorRequestId: number;
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  const { user } = useAppContext();
  const [loading, setLoading] = React.useState(false);

  const handleRegister = async () => {
    try {
      setLoading(true);
      const response =
        await tConnectService.api.tutorRequestRegisterTutorRequestCreate({
          tutorId: user?.id,
          tutorRequestId: tutorRequestId,
        });
      if (response.status === 201) {
        setLoading(false);
        toast.success("Đăng ký nhận lớp thành công");
        setIsOpen(false);
      }
    } catch (err) {
      setLoading(false);
      toast.error("Đăng ký nhận lớp không thành công");
    }
  };

  return (
    <>
      <Button
        onClick={() => setIsOpen(true)}
        type="primary"
        disabled={disabled}
      >
        {title}
      </Button>
      <Modal
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        okText="Đăng ký"
        cancelText="Hủy"
        title="Đăng ký nhận lớp"
        onOk={handleRegister}
        confirmLoading={loading}
      >
        Bạn xác nhận muốn đăng ký lớp này chứ?
      </Modal>
    </>
  );
};

export default RegisterTutorRequestButton;
