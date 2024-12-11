"use client";
import RegisterTutorSubjectForm from "@/app/(user)/tutors/[id]/register-tutor-subject/components/RegisterTutorSubjectForm";
import { useAppContext } from "@/components/provider/app-provider";
import { useTutorLearnerSubjectDetailByTutorRequestId } from "@/hooks/use-tutor-request";
import { Button, Modal } from "antd";
import { Eye } from "lucide-react";
import { useRouter } from "next/navigation";
import React, { useState } from "react";
import CreateClassroomFromTutorRequest from "./CreateClassroomFromTutorRequest";

const mockTutorLearnerSubjectDetailByTutorRequest = {
  tutorLearnerSubjectId: 102,
  tutorSubjectId: 1002,
  learnerId: "123e4567-e89b-12d3-a456-426614174001",
  tutorId: "987e6543-e21a-45d3-b456-426614170001",
  cityId: "Hồ Chí Minh",
  districtId: "Quận 1",
  wardId: "Bến Nghé",
  locationDetail: "Tòa nhà Bitexco",
  pricePerHour: 500000,
  notes: "Gia sư cần có kỹ năng giao tiếp tiếng Anh tốt.",
  sessionsPerWeek: 2,
  hoursPerSession: 1.5,
  preferredScheduleType: "Flexible",
  expectedStartDate: "2024-12-20T00:00:00Z",
  isVerified: false,
  schedules: null, // Chưa điền
  contractUrl: "https://example.com/contracts/102",
  isContractVerified: false,
  learnerEmail: "learner2@example.com",
  classType: "Online",
  subjectName: "Tiếng Anh giao tiếp",
  totalSessionsCompleted: 5,
  totalPaidAmount: 2500000,
};

const CreateTutorLearnerSubjectButton = ({
  tutorRequestId,
  disabled,
}: {
  tutorRequestId: number;
  disabled: boolean;
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const { user } = useAppContext();
  const router = useRouter();
  const { data, isLoading } =
    useTutorLearnerSubjectDetailByTutorRequestId(tutorRequestId);

  if (user && !user.roles.includes("tutor")) {
    router.push("/forbidden");
  }

  return (
    <div>
      <Button
        onClick={() => setIsOpen(true)}
        type="primary"
        disabled={disabled}
      >
        Tạo lớp
      </Button>
      <Modal
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        footer={null}
        title="Chi tiết yêu cầu"
        width={1000}
        style={{ top: 20 }}
      >
        <CreateClassroomFromTutorRequest
          tutorRequestId={tutorRequestId}
          id={user?.id!}
          tutorLearnerSubjectDetail={data}
        />
      </Modal>
    </div>
  );
};

export default CreateTutorLearnerSubjectButton;
