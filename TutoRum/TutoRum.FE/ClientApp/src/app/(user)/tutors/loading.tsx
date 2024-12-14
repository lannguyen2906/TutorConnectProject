import { Skeleton } from "@/components/ui/skeleton";
import React from "react";

const loading = () => {
    return (
        <div className="container mt-5">
            <div className="grid grid-cols-12 gap-4">

            </div>

            {/* Grid layout for TutorList and the right section */}
            <div className="grid grid-cols-12 gap-4">
                {/* TutorList takes 8 columns */}
                <div className="col-span-12 lg:col-span-9">
                    <Skeleton className="h-60 w-full mb-4" />
                    <Skeleton className="h-60 w-full mb-4" />
                    <Skeleton className="h-60 w-full mb-4" />
                    <Skeleton className="h-60 w-full mb-4" />
                </div>

                {/* TopTutor and TopSubject take 4 columns on large screens */}
                <div className="col-span-12 lg:col-span-3 space-y-4 ml-2">
                    <Skeleton className="h-60 w-full" />

                    <Skeleton className="h-60 w-full" />

                </div>
            </div>
        </div>
    );
};

export default loading;