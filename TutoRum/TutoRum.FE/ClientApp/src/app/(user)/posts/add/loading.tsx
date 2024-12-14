import { Skeleton } from "@/components/ui/skeleton";
import React from "react";

const loading = () => {
  return (
    <div className="container mt-5">
      <Skeleton className="mx-auto h-12 w-48 mb-10" />
      <div className="flex flex-col gap-10">
        <Skeleton className="h-20" />
        <Skeleton className="h-20" />
        <Skeleton className="h-20" />
        <Skeleton className="h-20" />
        <Skeleton className="h-60" />
      </div>
    </div>
  );
};

export default loading;
