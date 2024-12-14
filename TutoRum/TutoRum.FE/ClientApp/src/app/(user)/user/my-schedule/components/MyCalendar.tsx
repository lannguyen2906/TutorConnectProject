"use client";

import { useAppContext } from "@/components/provider/app-provider";
import { useTutorSchedule } from "@/hooks/use-tutor";
import Table, { TableProps } from "antd/es/table";
import dayjs from "dayjs";
import customParseFormat from "dayjs/plugin/customParseFormat";
import { ClockCircleOutlined, CalendarOutlined } from "@ant-design/icons";
import CalendarCard, { AddScheduleButton } from "./CalendarCard";
import { Button } from "antd";

dayjs.extend(customParseFormat);

const scheduleColumns: Record<number, string> = {
  1: "Chủ nhật",
  2: "Thứ hai",
  3: "Thứ ba",
  4: "Thứ tư",
  5: "Thứ năm",
  6: "Thứ sáu",
  7: "Thứ bảy",
};

const MyCalendar = () => {
  const { user } = useAppContext();
  const { data, isLoading } = useTutorSchedule(user?.id!);

  const columns: TableProps<object>["columns"] = Object.keys(
    scheduleColumns
  ).map((day) => ({
    title: scheduleColumns[Number(day)],
    dataIndex: day,
    key: day,
    align: "center",
    width: 200,
    onCell: () => ({
      style: { verticalAlign: "top" },
    }),
    render: () => (
      <div className="flex flex-col gap-3">
        {data
          ?.find((d) => d.dayOfWeek == Number(day))
          ?.schedules?.map((time, index) => {
            const title = `${dayjs(
              time.freeTimes?.at(0)?.startTime,
              "HH:mm:ss"
            ).format("HH:mm")} - ${dayjs(
              time.freeTimes?.at(0)?.endTime,
              "HH:mm:ss"
            ).format("HH:mm")}`;

            return (
              <>
                <CalendarCard
                  schedules={
                    data?.find((d) => d.dayOfWeek == Number(day))?.schedules ||
                    []
                  }
                  key={index}
                  time={time}
                  title={title}
                />
              </>
            );
          })}
        <div>
          <AddScheduleButton
            dayOfWeek={Number(day)}
            schedules={
              data?.find((d) => d.dayOfWeek == Number(day))?.schedules || []
            }
          />
        </div>
      </div>
    ),
  }));

  return (
    <div className="p-4">
      <div className="mb-4 text-xl font-semibold flex items-center gap-2">
        <CalendarOutlined className="text-blue-500" />
        <span>Lịch học của bạn</span>
      </div>
      <Table
        bordered
        loading={isLoading}
        columns={columns}
        pagination={false}
        dataSource={[{ key: "unique-key-1" }]}
        className="bg-white rounded-lg shadow-md w-full"
      />
    </div>
  );
};

export default MyCalendar;
