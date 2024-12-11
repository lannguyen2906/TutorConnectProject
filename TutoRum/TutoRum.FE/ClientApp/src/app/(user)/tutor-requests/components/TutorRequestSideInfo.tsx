import { Button } from "antd";
import { GraduationCap, Search } from "lucide-react";
import Link from "next/link";
import React from "react";

const subjects = [
  "Đàn Guitar",
  "Đàn Piano",
  "Trẻ chậm nói",
  "YOGA",
  "Đàn Violin",
  "Tiếng Việt Lớp 1",
  "Nhảy hiện đại (Dance)",
  "Toán Lớp 1",
  "Đàn Organ",
  "Tiếng Trung",
  "Vẽ",
  "Tiếng Hàn",
  "Khiêu Vũ giao tiếp",
  "Toán lớp 9",
  "Thanh nhạc",
  "Toán lớp 8",
  "Tiếng Anh Giao tiếp",
  "Lập trình",
  "Tiếng Việt cho người nước ngoài",
  "Tin học văn phòng",
];

const TutorRequestSideInfo = () => {
  return (
    <div className="flex flex-col gap-5">
      <div className="text-xl font-bold flex gap-2 items-center">
        Tìm kiếm gợi ý
        <Search size={20} />
      </div>
      <Link
        href="/tutors"
        className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
      >
        Lớp học gần nhất
      </Link>
      <Link
        href="/tutors"
        className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
      >
        Lớp học được trả phí cao nhất
      </Link>
      <Link
        href="/tutors"
        className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
      >
        Lớp học có số buổi ít nhất
      </Link>
      <div className="text-xl font-bold flex gap-2 items-center">
        Gia sư cần biết
        <GraduationCap size={20} />
      </div>
      <Link
        href="/tutors"
        className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
      >
        Quy trình nhận lớp
      </Link>
      <Link
        href="/tutors"
        className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
      >
        Hợp đồng mẫu
      </Link>
      <div className="text-xl font-bold flex gap-2 items-center">
        Các yêu cầu phổ biến
        <GraduationCap size={20} />
      </div>
      <div className="flex gap-2 flex-wrap">
        {subjects.map((subject, index) => (
          <Button type="dashed" key={index}>
            {subject}
          </Button>
        ))}
      </div>
    </div>
  );
};

export default TutorRequestSideInfo;
