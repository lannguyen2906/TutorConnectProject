import { Alert, Button, Table, TimePicker, Tooltip } from "antd";
import React, { useEffect } from "react";
import dayjs, { Dayjs } from "dayjs";
import { TableProps } from "antd/es/table";
import { useFormContext } from "react-hook-form";
import {
  RegisterLearnerDTOType,
  TutorType,
} from "@/utils/schemaValidations/tutor.schema";

const daysOfWeek: Record<number, string> = {
  1: "Chủ nhật",
  2: "Thứ hai",
  3: "Thứ ba",
  4: "Thứ tư",
  5: "Thứ năm",
  6: "Thứ sáu",
  7: "Thứ bảy",
};

const ClassroomLearningSchedule = ({
  disabled,
  tutorSchedule,
}: {
  disabled?: boolean;
  tutorSchedule: any;
}) => {
  const { setValue, watch } = useFormContext<RegisterLearnerDTOType>();

  const studentSchedule = watch("schedules");
  const sessionCount = watch("sessionsPerWeek");
  const sessionDuration = watch("hoursPerSession");

  useEffect(() => {
    setValue("schedules", []);
  }, [sessionCount, sessionDuration, setValue]);

  // Thêm khung giờ học mới vào một ngày cụ thể
  const addTimeSlot = (day: number, availabilityIndex: number) => {
    const existingTimeSlots = studentSchedule?.[day - 1]?.learningTimes || [];
    const newTimeSlot = { startTime: "", endTime: "", availabilityIndex };

    setValue(`schedules.${day - 1}`, {
      dayOfWeek: day,
      learningTimes: [...existingTimeSlots, newTimeSlot],
    });
  };

  // Cập nhật thời gian học cho một khung giờ
  const updateTimeSlot = (
    day: number,
    index: number,
    startTime: Dayjs | undefined
  ) => {
    if (startTime) {
      const formattedStartTime = startTime.format("HH:mm");
      const formattedEndTime = startTime
        .add(sessionDuration, "hour")
        .format("HH:mm");
      setValue(
        `schedules.${day - 1}.learningTimes.${index}.startTime`,
        formattedStartTime
      );
      setValue(
        `schedules.${day - 1}.learningTimes.${index}.endTime`,
        formattedEndTime
      );
    }
  };

  // Xóa một khung giờ học
  const deleteTimeSlot = (day: number, index: number) => {
    const remainingTimeSlots =
      studentSchedule?.[day - 1]?.learningTimes?.filter(
        (_: any, i: any) => i !== index
      ) || [];
    setValue(`schedules.${day - 1}`, {
      dayOfWeek: day,
      learningTimes: remainingTimeSlots,
    });
  };

  const columns: TableProps<object>["columns"] = Object.keys(daysOfWeek).map(
    (day) => ({
      title: daysOfWeek[Number(day)],
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
          availableSlots={tutorSchedule?.[Number(day) - 1]?.freeTimes}
          studentTimeSlots={
            studentSchedule?.[Number(day) - 1]?.learningTimes || []
          }
          onAddTimeSlot={addTimeSlot}
          onUpdateTimeSlot={updateTimeSlot}
          onDeleteTimeSlot={deleteTimeSlot}
          disabled={disabled}
        />
      ),
    })
  );

  return (
    <Table
      scroll={{ x: 1800 }}
      bordered
      columns={columns}
      pagination={false}
      dataSource={[{ key: "unique-key-1" }]}
    />
  );
};

interface DayColumnProps {
  day: number;
  studentTimeSlots:
    | { startTime: string; endTime: string; availabilityIndex: number }[]
    | undefined;
  availableSlots: { startTime: string; endTime: string }[] | undefined;
  onAddTimeSlot: (day: number, availabilityIndex: number) => void;
  onDeleteTimeSlot: (day: number, index: number) => void;
  onUpdateTimeSlot: (day: number, index: number, startTime: Dayjs) => void;
  disabled?: boolean;
}

