"use client";
import MarkdownRenderer from "@/components/markdown/MarkdownRenderer";
import { Avatar, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Post, PostsDTO } from "@/utils/services/Api";
import { AvatarFallback } from "@radix-ui/react-avatar";
import { CalendarDays, Edit3, Facebook, Share2 } from "lucide-react";
import Image from "next/image";
import React, { use, useState } from "react";
import { useRouter } from "next/navigation";
import { formatDateVi } from "@/utils/other/formatter";
import ShareButton from "./ShareButton";
import { mockPost } from "@/app/(user)/tutors/[id]/components/mockData";
import { useAppContext } from "@/components/provider/app-provider";
import { Tag } from "antd";
import { colorTag } from "../../components/PostCard";

interface PostDetailProps extends PostsDTO {}

const PostDetail: React.FC<PostDetailProps> = (params) => {
  const { user } = useAppContext();
  const [imageSrc, setImageSrc] = useState(
    params.thumbnail || mockPost.thumbnailUrl
  );

  const router = useRouter();

  return (
    <div>
      <div className="flex justify-between mb-4">
        <Tag color={colorTag[params.postType || 1]}>
          {params.postCategoryName}
        </Tag>
      </div>
      <h1 className="scroll-m-20 text-2xl md:text-4xl font-bold tracking-tight lg:text-5xl">
        {params.title}
      </h1>

      <div className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between my-8 mx-2 md:mx-4">
        <div className="flex items-center gap-4">
          <Avatar className="border-2 border-black">
            <AvatarImage src={mockPost.writer.avatarUrl} />
            <AvatarFallback>
              {params.author?.authorName?.slice(0, 1)}
            </AvatarFallback>
          </Avatar>
          <div className="space-y-1">
            <h4 className="scroll-m-20 md:text-xl font-semibold tracking-tight">
              {params.author?.authorName}
            </h4>
            <div className="text-sm text-muted-foreground flex items-center space-x-2">
              <CalendarDays className="w-4 h-4" />
              <p>{formatDateVi(new Date(params.createdDate || new Date()))}</p>
            </div>
          </div>
        </div>

        <div className="space-x-4">
          <Button>
            <Facebook />
          </Button>
          <ShareButton />
        </div>
      </div>

      {/* Thumbnail */}
      <Image
        src={imageSrc}
        alt={params.title || ""}
        width={1920} // Chiều rộng gốc của ảnh
        height={475} // Chiều cao gốc của ảnh
        layout="responsive"
        className="rounded-xl my-5"
        onError={() => setImageSrc(mockPost.thumbnailUrl)}
      />

      {/* Content */}
      <MarkdownRenderer content={params.content || ""} />
    </div>
  );
};

export default PostDetail;
