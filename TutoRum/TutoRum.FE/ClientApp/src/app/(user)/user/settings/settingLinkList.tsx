import {
  Bell,
  BookOpen,
  BookOpenText,
  Calendar,
  DollarSign,
  FileText,
  GraduationCap,
  Info,
  LayoutGrid,
  Lock,
  LogOut,
  MessageSquare,
  Settings,
  ThumbsUp,
  User,
  Wallet,
} from "lucide-react";
import {
  FaChalkboardTeacher,
  FaClipboardList,
  FaEnvelopeOpenText,
  FaUserFriends,
} from "react-icons/fa";
export interface ILink {
  key: string;
  label: string;
  icon?: JSX.Element;
  href: string;
  roles: string[];
  children?: ILink[];
}

// Danh sách các tùy chọn cài đặt
export const settingList: ILink[] = [
  {
    key: "1",
    label: "Thông tin cá nhân",
    icon: <User size={16} />,
    href: "/user/settings/user-profile",
    roles: ["admin", "learner", "tutor"],
  },
  {
    key: "2",
    label: "Thông báo", // Thay đổi nhãn để phân biệt
    icon: <Bell size={16} />,
    href: "/user/settings/notifications",
    roles: ["admin", "learner", "tutor"],
  },
  {
    key: "3",
    label: "Đổi mật khẩu", // Thay đổi nhãn để phân biệt
    icon: <Lock size={16} />,
    href: "/user/settings/password",
    roles: ["admin", "learner", "tutor"],
  },
  {
    key: "4",
    label: "Ví của tôi", // Thay đổi nhãn để phân biệt
    icon: <Wallet size={16} />,
    href: "/user/settings/wallet",
    roles: ["tutor"],
  },
];

export const classroomLinkList = (id: number, type: string) => [
  {
    href: `/user/${type}/${id}`,
    key: "1",
    label: "Thông tin chi tiết",
    icon: <Info size={16} />,
    roles: ["learner", "tutor", "admin"],
  },
  {
    href: `/user/${type}/${id}/billing-entries`,
    key: "2",
    label: "Buổi học",
    icon: <Calendar size={16} />,
    roles: ["learner", "tutor", "admin"],
  },
  {
    href: `/user/${type}/${id}/bills`,
    key: "4",
    label: "Hóa đơn",
    icon: <DollarSign size={16} />,
    roles: ["learner", "tutor"],
  },
  {
    href: `/user/${type}/${id}/contract`,
    key: "3",
    label: "Hợp đồng",
    icon: <FileText size={16} />,
    roles: ["learner", "tutor", "admin"],
  },
  {
    href: `/user/${type}/${id}/feedback`,
    key: "5",
    label: "Nhận xét",
    icon: <MessageSquare size={16} />,
    roles: ["learner"],
  },
  {
    href: `/user/${type}/${id}/close-classroom`,
    key: "5",
    label: "Kết thúc lớp",
    icon: <LogOut size={16} />,
    roles: ["tutor"],
  },
];

export const userLinkList = (size: number, hide: string[]): ILink[] =>
  [
    {
      key: "dashboard",
      label: "Dashboard",
      href: "/admin",
      roles: ["admin"],
      icon: <LayoutGrid size={size} />, // FA icon cho Dashboard
    },
    {
      key: "user-profile",
      label: "Thông tin cá nhân",
      href: "/user/settings/user-profile",
      roles: ["admin", "learner", "tutor"],
      icon: <User size={size} />,
    },
    {
      key: "tutor-info",
      label: "Thông tin gia sư",
      href: "/user/tutor-profile",
      roles: ["tutor"],
      icon: <FaChalkboardTeacher size={size} />, // FA icon cho gia sư
    },
    {
      key: "feedback-summary",
      label: "Đánh giá",
      href: "/user/feedback-summary",
      roles: ["tutor"],
      icon: <ThumbsUp size={size} />, // FA icon cho gia sư
    },
    {
      key: "my-schedule",
      label: "Lịch của tôi",
      href: "/user/my-schedule",
      roles: ["tutor"],
      icon: <Calendar size={size} />, // FA icon cho gia sư
    },
    {
      key: "wallet",
      label: "Ví của tôi",
      href: "/user/settings/wallet",
      roles: ["tutor"],
      icon: <Wallet size={size} />, // FA icon cho gia sư
    },
    {
      key: "teaching-classes",
      label: "Lớp đang dạy",
      href: "/user/teaching-classrooms",
      roles: ["tutor"],
      icon: <BookOpen size={size} />, // Lucide icon cho lớp học
    },
    {
      key: "learning-classes",
      label: "Lớp đang học",
      href: "/user/learning-classrooms",
      roles: ["learner"],
      icon: <BookOpenText size={size} />,
    },
    {
      key: "created-requests",
      label: "Yêu cầu đã tạo",
      href: "/user/tutor-requests",
      roles: ["learner"],
      icon: <FaClipboardList size={size} />, // FA icon cho yêu cầu
    },
    {
      key: "received-requests",
      label: "Yêu cầu đã nhận",
      href: "/user/registered-tutor-requests",
      roles: ["tutor"],
      icon: <FaEnvelopeOpenText size={size} />, // FA icon cho yêu cầu nhận
    },
    {
      key: "learner-connections",
      label: "Học viên đăng ký",
      href: "/user/registered-learners",
      roles: ["tutor"],
      icon: <FaUserFriends size={size} />, // FA icon cho kết nối học viên
    },
    {
      key: "tutor-connections",
      label: "Gia sư đăng ký",
      href: "/user/registered-tutors",
      roles: ["learner"],
      icon: <GraduationCap size={size} />,
    },
    {
      key: "settings",
      label: "Cài đặt",
      href: "/user/settings",
      roles: ["admin", "learner", "tutor"],
      icon: <Settings size={size} />, // Lucide icon cho cài đặt
    },
  ].filter((link) => !hide.includes(link.key));

