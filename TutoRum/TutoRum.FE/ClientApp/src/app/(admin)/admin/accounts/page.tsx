"use client";
import { useAccountList } from "@/hooks/admin/use-accounts";
import { formatDateVi } from "@/utils/other/formatter";
import { Input, Select, Switch, Table, TableColumnsType } from "antd";
import axios from "axios";
import React, { useEffect, useState } from "react";
import TagStatus from "./components/BadgeStatus";
import { Card } from "@/components/ui/card";
import { ContentLayout } from "../../components/content-layout";
import {
  AssignRoleButton,
  BlockUserButton,
  DetailButton,
} from "./components/action-buttons";
import { Search } from "lucide-react";
import RoleTag from "./components/BadgeStatus";

const fetchAddressById = async (id: string) => {
  const { data } = await axios.get(
    `https://esgoo.net/api-tinhthanh/5/${id}.htm`
  );
  return data.data.full_name;
};

const getColumns = (addresses: Record<string, string>): TableColumnsType => [
  {
    dataIndex: "userId",
    key: "userId",
    hidden: true,
  },
  {
    title: "Họ và tên",
    dataIndex: "fullname",
    key: "fullname",
  },
  {
    title: "Địa chỉ",
    dataIndex: "addressId",
    key: "addressId",
    render: (id: string) => addresses[id],
    hidden: true,
  },
  {
    title: "Địa chỉ",
    key: "addressDetail",
    render: (_: any, record: any) =>
      record.addressDetail
        ? `${record.addressDetail}, ${addresses[record.addressId]}`
        : "Chưa cập nhật",
  },
  {
    title: "Ngày sinh",
    dataIndex: "dob",
    key: "dob",
    render: (dob: string) =>
      dob ? formatDateVi(new Date(dob)) : "Chưa cập nhật",
  },
  {
    title: "Giới tính",
    dataIndex: "gender",
    key: "gender",
    showSorterTooltip: { target: "full-header" },
    filters: [
      { text: "Nam", value: true },
      { text: "Nữ", value: false },
    ],
    render: (gender: boolean) => (gender ? "Nam" : "Nữ"),
  },
  {
    title: "Vai trò",
    dataIndex: "roles",
    key: "roles",
    showSorterTooltip: { target: "full-header" },
    filters: [
      { text: "Học viên", value: "learner" },
      { text: "Gia sư", value: "tutor" },
      { text: "Nhân viên", value: "employee" },
      { text: "Quản trị", value: "admin" },
    ],
    render: (roles: string[], record) => (
      // Admin: red, Employee: blue, Tutor: green, Learner: orange
      <div>
        {roles.map((role) => (
          <RoleTag key={record.userId} role={role} />
        ))}
      </div>
    ),
  },
  {
    title: "Trạng thái",
    dataIndex: "lockoutEnabled",
    key: "lockoutEnabled",
    showSorterTooltip: { target: "full-header" },
    filters: [{ text: "Bị khóa", value: "blocked" }],
    render: (lockoutEnabled: boolean) => (
      <Switch
        checkedChildren="Hoạt động"
        unCheckedChildren="Bị khóa"
        checked={lockoutEnabled}
        disabled
      />
    ),
  },
  {
    title: "Hành động",
    dataIndex: "action",
    key: "action",
    render: (_: any, record: any) => (
      <div className="flex gap-3" key={record.userId}>
        <DetailButton />
        <AssignRoleButton id={record.userId} />
        <BlockUserButton
          id={record.userId}
          isBlocked={!record.lockoutEnabled}
        />
      </div>
    ),
  },
];

const AccountsPage = () => {
  const { data, isFetching } = useAccountList();
  const [addresses, setAddresses] = useState<Record<string, string>>({});

  useEffect(() => {
    if (data) {
      const fetchAddresses = async () => {
        const uniqueIds = [...new Set(data.map((item) => item.addressId))];
        const addressData: Record<string, string> = {};

        for (const id of uniqueIds) {
          if (id) addressData[id] = await fetchAddressById(id);
        }

        setAddresses(addressData);
      };

      fetchAddresses();
    }
  }, [data]);

  return (
    <ContentLayout title="Danh sách người dùng">
      <div className="space-y-4">
        <div className="flex justify-between">
          <Input
            addonAfter={<Search className="text-slategray" />}
            allowClear
            style={{ width: "50%" }}
            placeholder="Nhập từ khóa bạn muốn tìm kiếm"
          />
          <Select style={{ width: "20%" }} placeholder="Được tạo vào">
            <Select.Option value="week">1 tuần trước</Select.Option>
            <Select.Option value="week">1 tháng trước</Select.Option>
          </Select>
        </div>
        <Card>
          <Table
            dataSource={data}
            columns={getColumns(addresses)}
            showSorterTooltip={{ target: "sorter-icon" }}
          />
        </Card>
      </div>
    </ContentLayout>
  );
};

export default AccountsPage;
