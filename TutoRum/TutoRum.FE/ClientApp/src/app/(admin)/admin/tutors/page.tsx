import React from "react";
import { ContentLayout } from "../../components/content-layout";
import TutorsTable from "./components/TutorsTable";

const page = () => {
  return (
    <ContentLayout title="Danh sách gia sư">
      <TutorsTable />
    </ContentLayout>
  );
};

export default page;
