"use client";
import LinksAnchor from "../../settings/user-profile/components/LinksAnchor";
import CustomizedBreadcrumb from "@/app/(user)/components/Breadcrumb/CustomizedBreadcrumb";
import { ClassroomProvider } from "@/components/provider/classroom-provider";
import { classroomLinkList, ILink } from "../../settings/settingLinkList";
import ClassroomHighlights from "../../components/ClassroomHighlights";

const layout = ({
  children,
  params,
}: {
  children: React.ReactNode;
  params: { id: number };
}) => {
  return (
    <div className="container mt-10">
      <CustomizedBreadcrumb />
      <ClassroomHighlights tutorLearnerSubjectId={params.id} />
      <div className="flex lg:gap-10 mt-5">
        <div className="lg:w-1/6">
          <LinksAnchor
            linkList={classroomLinkList(params.id, "teaching-classrooms")}
          />
        </div>
        <div className="w-full lg:w-5/6">{children}</div>
      </div>
    </div>
  );
};

export default layout;
