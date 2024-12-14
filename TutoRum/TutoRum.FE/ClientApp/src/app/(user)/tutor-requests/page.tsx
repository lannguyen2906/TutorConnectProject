import React, { Suspense } from "react";
import TutorRequestListPage from "./components/TutorRequestListPage";
import { Spin } from "antd";

const page = () => {
  return (
    <Suspense fallback={<Spin />}>
      <TutorRequestListPage />
    </Suspense>
  );
};

export default page;
