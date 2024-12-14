"use client"; // Đảm bảo là client component

import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { mapRouteToName } from "@/utils/other/mapper";
import Link from "next/link";
import { usePathname } from "next/navigation";
import React from "react";

const CustomizedBreadcrumb = ({ currentpage }: { currentpage?: string }) => {
  const pathname = usePathname();

  // Phân tách đường dẫn thành các đoạn
  const pathnames = pathname.split("/").filter((x) => x);

  const isAdmin = pathname.startsWith("/admin");
  const isUser = pathname.startsWith("/user");

  return (
    <Breadcrumb className="mb-4">
      <BreadcrumbList>
        {!isAdmin && !isUser && (
          <>
            <BreadcrumbItem>
              <BreadcrumbLink asChild>
                <Link href="/">Trang chủ</Link>
              </BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
          </>
        )}

        {pathnames.map((name, index) => {
          const displayName = mapRouteToName(name) || name;

          const href = `/${pathnames.slice(0, index + 1).join("/")}`; // Tạo đường dẫn
          if (index < pathnames.length - 1) {
            return (
              <React.Fragment key={href}>
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link href={href} className="capitalize">
                      {displayName}
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
              </React.Fragment>
            );
          } else {
            return (
              <BreadcrumbItem key={href}>
                <BreadcrumbPage className="capitalize">
                  {currentpage ?? displayName}
                </BreadcrumbPage>
              </BreadcrumbItem>
            );
          }
        })}
      </BreadcrumbList>
    </Breadcrumb>
  );
};

export default CustomizedBreadcrumb;
