import { useQualificationLevelList } from "@/hooks/use-tutor";
import { Select } from "antd";
import React from "react";

const QualificationLevelSelect = ({
  field,
  multiple,
  disabled,
}: {
  field: any;
  multiple?: boolean;
  disabled?: boolean;
}) => {
  const { data } = useQualificationLevelList();
  return (
    <Select
      mode={multiple ? "multiple" : undefined}
      showSearch
      optionFilterProp="label"
      allowClear
      className="w-full"
      placeholder="Chọn trình độ"
      disabled={disabled}
      options={data?.map((q) => ({
        label: q.level,
        value: q.id?.toString(),
      }))}
      {...field}
    />
  );
};

export default QualificationLevelSelect;
