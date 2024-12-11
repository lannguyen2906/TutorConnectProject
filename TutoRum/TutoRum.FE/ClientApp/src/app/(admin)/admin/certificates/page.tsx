import React from "react";
import { ContentLayout } from "../../components/content-layout";
import CertificatesTable from "./components/CertificatesTable";

const page = () => {
  return (
    <ContentLayout title="Danh sách chứng chỉ">
      <CertificatesTable />
    </ContentLayout>
  );
};

export default page;
