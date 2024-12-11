"use client";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import { Button, Empty, Image, Tag } from "antd";
import React, { useState } from "react";
import UploadContracts from "./UploadContracts";
import useListFiles from "@/hooks/use-list-files";
import { FileText } from "lucide-react";
import { useAppContext } from "@/components/provider/app-provider";

const ContractPage = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const { data, isLoading } = useTutorLearnerSubjectDetail(
    tutorLearnerSubjectId
  );
  const { fileUrls } = useListFiles(data?.contractUrl + "/files");
  const [upload, setUpload] = useState(false);
  const { user } = useAppContext();
  if (isLoading) {
    return <div>Loading...</div>;
  }

  const isTutor = user?.id == data?.tutorId;

  return (
    <div>
      {data?.contractUrl ? (
        <div className="space-y-10">
          <div className="flex justify-between">
            <h1 className="text-3xl font-bold mb-4 flex items-center gap-4">
              Hợp đồng
              {data?.isContractVerified == null ? (
                <Tag color="orange">Đang chờ xác thực</Tag>
              ) : data?.isContractVerified ? (
                <Tag color="green">Đã xác thực</Tag>
              ) : (
                <Tag color="red">Bị từ chối</Tag>
              )}
            </h1>
            {isTutor && (
              <Button
                onClick={() => setUpload(!upload)}
                type="primary"
                icon={<FileText size={16} />}
                disabled={data?.isClosed === true}
              >
                Gửi lại hợp đồng
              </Button>
            )}
          </div>
          {upload && (
            <UploadContracts tutorLearnerSubjectId={tutorLearnerSubjectId} />
          )}
          <div className="flex gap-5">
            {fileUrls.map((url) => (
              <div key={url}>
                <Image src={url} alt="file" />
              </div>
            ))}
          </div>
        </div>
      ) : (
        <>
          {isTutor ? (
            <UploadContracts tutorLearnerSubjectId={tutorLearnerSubjectId} />
          ) : (
            <Empty description="Chưa có hợp đồng, vui lòng thông báo gia sư gửi hợp đồng"></Empty>
          )}
        </>
      )}
    </div>
  );
};

export default ContractPage;
