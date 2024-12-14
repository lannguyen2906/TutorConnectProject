import React from "react";
import CustomizedBreadcrumb from "../components/Breadcrumb/CustomizedBreadcrumb";
import { useAppContext } from "@/components/provider/app-provider";
import TutorProfileForm from "../user/settings/user-profile/components/TutorProfileForm";

const TutorRegister = () => {
  return (
    <div className="container mt-5">
      <CustomizedBreadcrumb currentpage="Đăng ký làm gia sư" />
      <div className="text-2xl font-bold border-b-2 w-fit mb-5">
        Đăng ký làm gia sư
      </div>
      <TutorProfileForm />
    </div>
  );
};

export default TutorRegister;
