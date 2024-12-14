import Link from "next/link";
import React from "react";
import { ITutor } from "./mockData";
import { scheduleColumns } from "@/utils/other/mapper";
import { TutorDto } from "@/utils/services/Api";

const RelatedSearch = ({ tutor }: { tutor: TutorDto }) => {
  return (
    <div>
      <div className="text-2xl font-bold mb-5 mt-10 border-b-2">
        Tìm kiếm liên quan
      </div>
      <div className="flex flex-col gap-10">
        <div className="flex flex-col xl:flex-row gap-10">
          <div className="w-full lg:w-1/2">
            <div className="text-xl font-bold">Theo môn học</div>
            <div className="flex flex-col ml-3">
              {tutor.tutorSubjects?.map((s) => (
                <Link
                  key={s.subjectId}
                  href={`/tutors?filter=subject:${s.subjectId}`}
                  className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
                >
                  Tìm các gia sư cũng dạy môn {s.subject?.subjectName}
                </Link>
              ))}
            </div>
          </div>
          <div className="w-full lg:w-1/2">
            <div className="text-xl font-bold">Theo khu vực</div>
            <div className="flex flex-col ml-3">
              {tutor.teachingLocations?.map((location, index) => (
                <Link
                  key={index}
                  href={`/tutors?filter=location:${location.districts?.map(
                    (d) => d.districtId
                  )}`}
                  className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
                >
                  Tìm các gia sư cũng dạy tại{" "}
                  {location.districts?.map((d) => d.districtName).join(", ")}
                </Link>
              ))}
            </div>
          </div>
        </div>
        <div className="flex flex-col xl:flex-row gap-10">
          <div className="w-full lg:w-1/2">
            <div className="text-xl font-bold">Theo kinh nghiệm</div>
            <div className="flex flex-col ml-3">
              <Link
                href={`/tutors?filter=experience:${tutor.experience}`}
                className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
              >
                Tìm các gia sư có kinh nghiệm dạy từ {tutor.experience} năm trở
                lên
              </Link>
              {tutor.rating && (
                <Link
                  href={`/tutors?filter=rating:${tutor.rating}`}
                  className="text-gray-700 hover:text-Blueviolet transition-colors duration-200 underline"
                >
                  Tìm các gia sư được đánh giá từ {tutor.rating} sao trở lên
                </Link>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RelatedSearch;
