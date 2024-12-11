"use client";

import React from "react";
import Image from "next/image";
import { Card, Button, Typography, Space, Tag } from "antd";
import { useRouter } from "next/navigation";
import { formatDateVi } from "@/utils/other/formatter";
import { mockPost } from "../../tutors/[id]/components/mockData";
import { usePostsHomepage } from "@/hooks/use-posts";
import {
  User as UserIcon,
  Calendar as CalendarIcon,
  Folder as FolderIcon,
} from "lucide-react";

const { Meta } = Card;
const { Text } = Typography;

export const colorTag: Record<number, string> = {
  1: "#4CAF50", // Công nghệ
  2: "#FF9800", // Đời sống
  3: "#2196F3", // Giáo dục
  4: "#9C27B0", // Kỹ năng mềm
};

const PostCard = () => {
  const router = useRouter();
  const [pageSize, setPageSize] = React.useState(10);
  const [pageNumber, setPageNumber] = React.useState(1);
  const { data, isLoading } = usePostsHomepage(pageNumber, pageSize);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      {data?.posts?.map((post) => (
        <Card
          key={post.postId}
          hoverable
          className="shadow-md hover:shadow-lg transition-shadow duration-300"
          cover={
            <div className="relative w-full h-48">
              <Image
                src={post.thumbnail || mockPost.thumbnailUrl} // Thay bằng đường dẫn của bạn
                alt="Thumbnail"
                fill
                style={{ objectFit: "cover" }}
                className="rounded-t-md"
              />
              <Tag
                color={colorTag[post.postType || 1]}
                className="absolute top-2 left-2"
              >
                {post.postCategoryName}
              </Tag>
            </div>
          }
          onClick={() => router.push(`/posts/${post.postId}`)}
        >
          <Meta
            title={
              <Text className="text-lg font-semibold whitespace-normal">
                {post.title}
              </Text>
            }
            description={
              <div className="space-y-2">
                <div className="flex justify-between items-center">
                  <Space size="small" className="text-sm text-gray-600">
                    <UserIcon size={16} />
                    <span>{post.author?.authorName || "Không rõ"}</span>
                  </Space>
                  <Space size="small" className="text-sm text-gray-600">
                    <CalendarIcon size={16} />
                    <span>
                      {formatDateVi(new Date(post.createdDate || new Date()))}
                    </span>
                  </Space>
                </div>
                <Text className="mt-2 text-gray-600 line-clamp-3">
                  {post.subContent || "Không có mô tả"}
                </Text>
              </div>
            }
          />
        </Card>
      ))}
    </div>
  );
};

export default PostCard;
