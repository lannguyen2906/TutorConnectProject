import BillPage from "@/app/(user)/user/components/BillPage";
import React from "react";

const page = ({ params }: { params: { id: number; billId: number } }) => {
  return (
    <div>
      <BillPage billId={params.billId} classroomId={params.id} />
    </div>
  );
};

export default page;