export const userDropdownLinkList = (size: number, hide: string[]): ILink[] =>
  [
    {
      key: "dashboard",
      label: "Dashboard",
      href: "/admin",
      roles: ["admin"],
      icon: <LayoutGrid size={size} />,
    },
    {
      key: "user-profile",
      label: "Thông tin cá nhân",
      href: "/user/settings/user-profile",
      roles: ["admin", "learner", "tutor"],
      icon: <User size={size} />,
    },
    {
      key: "tutor",
      label: "Gia sư",
      href: "",
      roles: ["tutor"],
      icon: <FaChalkboardTeacher size={size} />,
      children: [
        {
          key: "tutor-info",
          label: "Thông tin gia sư",
          href: "/user/tutor-profile",
          roles: ["tutor"],
          icon: <FaChalkboardTeacher size={size} />,
        },
        {
          key: "feedback-summary",
          label: "Đánh giá",
          href: "/user/feedback-summary",
          roles: ["tutor"],
          icon: <ThumbsUp size={size} />,
        },
        {
          key: "my-schedule",
          label: "Lịch của tôi",
          href: "/user/my-schedule",
          roles: ["tutor"],
          icon: <Calendar size={size} />,
        },
        {
          key: "wallet",
          label: "Ví của tôi",
          href: "/user/settings/wallet",
          roles: ["tutor"],
          icon: <Wallet size={size} />,
        },
        {
          key: "teaching-classes",
          label: "Lớp đang dạy",
          href: "/user/teaching-classrooms",
          roles: ["tutor"],
          icon: <BookOpen size={size} />,
        },
        {
          key: "received-requests",
          label: "Yêu cầu đã nhận",
          href: "/user/registered-tutor-requests",
          roles: ["tutor"],
          icon: <FaEnvelopeOpenText size={size} />,
        },
        {
          key: "learner-connections",
          label: "Học viên đăng ký",
          href: "/user/registered-learners",
          roles: ["tutor"],
          icon: <FaUserFriends size={size} />,
        },
      ],
    },
    {
      key: "learner",
      label: "Học viên",
      href: "",
      roles: ["learner"],
      icon: <GraduationCap size={size} />,
      children: [
        {
          key: "learning-classes",
          label: "Lớp đang học",
          href: "/user/learning-classrooms",
          roles: ["learner"],
          icon: <BookOpenText size={size} />,
        },
        {
          key: "created-requests",
          label: "Yêu cầu đã tạo",
          href: "/user/tutor-requests",
          roles: ["learner"],
          icon: <FaClipboardList size={size} />,
        },
        {
          key: "tutor-connections",
          label: "Gia sư đăng ký",
          href: "/user/registered-tutors",
          roles: ["learner"],
          icon: <GraduationCap size={size} />,
        },
      ],
    },
    {
      key: "settings",
      label: "Cài đặt",
      href: "/user/settings",
      roles: ["admin", "learner", "tutor"],
      icon: <Settings size={size} />,
    },
  ].filter((link) => !hide.includes(link.key));
