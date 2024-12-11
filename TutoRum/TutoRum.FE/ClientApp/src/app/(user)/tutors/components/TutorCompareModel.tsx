import { useTutorDetail } from "@/hooks/use-tutor";
import { Avatar, Card } from "antd";
import React from "react";

const TutorCompareModel = ({ params }: { params: { tutorId: string } }) => {
  const { data, isLoading } = useTutorDetail(params.tutorId);
  if (isLoading) {
    return <div>Loading...</div>;
  }
  return (
    <Card className="mb-10 w-full">
      <div className="flex items-center">
        <Avatar src={data?.avatarUrl || ""} size={64} />
        <div className="ml-4">
          <h3>{data?.fullName}</h3>
        </div>
      </div>
    </Card>
  );
};

export default TutorCompareModel;
