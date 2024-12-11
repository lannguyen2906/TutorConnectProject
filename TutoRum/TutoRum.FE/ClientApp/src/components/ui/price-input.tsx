import { cn } from "@/lib/utils";
import { numberInWords } from "@/utils/other/numberInWords";
import { InputNumber, Tooltip } from "antd";
import React, { useState } from "react";

const PriceInput = ({
  field,
  disabled,
  addonAfter = "VND / 1 giờ",
  minRate,
  maxRate,
  readonly,
}: {
  field: any;
  disabled?: boolean;
  addonAfter?: string;
  minRate?: number;
  maxRate?: number;
  readonly?: boolean;
}) => {
  const error =
    minRate && maxRate && (field.value < minRate || field.value > maxRate);

  return (
    <Tooltip
      title={
        error
          ? `Vui lòng nhập giá tiền trong khoảng ${minRate?.toLocaleString()} VND đến ${maxRate?.toLocaleString()} VND`
          : field.value
          ? numberInWords(field.value)
          : ""
      }
      placement="topLeft"
      color={error ? "red" : undefined}
    >
      <InputNumber<number>
        title="Giá tiền"
        className={cn("w-full", error ? "text-red" : "")}
        formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")}
        parser={(value) =>
          value?.replace(/\$\s?|(,*)/g, "") as unknown as number
        }
        addonAfter={addonAfter}
        placeholder="Giá tiền trên một giờ"
        disabled={disabled}
        readOnly={readonly}
        {...field}
        status={error ? "error" : undefined}
      />
    </Tooltip>
  );
};

export default PriceInput;
