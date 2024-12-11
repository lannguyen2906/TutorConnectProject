import { BookOpen, Info, LayoutGrid, Settings } from "lucide-react";
import {
  FaChalkboardTeacher,
  FaClipboardList,
  FaEnvelopeOpenText,
  FaUserFriends,
} from "react-icons/fa";
import { ILink } from "../../settings/settingLinkList";
import LinksAnchor from "../../settings/user-profile/components/LinksAnchor";
import CustomizedBreadcrumb from "@/app/(user)/components/Breadcrumb/CustomizedBreadcrumb";

const layout = ({
  children,
  params,
}: {
  children: React.ReactNode;
  params: { id: string };
}) => {
  const settingList: ILink[] = [
    {
      key: "tutorLearnerSubjectDetail",
      label: "Thông tin chi tiết",
      href: `/user/registered-tutors/${params.id}`,
      roles: ["tutor", "learner"],
      icon: <Info size={16} />, // FA icon cho Dashboard
    },
    {
      key: "tutor-info",
      label: "Thông tin gia sư",
      href: `/user/registered-tutors/${params.id}/tutor`,
      roles: ["learner"],
      icon: <FaChalkboardTeacher size={16} />, // FA icon cho gia sư
    },
  ];
  return (
    <div className="container mt-10">
      <CustomizedBreadcrumb />
      <div className="flex lg:gap-10 mt-5">
        <div className="lg:w-1/6">
          <LinksAnchor linkList={settingList} />
        </div>
        <div className="w-full lg:w-5/6">{children}</div>
      </div>
    </div>
  );
};

export default layout;
