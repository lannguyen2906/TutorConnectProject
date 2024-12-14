import {
  Tag,
  Users,
  Settings,
  Bookmark,
  SquarePen,
  LayoutGrid,
  LucideIcon,
  CircleHelp,
  GraduationCap,
  BookOpenText,
  UserSearch,
  Receipt,
  CalendarCheck,
  FileText,
  DollarSign,
  ChartCandlestick,
} from "lucide-react";

type Submenu = {
  href: string;
  label: string;
  active?: boolean;
};

type Menu = {
  href: string;
  label: string;
  active?: boolean;
  icon: LucideIcon;
  count?: number;
  roles?: string[];
  submenus?: Submenu[];
};

type Group = {
  groupLabel: string;
  menus: Menu[];
};

export function getMenuList(pathname: string): Group[] {
  return [
    {
      groupLabel: "",
      menus: [
        {
          href: "/admin",
          label: "Dashboard",
          icon: LayoutGrid,
          roles: ["admin"],
          submenus: [],
        },
      ],
    },
    {
      groupLabel: "Người dùng",
      menus: [
        {
          href: "/admin/accounts",
          label: "Tài khoản",
          icon: Users,
        },
        {
          href: "/admin/tutors",
          label: "Gia sư",
          icon: GraduationCap,
          count: 1,
        },
        {
          href: "/admin/payment-requests",
          label: "Yêu cầu rút tiền",
          icon: DollarSign,
        },
      ],
    },
    {
      groupLabel: "Lớp học",
      menus: [
        {
          href: "/admin/tutor-requests",
          label: "Yêu cầu gia sư",
          icon: UserSearch,
        },
        {
          href: "/admin/contracts",
          label: "Hợp đồng",
          icon: FileText,
        },
      ],
    },
    {
      groupLabel: "Khác",
      menus: [
        {
          href: "/admin/posts",
          label: "Bài viết",
          icon: SquarePen,
        },
        {
          href: "/admin/categories",
          label: "Thể loại",
          icon: Bookmark,
        },
        {
          href: "/admin/faqs",
          label: "Trợ giúp",
          icon: CircleHelp,
        },
      ],
    },
    {
      groupLabel: "Cài đặt",
      menus: [
        {
          href: "/admin/settings",
          label: "Cài đặt hệ thống",
          icon: Settings,
          roles: ["admin"],
        },
      ],
    },
  ];
}
