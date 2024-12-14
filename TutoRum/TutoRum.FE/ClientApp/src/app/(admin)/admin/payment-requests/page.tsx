import React from "react";
import { ContentLayout } from "../../components/content-layout";
import PaymentRequestsTableAdmin from "./components/PaymentRequestsTableAdmin";

const page = () => {
  return (
    <ContentLayout title="Danh sách yêu cầu rút tiền">
      <PaymentRequestsTableAdmin />
    </ContentLayout>
  );
};

export default page;
