import React from "react";
import TutorProfileForm from "../../../settings/user-profile/components/TutorProfileForm";
import TutorDetail from "./components/TutorDetail";

const page = ({ params }: { params: { id: number } }) => {
  return (
    <div>
      <TutorDetail tutorLearnerSubjectId={params.id} />
    </div>
  );
};

export default page;
