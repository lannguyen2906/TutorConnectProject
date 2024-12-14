import { Card, CardContent } from "@/components/ui/card";
import React from "react";
import ResetPasswordForm from "./components/ResetPasswordForm";

const page = () => {
    return (
        <Card className="mx-auto max-w-6xl mt-10 overflow-hidden">
            <CardContent className="p-0 flex">
                <div className="w-1/2 bg-cornflowerblue"></div>
                <ResetPasswordForm />
            </CardContent>
        </Card>
    )
}

export default page