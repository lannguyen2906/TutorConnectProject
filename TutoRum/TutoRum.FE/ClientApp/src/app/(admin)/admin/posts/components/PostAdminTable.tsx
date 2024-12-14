"use client";
import { AuthorDTO, PostsDTO } from "@/utils/services/Api";
import { Empty, Input, Tag } from "antd";
import Table, { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import { Edit, Search, Trash } from "lucide-react";
import React, { useState } from "react";
import { usePostsAdmin } from "@/hooks/use-posts";
import {
  AddPostButton,
  DeletePostButton,
  EditPostButton,
} from "./PostButtonAdmin";

const PostAdminTable = () => {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading } = usePostsAdmin(pageNumber, pageSize);
  const [search, setSearch] = useState("");

  const columns: ColumnsType<PostsDTO> = [
    {
      title: "#",
      dataIndex: "postId",
      key: "postId",
    },
    {
      title: "Tiêu đề",
      dataIndex: "title",
      key: "title",
    },
    {
      title: "Người tạo",
      dataIndex: "author",
      key: "author",
      render: (author: AuthorDTO) => author.authorName,
    },
    {
      title: "Chủ đề",
      dataIndex: "postCategoryName",
      key: "postCategoryName",
    },
    {
      title: "Ngày tạo",
      dataIndex: "createdDate",
      key: "createdDate",
      render: (date: string) => dayjs(date).format("HH:mm DD/MM/YYYY"),
    },
    {
      title: "Hành động",
      key: "action",
      render: (_, record) => (
        <div className="flex gap-2">
          <EditPostButton postId={record.postId || 0} />
          <DeletePostButton postId={record.postId || 0} />
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <div className="flex gap-5 justify-between">
        <Input
          addonBefore={<Search size={16} className="text-muted-foreground" />}
          placeholder="Tìm kiếm ..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <AddPostButton />
      </div>
      <Table
        pagination={{
          position: ["bottomCenter"],
          pageSize: pageSize,
          onChange(page, pageSize) {
            setPageNumber(page);
            setPageSize(pageSize);
          },
          pageSizeOptions: [20, 50, 100],
          showSizeChanger: true,
        }}
        columns={columns}
        dataSource={data?.posts ?? []}
        rowKey={"postId"}
        scroll={{ x: "max-content" }}
        locale={{ emptyText: <Empty description={"Không có dữ liệu"} /> }}
      />
    </div>
  );
};

export default PostAdminTable;
