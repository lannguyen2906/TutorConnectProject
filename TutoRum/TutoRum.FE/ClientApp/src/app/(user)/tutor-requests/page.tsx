import React, { Suspense } from "react";
import TutorRequestListPage from "./components/TutorRequestListPage";

const page = () => {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <TutorRequestListPage />
    </Suspense>
  );
};

export default page;
