import { VerifyButton } from "@/app/(admin)/admin/tutors/[id]/components/verify-buttons";
import { Drawer } from "antd";
import { Eye } from "lucide-react";
import React, { useState } from "react";

const TutorLearnerSubjectDrawer = ({
  id,
  isVerified,
}: {
  id: number;
  isVerified: boolean;
}) => {
  const [open, setOpen] = useState(false);
  return (
    <div>
      {/* <button onClick={() => setOpen(true)}>
        <Eye color="blueviolet" size={16} />
      </button> */}
      <Drawer
        title="Chi tiết yêu cầu"
        placement="right"
        size="large"
        onClose={() => setOpen(false)}
        open={open}
        footer={
          <VerifyButton
            guid={null}
            entityType={3}
            id={id}
            isVerified={isVerified}
            mode="tutor"
          />
        }
      ></Drawer>
    </div>
  );
};

export default TutorLearnerSubjectDrawer;
