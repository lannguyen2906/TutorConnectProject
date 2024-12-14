import React from "react";
import { Button, Input, Select } from "antd";
import { Controller, useFieldArray, useFormContext } from "react-hook-form";
import TextArea from "antd/es/input/TextArea";
import { Book } from "lucide-react";
import PriceInput from "@/components/ui/price-input";
import SubjectSelect from "@/app/(user)/tutor-requests/components/SubjectSelect";
import { useAllRateRanges } from "@/hooks/use-rate-range";
import { formatNumber } from "@/utils/other/formatter";
import { useDeleteTutorSubject } from "@/hooks/use-tutor";
import { useAppContext } from "@/components/provider/app-provider";
import { toast } from "react-toastify";

const SelectTeachingSubjects = ({ mode }: { mode?: string }) => {
  const { control, setValue, watch } = useFormContext(); // Lấy form context từ react-hook-form
  const { fields, append, remove } = useFieldArray({
    control,
    name: "subjects", // Tên field trong form
  });
  const { data: rateRanges } = useAllRateRanges();
  const { mutateAsync, isLoading } = useDeleteTutorSubject();
  const { user } = useAppContext();

  const handleDelete = async (tutorSubjectId: number) => {
    try {
      const data = await mutateAsync({
        tutorId: user?.id!,
        tutorSubjectId: tutorSubjectId,
      });
      if (data.status === 201) {
        toast.success("Xóa môn học thành công");
      }
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <div className="space-y-4">
      {fields.map((item, index) => (
        <div
          key={item.id}
          className="flex justify-between items-center mb-4 gap-4"
        >
          <div className="flex flex-col lg:flex-row gap-2 w-full lg:items-center">
            <div className="flex-1 space-y-2">
              <Controller
                name={`subjects.${index}.subjectName`}
                control={control}
                render={({ field }) => <SubjectSelect field={field} />}
              />
              <Controller
                name={`subjects.${index}.learnerType`}
                control={control}
                render={({ field }) => (
                  <Select
                    {...field}
                    options={rateRanges?.map((item) => ({
                      value: item.id,
                      label: `${item.level} - ${formatNumber(
                        item.minRate?.toString() || "0"
                      )} đến ${formatNumber(
                        item.maxRate?.toString() || "0"
                      )} VND`,
                    }))}
                    onChange={(v) => {
                      const rateRange = rateRanges?.find((r) => r.id == v);
                      field.onChange(v);
                      setValue(`subjects.${index}.minRate`, rateRange?.minRate);
                      setValue(`subjects.${index}.maxRate`, rateRange?.maxRate);
                    }}
                    placeholder="Đối tượng dạy học"
                    className="w-full"
                  />
                )}
              />
            </div>
            <div className="flex-1 space-y-2">
              {/* Subject Name Input */}
              <Controller
                name={`subjects.${index}.subjectType`}
                control={control}
                render={({ field }) => (
                  <Select
                    placeholder="Kiểu thời gian"
                    options={[
                      {
                        value: "FLEXIBLE",
                        label: "Linh hoạt",
                      },
                      {
                        value: "FIXED",
                        label: "Cố định",
                      },
                    ]}
                    className="w-full"
                    {...field}
                  />
                )}
              />
              <Controller
                name={`subjects.${index}.rate`}
                control={control}
                render={({ field }) => (
                  <PriceInput
                    disabled={
                      watch(`subjects.${index}.learnerType`) === undefined
                    }
                    field={field}
                    minRate={watch(`subjects.${index}.minRate`)}
                    maxRate={watch(`subjects.${index}.maxRate`)}
                    addonAfter={"VND/ 1 giờ"}
                  />
                )}
              />
            </div>

            <div className="flex-1 xl:w-1/2">
              {/* Description TextArea */}
              <Controller
                name={`subjects.${index}.description`}
                control={control}
                render={({ field }) => (
                  <TextArea
                    className="w-full"
                    placeholder="Mô tả"
                    rows={3}
                    {...field}
                  />
                )}
              />
            </div>
          </div>

          <Button
            danger
            type="dashed"
            onClick={() => remove(index)}
            className="self-center"
          >
            Xóa
          </Button>
        </div>
      ))}

      {/* Add Subject Button */}
      <Button
        type="dashed"
        className="w-full"
        onClick={() =>
          append({
            subjectName: undefined,
            subjectType: undefined,
            rate: undefined,
            description: undefined,
          })
        }
      >
        + Thêm môn học <Book size={16} className="text-slategray" />
      </Button>
    </div>
  );
};

export default SelectTeachingSubjects;
