import { Tag } from "antd";
import React from "react";

const tagColors: { [role: string]: string } = {
  learner: "orange",
  tutor: "green",
  employee: "blue",
  admin: "red",
};

const RoleTag = ({ role }: { role: string }) => {
  return <Tag color={tagColors[role]}>{role.toUpperCase()}</Tag>;
};

export default RoleTag;
