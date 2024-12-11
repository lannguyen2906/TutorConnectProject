import { ILink } from "@/app/(user)/user/settings/settingLinkList";
import { BookOpen, ChartCandlestick } from "lucide-react";

export const settingList: ILink[] = [
  {
    key: "1",
    label: "Khoảng giá",
    icon: <ChartCandlestick size={16} />,
    href: "/admin/settings/rate-ranges",
    roles: ["admin"],
  },
  {
    key: "2",
    label: "Môn học",
    icon: <BookOpen size={16} />,
    href: "/admin/settings/subjects",
    roles: ["admin"],
  },
];
