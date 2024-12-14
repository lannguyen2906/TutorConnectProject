import React from "react";
import UserProfileForm from "./components/UserProfileForm";
import ProfileAvatar from "./components/ProfileAvatar";
import FloatButtonMenu from "../../components/FloatButtonMenu";
import { settingList } from "../settingLinkList";

const page = () => {
  return (
    <div>
      <div className="text-2xl font-bold border-b-2 w-fit mb-5">
        Thông tin cá nhân
      </div>
      <div className="flex flex-col  gap-10">
        <div className="w-full">
          <ProfileAvatar />
        </div>
        <div className="w-full">
          <UserProfileForm />
        </div>
      </div>
    </div>
  );
};

export default page;
