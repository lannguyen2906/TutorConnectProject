import { useState } from "react";
import { Button, Modal, Input } from "antd";
import { Verified, X } from "lucide-react";
import { useVerifyContract } from "@/hooks/admin/use-contracts";
import { ContractDto } from "@/utils/services/Api";
import { toast } from "react-toastify";
import { useVerifyBillingEntry } from "@/hooks/use-billing-entry";

const { TextArea } = Input;

const VerifyBillingEntryButton = ({
  tutorLearnerSubjectId,
  billingEntryId,
  disabled,
}: {
  tutorLearnerSubjectId: number;
  billingEntryId: number;
  disabled?: boolean;
}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [reason, setReason] = useState("");
  const { mutateAsync, isLoading } = useVerifyBillingEntry(
    tutorLearnerSubjectId
  );

  const handleVerify = async (isVerified: boolean) => {
    try {
      const response = await mutateAsync({
        billingEntryId: billingEntryId,
        isVerified,
        reason: isVerified ? "" : reason,
      });

      toast.success(
        isVerified
          ? "Xác nhận buổi học thành công"
          : "Từ chối buổi học thành công"
      );
      setIsModalOpen(false); // Đóng modal sau khi xử lý xong
      setReason(""); // Reset lý do
    } catch (error) {
      console.error("Failed to verify:", error);
      toast.error("Thao tác không thành công");
    }
  };

  const openModal = () => setIsModalOpen(true);
  const closeModal = () => setIsModalOpen(false);

  return (
    <>
      <div className="flex gap-2">
        <Button
          type="primary"
          icon={<Verified />}
          onClick={() => handleVerify(true)}
          loading={isLoading}
          disabled={isLoading || disabled}
          title="Xác nhận buổi học"
        />
        <Button
          type="default"
          icon={<X />}
          onClick={openModal}
          loading={isLoading}
          disabled={isLoading || disabled}
          title="Từ chối buổi học"
          color="danger"
        />
        <Modal
          title="Lý do từ chối"
          open={isModalOpen}
          onOk={() => handleVerify(false)}
          onCancel={closeModal}
          okButtonProps={{ disabled: !reason.trim() }} // Chỉ cho phép nếu có lý do
          okText="Xác nhận"
          cancelText="Hủy"
        >
          <TextArea
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            rows={4}
            placeholder="Hãy nhập lý do từ chối..."
          />
        </Modal>
      </div>
    </>
  );
};

export default VerifyBillingEntryButton;