const DayColumn: React.FC<DayColumnProps> = ({
  day,
  studentTimeSlots,
  availableSlots,
  onAddTimeSlot,
  onDeleteTimeSlot,
  onUpdateTimeSlot,
  disabled,
}) => {
  const { watch } = useFormContext<RegisterLearnerDTOType>();
  const sessionCount = watch("sessionsPerWeek");
  const sessionDuration = watch("hoursPerSession");
  const totalSessions = watch("schedules")?.flatMap(
    (s: any) => s.learningTimes
  );
  const [warningTime, setWarningTime] = React.useState<number[]>([]);

  return (
    <div key={day} className="flex flex-col gap-2">
      {availableSlots?.map((availableTime, availabilityIndex) => {
        const startHour = dayjs(availableTime.startTime, "HH:mm:ss").hour();
        const endHour = dayjs(availableTime.endTime, "HH:mm:ss").hour();

        const studentSchedule = studentTimeSlots?.filter(
          (timeSlot, index) =>
            timeSlot.availabilityIndex === availabilityIndex &&
            index < studentTimeSlots?.length - 1
        );

        // Tạo một mảng giờ từ các khoảng thời gian trong studentSchedule
        const selectedHours =
          studentSchedule?.reduce((acc, schedule) => {
            const startHour = dayjs(schedule.startTime, "HH:mm").hour();
            const endHour = dayjs(schedule.endTime, "HH:mm").hour();

            // Tạo các giờ riêng lẻ từ startHour đến endHour - 1 và thêm vào mảng acc
            for (let hour = startHour; hour <= endHour; hour++) {
              acc.push(hour);
            }
            return acc;
          }, [] as number[]) || [];

        // Tạo danh sách các giờ không khả dụng ngoài thời gian rảnh
        const notAvailableHours = [
          ...Array.from({ length: startHour }, (_, i) => i),
          ...Array.from(
            { length: 24 - endHour + sessionDuration },
            (_, i) => endHour - sessionDuration + 1 + i
          ),
          ...selectedHours,
        ];

        const hoursAvailable = endHour - startHour;

        return (
          <div
            className="flex flex-col gap-2 w-full rounded-lg overflow-hidden pb-2 bg-muted"
            key={`day-${day}-availability-${availabilityIndex}`}
          >
            <p className="font-bold text-white w-full bg-Blueviolet">
              {dayjs(availableTime.startTime, "HH:mm:ss").format("HH:mm")} -{" "}
              {dayjs(availableTime.endTime, "HH:mm:ss").format("HH:mm")}
            </p>
            <div className="px-2 space-y-2">
              {studentTimeSlots?.map((timeSlot, index) => {
                if (timeSlot.availabilityIndex === availabilityIndex) {
                  return (
                    <div className="space-y-2" key={`day-${day}-slot-${index}`}>
                      <div className="flex gap-2 w-full">
                        <TimePicker
                          className="w-3/4"
                          onChange={(value) => {
                            onUpdateTimeSlot(day, index, value);

                            // Kiểm tra sự khác biệt chính xác là 30 phút
                            const differenceInMinutes = value.diff(
                              dayjs(availableTime.startTime, "HH:mm:ss"),
                              "minute"
                            );

                            // Nếu khác biệt là 30 phút và thời gian là 8:30, hiển thị cảnh báo
                            if (differenceInMinutes === 30) {
                              setWarningTime([...warningTime, index]);
                            }
                          }}
                          status={warningTime.includes(index) ? "warning" : ""}
                          placeholder="Từ"
                          format="HH:mm"
                          minuteStep={30}
                          disabledTime={() => ({
                            disabledHours: () => notAvailableHours,
                            disabledMinutes: (hour: number) => {
                              const expectedEndTime = dayjs()
                                .hour(hour)
                                .minute(30)
                                .add(sessionDuration, "hour");

                              if (
                                expectedEndTime.isAfter(
                                  dayjs(availableTime.endTime, "HH:mm:ss")
                                )
                              ) {
                                return [30];
                              }
                              return [];
                            },
                          })}
                          defaultOpenValue={dayjs(
                            availableTime.startTime,
                            "HH:mm:ss"
                          )}
                          value={
                            timeSlot.startTime
                              ? dayjs(timeSlot.startTime, "HH:mm")
                              : undefined
                          }
                          disabled={disabled}
                          needConfirm={false}
                        />

                        <TimePicker
                          className="w-3/4"
                          placeholder="Đến"
                          format="HH:mm"
                          disabled
                          value={
                            timeSlot.endTime
                              ? dayjs(timeSlot.endTime, "HH:mm")
                              : undefined
                          }
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
                      {warningTime.includes(index) && (
                        <Alert
                          type="warning"
                          closable
                          message="Không nên bỏ trống 30 phút đầu"
                          onClose={() =>
                            setWarningTime(
                              warningTime.filter((i) => i !== index)
                            )
                          }
                        />
                      )}
                    </div>
                  );
                }
                return null;
              })}
              <Button
                type="dashed"
                onClick={() => onAddTimeSlot(day, availabilityIndex)}
                style={{ width: "100%" }}
                disabled={
                  disabled ||
                  hoursAvailable < sessionDuration ||
                  !sessionCount ||
                  !sessionDuration ||
                  totalSessions?.length! >= sessionCount ||
                  hoursAvailable <
                    sessionDuration *
                      (studentTimeSlots?.filter(
                        (ts) => ts.availabilityIndex === availabilityIndex
                      ).length! +
                        1)
                }
              >
                + Thêm thời gian
              </Button>
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default ClassroomLearningSchedule;
