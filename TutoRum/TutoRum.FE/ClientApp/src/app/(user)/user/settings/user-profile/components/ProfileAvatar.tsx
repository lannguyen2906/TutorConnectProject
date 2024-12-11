"use client";
import { useAppContext } from "@/components/provider/app-provider";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Image } from "antd";
import { Mail, Camera, Check, Loader, Loader2 } from "lucide-react"; // Icon từ Lucide React
import React, { useState } from "react";
import useTempFileUpload from "@/hooks/use-upload-file";
import { useUpdateProfile } from "@/hooks/use-profile";
import { mockPost } from "@/app/(user)/tutors/[id]/components/mockData";
import { toast } from "react-toastify";

const ProfileAvatar = () => {
  const { user } = useAppContext();
  const { mutateAsync: updateProfile } = useUpdateProfile();
  const { handleFileChange, tempFileURL, moveFile, progress } =
    useTempFileUpload({
      tempFileURLDefault: null,
    });

  const [uploading, setUploading] = useState(false);

  const changeAvatar = async () => {
    try {
      setUploading(true); // Bắt đầu quá trình tải lên
      const url = await moveFile("avatar");

      const { data } = await updateProfile({ avatarUrl: url });

      if (data.status === 200) {
        toast.success("Đổi avatar thành công!");
      }
    } catch (err) {
      toast.error("Đổi avatar thất bại!");
    } finally {
      setUploading(false); // Kết thúc quá trình tải lên
    }
  };

  return (
    <Card className="p-6 shadow-md rounded-lg flex flex-col items-center space-y-4">
      {/* Avatar with Edit Icon */}
      <div className="relative">
        {/* Hiển thị vòng tròn progress khi đang tải lên */}
        {(uploading || (progress > 0 && progress < 100)) && (
          <div className="absolute z-10 w-full h-full flex items-center justify-center">
            <Loader className="animate-spin text-white w-16 h-16" />
          </div>
        )}

        <Image
          className="rounded-full border-4 border-gray-200 shadow-lg object-cover"
          width={150}
          height={150}
          alt="avatar"
          src={tempFileURL || user?.avatarUrl || mockPost.writer.avatarUrl}
          preview={{
            maskClassName:
              "rounded-full border-4 border-gray-200 shadow-lg object-cover",
          }}
        />
        <Label
          htmlFor="changeAvatar"
          className="absolute bottom-0 right-0 bg-white p-2 rounded-full shadow cursor-pointer"
        >
          <Camera className="text-gray-500" size={24} />
        </Label>
        <input
          onChange={handleFileChange}
          id="changeAvatar"
          type="file"
          hidden={true}
        />
        {tempFileURL && (
          <Button
            onClick={changeAvatar}
            className="absolute top-0 right-0 p-2 rounded-full shadow cursor-pointer"
          >
            <Check size={24} />
          </Button>
        )}
      </div>

      {/* User Information */}
      <div className="w-full text-center overflow-hidden">
        <h3 className="text-2xl font-bold text-gray-900 whitespace-pre-wrap">
          {user?.fullName || "Tên người dùng"}
        </h3>
        <div className="flex items-center justify-center space-x-2 mt-2">
          <Mail className="text-gray-500" size={18} />
          <span className="text-sm text-slategray font-medium">
            {user?.email || "user@example.com"}
          </span>
        </div>
      </div>
    </Card>
  );
};

export default ProfileAvatar;
