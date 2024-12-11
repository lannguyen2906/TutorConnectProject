import React, { useState, useMemo } from "react";
import { Button, Checkbox, Input, Modal, Tooltip } from "antd";
import { FaFileInvoice } from "react-icons/fa";
import { useGenBillFromBillingEntries } from "@/hooks/use-billing-entry";
import { tConnectService } from "@/utils/services/tConnectService";
import { useRouter } from "next/navigation";
import { toast } from "react-toastify";
import { TutorLearnerSubjectSummaryDetailDto } from "@/utils/services/Api";

interface GenBillButtonProps {
  tutorLearnerSubjectDetail: TutorLearnerSubjectSummaryDetailDto | undefined;
  selectedRowKeys: number[];
  learnerMail: string;
  disabled?: boolean;
}

const GenBillButton: React.FC<GenBillButtonProps> = ({
  disabled,
  tutorLearnerSubjectDetail,
  selectedRowKeys,
  learnerMail,
}) => {
  const [openModal, setOpenModal] = useState(false);
  const [sendMail, setSendMail] = useState(false);
  const [mail, setMail] = useState(learnerMail);
  const { mutateAsync, isLoading } = useGenBillFromBillingEntries(
    tutorLearnerSubjectDetail?.tutorLearnerSubjectId!
  );
  const router = useRouter();

  // Tooltip content
  const tooltipContent = useMemo(() => {
    if (!tutorLearnerSubjectDetail?.contractUrl) {
      return "Thêm hợp đồng để dùng được tính năng này";
    }
    if (!tutorLearnerSubjectDetail?.isContractVerified) {
      return "Hợp đồng phải được xác nhận để dùng được tính năng này";
    }
    if (selectedRowKeys.length === 0) {
      return "Vui lòng chọn buổi học trước";
    }
    return "";
  }, [tutorLearnerSubjectDetail, selectedRowKeys]);

  // Button disabled state
  const isButtonDisabled = useMemo(
    () =>
      disabled ||
      !tutorLearnerSubjectDetail?.contractUrl ||
      !tutorLearnerSubjectDetail?.isContractVerified ||
      selectedRowKeys.length === 0,
    [tutorLearnerSubjectDetail, selectedRowKeys, disabled]
  );

  // Generate bill handler
  const handleGenBill = async () => {
    try {
      const response = await mutateAsync(selectedRowKeys);
      if (response.status === 201) {
        toast.success("Tạo hóa đơn thành công");

        if (sendMail) {
          // TODO: Implement sending email logic
          console.log("Hóa đơn sẽ được gửi qua email.");
          const res = await tConnectService.api.billSendBillEmailCreate({
            billId: response.data.data,
            parentEmail: mail,
          });
          if (res.status == 200) {
            setOpenModal(false);
            router.push(
              `/user/teaching-classrooms/${tutorLearnerSubjectDetail?.tutorLearnerSubjectId}/bills`
            );
          }
        }
      }
    } catch (error) {
      console.error("Lỗi khi tạo hóa đơn:", error);
      toast.error("Lỗi khi tạo hóa đơn");
    }
  };

  return (
    <>
      <Tooltip title={tooltipContent}>
        <Button
          icon={<FaFileInvoice size={16} />}
          className="w-full"
          type="primary"
          disabled={isButtonDisabled}
          loading={isLoading}
          onClick={() => setOpenModal(true)}
        >
          Xuất hóa đơn
        </Button>
      </Tooltip>

      {/* Modal for confirming email sending */}
      <Modal
        title="Xuất hóa đơn"
        open={openModal}
        onOk={handleGenBill}
        onCancel={() => setOpenModal(false)}
        okText="Xuất hóa đơn"
        cancelText="Hủy"
        confirmLoading={isLoading}
      >
        <div className="space-y-2">
          <p>
            Bạn có muốn gửi hóa đơn qua email cho học viên để xác nhận không?
          </p>
          <Checkbox
            checked={sendMail}
            onChange={(e) => setSendMail(e.target.checked)}
          >
            Gửi hóa đơn qua email
          </Checkbox>
          {sendMail && (
            <Input
              type="email"
              value={mail}
              onChange={(e) => setMail(e.target.value)}
              placeholder="Nhập địa chỉ email"
            />
          )}
        </div>
      </Modal>
    </>
  );
};

export default GenBillButton;
