import React from "react";
import CloseClassButton from "./CloseClassButton";

const page = ({ params }: { params: { id: number } }) => {
  return (
    <div>
      <CloseClassButton id={params.id} />
    </div>
  );
};

export default page;
