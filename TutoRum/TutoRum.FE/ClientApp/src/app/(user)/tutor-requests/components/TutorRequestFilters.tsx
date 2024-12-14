import {
  FormControl,
  FormField,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input, Select } from "antd";
import { useFormContext } from "react-hook-form";
import FormItem from "antd/es/form/FormItem";
import { useCallback, useEffect } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import qs from "query-string"; // Sử dụng query-string để tạo URL query
import SubjectSelect from "./SubjectSelect";
import SelectPriceRange from "./SelectPriceRange";
import SelectLocation from "./SelectLocation";
import QualificationLevelSelect from "../../tutors/[id]/components/QualificationLevelSelect";

const { Option } = Select;

const TutorRequestFilters = () => {
  const { control, setValue, watch } = useFormContext();
  const router = useRouter();
  const pathname = usePathname();
  const formValues = watch(); // Lắng nghe tất cả giá trị trong form

  const searchParams = useSearchParams();

  // Lắng nghe sự thay đổi của formValues và cập nhật query
  useEffect(() => {
    const newQuery = qs.stringify({ ...searchParams, ...formValues });
    router.replace(`${pathname}?${newQuery}`, undefined);
  }, [formValues, pathname, router, searchParams]);

  return (
    <>
      <div className="flex flex-col xl:flex-row w-full gap-4">
        <div className="w-full xl:w-1/2 flex flex-col xl:flex-row gap-4">
          {/* Chọn môn học */}
          <FormField
            control={control}
            name="subjects"
            render={({ field }) => (
              <FormItem className="w-full xl:w-1/2">
                <FormLabel>Môn học</FormLabel>
                <FormControl>
                  <SubjectSelect multiple field={field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {/* Khoảng giá 1 giờ */}
          <FormField
            control={control}
            name="priceRange"
            render={({ field }) => (
              <FormItem className="w-full xl:w-1/2">
                <FormLabel>Khoảng giá</FormLabel>
                <FormControl>
                  <SelectPriceRange field={field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <div className="w-full xl:w-1/2 flex flex-col xl:flex-row gap-4">
          <FormItem className="w-full">
            <FormLabel>Khu vực</FormLabel>
            <FormControl>
              <SelectLocation />
            </FormControl>
            <FormMessage />
          </FormItem>
        </div>
      </div>
      <div className="w-full flex flex-col xl:flex-row gap-4">
        <div className="w-full xl:w-1/2 flex flex-col xl:flex-row gap-4">
          <div className="w-full xl:w-1/2 flex flex-col xl:flex-row gap-2">
            {/* Giới tính gia sư */}
            <FormField
              control={control}
              name="tutorGender"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel>Giới tính gia sư</FormLabel>
                  <FormControl>
                    <Select
                      className="w-full"
                      placeholder="Chọn giới tính"
                      {...field}
                    >
                      <Option value="male">Nam</Option>
                      <Option value="female">Nữ</Option>
                      <Option value="Khác">Khác</Option>
                    </Select>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          <FormField
            control={control}
            name="qualification"
            render={({ field }) => (
              <FormItem className="w-full xl:w-1/2">
                <FormLabel>Trình độ gia sư</FormLabel>
                <FormControl>
                  <QualificationLevelSelect field={field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        {/* Tìm kiếm theo từ khóa */}
        <FormField
          control={control}
          name="search"
          render={({ field }) => (
            <FormItem className="w-full xl:w-1/2">
              <FormLabel>Tìm kiếm theo tên hoặc từ khóa</FormLabel>
              <FormControl>
                <Input placeholder="Nhập từ khóa" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </>
  );
};

export default TutorRequestFilters;
