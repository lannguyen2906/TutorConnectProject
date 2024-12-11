import { formatNumber } from "@/utils/other/formatter";
import { Select, Slider } from "antd";
import React from "react";

const SelectPriceRange = ({ field }: { field: any }) => {
  return (
    <Select
      allowClear
      className="w-full"
      placeholder="Chọn khoảng giá"
      {...field}
      value={
        field.value?.length > 0 &&
        `${formatNumber(field.value[0])} - ${formatNumber(field.value[1])}`
      }
      dropdownRender={(menu) => (
        <div className="px-2">
          <Slider
            max={1000000}
            range
            step={50000}
            tooltip={{
              formatter: (value) => formatNumber(value?.toString() || "0"),
            }}
            {...field}
          />
          <div className="flex justify-between">
            <span>0</span>
            <span>1,000,000 VND</span>
          </div>
        </div>
      )}
    />
  );
};

export default SelectPriceRange;
