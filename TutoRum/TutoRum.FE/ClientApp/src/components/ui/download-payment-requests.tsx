"use client";
import React from "react";
import { Button } from "antd";
import { FaFileContract } from "react-icons/fa";
import axios from "axios";

const DownloadPaymentRequestsButton = () => {
  const downloadPaymentRequests = async () => {
    try {
      // Gọi API và thiết lập responseType để nhận dữ liệu dưới dạng Blob
      const response = await axios.get(
        `https://tutorconnectapiservice-hkhecjd7azg2gcfm.southeastasia-01.azurewebsites.net/api/PaymentRequest/download-payment-requests`, // Đảm bảo URL chính xác
        {
          responseType: "blob", // Thiết lập responseType là 'blob' để nhận file
        }
      );

      // Tạo URL cho file Blob nhận được
      const href = window.URL.createObjectURL(response.data);

      // Tạo element "a" HTML với href để tải file
      const link = document.createElement("a");
      link.href = href;
      link.setAttribute("download", "PaymentRequests.xlsx"); // Đặt tên file tải về
      document.body.appendChild(link);
      link.click(); // Mô phỏng click để tải file

      // Dọn dẹp sau khi tải xong
      document.body.removeChild(link);
      URL.revokeObjectURL(href);
    } catch (error) {
      console.error("Lỗi khi tải bản kê:", error);
    }
  };

  return (
    <Button icon={<FaFileContract />} onClick={downloadPaymentRequests}>
      Tải bản kê
    </Button>
  );
};

export default DownloadPaymentRequestsButton;
