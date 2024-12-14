"use client";
import { Card, Empty, Tag } from "antd";
import { ITutor } from "./mockData";
import { ArrowRight, BookOpen, VerifiedIcon } from "lucide-react";
import PriceTag from "./PriceTag";
import { useAppContext } from "@/components/provider/app-provider";
import { VerifyButton } from "@/app/(admin)/admin/tutors/[id]/components/verify-buttons";
import { usePathname } from "next/navigation";
import { TutorDto, TutorSubjectDto } from "@/utils/services/Api";
import { cn } from "@/lib/utils";

type TutorBasicInfoProps = {
  tutorSubjects: TutorSubjectDto[];
  tutorId: string;
};

const TutorSubjects = ({ tutorSubjects, tutorId }: TutorBasicInfoProps) => {
  const { user } = useAppContext();
  const pathName = usePathname();

  const isAdmin =
    user?.roles &&
    Array.isArray(user?.roles) &&
    user?.roles.includes("admin") &&
    pathName.startsWith("/admin");

  const displayedSubject = isAdmin
    ? tutorSubjects
    : tutorSubjects?.filter((s) => s.isVerified);

  if (displayedSubject?.length == 0 && !isAdmin) {
    return <></>;
  }

  return (
    <div className="my-8 w-full">
      <div className="text-2xl font-bold mb-5 mt-10 border-b-2">
        Các môn học tôi đang dạy
      </div>
      <div className="space-y-5 w-full">
        <div className="flex flex-wrap gap-5">
          {displayedSubject?.map((s) => (
            <Card
              key={s.tutorSubjectId}
              hoverable
              className="shadow-md flex-1 min-w-[250px]"
              title={
                <span className="text-Blueviolet flex gap-2 items-center">
                  {s.subject?.subjectName}{" "}
                  {s.isVerified ? <VerifiedIcon size={16} /> : ""}
                </span>
              }
              extra={<PriceTag rate={s.rate!} />}
              actions={[
                <div
                  className="text-midnightblue"
                  key={`${s.tutorSubjectId}-action`}
                >
                  {isAdmin ? (
                    <VerifyButton
                      entityType={1}
                      guid={tutorId ?? null}
                      id={s.tutorSubjectId!}
                      isVerified={s?.isVerified ?? null}
                    />
                  ) : (
                    <span>
                      {s.numberOfClass && s.numberOfClass > 0 ? (
                        <>
                          Đã dạy{" "}
                          <p className="font-semibold inline">
                            {s.numberOfClass}
                          </p>{" "}
                          học viên
                        </>
                      ) : (
                        "Chưa có học viên"
                      )}
                    </span>
                  )}
                </div>,
              ]}
              styles={{
                body: {
                  height: "100px",
                  overflow: "hidden",
                },
              }}
            >
              <p className="mb-2">
                {s.description && s.description.length > 0
                  ? s.description
                  : `Dạy ${s.subject?.subjectName} cơ bản`}
              </p>
            </Card>
          ))}
        </div>
      </div>
    </div>
  );
};

export default TutorSubjects;
