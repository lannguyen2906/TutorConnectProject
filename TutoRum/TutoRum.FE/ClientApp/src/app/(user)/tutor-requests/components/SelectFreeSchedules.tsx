"use client";

import React from "react";
import { Select, TimePicker, Button } from "antd";
import { useFormContext } from "react-hook-form";
import { PlusOutlined, DeleteOutlined } from "@ant-design/icons";
import { TutorRequestType } from "@/utils/schemaValidations/tutor.request.schema";
import dayjs from "dayjs";
import { X } from "lucide-react";

const daysOfWeek = [
  { label: "Thứ 2", value: 2 },
  { label: "Thứ 3", value: 3 },
  { label: "Thứ 4", value: 4 },
  { label: "Thứ 5", value: 5 },
  { label: "Thứ 6", value: 6 },
  { label: "Thứ 7", value: 7 },
  { label: "Chủ nhật", value: 1 },
];

const SelectFreeSchedules = () => {
  const { watch, setValue } = useFormContext<TutorRequestType>();
  const freeSchedules = watch("freeSchedules") || [];

  const handleAddSchedule = () => {
    setValue("freeSchedules", [
      ...freeSchedules,
      { daysOfWeek: [], freeTimes: [{ startTime: "", endTime: "" }] },
    ]);
  };

  const handleAddTimeSlot = (index: number) => {
    const updatedSchedules = [...freeSchedules];
    updatedSchedules[index]?.freeTimes.push({
      startTime: "",
      endTime: "",
    });
    setValue("freeSchedules", updatedSchedules);
  };

  const handleRemoveSchedule = (index: number) => {
    const updatedSchedules = [...freeSchedules];
    updatedSchedules.splice(index, 1);
    setValue("freeSchedules", updatedSchedules);
  };

  const handleRemoveTimeSlot = (
    scheduleIndex: number,
    timeSlotIndex: number
  ) => {
    const updatedSchedules = [...freeSchedules];
    updatedSchedules[scheduleIndex]?.freeTimes.splice(timeSlotIndex, 1);
    setValue("freeSchedules", updatedSchedules);
  };

  return (
    <div>
      {freeSchedules.map((schedule, scheduleIndex) => (
        <div
          key={scheduleIndex}
          style={{
            marginBottom: "16px",
            padding: "16px",
            border: "1px solid #d9d9d9",
            borderRadius: "8px",
          }}
        >
          <p className="text-sm font-medium mb-2">Chọn các ngày trong tuần</p>
          <div className="flex gap-5 items-center mb-3">
            {/* Days of Week Select */}
            <Select
              mode="multiple"
              options={daysOfWeek.map((day) => ({
                label: day.label,
                value: day.value,
                disabled:
                  freeSchedules.length > 1
                    ? freeSchedules
                        .at(scheduleIndex - 1)
                        ?.daysOfWeek.includes(day.value)
                    : false,
              }))}
              placeholder="Chọn ngày"
              value={schedule.daysOfWeek}
              onChange={(value) =>
                setValue(`freeSchedules.${scheduleIndex}.daysOfWeek`, value)
              }
              className="flex-1"
            />
            <Button
              type="text"
              icon={<DeleteOutlined />}
              danger
              onClick={() => handleRemoveSchedule(scheduleIndex)}
            />
          </div>
          <p className="text-sm font-medium mb-2">Chọn các khung giờ rảnh</p>
          <div className="flex items-center flex-wrap gap-2">
            {schedule.freeTimes.map((timeSlot, timeSlotIndex) => (
              <div
                key={timeSlotIndex}
                className="flex gap-3 items-center rounded-lg border-muted border-[1px] p-2"
              >
                <TimePicker.RangePicker
                  style={{ width: "200px" }}
                  format="HH:mm"
                  minuteStep={30}
                  value={
                    timeSlot.startTime && timeSlot.endTime
                      ? [
                          dayjs(timeSlot.startTime, "HH:mm"),
                          dayjs(timeSlot.endTime, "HH:mm"),
                        ]
                      : undefined
                  }
                  onChange={(value) => {
                    const updatedTimeSlot = {
                      startTime: value?.[0]?.format("HH:mm") || "",
                      endTime: value?.[1]?.format("HH:mm") || "",
                    };
                    setValue(
                      `freeSchedules.${scheduleIndex}.freeTimes.${timeSlotIndex}`,
                      updatedTimeSlot
                    );
                  }}
                  placeholder={["Từ", "Đến"]}
                />
                <Button
                  type="text"
                  icon={<X />}
                  danger
                  onClick={() =>
                    handleRemoveTimeSlot(scheduleIndex, timeSlotIndex)
                  }
                />
              </div>
            ))}

            <Button
              type="dashed"
              icon={<PlusOutlined />}
              onClick={() => handleAddTimeSlot(scheduleIndex)}
            >
              Thêm khoảng thời gian
            </Button>
          </div>
        </div>
      ))}

      <Button
        type="dashed"
        icon={<PlusOutlined />}
        onClick={handleAddSchedule}
        style={{ marginTop: "16px" }}
      >
        Thêm lịch rảnh
      </Button>
    </div>
  );
};

export default SelectFreeSchedules;
