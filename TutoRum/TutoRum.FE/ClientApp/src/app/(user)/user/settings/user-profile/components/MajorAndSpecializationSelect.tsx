"use client";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { useMajorsAndSpecializations } from "@/hooks/use-tutor";
import { Select } from "antd";
import { useEffect, useState } from "react";

const MajorAndSpecializationForm = ({ form }: { form: any }) => {
  const [specializations, setSpecializations] = useState<string[]>([]);
  const { data: majorsData } = useMajorsAndSpecializations();
  const selectedMajor = form.watch("major");

  // Khi `selectedMajor` thay đổi, cập nhật `specializations`
  useEffect(() => {
    if (majorsData && majorsData.length > 0 && selectedMajor) {
      const majorData = majorsData.find((m) => m.major === selectedMajor);
      setSpecializations(majorData ? majorData?.minors! : []);
    }
  }, [selectedMajor, majorsData]);

  return (
    <>
      <FormField
        control={form.control}
        name="major"
        render={({ field }) => (
          <FormItem className="w-full md:w-1/2">
            <FormLabel required>Chuyên ngành</FormLabel>
            <FormControl>
              <Select
                className="w-full"
                placeholder="Chọn chuyên ngành"
                options={majorsData?.map((major) => ({
                  value: major.major,
                  label: major.major,
                }))}
                onChange={(e) => {
                  field.onChange(e);
                }}
                value={field.value}
                showSearch
                optionFilterProp="label"
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <FormField
        control={form.control}
        name="specialization"
        render={({ field }) => (
          <FormItem className="w-full md:w-1/2">
            <FormLabel required>Chuyên ngành hẹp</FormLabel>
            <FormControl>
              <Select
                className="w-full"
                placeholder="Chọn chuyên ngành hẹp"
                mode="tags"
                options={specializations.map((minor) => ({
                  value: minor,
                  label: minor,
                }))}
                onChange={(e) => {
                  field.onChange(e && e.at(e.length - 1));
                }}
                value={field.value}
                showSearch
                optionFilterProp="label"
                disabled={!selectedMajor} // Vô hiệu hóa khi chưa chọn chuyên ngành
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
    </>
  );
};

export default MajorAndSpecializationForm;
