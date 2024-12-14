import { Skeleton } from "@/components/ui/skeleton";
import React from "react";

const loading = () => {
  return (
    <div className="container mt-5">
      <Skeleton className="h-12 w-48 mb-10" />
      <div className="flex gap-10 mb-10">
        <Skeleton className="h-60 w-1/2" />
        <Skeleton className="h-60 w-1/2" />
      </div>
      <Skeleton className="h-60" />
    </div>
  );
};

export default loading;
