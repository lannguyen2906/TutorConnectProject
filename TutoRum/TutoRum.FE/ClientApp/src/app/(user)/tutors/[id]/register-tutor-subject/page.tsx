import CustomizedBreadcrumb from "@/app/(user)/components/Breadcrumb/CustomizedBreadcrumb";
import React from "react";
import RegisterTutorSubjectForm from "./components/RegisterTutorSubjectForm";

const page = ({ params }: { params: { id: string } }) => {
  return (
    <div className="container mt-5">
      <div className="text-2xl font-bold mb-10 mt-10 border-b-2 w-fit">
        Đăng ký môn học
      </div>
      <RegisterTutorSubjectForm id={params.id} />
    </div>
  );
};

export default page;
