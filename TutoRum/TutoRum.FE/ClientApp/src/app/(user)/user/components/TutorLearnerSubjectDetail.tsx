"use client";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import React, { use } from "react";
import RegisterTutorSubjectForm from "../../tutors/[id]/register-tutor-subject/components/RegisterTutorSubjectForm";
import TutorLearnerSubjectDetailForm from "../../tutors/[id]/register-tutor-subject/components/TutorLearnerSubjectDetailForm";

const TutorLearnerSubjectDetail = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const { data: tutorLearnerSubjectDetail, isLoading } =
    useTutorLearnerSubjectDetail(tutorLearnerSubjectId);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <TutorLearnerSubjectDetailForm
        tutorLearnerSubjectDetail={tutorLearnerSubjectDetail}
      />
    </div>
  );
};

export default TutorLearnerSubjectDetail;
