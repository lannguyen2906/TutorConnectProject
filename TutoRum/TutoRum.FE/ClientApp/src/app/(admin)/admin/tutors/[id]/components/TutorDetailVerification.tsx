"use client";

import { ContentLayout } from "@/app/(admin)/components/content-layout";
import ProfileDescription from "@/app/(user)/tutors/[id]/components/ProfileDescription";
import TutorBasicInfo from "@/app/(user)/tutors/[id]/components/TutorBasicInfo";
import { useTutorDetail } from "@/hooks/use-tutor";
import React from "react";
import TutorSubjects from "@/app/(user)/tutors/[id]/components/TutorSubjects";
import { TutorSubjectDto } from "@/utils/services/Api";
import TutorCertificates from "@/app/(user)/tutors/[id]/components/Certificates";
import { Card } from "antd";
import { VerifyButton } from "./verify-buttons";
import { mockTutor } from "@/app/(user)/tutors/[id]/components/mockData";
import { CheckCircleOutlined } from "@ant-design/icons"; // Ant Design icons

const TutorDetailVerification = ({ id }: { id: string }) => {
  const { data: tutorDetail, isLoading } = useTutorDetail(id);
  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <ContentLayout title={tutorDetail?.fullName || ""}>
      <div className="flex gap-10 overflow-visible h-full">
        <Card>
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
          <TutorSubjects
            tutorSubjects={tutorDetail?.tutorSubjects}
            tutorId={id}
          />

          <TutorCertificates
            certificates={tutorDetail?.certificates}
            tutorId={id}
          />
        </Card>
        <div className="sticky top-1/2 h-fit">
          <Card title="Xác thực" extra={<CheckCircleOutlined />}>
            <VerifyButton
              entityType={0}
              guid={tutorDetail?.tutorId || null}
              id={null}
              isVerified={tutorDetail?.isVerified ?? null}
            />
          </Card>
        </div>
      </div>
    </ContentLayout>
  );
};

export default TutorDetailVerification;
