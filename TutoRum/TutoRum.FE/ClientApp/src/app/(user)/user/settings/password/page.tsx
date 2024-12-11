import React from "react";
import ChangePasswordForm from "./components/ChangePasswordForm";

const page = () => {
  return (
    <div>
      <div className="text-2xl font-bold border-b-2 w-fit mb-5">
        Đổi mật khẩu
      </div>
      <ChangePasswordForm />
    </div>
  );
};

export default page;
