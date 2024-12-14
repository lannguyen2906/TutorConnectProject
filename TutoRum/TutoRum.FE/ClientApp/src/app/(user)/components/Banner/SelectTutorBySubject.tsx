"use client";
import { useSubjectList } from "@/hooks/use-subject";
import { Select, Button } from "antd";
import { Search } from "lucide-react";
import { useRouter } from "next/navigation";
import { useState } from "react";

const SelectTutorBySubject = () => {
  const { data } = useSubjectList();
  const [subject, setSubject] = useState("");
  const router = useRouter();
  const handleClick = () => {
    router.push(`/tutors?subjects=${subject}`);
  };

  const handleChange = (value: string) => {
    console.log("Selected subject:", value);
  };

  return (
    <div className="flex items-center gap-2">
      <Select
        className="w-full rounded-full"
        options={data?.map((item) => ({
          value: item.subjectId,
          label: item.subjectName,
        }))}
        showSearch
        optionFilterProp="label"
        placeholder="Chọn môn học"
        onChange={setSubject}
        allowClear
        style={{
          height: "50px",
        }}
      />
      <Button
        disabled={!subject}
        icon={<Search />}
        type="primary"
        shape="circle"
        size="large"
        onClick={handleClick}
      />
    </div>
  );
};

export default SelectTutorBySubject;
