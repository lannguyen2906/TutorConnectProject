"use client";

import Link from "next/link";
import { Ellipsis, LogOut } from "lucide-react";
import { usePathname } from "next/navigation";

import { cn } from "@/lib/utils";
import { getMenuList } from "@/lib/menu-list";
import { Button } from "@/components/ui/button";
import { CollapseMenuButton } from "@/app/(admin)/components/collapse-menu-button";
import {
  Tooltip,
  TooltipTrigger,
  TooltipContent,
  TooltipProvider,
} from "@/components/ui/tooltip";
import { useAppContext } from "@/components/provider/app-provider";
import { useAdminMenuAction } from "@/hooks/admin/use-admin";

interface MenuProps {
  isOpen: boolean | undefined;
}

export function Menu({ isOpen }: MenuProps) {
  const pathname = usePathname();
  const { user } = useAppContext();
  const { data, isLoading } = useAdminMenuAction();

  const menuList = getMenuList(pathname).map((g) => ({
    ...g,
    menus: g.menus.map((m) => ({
      ...m,
      count: data?.find((x) => x.href === m.href)?.numberOfAction || 0,
    })),
  }));

  return (
    <nav className="mt-8 h-full w-full">
      <ul className="flex flex-col min-h-[calc(100vh-48px-36px-16px-32px)] lg:min-h-[calc(100vh-32px-40px-32px)] items-start space-y-1 px-2">
        {menuList.map(({ groupLabel, menus }, index) => (
          <li className={cn("w-full", groupLabel ? "pt-5" : "")} key={index}>
            {(isOpen && groupLabel) || isOpen === undefined ? (
              <p className="text-sm font-medium text-muted-foreground px-4 pb-2 max-w-[248px] truncate">
                {groupLabel}
              </p>
            ) : !isOpen && isOpen !== undefined && groupLabel ? (
              <TooltipProvider>
                <Tooltip delayDuration={100}>
                  <TooltipTrigger className="w-full">
                    <div className="w-full flex justify-center items-center">
                      <Ellipsis className="h-5 w-5" />
                    </div>
                  </TooltipTrigger>
                  <TooltipContent side="right">
                    <p>{groupLabel}</p>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            ) : (
              <p className="pb-2"></p>
            )}
            {menus.map(
              (
                { href, label, icon: Icon, active, submenus, count, roles },
                index
              ) => {
                const matchedRoles =
                  !roles || roles.length === 0
                    ? true
                    : roles.filter((role) => user?.roles?.includes(role))
                        .length > 0;

                if (!matchedRoles) return null;
                return !submenus || submenus.length === 0 ? (
                  <div className="w-full" key={index}>
                    <TooltipProvider disableHoverableContent>
                      <Tooltip delayDuration={100}>
                        <TooltipTrigger asChild>
                          <Button
                            variant={
                              (active === undefined &&
                                pathname.startsWith(href)) ||
                              active
                                ? "secondary"
                                : "ghost"
                            }
                            className="w-full justify-start h-10 mb-1"
                            asChild
                          >
                            <Link
                              href={href}
                              className={cn(
                                "flex",
                                isOpen == false
                                  ? "justify-center relative"
                                  : "justify-start"
                              )}
                            >
                              <span
                                className={cn(
                                  "flex items-center",
                                  isOpen === false ? "" : "mr-4"
                                )}
                              >
                                <Icon size={18} />
                              </span>
                              <p
                                className={cn(
                                  "max-w-[200px] truncate",
                                  isOpen === false
                                    ? "-translate-x-96 opacity-0"
                                    : "translate-x-0 opacity-100"
                                )}
                              >
                                {label}
                              </p>
                              {count > 0 && (
                                <div
                                  className={cn(
                                    isOpen == false
                                      ? "absolute top-0 right-0"
                                      : "flex-1 flex justify-end"
                                  )}
                                >
                                  <div
                                    className={cn(
                                      "text-white bg-Blueviolet rounded-full flex items-center justify-center text-xs",
                                      isOpen == false ? "w-2 h-2" : "w-4 h-4"
                                    )}
                                  >
                                    {isOpen == true && count}
                                  </div>
                                </div>
                              )}
                            </Link>
                          </Button>
                        </TooltipTrigger>
                        {isOpen === false && (
                          <TooltipContent side="right">{label}</TooltipContent>
                        )}
                      </Tooltip>
                    </TooltipProvider>
                  </div>
                ) : (
                  <div className="w-full" key={index}>
                    <CollapseMenuButton
                      icon={Icon}
                      label={label}
                      active={
                        active === undefined
                          ? pathname.startsWith(href)
                          : active
                      }
                      submenus={submenus}
                      isOpen={isOpen}
                    />
                  </div>
                );
              }
            )}
          </li>
        ))}
      </ul>
    </nav>
  );
}
