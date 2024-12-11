"use client";
import React, { useEffect } from "react";
import TutorProfileForm from "../../settings/user-profile/components/TutorProfileForm";
import { useTutorDetail } from "@/hooks/use-tutor";
import { useAppContext } from "@/components/provider/app-provider";
import { Button, Tooltip } from "antd";
import { LogIn, Verified, VerifiedIcon } from "lucide-react";
import { useRouter } from "next/navigation";
import TutorProfileUpdateForm from "../../settings/user-profile/components/TutorProfileUpdateForm";

const TutorDetail = () => {
  const { user } = useAppContext();
  const userId = user?.id;
  // Chỉ gọi hook khi có userId
  const { data: tutor, isLoading, error } = useTutorDetail(userId!);
  const route = useRouter();

  return (
    <div>
      {tutor ? (
        <div>
          <div className="text-3xl font-bold mb-7 border-b-2 w-fit relative">
            Thông tin gia sư
            <div className="absolute top-0 -right-10">
              <Tooltip
                title={tutor.isVerified ? "Đã xác minh" : "Chưa xác minh"}
              >
                {tutor.isVerified ? (
                  <Verified fill="blueviolet" color="white" />
                ) : (
                  <Verified fill="gray" color="white" />
                )}
              </Tooltip>
            </div>
          </div>
          <TutorProfileUpdateForm tutor={tutor} />
        </div>
      ) : (
        <div className="flex flex-col items-center">
          <span>Bạn chưa đăng ký làm gia sư</span>
          <Button
            icon={<LogIn />}
            onClick={() => route.push("/tutor-register")}
            className="mt-4"
            type="primary"
          >
            Đăng ký ngay
          </Button>
        </div>
      )}
    </div>
  );
};

export default TutorDetail;
