import { cn } from "@/lib/utils";
import { formatDateVi, formatNumber } from "@/utils/other/formatter";
import { TutorRequestDTO } from "@/utils/services/Api";
import { Badge, Button, Card } from "antd";
import dayjs from "dayjs";
import {
  ArrowDown,
  BookOpen,
  Calendar,
  CalendarCheck2,
  Clock,
  Map,
  MapPin,
  User,
} from "lucide-react";
import React from "react";
import { FaVenusMars } from "react-icons/fa";
import customParseFormat from "dayjs/plugin/customParseFormat";
import { useAppContext } from "@/components/provider/app-provider";
import RegisterTutorRequestButton from "./RegisterTutorRequestButton";

dayjs.extend(customParseFormat);

const TutorRequestListCard = ({
  tutorRequest,
}: {
  tutorRequest: TutorRequestDTO;
}) => {
  const {
    requestSummary,
    detailedDescription,
    subject,
    tutorGender,
    fee,
    sessionsPerWeek,
    teachingLocation,
    numberOfStudents,
    timePerSession,
    startDate,
    studentGender,
    tutorQualificationId,
    freeSchedules,
    tutorQualificationName,
    createdUserId,
    id,
    registeredTutorIds,
  } = tutorRequest;
  const { user } = useAppContext();

  const [isFull, setIsFull] = React.useState(false);
  const isRegistered = registeredTutorIds?.includes(user?.id!);

  const TutorRequestCard = () => (
    <Card className="shadow-sm shadow-Blueviolet transition-all duration-500">
      <div className="flex flex-col gap-5">
        {/* Summary */}
        <div className="text-xl font-bold">{requestSummary} </div>

        {/* Description and Fee */}
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 md:gap-10">
          <div className="p-2 border-dashed border-2 md:w-2/3 w-full">
            {detailedDescription && detailedDescription.trim()?.length > 0
              ? detailedDescription
              : "Không có mô tả chi tiết"}
          </div>
          <div className="text-2xl font-bold md:w-1/3 w-full text-right text-Blueviolet flex justify-end">
            {formatNumber(fee?.toString() || "")}
            <span className="hidden xl:block">/Buổi</span>
            <span className="block xl:hidden">/B</span>
          </div>
        </div>

        {/* Subject, Tutor Gender, Sessions */}
        <div className="flex flex-col md:flex-row justify-between gap-4 md:gap-5">
          <div className="md:w-1/3 w-full flex items-center gap-2">
            <BookOpen />
            <div className="flex flex-col">
              <span className="font-bold hidden xl:block">Môn học</span>
              <span>{subject}</span>
            </div>
          </div>

          <div className="md:w-1/3 w-full flex items-center gap-2">
            <FaVenusMars size={24} />
            <div className="flex flex-col">
              <span className="font-bold hidden xl:block">Yêu cầu gia sư</span>
              <span>
                ({tutorGender == "male" ? "Nam" : "Nữ"}){" "}
                {tutorQualificationName}
              </span>
            </div>
          </div>

          <div className="md:w-1/3 w-full flex items-center gap-2">
            <Calendar />
            <div className="flex flex-col">
              <span className="font-bold hidden xl:block">Tuần học</span>
              <span>
                {sessionsPerWeek} buổi (
                {dayjs(timePerSession, "HH:mm:ss").format("HH:mm")}
                /buổi)
              </span>
            </div>
          </div>
        </div>

        {/* Detailed information */}
        <div
          className={cn(
            "overflow-hidden transition-all duration-500 ease-in-out flex flex-col gap-2",
            isFull ? "max-h-[1000px] opacity-100" : "max-h-0 opacity-0"
          )}
        >
          <div className="flex gap-2">
            <User />
            <span className="font-bold hidden xl:block">Học viên: </span>
            <span>
              {numberOfStudents} ({studentGender == "male" ? "Nam" : "Nữ"})
            </span>
          </div>
          <div className="flex gap-2">
            <MapPin />
            <span className="font-bold hidden xl:block">Địa điểm học: </span>
            <span>{teachingLocation}</span>
            <a
              target="_blank"
              href={`https://www.google.com/maps?q=${teachingLocation}&hl=vi&ie=UTF8`}
              className="inline-flex gap-1 items-center"
            >
              (Mở trong bản đồ) <Map size={16} />
            </a>
          </div>
          <div className="flex gap-2">
            <CalendarCheck2 />
            <span className="font-bold hidden xl:block">Thời gian rảnh: </span>
            <span>{freeSchedules}</span>
          </div>
          <div className="flex gap-2">
            <Clock />
            <span className="font-bold hidden xl:block">
              Thời gian học dự kiến:
            </span>
            <span>{formatDateVi(new Date(startDate || ""))}</span>
          </div>
        </div>

        <div className="flex justify-between">
          {/* Toggle button */}
          <button
            onClick={() => setIsFull(!isFull)}
            className="flex justify-between w-fit items-center gap-2 transition-all duration-500 hover:text-Blueviolet underline"
          >
            {isFull ? "Thu gọn" : "Xem thêm"}
            <ArrowDown
              size={16}
              className={cn(
                isFull ? "-rotate-180" : "rotate-0",
                "transition-transform duration-500"
              )}
            />
          </button>
          <RegisterTutorRequestButton
            title={isRegistered ? "Đã đăng ký" : "Đăng ký nhận lớp"}
            disabled={isRegistered}
            tutorRequestId={id!}
          />
        </div>
      </div>
    </Card>
  );

  return (
    <>
      {createdUserId == user?.id ? (
        <Badge.Ribbon text="Yêu cầu của bạn" color="cyan">
          <TutorRequestCard />
        </Badge.Ribbon>
      ) : (
        <TutorRequestCard />
      )}
    </>
  );
};

export default TutorRequestListCard;
