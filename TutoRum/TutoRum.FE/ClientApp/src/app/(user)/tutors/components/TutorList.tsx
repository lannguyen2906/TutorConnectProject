"use client";

import { useTutorHomePage } from "@/hooks/use-tutor";
import React, { useEffect, useState } from "react";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardFooter,
} from "@/components/ui/card";
import {
  Star,
  House,
  MoreVertical,
  CalendarRange,
  BriefcaseBusiness,
  GraduationCap,
  MapIcon,
  BookOpen,
  UserRoundSearch,
  Search,
  User,
} from "lucide-react"; // Icon for rating
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import Pagination from "../../components/Pagination/Pagination";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { Badge, Button, Empty, Rate, Spin } from "antd";
import { mockTutor } from "../[id]/components/mockData";
import CompareModal from "./SelectedTutorCompareModal";
import { useFormContext } from "react-hook-form";
import TutorCard from "./TutorCard";
import { useAppContext } from "@/components/provider/app-provider";

const TutorList = () => {
  const pageSize = 5;
  const [currentPage, setCurrentPage] = useState(1);
  const router = useRouter();
  const form = useFormContext();
  const { data, isLoading, refetch } = useTutorHomePage(
    form.watch(),
    currentPage,
    pageSize
  );
  const totalRecords = data?.totalRecordCount || 0;
  const totalPages = Math.ceil(totalRecords / pageSize);
  const { user } = useAppContext();
  const [selectedTutors, setSelectedTutors] = useState<string[]>([]);
  const [isModalVisible, setIsModalVisible] = useState(false);

  useEffect(() => {
    refetch(form.watch());
  }, [currentPage, form, refetch]);

  const handleCompareSelect = (tutorId: string) => {
    setIsModalVisible(true);

    if (selectedTutors.includes(tutorId)) return;

    setSelectedTutors((prevTutors) => {
      if (prevTutors.length < 2) {
        return [...prevTutors, tutorId];
      } else {
        return [prevTutors[1], tutorId].filter(
          (value) => value !== undefined
        ) as string[];
      }
    });
  };

  const handleCloseModal = () => {
    setIsModalVisible(false);
  };

  const removeTutor = (tutorId: string) => {
    setSelectedTutors((prevTutors) =>
      prevTutors.filter((id) => id !== tutorId)
    );
  };

  // Sử dụng useEffect để lắng nghe sự thay đổi của selectedTutors
  useEffect(() => {
    console.log("Các ID đã chọn:", selectedTutors);
  }, [selectedTutors]);
  if (isLoading) {
    return (
      <div className="text-center py-8">
        <Spin />
      </div>
    );
  }

  if (data?.tutors?.length === 0)
    return <Empty description="Không tìm thấy gia sư tương ứng" />;
  return (
    <div>
      <div className="space-y-4">
        {data?.tutors?.map((tutor) =>
          user?.id == tutor?.tutorId ? (
            <Badge.Ribbon
              text="Đây là tôi"
              key={tutor.tutorId}
              placement="start"
              color="cyan"
            >
              <TutorCard
                tutor={tutor}
                key={tutor.tutorId}
                handleCompareSelect={handleCompareSelect}
                selectedTutors={selectedTutors}
              />
            </Badge.Ribbon>
          ) : (
            <TutorCard
              tutor={tutor}
              key={tutor.tutorId}
              handleCompareSelect={handleCompareSelect}
              selectedTutors={selectedTutors}
            />
          )
        )}
      </div>
      <Pagination
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={setCurrentPage}
      />
      <CompareModal
        visible={isModalVisible}
        onClose={handleCloseModal}
        selectedTutors={selectedTutors}
        removeTutor={removeTutor}
      />
    </div>
  );
};

export default TutorList;
