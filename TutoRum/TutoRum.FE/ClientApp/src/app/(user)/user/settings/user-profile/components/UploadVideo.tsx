"use client";
import { mockPost } from "@/app/(user)/tutors/[id]/components/mockData";
import useUploadFileEasy, {
  UseUploadFileResult,
} from "@/hooks/use-upload-file-easy";
import { Button, Modal, Tooltip, Upload, UploadFile } from "antd";
import { Video } from "lucide-react";
import React, { useState } from "react";
import { useFormContext } from "react-hook-form";

export function isValidUrl(urlString: string): boolean {
  try {
    new URL(urlString); // Nếu urlString là một URL hợp lệ, đối tượng URL sẽ được khởi tạo thành công
    return true;
  } catch (error) {
    return false; // Nếu lỗi xảy ra, urlString không phải là URL hợp lệ
  }
}

const UploadVideo = ({
  handleVideoChange,
}: Pick<UseUploadFileResult, "handleVideoChange">) => {
  const [videoSrc, seVideoSrc] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { watch } = useFormContext();
  const videoUrl = watch("videoUrl");

  const handleChange = ({ file }: { file: UploadFile }) => {
    var url = URL.createObjectURL(file.originFileObj!);
    handleVideoChange(file);
    seVideoSrc(url);
  };
  return (
    <>
      <div>
        <Upload
          className="mt-3 mb-3"
          accept=".mp4"
          listType="picture"
          maxCount={1}
          onChange={handleChange}
          defaultFileList={
            videoUrl &&
            isValidUrl(videoUrl) && [
              {
                uid: "1",
                url: videoUrl!, // Đảm bảo rằng tutor và videoUrl không phải undefined
                name: "video.mp4",
                status: "done",
                thumbUrl: mockPost.writer.avatarUrl,
              },
            ]
          }
          itemRender={(_) => (
            <Tooltip title="Ấn vào để xem video" defaultOpen>
              <div
                className="cursor-pointer"
                onClick={() => setIsModalOpen(true)}
              >
                {_}
              </div>
            </Tooltip>
          )}
        >
          <Button type="dashed" icon={<Video />}>
            Tải lên video
          </Button>
        </Upload>
      </div>
      {videoSrc && videoSrc.length > 0 && (
        <Modal
          open={isModalOpen}
          onCancel={() => setIsModalOpen(false)}
          footer={null}
          title="Xem video"
          width={1000}
          getContainer={false}
        >
          <div className="mt-10">
            <video width="100%" height={500} controls src={videoSrc} />
          </div>
        </Modal>
      )}
    </>
  );
};

export default UploadVideo;
