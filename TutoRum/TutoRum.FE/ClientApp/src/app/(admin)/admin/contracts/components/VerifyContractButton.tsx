import { useState } from "react";
import { Button, Modal, Input } from "antd";
import { Verified, X } from "lucide-react";
import { useVerifyContract } from "@/hooks/admin/use-contracts";
import { ContractDto } from "@/utils/services/Api";
import { toast } from "react-toastify";

const { TextArea } = Input;

const VerifyContractButton = ({ contract }: { contract: ContractDto }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [reason, setReason] = useState("");
  const { mutateAsync, isLoading } = useVerifyContract();

  const handleVerify = async (isVerified: boolean) => {
    try {
      const response = await mutateAsync({
        contractId: contract.contractId!,
        isVerified,
        reason: isVerified ? "" : reason,
      });

      toast.success(
        isVerified
          ? "Xác nhận hợp đồng thành công"
          : "Từ chối hợp đồng thành công"
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
      {contract.isVerified ? (
        <Button type="dashed" icon={<Verified />} disabled>
          Đã kiểm duyệt
        </Button>
      ) : (
        <div className="flex gap-2">
          <Button
            type="primary"
            icon={<Verified />}
            onClick={() => handleVerify(true)}
            loading={isLoading}
            disabled={isLoading}
            title="Xác nhận hợp đồng"
          />
          <Button
            type="default"
            icon={<X />}
            onClick={openModal}
            loading={isLoading}
            disabled={isLoading}
            title="Từ chối hợp đồng"
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
      )}
    </>
  );
};

export default VerifyContractButton;
