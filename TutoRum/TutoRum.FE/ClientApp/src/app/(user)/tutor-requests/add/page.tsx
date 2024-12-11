import React from "react";
import TutorRequestForm from "../components/TutorRequestForm";
import CustomizedBreadcrumb from "../../components/Breadcrumb/CustomizedBreadcrumb";

const page = () => {
  return (
    <div className="container mt-5">
      <CustomizedBreadcrumb currentpage="Tạo yêu cầu mới" />
      <TutorRequestForm />
    </div>
  );
};

export default page;
