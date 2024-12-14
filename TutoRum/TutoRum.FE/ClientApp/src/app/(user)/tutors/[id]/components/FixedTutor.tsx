"use client";
import React from "react";
import { Button, Card, Image, Select } from "antd";
import VerifiedName from "./VerifiedName";
import { useRouter } from "next/navigation";
import { TutorDto } from "@/utils/services/Api";
import { isValidUrl } from "@/app/(user)/user/settings/user-profile/components/UploadVideo";
import { GraduationCap } from "lucide-react";
import { mockTutor } from "./mockData";

const FixedTutor = ({ tutor }: { tutor: TutorDto }) => {
  const route = useRouter();

  return (
    <Card className="w-full shadow">
      <div className="space-y-3">
        <div>
          {isValidUrl(tutor?.videoUrl!) ? (
            <video
              style={{
                maxHeight: "300px",
                width: "100%",
                objectFit: "cover",
              }}
              controls
              src={tutor?.videoUrl!}
            />
          ) : (
            <Image
              src={tutor?.avatarUrl || mockTutor.avatarUrl}
              alt="Tutor Image"
              width={"100%"}
              height={150}
              className="object-cover"
            />
          )}
        </div>
        <VerifiedName
          fullName={tutor?.fullName!}
          isVerified={tutor?.isVerified!}
          size={1}
        />
        <div className="text-sm">{tutor?.briefIntroduction}</div>
        <div>
          <a
            href={`/tutors/${tutor?.tutorId}/register-tutor-subject`}
            target="_blank"
          >
            <Button type="primary" icon={<GraduationCap />} className="w-full">
              Đăng ký ngay
            </Button>
          </a>
        </div>
      </div>
    </Card>
  );
};

export default FixedTutor;
