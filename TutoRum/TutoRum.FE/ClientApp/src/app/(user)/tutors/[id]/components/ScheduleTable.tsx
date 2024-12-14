"use client";
import { Table, TableProps, Tag, TimePicker } from "antd";
import { scheduleColumns } from "@/utils/other/mapper";
import { FreeTimeDTO, ScheduleDTO, TutorDto } from "@/utils/services/Api";

type ScheduleTableProps = {
  schedules?: ScheduleDTO[];
  title?: string;
};

const ScheduleTable = ({
  schedules,
  title = " Tôi có thể dạy vào",
}: ScheduleTableProps) => {
  const columns: TableProps<object>["columns"] = Object.keys(
    scheduleColumns
  ).map((day) => ({
    title: scheduleColumns[Number(day)],
    dataIndex: day,
    key: day,
    align: "center",
    width: 100,
    onCell: () => ({
      style: { verticalAlign: "top" },
    }),
    render: () => (
      <DayColumn
        key={day}
        day={Number(day)}
        schedule={
          schedules?.find((s) => s.dayOfWeek?.toString() == day)?.freeTimes!
        }
      />
    ),
  }));

  return (
    <>
      <div className="text-2xl font-bold mb-5 mt-10 border-b-2">{title}</div>
      <Table
        dataSource={[
          {
            key: 1,
          },
        ]}
        columns={columns}
        pagination={false}
        className="my-8"
        scroll={{ x: title ? 0 : 1800 }}
        bordered
      />
    </>
  );
};

interface DayColumnProps {
  day: number;
  schedule: FreeTimeDTO[] | undefined;
}

const DayColumn: React.FC<DayColumnProps> = ({ day, schedule }) => (
  <div key={day} className="flex flex-col gap-2">
    {schedule?.map((timeSlot, index) => {
      return (
        <a
          href="#"
          className="whitespace-nowrap"
          key={`day-${day}-time-${index}`}
        >
          {timeSlot.startTime} - {timeSlot.endTime}
        </a>
      );
    })}
  </div>
);

export default ScheduleTable;
