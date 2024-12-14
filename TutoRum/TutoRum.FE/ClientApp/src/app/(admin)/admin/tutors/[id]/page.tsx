import TutorDetail from "@/app/(user)/user/tutor-profile/components/TutorDetail";
import React from "react";
import TutorDetailVerification from "./components/TutorDetailVerification";

const page = ({ params }: { params: { id: string } }) => {
  return <TutorDetailVerification id={params.id} />;
};

export default page;
