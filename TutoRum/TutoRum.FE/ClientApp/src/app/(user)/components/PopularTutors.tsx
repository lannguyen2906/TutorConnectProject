import { useTopTutor } from "@/hooks/use-tutor";
import { Button, Card, Rate, Tag } from "antd";
import Image from "next/image";
import React from "react";
import { mockTutor } from "../tutors/[id]/components/mockData";
import {
  ArrowRight,
  BookOpen,
  GraduationCap,
  Search,
  Users,
} from "lucide-react";
import { Separator } from "@/components/ui/separator";
import Link from "next/link";
import { useRouter } from "next/navigation";

const PopularTutors = () => {
  const { data, isLoading } = useTopTutor(3);
  const router = useRouter();

  return (
    <div className="bg-gradient-to-r from-lightblue via-sky-100 to-lightblue py-12 px-6 lg:px-12">
      <div className="max-w-7xl mx-auto">
        {/* Tiêu đề và nút "Xem thêm" */}
        <div className="flex justify-between items-center mb-8">
          <h2 className="text-midnightblue text-4xl font-bold">
            Top gia sư phổ biến
          </h2>
          <Link
            href={"/tutors"}
            className="flex items-center text-Blueviolet hover:text-blue-800 gap-2 text-lg font-medium underline"
          >
            Xem thêm các gia sư khác
            <ArrowRight className="animate-pulse" size={20} />
          </Link>
        </div>

        {/* Hiển thị danh sách gia sư */}
        <div className="flex flex-wrap justify-center gap-6">
          {data?.map((tutor) => (
            <Card
              key={tutor.tutorId}
              className="w-[320px] p-4 bg-white shadow-lg hover:shadow-xl transition-shadow duration-300 rounded-lg"
              cover={
                <div className="flex justify-center p-4">
                  <Image
                    src={tutor.avatarUrl || mockTutor.avatarUrl}
                    alt="avatar"
                    width={120}
                    height={120}
                    className="w-full shadow-md"
                  />
                </div>
              }
            >
              <div className="text-start">
                {/* Tên gia sư */}
                <div className="font-bold text-2xl text-midnightblue mb-2">
                  {tutor.fullName}
                </div>

                {/* Môn học */}
                <div className="flex justify-start items-center text-gray-600 mb-2">
                  <BookOpen className="text-Blueviolet" size={20} />
                  <span className="ml-2">
                    {tutor.tutorSubjects
                      ?.map((s) => s.subject?.subjectName)
                      .join(", ")}
                  </span>
                </div>

                {/* Kinh nghiệm */}
                <div className="flex justify-start items-center text-gray-600 mb-2">
                  <GraduationCap className="text-Blueviolet" size={20} />
                  <span className="ml-2">
                    {tutor.experience} năm kinh nghiệm
                  </span>
                </div>

                {/* Đánh giá và nút "Xem chi tiết" */}
                <div className="flex justify-between items-center mt-4">
                  {tutor.rating ? (
                    <div className="flex items-center">
                      <Rate value={tutor.rating ?? 0} allowHalf disabled />
                    </div>
                  ) : (
                    <Tag color="blue" className="text-base">
                      MỚI
                    </Tag>
                  )}
                  <Button
                    type="primary"
                    onClick={() => router.push(`/tutors/${tutor.tutorId}`)}
                    icon={<Search size={18} />}
                    className="rounded-md bg-bluetext-Blueviolet hover:bg-blue-700"
                  >
                    Chi tiết
                  </Button>
                </div>
              </div>

              {/* Separator */}
              <Separator className="my-4" />

              {/* Thông tin học viên */}
              <div className="flex items-center justify-center gap-2 text-gray-600">
                <Users className="text-Blueviolet" size={20} />
                <span className="text-base font-medium">
                  {tutor.numberOfStudents} học viên
                </span>
              </div>
            </Card>
          ))}
        </div>
      </div>
    </div>
  );
};

export default PopularTutors;
