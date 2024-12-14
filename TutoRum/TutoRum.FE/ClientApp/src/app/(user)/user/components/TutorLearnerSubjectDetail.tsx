"use client";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import React, { use } from "react";
import RegisterTutorSubjectForm from "../../tutors/[id]/register-tutor-subject/components/RegisterTutorSubjectForm";
import TutorLearnerSubjectDetailForm from "../../tutors/[id]/register-tutor-subject/components/TutorLearnerSubjectDetailForm";
import { Skeleton } from "antd";
import { usePathname } from "next/navigation";

const TutorLearnerSubjectDetail = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const { data: tutorLearnerSubjectDetail, isLoading } =
    useTutorLearnerSubjectDetail(tutorLearnerSubjectId);
  const pathname = usePathname();

  if (isLoading) {
    return <Skeleton />;
  }

  return (
    <div>
      <TutorLearnerSubjectDetailForm
        tutorLearnerSubjectDetail={tutorLearnerSubjectDetail}
        showButton={
          !(
            pathname.startsWith("/user/registered-tutors") ||
            pathname.startsWith("/user/registered-learners")
          )
        }
      />
    </div>
  );
};

export default TutorLearnerSubjectDetail;
