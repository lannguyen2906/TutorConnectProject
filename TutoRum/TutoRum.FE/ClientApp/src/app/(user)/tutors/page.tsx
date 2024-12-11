import React from "react";
import { Skeleton } from "@/components/ui/skeleton";
import SearchBar from "./components/SearchBar";
import TutorList from "./components/TutorList";
import TopTutor from "./components/TopTutor";
import TopSubject from "./components/TopSubject";
import CustomizedBreadcrumb from "../components/Breadcrumb/CustomizedBreadcrumb";
import TutorListPage from "./components/TutorListPage";
const page = () => {
  return (
    <div className="px-8 container mt-5">
      <CustomizedBreadcrumb currentpage="Danh sách gia sư" />
      <div className="text-2xl font-bold border-b-2 w-fit mb-5">
        Danh sách gia sư
      </div>
      <TutorListPage />
    </div>
  );
};

export default page;
