import React, { useState } from "react";
import { Button, Modal, Popconfirm, TimePicker } from "antd";
import dayjs from "dayjs";
import Link from "next/link";
import { ScheduleViewDTO } from "@/utils/services/Api";
import {
  useAddSchedule,
  useDeleteSchedule,
  useUpdateSchedule,
} from "@/hooks/use-tutor";
import { useAppContext } from "@/components/provider/app-provider";
import { toast } from "react-toastify";
import { Plus } from "lucide-react";

export const AddScheduleButton = ({
  schedules,
  dayOfWeek,
}: {
  schedules: ScheduleViewDTO[];
  dayOfWeek: number;
}) => {
  const [freeTimes, setFreeTimes] = useState({ startTime: "", endTime: "" });
  const [isOpen, setIsOpen] = useState(false);
  const { user } = useAppContext();
  const { mutateAsync: mutateAsyncAdd, isLoading: isLoadingAdd } =
    useAddSchedule(user?.id!);
  const chosenHours = [] as number[];
  const timeOfDay = schedules?.filter((s) => s.dayOfWeek == dayOfWeek);

  timeOfDay?.forEach((s) => {
    const startTime = dayjs(s.freeTimes?.at(0)?.startTime, "HH:mm:ss");
    const endTime = dayjs(s.freeTimes?.at(0)?.endTime, "HH:mm:ss");
    for (let hour = startTime.hour(); hour < endTime.hour(); hour++) {
      chosenHours.push(hour);
    }
  });
  const error =
    freeTimes?.startTime &&
    freeTimes?.endTime &&
    dayjs(freeTimes.startTime, "HH:mm").hour() < Math.min(...chosenHours) &&
    dayjs(freeTimes.endTime, "HH:mm").hour() > Math.max(...chosenHours);
  const handleAdd = async () => {
    try {
      if (error) {
        toast.error("Thời gian không hợp lệ");
        return;
      }
      const data = await mutateAsyncAdd({
        dayOfWeek: dayOfWeek,
        freeTimes: [freeTimes || { startTime: "", endTime: "" }],
      });
      if (data.status == 201) {
        toast.success("Thêm mới thành công");
        setIsOpen(false);
      }
    } catch (error) {
      toast.error("Lỗi khi thêm mới");
      console.log(error);
    }
  };

  return (
    <>
      <Button
        type="dashed"
        title="Thêm mới"
        className="w-full"
        icon={<Plus size={16} />}
        onClick={() => setIsOpen(true)}
        size="small"
      />
      <Modal
        title="Thêm mới thời gian rảnh"
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        okText="Thêm mới"
        cancelText="Hủy"
        onOk={handleAdd}
        confirmLoading={isLoadingAdd}
      >
        <TimePicker.RangePicker
          style={{ width: "100%" }}
          format="HH:mm"
          minuteStep={30}
          value={
            freeTimes?.startTime && freeTimes?.endTime
              ? [
                  dayjs(freeTimes.startTime, "HH:mm"),
                  dayjs(freeTimes.endTime, "HH:mm"),
                ]
              : undefined
          }
          onChange={(value) => {
            const updatedTimeSlot = {
              startTime: value?.[0]?.format("HH:mm") || "",
              endTime: value?.[1]?.format("HH:mm") || "",
            };
            setFreeTimes(updatedTimeSlot);
          }}
          disabledTime={(date, type) => {
            return {
              disabledHours: () => chosenHours,
            };
          }}
          placeholder={["Từ", "Đến"]}
          needConfirm={false}
          status={error ? "error" : ""}
        />
      </Modal>
    </>
  );
};

