import MarkdownRenderer from "@/components/markdown/MarkdownRenderer";
import React from "react";

const ProfileDescription = ({
  profileDescription,
}: {
  profileDescription: string;
}) => {
  return (
    <div>
      <div className="text-2xl font-bold mb-5 mt-10 border-b-2">Về tôi</div>
      <MarkdownRenderer content={profileDescription} maxHeight={100} />
    </div>
  );
};

export default ProfileDescription;
