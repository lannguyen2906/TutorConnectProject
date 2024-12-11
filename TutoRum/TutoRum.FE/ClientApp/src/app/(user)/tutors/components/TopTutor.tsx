"use client";
import { useTopTutor } from "@/hooks/use-tutor";
import Image from "next/image";
import React from "react";
import { mockTutor } from "../[id]/components/mockData";
import Link from "next/link";
import { Badge } from "antd";

const rankStyles = [
  { color: "gold", text: "1st" },
  { color: "silver", text: "2nd" },
  { color: "volcano", text: "3rd" },
];

const TopTutor = () => {
  const { data, isLoading } = useTopTutor(3);

  return (
    <div className="container mt-5">
      <h2 className="text-2xl font-bold mb-4 text-midnightblue">
        Top Gia Sư Tháng
      </h2>
      <div className="flex flex-col space-y-6">
        {data?.map((tutor, index) => (
          <Badge.Ribbon
            text={rankStyles[index]?.text}
            color={rankStyles[index]?.color}
            key={tutor.tutorId}
            placement="end"
          >
            <Link
              href={`/tutors/${tutor.tutorId}`}
              key={tutor.tutorId}
              className="flex items-center bg-white shadow-md rounded-lg p-4 hover:shadow-lg transition-shadow duration-300"
            >
              <div className="flex items-center">
                {/* Avatar */}
                <Image
                  width={50}
                  height={50}
                  src={tutor.avatarUrl || mockTutor.avatarUrl}
                  alt={`${tutor.fullName} Avatar`}
                  className=" mr-4"
                  objectFit="cover"
                />
                {/* Thông tin gia sư */}
                <div className="flex flex-col">
                  <span className="font-semibold text-muted-foreground whitespace-nowrap">
                    {tutor.fullName}
                  </span>
                </div>
              </div>
            </Link>
          </Badge.Ribbon>
        ))}
      </div>
    </div>
  );
};

export default TopTutor;
