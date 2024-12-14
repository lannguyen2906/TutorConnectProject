"use client";
import Image from "next/image";
import {
  BookOpen,
  BriefcaseBusiness,
  GraduationCap,
  Home,
  MapPin,
} from "lucide-react";
import VerifiedName from "./VerifiedName";
import { TutorDto } from "@/utils/services/Api";
import { mockTutor } from "./mockData";
import { cn } from "@/lib/utils";

type TutorBasicInfoProps = Pick<
  TutorDto,
  | "avatarUrl"
  | "fullName"
  | "specialization"
  | "experience"
  | "teachingLocations"
  | "status"
  | "tutorSubjects"
  | "addressDetail"
  | "briefIntroduction"
  | "major"
  | "isVerified"
>;

const TutorBasicInfo = ({
  avatarUrl,
  fullName,
  specialization,
  experience,
  teachingLocations,
  status,
  tutorSubjects,
  addressDetail,
  briefIntroduction,
  major,
  isVerified,
}: TutorBasicInfoProps) => {
  return (
    <div className="flex flex-col xl:flex-row gap-5">
      <div className="w-full xl:w-1/3">
        <Image
          src={avatarUrl || mockTutor.avatarUrl}
          alt={`${fullName}'s Avatar`}
          className="rounded-sm border-2 border-Blueviolet"
          width={100}
          height={100}
          layout="responsive"
        />
      </div>
      <div
        className={cn(
          "space-y-4 my-2 w-full xl:w-2/3",
          !isVerified && "text-red"
        )}
      >
        <VerifiedName
          fullName={fullName!}
          isVerified={status == "VERIFIED"}
          size={3}
        />
        <div className="text-lg">{briefIntroduction}</div>
        <div className="space-y-2">
          <div className="flex gap-4">
            <BriefcaseBusiness className="text-midnightblue" />
            <span>
              <span className="font-bold">
                {major} - {specialization}
              </span>
            </span>
          </div>
          <div className="flex gap-4">
            <GraduationCap className="text-midnightblue" />
            <span>
              Đã có <span className="font-bold">{experience}</span> năm kinh
              nghiệm giảng dạy
            </span>
          </div>
          <div className="flex gap-4">
            <Home className="text-midnightblue" />
            <span>
              Đang ở tại <span className="font-bold">{addressDetail}</span>
            </span>
          </div>
          <div className="flex gap-4">
            <MapPin className="text-midnightblue" />
            <div className="flex flex-col">
              <span>Có thể dạy tại</span>
              <div>
                {teachingLocations?.map((l) => (
                  <div className="ml-3 flex gap-5 items-center" key={l.cityId}>
                    <span className="font-bold">{l.cityName}</span>
                    <span>
                      {l.districts?.map((d) => d.districtName).join(", ")}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          </div>
          <div className="flex gap-4">
            <BookOpen className="text-midnightblue" />
            <span>
              Có thể dạy{" "}
              <span className="font-bold">
                {tutorSubjects?.map((s) => s.subject?.subjectName).join(", ")}
              </span>
            </span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TutorBasicInfo;
