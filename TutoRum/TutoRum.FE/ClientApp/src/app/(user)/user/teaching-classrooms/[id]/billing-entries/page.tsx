import React from "react";
import BillingEntriesTable from "../../../components/BillingEntriesTable";

const page = ({ params }: { params: { id: number } }) => {
  return (
    <div>
      <BillingEntriesTable tutorLearnerSubjectId={params.id} />
    </div>
  );
};

export default page;
