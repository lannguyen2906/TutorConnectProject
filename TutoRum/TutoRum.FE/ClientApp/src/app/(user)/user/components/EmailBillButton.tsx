import { useState } from "react";
import { Button, Input, Modal } from "antd";
import { tConnectService } from "@/utils/services/tConnectService";
import { Send } from "lucide-react";
import { toast } from "react-toastify";

const EmailBillButton = ({
  billId,
  disabled,
  type,
  learnerMail,
}: {
  billId: number;
  disabled: boolean;
  type: "button" | "icon";
  learnerMail: string;
}) => {
  const [openModal, setOpenModal] = useState(false);
  const [mail, setMail] = useState(learnerMail);
  const [loading, setLoading] = useState(false);

  // Generate bill handler
  const handleSendMail = async () => {
    try {
      setLoading(true);
      const res = await tConnectService.api.billSendBillEmailCreate({
        billId: billId,
        parentEmail: mail,
      });
      setLoading(false);
      if (res.status == 201) {
        setOpenModal(false);
        toast.success("Gửi hóa đơn thành công qua mail " + mail);
      }
    } catch (error) {
      console.error("Lỗi khi gửi hóa đơn:", error);
      toast.error("Lỗi khi gửi hóa đơn");
    }
  };

  return (
    <>
      <Button
        disabled={disabled}
        title="Gửi mail hóa đơn"
        onClick={() => setOpenModal(true)}
        type="primary"
        icon={<Send size={16} />}
      >
        {type === "button" && "Gửi mail yêu cầu xác nhận"}
      </Button>

      {/* Modal for confirming email sending */}
      <Modal
        title="Xuất hóa đơn"
        open={openModal}
        onOk={handleSendMail}
        confirmLoading={loading}
        onCancel={() => setOpenModal(false)}
        okText="Gửi hóa đơn"
        cancelText="Hủy"
      >
        <div className="space-y-2">
          <p>Bạn muốn gửi hóa đơn qua email này?</p>
          <Input
            type="email"
            value={mail}
            onChange={(e) => setMail(e.target.value)}
            placeholder="Nhập địa chỉ email"
          />
        </div>
      </Modal>
    </>
  );
};

export default EmailBillButton;
