"use client";
import { useEffect, useState } from "react";
import { getFilesByUrl } from "@/hooks/use-list-files";
import { formatNumber } from "@/utils/other/formatter";
import { Button, Image, Table, Tag } from "antd";
import { Verified } from "lucide-react";
import {
  useAdminContractList,
  useVerifyContract,
} from "@/hooks/admin/use-contracts";
import { ColumnsType } from "antd/es/table";
import { ContractDto } from "@/utils/services/Api";
import dayjs from "dayjs";
import VerifyContractButton from "./VerifyContractButton";

const ContractsTable = () => {
  const [data, setData] = useState<any[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const { data: contracts } = useAdminContractList(page, pageSize);

  console.log(page, pageSize);

  const certificateColumns: ColumnsType<ContractDto> = [
    {
      title: "ID",
      dataIndex: "contractId",
      key: "contractId",
    },
    {
      title: "Tên lớp",
      dataIndex: "className",
      key: "className",
    },
    {
      title: "Hình ảnh",
      dataIndex: "imgUrls",
      key: "imgUrls",
      render: (imgUrls: string[]) => (
        <div className="relative group" style={{ width: "100px" }}>
          <Image.PreviewGroup items={imgUrls}>
            <Image
              width={100}
              alt="imgUrl"
              src={imgUrls.length > 0 ? imgUrls[0] : ""}
              className="rounded-md shadow-md transition-transform duration-300 group-hover:scale-105"
            />
          </Image.PreviewGroup>
          <div className="absolute bottom-0 left-0 w-full bg-gradient-to-t from-black/70 via-black/40 to-transparent text-white text-sm text-center py-1 rounded-b-md opacity-0 group-hover:opacity-100 transition-opacity duration-300">
            {imgUrls.length} ảnh
          </div>
        </div>
      ),
    },
    {
      title: "Người dạy",
      dataIndex: "tutorName",
      key: "tutorName",
    },
    {
      title: "Giá trên giờ",
      dataIndex: "rate",
      key: "rate",
      render: (rate: string) => formatNumber(rate) + " VND",
    },
    {
      title: "Ngày bắt đầu",
      dataIndex: "startDate",
      key: "startDate",
      render: (date: string) => dayjs(date).format("YYYY-MM-DD"),
    },
    {
      title: "Trạng thái",
      dataIndex: "isVerified",
      key: "isVerified",
      render: (isVerified?: boolean) => {
        if (isVerified) {
          return <Tag color="green">Đã kiểm duyệt</Tag>;
        } else if (isVerified == false) {
          return <Tag color="red">Bị từ chối</Tag>;
        } else {
          return <Tag color="yellow">Chưa kiểm duyệt</Tag>;
        }
      },
    },
    {
      title: "Hành động",
      dataIndex: "action",
      key: "action",
      render: (_, record) =>
        record.contractId && <VerifyContractButton contract={record} />,
    },
  ];

  useEffect(() => {
    const fetchData = async () => {
      if (contracts?.items) {
        const loadedData = await Promise.all(
          contracts.items.map(async (item) => {
            const imgUrls = await getFilesByUrl(
              `${item.contractImg}/files` || ""
            );
            return { ...item, imgUrls }; // Append imgUrls to each item
          })
        );
        setData(loadedData);
      }
    };

    fetchData();
  }, [contracts]);

  return (
    <Table
      columns={certificateColumns}
      dataSource={data}
      rowKey="contractId"
      pagination={{
        current: page,
        pageSize: pageSize,
        total: contracts?.totalRecords,
        onChange: (page, pageSize) => {
          setPage(page);
          setPageSize(pageSize);
        },
      }}
    />
  );
};

export default ContractsTable;
