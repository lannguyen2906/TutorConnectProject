import { formatNumber } from "@/utils/other/formatter";
import { Tag } from "antd";
import React from "react";

const PriceTag = ({ rate }: { rate: number }) => {
  const price = rate / 1000;
  const color = price <= 100 ? "green" : price >= 200 ? "volcano" : "gold";
  return <Tag color={color}>{formatNumber(rate.toString())} VND</Tag>;
};

export default PriceTag;
