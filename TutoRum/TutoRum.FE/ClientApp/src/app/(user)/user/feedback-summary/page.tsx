import React from "react";
import SummaryFeedbackPage from "./components/SummaryFeedbackPage";

const page = () => {
  return (
    <div className="container mt-10">
      <div className="text-2xl font-bold border-b-2 w-fit mb-5">
        Tổng hợp feedback
      </div>
      <SummaryFeedbackPage />
    </div>
  );
};

export default page;
