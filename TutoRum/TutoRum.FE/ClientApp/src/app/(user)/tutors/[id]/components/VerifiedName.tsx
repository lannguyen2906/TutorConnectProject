import { cn } from "@/lib/utils";
import { Verified } from "lucide-react";
import React from "react";

const VerifiedName = ({
  size,
  isVerified,
  fullName,
}: {
  size: number;
  isVerified: boolean;
  fullName: string;
}) => {
  // Map size values to the corresponding Tailwind classes
  const sizeClasses: Record<number, string> = {
    1: "text-xl",
    2: "text-2xl",
    3: "text-3xl",
    4: "text-4xl",
  };

  // Map size to absolute right position
  const rightOffset: Record<number, string> = {
    1: "-right-7",
    2: "-right-8",
    3: "-right-9",
    4: "-right-10",
  };

  const maxWidth: Record<number, string> = {
    1: "200px",
    2: "350px",
    3: "400px",
    4: "450px",
  };

  return (
    <div
      className={cn(
        `${sizeClasses[size]} font-bold relative w-fit truncate max-w-[${maxWidth[size]}]`
      )}
    >
      {fullName}
      {isVerified && (
        <Verified
          className={cn(
            `absolute top-0 ${rightOffset[size]} text-white fill-Blueviolet`
          )}
        />
      )}
    </div>
  );
};

export default VerifiedName;
