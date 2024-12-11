import React from "react";
import AccordionFaqs from "./components/AccordionFaqs";
import CustomizedBreadcrumb from "../components/Breadcrumb/CustomizedBreadcrumb";
import { AddFaqButton } from "./components/AddFaqButton";

const page = () => {
  return (
    <div className="container mt-5 flex flex-col gap-5 relative">
      <CustomizedBreadcrumb currentpage="Câu hỏi thường gặp" />
      <h1 className="text-6xl font-extrabold text-center">
        Câu hỏi thường gặp
      </h1>
      <div className="text-slategray text-center">
        Bạn cần giúp gì vậy? Cứ để{" "}
        <span className="font-bold">TutorConnect</span> giúp bạn
      </div>
      <AccordionFaqs />
      <div className="absolute top-10 right-10">
        <AddFaqButton />
      </div>
    </div>
  );
};

export default page;
