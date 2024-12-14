import React from "react";
import TutorFeedbackForm from "../../../components/TutorFeedbackForm";

const page = ({ params }: { params: { id: number } }) => {
  return (
    <div>
      <TutorFeedbackForm tutorLearnerSubjectId={params.id} />
    </div>
  );
};

export default page;
