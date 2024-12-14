"use client";
import React from "react";
import { Button, Modal, UploadProps } from "antd";
import { FaFileContract } from "react-icons/fa";
import { useUploadPaymentRequestExcel } from "../../hooks/use-payment-request";
import { toast } from "react-toastify";
import Dragger from "antd/es/upload/Dragger";
import { InboxOutlined } from "@ant-design/icons";

const UploadPaymentRequestsButton = () => {
  const [isModalOpen, setIsModalOpen] = React.useState(false);
  const [uploadFile, setUploadFile] = React.useState<File | null>(null);
  const { mutateAsync, isLoading } = useUploadPaymentRequestExcel();

  const handleOk = async () => {
    try {
      const data = await mutateAsync(uploadFile as File);
      if (data.status === 201) {
        toast.success("Cập nhật bản kê thành công");
        setIsModalOpen(false);
        setUploadFile(null);
      }
    } catch (error) {
      console.error("Error uploading file:", error);
    }
  };

  const props: UploadProps = {
    name: "file",
    multiple: false,
    onChange(info) {
      const { status } = info.file;
      if (status !== "uploading") {
        console.log(info.file, info.fileList);
      }
      if (status === "done") {
        toast.success(`${info.file.name} được tải lên thành công`);
        setUploadFile(info.file.originFileObj as File);
      } else if (status === "error") {
        toast.error(`${info.file.name} file upload failed.`);
      }
    },
    onDrop(e) {
      console.log("Dropped files", e.dataTransfer.files);
    },
  };

  return (
    <>
      <Button
        type="primary"
        icon={<FaFileContract />}
        onClick={() => setIsModalOpen(true)}
      >
        Cập nhật bản kê
      </Button>
      <Modal
        title="Cập nhật bản kê"
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        okText="Cập nhật"
        cancelText="Hủy"
        confirmLoading={isLoading}
        onOk={handleOk}
        okButtonProps={{ disabled: !uploadFile || isLoading }}
      >
        <Dragger {...props}>
          <p className="ant-upload-drag-icon">
            <InboxOutlined />
          </p>
          <p className="ant-upload-text">
            Nhấp hoặc kéo tệp vào khu vực này để tải lên
          </p>
        </Dragger>
      </Modal>
    </>
  );
};

export default UploadPaymentRequestsButton;
