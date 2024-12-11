import React from "react";
import { ISubject } from "./mockData";

const TutorSubject = ({
  subject,
  disabled = true,
}: {
  subject: ISubject;
  disabled?: boolean;
}) => {
  const { subjectId, subjectName } = subject;

  return (
    <button
      className={`flex items-center justify-center gap-5 rounded-lg p-2 transition-all ${
        disabled ? "hover:bg-none" : "hover:bg-semiblueviolet"
      }`}
      disabled={disabled} // Thêm thuộc tính disabled
      key={subjectId}
    >
      <span>{subjectName}</span>
    </button>
  );
};

export default TutorSubject;
