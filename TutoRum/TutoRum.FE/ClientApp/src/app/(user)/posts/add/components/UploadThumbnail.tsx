"use client";
import React, { useState } from "react";
import { InboxOutlined } from "@ant-design/icons";
import type { UploadFile, UploadProps } from "antd";
import { Button, Image, message, Modal, Tooltip, Upload } from "antd";

const { Dragger } = Upload;

const props: UploadProps = {
  name: "file",
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
      <button type="button" onClick={() => setOpen(true)}>
        {element}
      </button>

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

const UploadThumbnail = ({
  handleFileChange,
  tempUrl,
}: {
  handleFileChange: (fileList: UploadFile[]) => void;
  tempUrl: string;
}) => {
  console.log(tempUrl);

  return (
    <div className="flex flex-col gap-5">
      <Dragger {...props} onChange={(info) => handleFileChange(info.fileList)}>
        <p className="ant-upload-drag-icon">
          <InboxOutlined />
        </p>
        <p className="ant-upload-text">
          Nhấp hoặc kéo tệp vào khu vực này để tải ảnh thumbnail
        </p>
      </Dragger>
    </div>
  );
};

export default UploadThumbnail;
