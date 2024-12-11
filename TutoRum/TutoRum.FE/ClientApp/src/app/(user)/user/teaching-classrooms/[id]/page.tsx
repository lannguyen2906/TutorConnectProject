import React from "react";
import TutorLearnerSubjectDetail from "../../components/TutorLearnerSubjectDetail";

const page = ({ params }: { params: { id: number } }) => {
  return (
    <div className="container mt-10">
      <TutorLearnerSubjectDetail tutorLearnerSubjectId={params.id} />
    </div>
  );
};

export default page;
