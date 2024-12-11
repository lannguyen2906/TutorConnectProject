import { Skeleton } from "@/components/ui/skeleton";
import React from "react";

const loading = () => {
  return (
    <div className="container mt-5">
      <Skeleton className="w-1/2"></Skeleton>
      <div className="w-1/2 flex flex-col items-center p-10">
        <Skeleton className="h-12 w-48 mb-10" />
        <Skeleton className="h-12 w-64 mb-10" />
        <Skeleton className="h-12 w-64 mb-10" />
        <Skeleton className="h-12 w-64 mb-10" />
      </div>
    </div>
  );
};

export default loading;
