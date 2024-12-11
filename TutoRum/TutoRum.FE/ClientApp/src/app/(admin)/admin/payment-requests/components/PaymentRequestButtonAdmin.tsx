"use client";
import {
  useApprovePaymentRequest,
  useRejectPaymentRequest,
} from "@/hooks/use-payment-request";
import { Button, Modal } from "antd";
import TextArea from "antd/es/input/TextArea";
import { Check, Cross, X } from "lucide-react";
import React from "react";
import { toast } from "react-toastify";

export const ApprovePaymentRequest = ({
  paymentRequestId,
  disabled,
}: {
  paymentRequestId: number;
  disabled: boolean;
}) => {
  const { mutateAsync, isLoading } = useApprovePaymentRequest();

  const handleApprove = async () => {
    try {
      const result = await mutateAsync(paymentRequestId);
      if (result.status == 201) {
        toast.success("Cập nhật trạng thái thành công");
      }
    } catch (error) {
      toast.error("Cập nhật trạng thái thất bại");
      console.log(error);
    }
  };

  return (
    <Button
      disabled={disabled}
      onClick={handleApprove}
      title="Thành công"
      icon={<Check size={16} />}
      loading={isLoading}
    />
  );
};

export const RejectPaymentRequest = ({
  paymentRequestId,
  disabled,
}: {
  paymentRequestId: number;
  disabled: boolean;
}) => {
  const { mutateAsync, isLoading } = useRejectPaymentRequest(paymentRequestId);
  const [rejectReason, setRejectReason] = React.useState("");
  const [open, setOpen] = React.useState(false);

  const handleApprove = async () => {
    try {
      const result = await mutateAsync(rejectReason);
      if (result.status == 201) {
        toast.success("Cập nhật trạng thái thành công");
        setOpen(false);
        setRejectReason("");
      }
    } catch (error) {
      toast.error("Cập nhật trạng thái thất bại");
      console.log(error);
    }
  };

  return (
    <>
      <Button
        disabled={disabled}
        onClick={() => setOpen(true)}
        title="Từ chối"
        danger
        icon={<X size={16} />}
        loading={isLoading}
      />
      <Modal
        title="Lý do từ chối"
        open={open}
        onCancel={() => setOpen(false)}
        okText="Xác nhận"
        cancelText="Hủy"
        onOk={handleApprove}
      >
        <span>Vui lòng nhập lí do từ chối</span>
        <TextArea
          value={rejectReason}
          onChange={(v) => setRejectReason(v.target.value)}
          placeholder="Lý do"
        />
      </Modal>
    </>
  );
};
