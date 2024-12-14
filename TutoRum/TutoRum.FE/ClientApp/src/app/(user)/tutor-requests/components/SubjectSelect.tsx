import { useSubjectList } from "@/hooks/use-subject";
import { Select } from "antd";
import React from "react";

const SubjectSelect = ({
  field,
  multiple,
  disabled,
}: {
  field: any;
  multiple?: boolean;
  disabled?: boolean;
}) => {
  const { data } = useSubjectList();
  return (
    <Select
      mode={multiple ? "multiple" : "tags"}
      showSearch
      optionFilterProp="label"
      className="w-full"
      placeholder="Chọn môn học"
      disabled={disabled}
      options={data?.map((subject) => ({
        label: subject.subjectName,
        value: subject.subjectName,
      }))}
      {...field}
      onChange={(value) => {
        field.onChange(value[value.length - 1]);
      }}
    />
  );
};

export default SubjectSelect;
