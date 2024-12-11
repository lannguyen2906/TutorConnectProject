"use client";
import React from "react";
import { Button } from "antd";
import { FaFileContract } from "react-icons/fa";
import { tConnectService } from "@/utils/services/tConnectService";
import axios from "axios";

const DownloadBillButton = ({ billId }: { billId: number }) => {
  const downloadContract = async () => {
    try {
      // Gọi API và thiết lập responseType để nhận dữ liệu dưới dạng Blob
      const response = await axios.get(
        `https://tutorconnectapi-d8gafsgrdka9gkbs.southafricanorth-01.azurewebsites.net/api/Bill/GenerateBillPdf?billId=${billId}`,
        {
          responseType: "blob",
        }
      );

      const href = URL.createObjectURL(response.data);

      // create "a" HTML element with href to file & click
      const link = document.createElement("a");
      link.href = href;
      link.setAttribute("download", `Hoa_don_${billId}.pdf`); //or any other extension
      document.body.appendChild(link);
      link.click();

      // clean up "a" element & remove ObjectURL
      document.body.removeChild(link);
      URL.revokeObjectURL(href);
    } catch (error) {
      console.error("Lỗi khi tải hóa đơn:", error);
    }
  };

  return (
    <Button icon={<FaFileContract />} onClick={downloadContract}>
      Tải Hóa đơn
    </Button>
  );
};

export default DownloadBillButton;
