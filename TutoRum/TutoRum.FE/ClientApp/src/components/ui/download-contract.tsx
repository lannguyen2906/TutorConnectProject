"use client";
import React from "react";
import { Button } from "antd";
import { FaFileContract } from "react-icons/fa";
import { tConnectService } from "@/utils/services/tConnectService";
import axios from "axios";

const DownloadContractButton = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const downloadContract = async () => {
    try {
      // Gọi API và thiết lập responseType để nhận dữ liệu dưới dạng Blob
      const response = await axios.get(
        `https://tutorconnectapi-d8gafsgrdka9gkbs.southeastasia-01.azurewebsites.net/api/TutorLearnerSubject/download-contract/${tutorLearnerSubjectId}`,
        // `http://localhost:7026/api/TutorLearnerSubject/download-contract/${tutorLearnerSubjectId}`,
        {
          responseType: "blob",
        }
      );

      const href = URL.createObjectURL(response.data);

      // create "a" HTML element with href to file & click
      const link = document.createElement("a");
      link.href = href;
      link.setAttribute("download", "hop_dong_gia_su.docx"); //or any other extension
      document.body.appendChild(link);
      link.click();

      // clean up "a" element & remove ObjectURL
      document.body.removeChild(link);
      URL.revokeObjectURL(href);
    } catch (error) {
      console.error("Lỗi khi tải hợp đồng:", error);
    }
  };

  return (
    <Button icon={<FaFileContract />} onClick={downloadContract}>
      Tải Hợp Đồng
    </Button>
  );
};

export default DownloadContractButton;
