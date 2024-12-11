import React from "react";
import { ContentLayout } from "../../components/content-layout";
import TutorRequestsTable from "./components/TutorRequestsTable";

const page = () => {
  return (
    <ContentLayout title="Danh sách yêu cầu">
      <TutorRequestsTable />
    </ContentLayout>
  );
};

export default page;
