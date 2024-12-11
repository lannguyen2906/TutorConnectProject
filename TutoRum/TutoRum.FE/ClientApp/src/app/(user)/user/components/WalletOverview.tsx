"use client";
import { useWalletOverview } from "@/hooks/use-payment-request";
import { Card, Statistic, StatisticProps } from "antd";
import { DollarSign, TrendingUp, AlertCircle } from "lucide-react";
import React from "react";
import CountUp from "react-countup";

const formatter: StatisticProps["formatter"] = (value) => (
  <CountUp end={value as number} separator="," />
);

const WalletOverview = () => {
  const { data, isLoading } = useWalletOverview();

  return (
    <div className="space-y-5">
      <div className="flex gap-10">
        {/* Số dư hiện tại */}
        <Card className="rounded shadow-md flex-1">
          <Statistic
            title="Số dư có thể rút"
            value={data?.currentBalance ?? 0}
            valueStyle={{ color: "blueviolet" }}
            suffix={" VND"}
            prefix={<DollarSign size={16} />}
            loading={isLoading}
            formatter={formatter}
          />
        </Card>

        {/* Số tiền kiếm được trong tháng */}
        <Card className="rounded shadow-md flex-1">
          <Statistic
            title="Số tiền kiếm được trong tháng"
            value={data?.totalEarningsThisMonth ?? 0}
            valueStyle={{
              color:
                data?.totalEarningsThisMonth ?? 0 > 0 ? "#3f8600" : "#8c8c8c",
            }}
            suffix={" VND"}
            prefix={<TrendingUp size={16} />}
            loading={isLoading}
            formatter={formatter}
          />
        </Card>

        {/* Yêu cầu rút tiền chưa được xử lý */}
        <Card className="rounded shadow-md flex-1">
          <Statistic
            title="Yêu cầu rút tiền chưa được xử lý"
            value={data?.pendingWithdrawals ?? 0}
            valueStyle={{
              color: (data?.pendingWithdrawals ?? 0) > 0 ? "orange" : "#8c8c8c",
            }}
            suffix={" yêu cầu"}
            prefix={<AlertCircle size={16} />}
            loading={isLoading}
            formatter={formatter}
          />
        </Card>
      </div>
    </div>
  );
};

export default WalletOverview;
