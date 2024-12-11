import React from "react";
import BillsTable from "../../../components/BillsTable";

const page = ({ params }: { params: { id: number } }) => {
  return (
    <div>
      <BillsTable
        tutorLearnerSubjectId={params.id}
        type="learning-classrooms"
      />
    </div>
  );
};

export default page;
