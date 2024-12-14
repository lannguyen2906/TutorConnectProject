"use client";
import { useAppContext } from "@/components/provider/app-provider";
import { cn } from "@/lib/utils";
import { Bell, Contact, Lock, Settings, Trash, User } from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import React from "react";
import { ILink } from "../../settingLinkList";
import FloatButtonMenu from "../../../components/FloatButtonMenu";

const LinksAnchor = ({ linkList }: { linkList: ILink[] }) => {
  const path = usePathname();
  const { user } = useAppContext();

  return (
    <>
      <div className="block lg:hidden">
        <FloatButtonMenu linkList={linkList} />
      </div>
      <div className="hidden lg:block">
        {/* Tạo khoảng cách giữa các link */}
        {linkList.map(({ key, label, icon, href, roles }) => {
          const matchedRoles = user?.roles?.filter((role) =>
            roles.includes(role)
          );
          if (matchedRoles && matchedRoles.length > 0) {
            return (
              <Link
                key={key}
                href={href}
                className={cn(
                  "flex border-s-4 border-white items-center rounded-md p-2 transition-colors duration-200 whitespace-nowrap w-full", // Thêm whitespace-nowrap
                  path === `${href}/`
                    ? "border-s-4 hover:bg-muted border-Blueviolet font-bold"
                    : "hover:text-Blueviolet text-gray-600 hover:bg-muted"
                )}
              >
                {icon} <span className="ml-2">{label}</span>{" "}
                {/* Thêm khoảng cách giữa icon và label */}
              </Link>
            );
          }
        })}
      </div>
    </>
  );
};

export default LinksAnchor;
