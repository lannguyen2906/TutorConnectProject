import { useVerifyStatus } from "@/hooks/admin/use-tutors";
import { Button, Modal } from "antd";
import { useState } from "react";
import { CheckCircleOutlined, CloseCircleOutlined } from "@ant-design/icons";
import { z } from "zod";
import TextArea from "antd/es/input/TextArea";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { toast } from "react-toastify";

const reasonSchema = z.object({
  reason: z.string().nonempty("Lý do không xác thực là bắt buộc."),
});

export const VerifyButton = ({
  entityType,
  guid,
  id,
  isVerified,
  mode = "admin",
}: {
  entityType: number;
  guid: string | null;
  id: number | null;
  isVerified: boolean | null;
  mode?: "admin" | "tutor";
}) => {
  const [buttonIsVerified, setButtonIsVerified] = useState<boolean | null>(
    isVerified
  );
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [modalContent, setModalContent] = useState<
    "verify" | "unverify" | null
  >(null);
  const { mutateAsync } = useVerifyStatus();

  const form = useForm({
    resolver: zodResolver(reasonSchema),
    defaultValues: {
      reason: "",
    },
  });

  const showModal = (action: "verify" | "unverify") => {
    setModalContent(action);
    setIsModalVisible(true);
  };

  const handleVerify = async () => {
    try {
      const response = await mutateAsync({
        entityType,
        guidId: guid ?? undefined,
        id: id ?? undefined,
        isVerified: true,
      });
      console.log(response);
      if (response.status === 201) {
        toast.success(
          mode === "admin" ? "Xác thực thành công" : "Yêu cầu đã được chấp nhận"
        );
        setButtonIsVerified(true);
        setIsModalVisible(false);
      }
    } catch (error) {
      toast.error(
        mode === "admin" ? "Xác thực thất bại" : "Không thể chấp nhận yêu cầu"
      );
      console.error("Failed to verify:", error);
    }
  };

  const handleUnverify = form.handleSubmit(async (data) => {
    try {
      await mutateAsync({
        entityType,
        guidId: guid ?? undefined,
        id: id ?? undefined,
        isVerified: false,
        reason: data.reason,
      });
      toast(
        mode === "admin" ? "Không xác thực thành công" : "Yêu cầu đã bị từ chối"
      );
      setButtonIsVerified(false);
      setIsModalVisible(false);
    } catch (error) {
      toast(
        mode === "admin"
          ? "Không xác thực thất bại"
          : "Không thể từ chối yêu cầu"
      );
      console.error("Failed to unverify:", error);
    }
  });

  const handleCancel = () => {
    setIsModalVisible(false);
    form.reset();
  };

  return (
    <>
      {buttonIsVerified ? (
        <Button
          onClick={() => showModal("unverify")}
          type="default"
          icon={<CloseCircleOutlined />}
          danger
        >
          {mode === "admin" ? "Hủy xác thực" : "Từ chối"}
        </Button>
      ) : buttonIsVerified == null ? (
        <div className="w-full flex justify-center items-center flex-wrap gap-2">
          <Button
            onClick={() => showModal("unverify")}
            type="default"
            icon={<CloseCircleOutlined />}
            danger
          >
            {mode === "admin" ? "Từ chối" : "Từ chối yêu cầu"}
          </Button>
          <Button
            onClick={() => showModal("verify")}
            type="primary"
            icon={<CheckCircleOutlined />}
          >
            {mode === "admin" ? "Xác thực" : "Chấp nhận yêu cầu"}
          </Button>
        </div>
      ) : (
        <Button
          onClick={() => showModal("verify")}
          type="primary"
          icon={<CheckCircleOutlined />}
        >
          {mode === "admin" ? "Xác thực" : "Chấp nhận yêu cầu"}
        </Button>
      )}

      <Modal
        title={
          modalContent === "verify"
            ? mode === "admin"
              ? "Xác thực"
              : "Chấp nhận yêu cầu"
            : mode === "admin"
            ? "Hủy xác thực"
            : "Từ chối yêu cầu"
        }
        open={isModalVisible}
        onOk={modalContent === "verify" ? handleVerify : handleUnverify}
        onCancel={handleCancel}
        okText={
          modalContent === "verify"
            ? mode === "admin"
              ? "Xác thực"
              : "Chấp nhận"
            : mode === "admin"
            ? "Hủy xác thực"
            : "Từ chối"
        }
        okButtonProps={
          modalContent === "verify"
            ? { icon: <CheckCircleOutlined /> }
            : { icon: <CloseCircleOutlined />, danger: true }
        }
        cancelText="Trở về"
      >
        {modalContent === "verify" ? (
          <p>
            {mode === "admin"
              ? "Bạn đồng ý xác thực thông tin này?"
              : "Bạn có chắc muốn chấp nhận yêu cầu kết nối này?"}
          </p>
        ) : (
          <Form {...form}>
            <FormField
              name="reason"
              control={form.control}
              render={({ field }) => (
                <FormItem>
                  <FormLabel required>
                    {mode === "admin"
                      ? "Lý do không xác thực"
                      : "Lý do từ chối yêu cầu"}
                  </FormLabel>
                  <TextArea rows={4} {...field} />
                  <FormMessage />
                </FormItem>
              )}
            />
          </Form>
        )}
      </Modal>
    </>
  );
};
