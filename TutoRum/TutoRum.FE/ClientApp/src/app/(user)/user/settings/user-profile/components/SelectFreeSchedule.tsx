import { Button, Table, TimePicker, Space } from "antd";
import React, { useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import { TableProps } from "antd/es/table";
import { useFieldArray, useFormContext } from "react-hook-form";
import { TutorType } from "@/utils/schemaValidations/tutor.schema";

const scheduleColumns: Record<number, string> = {
  1: "Chủ nhật",
  2: "Thứ hai",
  3: "Thứ ba",
  4: "Thứ tư",
  5: "Thứ năm",
  6: "Thứ sáu",
  7: "Thứ bảy",
};

const SelectFreeSchedule = ({ disabled }: { disabled?: boolean }) => {
  const { control, setValue, watch } = useFormContext<TutorType>(); // Lấy form context từ react-hook-form
  const { fields: schedule, update } = useFieldArray({
    control,
    name: "schedule", // Tên field trong form
  });

  // Function to add a new time slot for a specific day
  const handleAddTimeSlot = (day: number) => {
    const currFreeTimes = schedule?.at(day - 1)?.freeTimes || [];
    if (currFreeTimes.length > 0) {
      update(day - 1, {
        dayOfWeek: day,
        freeTimes: [...currFreeTimes, { startTime: "", endTime: "" }],
      });
    } else {
      update(day - 1, {
        dayOfWeek: day,
        freeTimes: [{ startTime: "", endTime: "" }],
      });
    }
  };

  // Function to update the start or end time of a specific time slot
  const handleTimeChange = (
    day: number,
    index: number,
    timeFrame: Dayjs[] | undefined
  ) => {
    if (timeFrame && timeFrame.length >= 2) {
      const freetimes = watch(`schedule.${day - 1}.freeTimes`);
      const startTime = timeFrame[0]?.format("HH:mm") || "";
      const endTime = timeFrame[1]?.format("HH:mm") || "";
      setValue(`schedule.${day - 1}.freeTimes.${index}`, {
        startTime,
        endTime,
      });
    }
  };

  const handleDeleteTimeSlot = (day: number, index: number) => {
    const currFreeTimes = schedule?.at(day - 1)?.freeTimes || [];
    update(day - 1, {
      dayOfWeek: day,
      freeTimes: [...currFreeTimes.filter((_, i) => i !== index)],
    });
  };

  const columns: TableProps<object>["columns"] = Object.keys(
    scheduleColumns
  ).map((day) => ({
    title: scheduleColumns[Number(day)],
    dataIndex: day,
    key: day,
    align: "center",
    width: 150,
    onCell: () => ({
      style: { verticalAlign: "top" },
    }),
    render: () => (
      <DayColumn
        key={day}
        day={Number(day)}
        schedule={schedule?.at(Number(day) - 1)?.freeTimes}
        onAddTimeSlot={handleAddTimeSlot}
        onTimeChange={handleTimeChange}
        onDeleteTimeSlot={handleDeleteTimeSlot}
        disabled={disabled}
      />
    ),
  }));

  return (
    <Table
      scroll={{ x: 1800 }}
      bordered
      columns={columns}
      pagination={false}
      dataSource={[{ key: "unique-key-1" }]} // Đảm bảo có `key`
    />
  );
};

interface DayColumnProps {
  day: number;
  schedule: { startTime: string; endTime: string }[] | undefined;
  onAddTimeSlot: (day: number) => void;
  onDeleteTimeSlot: (day: number, index: number) => void;
  onTimeChange: (day: number, index: number, timeFrame: Dayjs[]) => void;
  disabled?: boolean;
}

const DayColumn: React.FC<DayColumnProps> = ({
  day,
  schedule,
  onAddTimeSlot,
  onDeleteTimeSlot,
  onTimeChange,
  disabled,
}) => (
  <div key={day} className="flex flex-col gap-2">
    {schedule?.map((timeSlot, index) => {
      var currentFreetimes: number[] = [];

      schedule.forEach((ts, i) => {
        if (i < index) {
          const { startTime, endTime } = ts;
          const startTimeHour = dayjs(startTime, "HH:mm").hour();
          const endTimeHour = dayjs(endTime, "HH:mm").hour();
          currentFreetimes = currentFreetimes.concat(
            Array.from(
              { length: endTimeHour - startTimeHour + 1 },
              (_, index) => startTimeHour + index
            )
          );
        }
      });

      return (
        <div className="flex gap-2 w-full" key={`day-${day}-time-${index}`}>
          <TimePicker.RangePicker
            className="w-3/4"
            onChange={(value) => onTimeChange(day, index, value as Dayjs[])}
            placeholder={["Từ", "Đến"]}
            format={"HH:mm"}
            minuteStep={30}
            disabledTime={() => ({
              disabledHours: () => currentFreetimes,
            })}
            value={
              timeSlot.startTime && timeSlot.endTime
                ? [
                    dayjs(timeSlot.startTime, "HH:mm"),
                    dayjs(timeSlot.endTime, "HH:mm"),
                  ]
                : undefined
            }
            disabled={disabled}
          />
          {!disabled && (
            <Button
              className="w-1/4"
              danger
              type="dashed"
              onClick={() => onDeleteTimeSlot(day, index)}
            >
              X
            </Button>
          )}
        </div>
      );
    })}
    {!disabled && (
      <Button
        type="dashed"
        onClick={() => onAddTimeSlot(day)}
        style={{ width: "100%" }}
      >
        + Thêm thời gian
      </Button>
    )}
  </div>
);

export default SelectFreeSchedule;
