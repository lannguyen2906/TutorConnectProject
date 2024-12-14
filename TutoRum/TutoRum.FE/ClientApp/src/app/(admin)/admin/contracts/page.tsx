import React from "react";
import { ContentLayout } from "../../components/content-layout";
import ContractsTable from "./components/ContractsTable";

const page = () => {
  return (
    <ContentLayout title="Danh sách hợp đồng">
      <ContractsTable />
    </ContentLayout>
  );
};

export default page;
