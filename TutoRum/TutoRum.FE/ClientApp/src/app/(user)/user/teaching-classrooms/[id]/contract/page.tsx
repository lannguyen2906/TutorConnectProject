import React from "react";
import ContractPage from "../../../components/ContractPage";

const page = ({ params }: { params: { id: number } }) => {
  return (
    <div>
      <ContractPage tutorLearnerSubjectId={params.id} />
    </div>
  );
};

export default page;
