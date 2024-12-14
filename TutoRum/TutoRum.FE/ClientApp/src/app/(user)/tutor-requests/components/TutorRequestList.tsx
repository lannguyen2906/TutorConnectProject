import React from "react";
import TutorRequestListCard from "./TutorRequestListCard";
import { TutorRequestDTO } from "@/utils/services/Api";
import { Empty } from "antd";

const TutorRequestList = ({ data }: { data: TutorRequestDTO[] }) => {
  if (data.length == 0) {
    return <Empty description="Hiện tại không có yêu cầu nào" />;
  }

  return (
    <div className="flex flex-col gap-5">
      {data?.map((tutorRequest) => (
        <TutorRequestListCard
          key={tutorRequest.id}
          tutorRequest={tutorRequest}
        />
      ))}
    </div>
  );
};

export default TutorRequestList;
