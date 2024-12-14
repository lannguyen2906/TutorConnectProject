import { useTopSubject } from "@/hooks/use-subject";
import { Button } from "antd";
import { useRouter } from "next/navigation";
import React from "react";

const TopSubject = ({ form }: { form: any }) => {
  const { data: topSubjects } = useTopSubject(5);
  const router = useRouter();
  return (
    <div className="container mt-5">
      <h2 className="text-2xl font-bold mb-4">Top Môn học</h2>
      <div className="flex flex-wrap gap-4 mt-4">
        {topSubjects?.map((subject) => (
          <Button
            onClick={() =>
              form.setValue("subjects", [subject.subjectId?.toString()])
            }
            key={subject.subjectId}
            type="dashed"
          >
            {subject.subjectName}
          </Button>
        ))}
      </div>
    </div>
  );
};

export default TopSubject;
