"use client";
import React from "react";
import { FloatButton } from "antd";
import { Menu } from "lucide-react";
import { ILink } from "../settings/settingLinkList";
import { usePathname, useRouter } from "next/navigation";

const FloatButtonMenu = ({ linkList }: { linkList: ILink[] }) => {
  const pathName = usePathname();
  const router = useRouter();
  return (
    <div className="block lg:hidden">
      <FloatButton.Group
        trigger="click"
        type="primary"
        style={{ insetInlineEnd: 24 }}
        icon={<Menu size={16} />}
        placement="top"
      >
        {linkList.map(({ key, label, icon, href }) => {
          return (
            <FloatButton
              key={key}
              icon={icon}
              onClick={(e) => {
                e.preventDefault();
                router.push(href);
              }}
              type={pathName == `${href}/` ? "primary" : "default"}
            >
              {label}
            </FloatButton>
          );
        })}
      </FloatButton.Group>
    </div>
  );
};

export default FloatButtonMenu;
