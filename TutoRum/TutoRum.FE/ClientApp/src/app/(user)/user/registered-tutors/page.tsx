import React from "react";
import TutorLearnerSubjectTable from "../components/TutorLearnerSubjectTable";

const page = () => {
  return (
    <div className="container mt-10">
      <TutorLearnerSubjectTable viewType="viewTutors" />
    </div>
  );
};

export default page;
