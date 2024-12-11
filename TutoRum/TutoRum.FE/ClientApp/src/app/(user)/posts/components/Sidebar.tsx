"use client";
import { Input } from "@/components/ui/input";
import Image from "next/image";
import React from "react";

import { Card, CardContent } from "@/components/ui/card";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { usePostsHomepage } from "@/hooks/use-posts";
import { mockPost } from "../../tutors/[id]/components/mockData";

const Sidebar = () => {
  const [pageSize, setPageSize] = React.useState(10);
  const [pageNumber, setPageNumber] = React.useState(1);
  const { data, isLoading } = usePostsHomepage(pageNumber, pageSize);

  return (
    <div>
      <div className="mb-4">
        <Input placeholder="Tìm kiếm bài viết..." className="w-full" />
      </div>
      <div className="flex justify-center">
        <Carousel className="w-full max-w-xs">
          <CarouselContent className="flex items-center">
            {data?.posts?.map((_, index) => (
              <CarouselItem key={index}>
                <div className="p-1">
                  <Card>
                    <CardContent className="flex flex-col items-center justify-center p-6">
                      {/* Thumbnail */}
                      <div className="w-full h-32 relative mb-2">
                        <Image
                          src={_.thumbnail || mockPost.thumbnailUrl} // Thay bằng đường dẫn của bạn
                          alt="Thumbnail"
                          layout="fill"
                          objectFit="cover"
                          className="rounded-md"
                        />
                      </div>

                      {/* Description */}
                      <p className="text-sm text-gray-600 text-center line-clamp-2">
                        {_.title}
                      </p>
                    </CardContent>
                  </Card>
                </div>
              </CarouselItem>
            ))}
          </CarouselContent>
          <CarouselPrevious />
          <CarouselNext />
        </Carousel>
      </div>
    </div>
  );
};

export default Sidebar;
