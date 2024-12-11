"use client";
import { Button, Image, Table, Tag } from "antd";
import dayjs from "dayjs";
import { Verified } from "lucide-react";

const mockCertificateData = [
  {
    id: 1,
    imgUrl:
      "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/certificates%2Ffiles%2F1730107945674_rc-upload-1730107791069-5?alt=media&token=e3b64c4c-c55e-40cf-91ed-dd6b48be08f4",
    description: "Chứng chỉ Lập trình viên Frontend",
    issueDate: "2024-10-01T10:37:48.710Z",
    expiryDate: "",
    isVerified: true,
    tutor: { name: "Nguyễn Văn A", id: 101 },
  },
  {
    id: 2,
    imgUrl:
      "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/certificates%2Ffiles%2F1730107945674_rc-upload-1730107791069-5?alt=media&token=e3b64c4c-c55e-40cf-91ed-dd6b48be08f4",
    description: "Chứng chỉ Lập trình viên Backend",
    issueDate: "2024-09-15T10:37:48.710Z",
    expiryDate: "2025-09-15T10:37:48.710Z",
    isVerified: false,
    tutor: { name: "Trần Thị B", id: 102 },
  },
  {
    id: 3,
    imgUrl:
      "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/certificates%2Ffiles%2F1730107945674_rc-upload-1730107791069-5?alt=media&token=e3b64c4c-c55e-40cf-91ed-dd6b48be08f4",
    description: "Chứng chỉ Khoa học dữ liệu",
    issueDate: "2024-08-20T10:37:48.710Z",
    expiryDate: "2025-08-20T10:37:48.710Z",
    isVerified: true,
    tutor: { name: "Lê Văn C", id: 103 },
  },
  {
    id: 4,
    imgUrl:
      "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/certificates%2Ffiles%2F1730107945674_rc-upload-1730107791069-5?alt=media&token=e3b64c4c-c55e-40cf-91ed-dd6b48be08f4",
    description: "Chứng chỉ Kỹ sư phần mềm",
    issueDate: "2024-10-28T10:37:48.710Z",
    expiryDate: "2025-10-28T10:37:48.710Z",
    isVerified: true,
    tutor: { name: "Nguyễn Thị D", id: 104 },
  },
  {
    id: 5,
    imgUrl:
      "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/certificates%2Ffiles%2F1730107945674_rc-upload-1730107791069-5?alt=media&token=e3b64c4c-c55e-40cf-91ed-dd6b48be08f4",
    description: "Chứng chỉ An ninh mạng",
    issueDate: "2024-07-10T10:37:48.710Z",
    expiryDate: "2025-07-10T10:37:48.710Z",
    isVerified: false,
    tutor: { name: "Phạm Văn E", id: 105 },
  },
];

const certificateColumns = [
  {
    title: "ID",
    dataIndex: "id",
    key: "id",
  },
  {
    title: "Mô tả",
    dataIndex: "description",
    key: "description",
  },
  {
    title: "Hình ảnh",
    dataIndex: "imgUrl",
    key: "imgUrl",
    render: (imgUrl: string) => (
      <Image
        src={imgUrl}
        alt="Certificate"
        style={{ width: 100, height: 100 }}
        className="object-cover"
      />
    ),
  },
  {
    title: "Ngày cấp",
    dataIndex: "issueDate",
    key: "issueDate",
    render: (date: string) => dayjs(date).format("YYYY-MM-DD"),
  },
  {
    title: "Ngày hết hạn",
    dataIndex: "expiryDate",
    key: "expiryDate",
    render: (date: string) =>
      date ? dayjs(date).format("YYYY-MM-DD") : "Không có",
  },
  {
    title: "Đã duyệt",
    dataIndex: "isVerified",
    key: "isVerified",
    render: (isVerified: boolean) => (
      <Tag color={isVerified ? "green" : "red"}>
        {isVerified ? "Đã duyệt" : "Chưa duyệt"}
      </Tag>
    ),
  },
  {
    title: "Hành động",
    dataIndex: "action",
    key: "action",
    render: () => (
      <Button type="primary" icon={<Verified />}>
        Kiểm duyệt
      </Button>
    ),
  },
];

import React from "react";

const CertificatesTable = () => {
  return (
    <Table
      columns={certificateColumns}
      dataSource={mockCertificateData}
      rowKey={"id"}
    />
  );
};

export default CertificatesTable;
