import React from "react";
import TeachingClassroomsTable from "../components/ClassroomsTable";
const page = () => {
  return (
    <div className="container mt-10">
      <TeachingClassroomsTable viewType="viewLearners" />
    </div>
  );
};

export default page;
