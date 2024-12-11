"use client";
import React, { useState } from "react";
import { InboxOutlined } from "@ant-design/icons";
import type { UploadFile, UploadProps } from "antd";
import { Button, Image, message, Modal, Tooltip, Upload } from "antd";
import DownloadContractButton from "@/components/ui/download-contract";
import { FileText } from "lucide-react";
import useUploadFileEasy from "@/hooks/use-upload-file-easy";
import {
  useUpdateClassroom,
  useUploadContract,
} from "@/hooks/use-tutor-learner-subject";
import {
  HandleContractUploadDTO,
  RegisterLearnerDTO,
} from "@/utils/services/Api";
import { toast } from "react-toastify";
import { useAppContext } from "@/components/provider/app-provider";

const { Dragger } = Upload;

const props: UploadProps = {
  name: "file",
  multiple: true,
  onChange(info) {
    const { status } = info.file;
    if (status !== "uploading") {
      console.log(info.file, info.fileList);
    }
    if (status === "done") {
      message.success(`${info.file.name} file uploaded successfully.`);
    } else if (status === "error") {
      message.error(`${info.file.name} file upload failed.`);
    }
  },
  listType: "picture",
  onDrop(e) {
    console.log("Dropped files", e.dataTransfer.files);
  },
  accept: ".img, .png, .jpg, .jpeg",
  itemRender: (_, file) => <Item key={file.uid} file={file} element={_} />,
};

const Item = ({
  file,
  element,
}: {
  file: UploadFile;
  element: React.ReactNode;
}) => {
  const [open, setOpen] = useState(false);

  const handlePreview = () => {
    if (file.originFileObj) {
      const url = URL.createObjectURL(file.originFileObj);
      return url;
    }
    return "";
  };

  return (
    <div className="flex flex-col gap-3">
      <button onClick={() => setOpen(true)}>{element}</button>

      <Modal
        open={open}
        onCancel={() => setOpen(false)}
        footer={null}
        title="Xem ảnh"
        getContainer={false}
      >
        <Image alt={file.name} src={handlePreview()} />
      </Modal>
    </div>
  );
};

const UploadContracts = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const { selectedFiles, uploadFiles, handleFileChange } = useUploadFileEasy();
  const { user } = useAppContext();
  const { mutateAsync, isLoading } = useUploadContract();

  const handleSubmitFiles = async () => {
    const contractUrl = "contracts/" + tutorLearnerSubjectId;
    await uploadFiles(contractUrl);
    const data: HandleContractUploadDTO = {
      contractUrl,
      tutorId: user?.id!,
      tutorLearnerSubjectId,
    };
    const res = await mutateAsync(data);
    if (res.status == 201) {
      toast.success("Gửi hợp đồng thành công");
    }
  };

  return (
    <div className="flex flex-col gap-5">
      <div className="flex justify-between">
        <DownloadContractButton tutorLearnerSubjectId={tutorLearnerSubjectId} />
        <Button
          onClick={handleSubmitFiles}
          type="primary"
          icon={<FileText size={16} />}
          loading={isLoading}
          disabled={selectedFiles.length === 0}
        >
          Gửi hợp đồng
        </Button>
      </div>

      <Dragger {...props} onChange={(info) => handleFileChange(info.fileList)}>
        <p className="ant-upload-drag-icon">
          <InboxOutlined />
        </p>
        <p className="ant-upload-text">
          Nhấp hoặc kéo tệp vào khu vực này để tải lên
        </p>
      </Dragger>
    </div>
  );
};

export default UploadContracts;
