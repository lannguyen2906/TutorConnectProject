"use client";
import React from "react";
import AdminPanelLayout from "@/app/(admin)/components/admin-panel-layout";
import { ContentLayout } from "./components/content-layout";

const layout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div>
      <AdminPanelLayout>{children}</AdminPanelLayout>
    </div>
  );
};

export default layout;
