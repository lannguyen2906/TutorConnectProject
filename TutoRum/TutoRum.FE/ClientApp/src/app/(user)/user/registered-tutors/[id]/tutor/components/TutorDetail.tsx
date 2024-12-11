"use client";

import TutorCertificates from "@/app/(user)/tutors/[id]/components/Certificates";
import { mockTutor } from "@/app/(user)/tutors/[id]/components/mockData";
import ProfileDescription from "@/app/(user)/tutors/[id]/components/ProfileDescription";
import ScheduleTable from "@/app/(user)/tutors/[id]/components/ScheduleTable";
import TutorBasicInfo from "@/app/(user)/tutors/[id]/components/TutorBasicInfo";
import TutorFeedbacks from "@/app/(user)/tutors/[id]/components/TutorFeedbacks";
import TutorSubjects from "@/app/(user)/tutors/[id]/components/TutorSubjects";
import { useTutorDetail } from "@/hooks/use-tutor";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import { Button } from "antd";

const TutorDetail = ({
  tutorLearnerSubjectId,
}: {
  tutorLearnerSubjectId: number;
}) => {
  const { data: tutorLearnerSubjectDetail } = useTutorLearnerSubjectDetail(
    tutorLearnerSubjectId
  );

  const { data: tutorDetail, isLoading } = useTutorDetail(
    tutorLearnerSubjectDetail?.tutorId!
  );

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <div className="flex flex-col overflow-hidden gap-10 h-full">
        <TutorBasicInfo
          major={tutorDetail?.major || mockTutor.major}
          briefIntroduction={
            tutorDetail?.briefIntroduction || mockTutor.briefIntroduction
          }
          addressDetail={tutorDetail?.addressDetail || mockTutor.address}
          status={""}
          avatarUrl={tutorDetail?.avatarUrl!}
          fullName={tutorDetail?.fullName || mockTutor.fullName}
          specialization={tutorDetail?.specialization || ""}
          experience={
            tutorDetail?.experience?.toString() || mockTutor.experience
          }
          teachingLocations={tutorDetail?.teachingLocations}
          tutorSubjects={tutorDetail?.tutorSubjects}
          isVerified={tutorDetail?.isVerified}
        />
        <ProfileDescription
          profileDescription={
            tutorDetail?.profileDescription || mockTutor.profileDescription
          }
        />
        <TutorSubjects tutorSubjects={tutorDetail?.tutorSubjects} />
        <TutorFeedbacks tutorId={tutorDetail?.tutorId!} />
        <ScheduleTable schedules={tutorDetail?.schedules || []} />

        <TutorCertificates certificates={tutorDetail?.certificates} />
      </div>
    </div>
  );
};

export default TutorDetail;
