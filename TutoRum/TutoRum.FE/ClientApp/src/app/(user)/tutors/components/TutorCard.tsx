"use client";

import { TutorSummaryDto } from "@/utils/services/Api";
import { Button, Card, Dropdown, MenuProps, Tooltip } from "antd";
import Image from "next/image";
import React from "react";
import { mockTutor } from "../[id]/components/mockData";
import {
  ArrowDown,
  BookOpen,
  GraduationCap,
  MapPin,
  MoreVertical,
  Search,
  Star,
  Users,
} from "lucide-react";
import { cn } from "@/lib/utils";

const TutorCard = ({
  tutor,
  handleCompareSelect,
  selectedTutors,
}: {
  tutor: TutorSummaryDto;
  handleCompareSelect: (tutorId: string) => void;
  selectedTutors: string[];
}) => {
  const [isFull, setIsFull] = React.useState(false);
  const selectedComparedTutor = selectedTutors.includes(tutor?.tutorId ?? "");

  const items: MenuProps["items"] = [
    {
      label: (
        <a
          href={`/tutors/${tutor.tutorId}/register-tutor-subject`}
          target="_blank"
        >
          Đăng ký ngay
        </a>
      ),
      key: "0",
    },
    {
      label: (
        <div onClick={() => handleCompareSelect(tutor.tutorId ?? "")}>
          So sánh
        </div>
      ),
      key: "1",
    },
  ];

  return (
    <Card
      className={cn(
        "transition-all relative shadow-sm hover:shadow-md hover:shadow-Blueviolet cursor-pointer",
        selectedComparedTutor && "shadow-md shadow-Blueviolet"
      )}
    >
      <div className="flex flex-col lg:flex-row gap-4">
        {/* Avatar */}
        <div className="flex-shrink-0 w-full lg:w-1/4">
          <Image
            src={tutor.avatarUrl || mockTutor.avatarUrl}
            alt="Tutor Image"
            width={200}
            height={200}
            objectFit="cover"
            className="rounded-md w-full"
          />
        </div>

        {/* Info */}
        <div className="flex-grow space-y-2 w-full lg:w-2/4">
          <div className="flex items-center justify-between">
            <div className="text-lg font-bold">{tutor.fullName}</div>
            <div className="flex lg:hidden items-center gap-2 text-lg font-bold text-Blueviolet">
              <span>{tutor.rating ?? "MỚI"}</span>
              {tutor.rating && tutor.rating > 0 && (
                <Star color="gold" fill="yellow" size={24} />
              )}
            </div>
          </div>
          <div className="text-black text-sm space-y-1">
            <div title="Chuyên ngành" className="flex items-center gap-2">
              <GraduationCap className="text-midnightblue " size={16} />
              <span>{tutor.specialization}</span>
            </div>
            <div title="Môn học dạy" className="flex items-center gap-2">
              <BookOpen className="text-midnightblue" size={16} />
              <span>
                {tutor.tutorSubjects
                  ?.map((s) => s.subject?.subjectName)
                  .join(", ")}
              </span>
            </div>
            <div title="Số học viên" className="flex items-center gap-2">
              <Users className="text-midnightblue" size={16} />
              <span>
                {tutor.numberOfStudents
                  ? `Đã dạy ${tutor.numberOfStudents} học viên`
                  : "Chưa có học viên"}
              </span>
            </div>
            <div className="flex items-center gap-2">
              <MapPin className="text-midnightblue" size={16} />
              <div>
                {tutor.teachingLocations?.map((l) => (
                  <div className="flex gap-2 items-center" key={l.cityId}>
                    <span className="font-semibold whitespace-nowrap">
                      {l.cityName}:
                    </span>
                    <Tooltip
                      title={l.districts?.map((d) => d.districtName).join(", ")}
                    >
                      <span className="whitespace-nowrap max-w-[150px] truncate">
                        {l.districts?.map((d) => d.districtName).join(", ")}
                      </span>
                    </Tooltip>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* Brief Introduction */}
          <div
            className={cn(
              "overflow-hidden transition-all duration-200 ease-in-out italic border-muted rounded-md border-[1px] p-2 text-muted-foreground font-medium",
              isFull ? "max-h-[1000px]" : "max-h-[60px]"
            )}
          >
            {tutor.briefIntroduction}
          </div>
          {tutor.briefIntroduction && tutor.briefIntroduction.length > 120 && (
            <button
              onClick={() => setIsFull(!isFull)}
              className="flex items-center gap-2 text-sm underline text-Blueviolet"
            >
              {isFull ? "Thu gọn" : "Xem thêm"}
              <ArrowDown
                size={16}
                className={cn(
                  "transition-transform duration-150",
                  isFull ? "-rotate-180" : "rotate-0"
                )}
              />
            </button>
          )}
        </div>

        {/* Action */}
        <div className="flex flex-col justify-between lg:w-1/4 items-center">
          <div className="hidden lg:flex items-center gap-2 text-lg font-bold text-Blueviolet">
            <span>{tutor.rating ?? "MỚI"}</span>
            {tutor.rating && tutor.rating > 0 && (
              <Star color="gold" fill="yellow" size={16} />
            )}
          </div>
          <a href={`/tutors/${tutor.tutorId}`} target="_blank">
            <Button type="primary" icon={<Search size={16} />}>
              Xem chi tiết
            </Button>
          </a>
        </div>

        {/* Dropdown */}
        <div className="absolute top-4 right-4 hidden lg:block">
          <Dropdown menu={{ items: items }} trigger={["click"]}>
            <Button type="text" icon={<MoreVertical />} />
          </Dropdown>
        </div>
      </div>
    </Card>
  );
};

export default TutorCard;
