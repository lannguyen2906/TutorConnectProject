import React, { Suspense } from "react";
import CustomizedBreadcrumb from "../components/Breadcrumb/CustomizedBreadcrumb";
import TutorComparePage from "./components/TutorComparePage";
const page = () => {
  return (
    <div className="container mt-5">
      <CustomizedBreadcrumb currentpage="So sánh gia sư" />

      <Suspense fallback={<div>Loading...</div>}>
        <TutorComparePage />
      </Suspense>
    </div>
  );
};

export default page;