const CalendarCard = ({
  schedules,
  time,
  title,
}: {
  schedules: ScheduleViewDTO[];
  time: ScheduleViewDTO;
  title: string;
}) => {
  const [freeTimes, setFreeTimes] = useState(time.freeTimes?.at(0));
  const [isOpen, setIsOpen] = useState(false);
  const { user } = useAppContext();
  const { mutateAsync, isLoading } = useUpdateSchedule(user?.id!);
  const { mutateAsync: mutateAsyncDelete, isLoading: isLoadingDelete } =
    useDeleteSchedule(user?.id!);

  const handleSave = async () => {
    try {
      const data = await mutateAsync({
        id: time.id,
        dayOfWeek: time.dayOfWeek || 0,
        freeTimes: [freeTimes || { startTime: "", endTime: "" }],
        tutorID: user?.id,
        tutorLearnerSubjectID: time.tutorLearnerSubjectId || undefined,
      });
      if (data.status == 201) {
        toast.success("Chỉnh sửa thành công");
        setIsOpen(false);
      }
    } catch (error) {
      toast.error("Lỗi khi chỉnh sửa");
      console.log(error);
    }
  };

  const handleDelete = async () => {
    try {
      const data = await mutateAsyncDelete({
        dayOfWeek: time.dayOfWeek || 0,
        freeTimes: [freeTimes || { startTime: "", endTime: "" }],
        scheduleId: time.id,
      });
      if (data.status == 201) {
        toast.success("Chỉnh sửa thành công");
        setIsOpen(false);
      }
    } catch (error) {
      toast.error("Lỗi khi chỉnh sửa");
      console.log(error);
    }
  };

  const index = schedules.findIndex((s) => s.id === time.id);
  const minEditTime = schedules[index - 1]?.freeTimes?.at(0)?.endTime;
  const maxEditTime = schedules[index + 1]?.freeTimes?.at(0)?.startTime;

  return (
    <>
      <div className="flex flex-col gap-2 w-full rounded-lg overflow-hidden border-[1px] border-muted shadow-md">
        <span className="font-bold text-white w-full bg-Blueviolet">
          {title}
        </span>
        {time.tutorLearnerSubjectId ? (
          <Link
            href={`/user/teaching-classrooms/${time.tutorLearnerSubjectId}`}
            className="font-semibold text-sm p-3"
          >
            {time.subjectNames} - #{time.tutorLearnerSubjectId}
          </Link>
        ) : (
          <>
            <span className="text-muted-foreground text-sm italic p-3">
              {time.subjectNames}
            </span>
            <div className="border-t-[1px] border-muted-foreground w-full flex justify-between p-2">
              <span
                onClick={() => setIsOpen(true)}
                className="text-xs underline w-full hover:text-Blueviolet cursor-pointer transition-all ease-in-out"
              >
                Chỉnh sửa
              </span>
              <Popconfirm
                title="Xóa lịch rảnh"
                onConfirm={handleDelete}
                okText="Xóa"
                cancelText="Hủy"
                okType="danger"
              >
                <span className="text-xs underline w-full hover:no-underline cursor-pointer transition-all ease-in-out text-red text-end">
                  Xóa
                </span>
              </Popconfirm>
            </div>
          </>
        )}
      </div>
      <Modal
        title="Chỉnh sửa thời gian rảnh"
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        okText="Chỉnh sửa"
        cancelText="Hủy"
        onOk={handleSave}
        confirmLoading={isLoading}
      >
        <TimePicker.RangePicker
          style={{ width: "100%" }}
          format="HH:mm"
          minuteStep={30}
          value={
            freeTimes?.startTime && freeTimes?.endTime
              ? [
                  dayjs(freeTimes.startTime, "HH:mm"),
                  dayjs(freeTimes.endTime, "HH:mm"),
                ]
              : undefined
          }
          onChange={(value) => {
            const updatedTimeSlot = {
              startTime: value?.[0]?.format("HH:mm") || "",
              endTime: value?.[1]?.format("HH:mm") || "",
            };
            setFreeTimes(updatedTimeSlot);
          }}
          disabledTime={(date, type) => {
            const minTime = minEditTime ? dayjs(minEditTime, "HH:mm") : null;
            const maxTime = maxEditTime ? dayjs(maxEditTime, "HH:mm") : null;

            return {
              disabledHours: () => {
                const hours: number[] = [];
                if (type === "start" && minTime) {
                  for (let i = 0; i < 24; i++) {
                    if (dayjs().hour(i).isBefore(minTime, "hour")) {
                      hours.push(i);
                    }
                  }
                }
                if (type === "end" && maxTime) {
                  for (let i = 0; i < 24; i++) {
                    if (dayjs().hour(i).isAfter(maxTime, "hour")) {
                      hours.push(i);
                    }
                  }
                }
                return hours;
              },
              disabledMinutes: (selectedHour) => {
                const minutes: number[] = [];
                if (type === "start" && minTime) {
                  if (selectedHour === minTime.hour()) {
                    for (let i = 0; i < 60; i++) {
                      if (dayjs().minute(i).isBefore(minTime, "minute")) {
                        minutes.push(i);
                      }
                    }
                  }
                }
                if (type === "end" && maxTime) {
                  if (selectedHour === maxTime.hour()) {
                    for (let i = 0; i < 60; i++) {
                      if (dayjs().minute(i).isAfter(maxTime, "minute")) {
                        minutes.push(i);
                      }
                    }
                  }
                }
                return minutes;
              },
            };
          }}
          placeholder={["Từ", "Đến"]}
          needConfirm={false}
        />
      </Modal>
    </>
  );
};

export default CalendarCard;
