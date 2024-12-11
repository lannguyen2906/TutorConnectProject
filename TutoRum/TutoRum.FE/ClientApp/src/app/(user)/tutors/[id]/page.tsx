"use client";

import CustomizedBreadcrumb from "../../components/Breadcrumb/CustomizedBreadcrumb";
import TutorCertificates from "./components/Certificates";
import { mockTutor } from "./components/mockData";
import ProfileDescription from "./components/ProfileDescription";
import ScheduleTable from "./components/ScheduleTable";
import SuggestedTutors from "./components/SuggestedTutors";
import TutorBasicInfo from "./components/TutorBasicInfo";
import TutorFeedbacks from "./components/TutorFeedbacks";
import TutorSubjects from "./components/TutorSubjects";
import RelatedSearch from "./components/RelatedSearch";
import FixedTutor from "./components/FixedTutor";
import { useTutorDetail, useTutorHomePage } from "@/hooks/use-tutor";
import { Button } from "antd";

const TutorPage = ({ params }: { params: { id: string } }) => {
  const { data: tutorDetail, isLoading } = useTutorDetail(params.id);
  const { data: tutors } = useTutorHomePage(
    {
      subjects:
        tutorDetail?.tutorSubjects?.map((ts) => ts.subject?.subjectId ?? 0) ||
        [],
    },
    1,
    5
  );
  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="mt-5 container">
      <CustomizedBreadcrumb
        currentpage={tutorDetail?.fullName || mockTutor.fullName}
      />
      <div className="flex flex-col 2xl:flex-row overflow-hidden 2xl:overflow-visible gap-24 h-full">
        <div className="w-full 2xl:w-2/3">
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
          {/* <TutorSubjects tutorSubjects={tutorSubjects} /> */}
          <TutorFeedbacks tutorId={tutorDetail?.tutorId || ""} />
          <ScheduleTable schedules={tutorDetail?.schedules || []} />

          <TutorCertificates certificates={tutorDetail?.certificates} />
          <SuggestedTutors suggestedTutors={tutors} />
          <RelatedSearch tutor={mockTutor} />
        </div>
        <div className="w-full 2xl:w-1/3 sticky top-40 right-0 hidden 2xl:block h-fit">
          <FixedTutor tutor={tutorDetail!} />
        </div>
        <div className="w-full fixed bottom-0 left-0 right-0 block 2xl:hidden p-5">
          <a
            href={`/tutors/${params.id}/register-tutor-subject`}
            target="_blank"
          >
            <Button size="large" type="primary" className="w-full">
              Đăng ký ngay
            </Button>
          </a>
        </div>
      </div>
    </div>
  );
};

export default TutorPage;
