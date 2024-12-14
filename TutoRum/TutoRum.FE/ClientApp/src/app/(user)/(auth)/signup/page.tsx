import { Card, CardContent } from "@/components/ui/card";
import React from "react";
import { SignupForm } from "./components/SignupForm";
import Image from "next/image";

export default function page() {
  return (
    <div className="container mt-5">
      <Card className="overflow-hidden">
        <CardContent className="p-0 flex">
          <div className="w-1/2 bg-cornflowerblue justify-center items-center hidden lg:flex px-5">
            <Image
              src="/login.svg"
              alt="signup-image"
              width={500}
              height={500}
            />
          </div>
          <div className="w-full lg:w-1/2 p-12 md:p-20">
            <SignupForm />
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
