"use client";

import React, { useEffect, useState } from "react";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { zodResolver } from "@hookform/resolvers/zod";

import { Input, Select, Spin } from "antd";
import { useSubjectList } from "@/hooks/use-subject";
import { useForm, useFormContext } from "react-hook-form";
import SelectTeachingLocations from "../../user/settings/user-profile/components/SelectTeachingLocations";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import qs from "query-string"; // Sử dụng query-string để tạo URL query
import {
  AddressProvinceResponse,
  AddressProvinceType,
  fetchAddress2,
} from "../../user/settings/user-profile/components/AddressAutocomplete";
import PriceInput from "@/components/ui/price-input";

const SearchBar = () => {
  const { data, isLoading } = useSubjectList();
  const form = useFormContext();
  const router = useRouter();
  const pathname = usePathname();
  const formValues = form.watch(); // Lắng nghe tất cả giá trị trong form
  const [cities, setCities] = useState<AddressProvinceResponse[]>([]);
  const [districts, setDistricts] = useState<AddressProvinceResponse[]>([]);

  const searchParams = useSearchParams();
  const selectedCity = form.watch("city");

  useEffect(() => {
    const fetchCities = async () => {
      const data = await fetchAddress2({
        addressType: AddressProvinceType.City,
      });
      setCities(data);
    };
    fetchCities();

    if (selectedCity) {
      const fetchDistricts = async () => {
        const data = await fetchAddress2({
          addressType: AddressProvinceType.District,
          code: selectedCity,
        });
        setDistricts(data);
      };
      fetchDistricts();
    }
  }, [selectedCity]);

  // Lắng nghe sự thay đổi của formValues và cập nhật query
  useEffect(() => {
    const newQuery = qs.stringify({ ...searchParams, ...formValues });
    router.replace(`${pathname}?${newQuery}`, undefined);
  }, [formValues, pathname, router, searchParams]);
  return (
    <div>
      <div className="space-y-4">
        <div className="w-full flex gap-4 justify-between">
          <FormField
            control={form.control}
            name="subjects"
            render={({ field, fieldState }) => (
              <FormItem className="flex-1">
                <FormLabel>Chọn môn học</FormLabel>
                <FormControl>
                  <Select
                    mode="multiple"
                    placeholder="Chọn môn học"
                    optionFilterProp="label"
                    loading={isLoading}
                    className="w-full"
                    {...field}
                    options={
                      data?.map((subject) => ({
                        value: subject.subjectId?.toString(),
                        label: subject.subjectName,
                      })) || []
                    }
                    onChange={(value) => {
                      field.onChange(value);
                      console.log(value);
                    }} // Cập nhật giá trị khi chọn
                  />
                </FormControl>

                {fieldState.error ? (
                  <FormMessage>{fieldState.error.message}</FormMessage>
                ) : null}
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="minPrice"
            render={({ field, fieldState }) => (
              <FormItem className="flex-1">
                <FormLabel>Giá tiền tối thiểu</FormLabel>
                <FormControl>
                  <PriceInput field={field} />
                </FormControl>
                {fieldState.error ? (
                  <FormMessage>{fieldState.error.message}</FormMessage>
                ) : null}
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="maxPrice"
            render={({ field, fieldState }) => (
              <FormItem className="flex-1">
                <FormLabel>Giá tiền tối đa</FormLabel>
                <FormControl>
                  <PriceInput field={field} />
                </FormControl>
                {fieldState.error ? (
                  <FormMessage>{fieldState.error.message}</FormMessage>
                ) : null}
              </FormItem>
            )}
          />
        </div>

        <div className="w-full flex gap-4 justify-between">
          <FormField
            control={form.control}
            name="city"
            render={({ field }) => (
              <FormItem className="flex-1 flex flex-col gap-3">
                <FormLabel>Thành phố mong muốn</FormLabel>
                <FormControl>
                  <Select
                    {...field}
                    options={cities.map((city) => ({
                      value: city.code.toString(),
                      label: city.name,
                    }))}
                    onChange={(value) => {
                      field.onChange(value);
                      form.setValue("district", null);
                    }}
                    placeholder="Chọn thành phố"
                    allowClear
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="district"
            render={({ field }) => (
              <FormItem className="flex-1 flex flex-col gap-3">
                <FormLabel>Quận huyện mong muốn</FormLabel>
                <FormControl>
                  <Select
                    {...field}
                    disabled={!selectedCity}
                    options={districts.map((district) => ({
                      value: district.code.toString(),
                      label: district.name,
                    }))}
                    placeholder="Chọn quận huyện"
                    allowClear
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="searchingQuery"
            render={({ field, fieldState }) => (
              <FormItem className="flex-1">
                <FormLabel>Tìm kiếm</FormLabel>
                <FormControl>
                  <Input
                    placeholder="Tìm kiếm theo tên hoặc từ khóa"
                    {...field}
                  />
                </FormControl>
                {fieldState.error ? (
                  <FormMessage>{fieldState.error.message}</FormMessage>
                ) : null}
              </FormItem>
            )}
          />
        </div>
      </div>
    </div>
  );
};

export default SearchBar;
