"use client";
import { cn } from "@/lib/utils";
import { usePathname, useRouter } from "next/navigation";
import React from "react";
import { useAppContext } from "@/components/provider/app-provider";
import { ILink } from "../settings/settingLinkList";

const LinkAnchorHorizontal = ({ linkList }: { linkList: ILink[] }) => {
  const pathName = usePathname();
  const { user } = useAppContext();
  const router = useRouter();
  return (
    <div className="flex border-b-2 border-muted overflow-x-auto">
      {linkList.map((item) => {
        const matchedRoles = user?.roles?.filter((role) =>
          item.roles.includes(role)
        );
        if (matchedRoles && matchedRoles.length > 0) {
          return (
            <div
              className={cn(
                "transition-all ease-in-out",
                pathName.startsWith(item.href) && "border-b-4 border-Blueviolet"
              )}
              key={item.key}
            >
              <button
                className={cn(
                  "px-5 py-3 hover:bg-muted rounded-md transition-all ease-in-out flex gap-2 items-center text-muted-foreground",
                  pathName.startsWith(item.href) && "text-Blueviolet"
                )}
                onClick={() => router.push(item.href)}
              >
                {item.icon}
                <span className="whitespace-nowrap">{item.label}</span>
              </button>
            </div>
          );
        }
      })}
    </div>
  );
};

export default LinkAnchorHorizontal;
