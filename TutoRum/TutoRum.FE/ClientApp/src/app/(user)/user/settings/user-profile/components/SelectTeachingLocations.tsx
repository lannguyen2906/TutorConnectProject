"use client";
import React, { useEffect, useState } from "react";
import { Button, Select } from "antd";
import {
  Controller,
  useFieldArray,
  useFormContext,
  useWatch,
} from "react-hook-form";
import {
  AddressProvinceResponse,
  AddressProvinceType,
  fetchAddress2,
} from "./AddressAutocomplete";
import { Home } from "lucide-react";

type Districts = {
  province_code: string;
  districts: AddressProvinceResponse[];
}[];

const SelectTeachingLocations = () => {
  const { control, setValue } = useFormContext(); // Lấy form context từ react-hook-form
  const [cities, setCities] = useState<AddressProvinceResponse[]>([]);
  const [districts, setDistricts] = useState<Districts>([]);
  const { fields, append, remove } = useFieldArray({
    control,
    name: "teachingLocations", // Tên field trong form
  });

  // Theo dõi các giá trị của tất cả các thành phố trong form
  const selectedCities = useWatch({
    control,
    name: fields.map((_, index) => `teachingLocations.${index}.cityId`),
  });

  useEffect(() => {
    const fetchCities = async () => {
      const data = await fetchAddress2({
        addressType: AddressProvinceType.City,
      });
      setCities(data);
    };

    if (selectedCities.length > 0) {
      selectedCities.forEach((item, index: number) => {
        fetchDistricts(item);
      });
    }

    fetchCities();
  }, [selectedCities]);

  const fetchDistricts = async (cityId: string) => {
    const data = await fetchAddress2({
      addressType: AddressProvinceType.District,
      code: cityId,
    });
    setDistricts((prev) => {
      const index = prev.findIndex((item) => item.province_code == cityId);
      if (index !== -1) {
        return [
          ...prev.slice(0, index),
          { province_code: cityId, districts: data },
          ...prev.slice(index + 1),
        ];
      }
      return [...prev, { province_code: cityId, districts: data }]; // If not found, add new item
    });
  };

  const handleCityChange = async (index: number, cityId: string) => {
    if (cityId) {
      setValue(`teachingLocations.${index}.districtIds`, []); // Reset districtIds khi city thay đổi
      setValue(`teachingLocations.${index}.cityId`, cityId); // Cập nhật giá trị cityId
      fetchDistricts(cityId);
    }
  };

  return (
    <div>
      {fields.map((item, index: number) => (
        <div
          key={item.id}
          className="w-full flex justify-between mb-2 gap-2 items-center"
        >
          <div className="flex flex-col lg:flex-row flex-1 gap-2">
            <div className="flex-1">
              {/* City Select */}
              <Controller
                name={`teachingLocations.${index}.cityId`}
                control={control}
                render={({ field }) => (
                  <Select
                    {...field}
                    showSearch
                    placeholder="Chọn thành phố"
                    optionFilterProp="label"
                    className="w-full"
                    options={cities?.map((city) => ({
                      value: city.code.toString(),
                      label: city.name,
                    }))}
                    onChange={(value) => handleCityChange(index, value)} // Gọi hàm thay đổi city
                  />
                )}
              />
            </div>
            <div className="flex-1">
              {/* District Multiple Select */}
              <Controller
                name={`teachingLocations.${index}.districtIds`}
                control={control}
                render={({ field }) => (
                  <Select
                    {...field}
                    mode="multiple"
                    placeholder="Chọn quận"
                    optionFilterProp="label"
                    showSearch
                    className="w-full"
                    onChange={(value) => field.onChange(value)}
                    options={districts
                      .find(
                        (item) => item.province_code == selectedCities[index]
                      )
                      ?.districts?.map((district) => ({
                        value: district.code.toString(),
                        label: district.name,
                      }))}
                    disabled={!selectedCities[index]} // Disable nếu chưa chọn city
                  />
                )}
              />
            </div>
          </div>

          {/* Nút xóa */}
          <Button
            danger
            type="dashed"
            onClick={() => remove(index)}
            className="h-full"
          >
            Xóa
          </Button>
        </div>
      ))}

      {/* Nút thêm địa điểm mới */}
      <Button
        type="dashed"
        className="w-full"
        onClick={() => append({ cityId: "", districtIds: [] })}
      >
        + Thêm thành phố <Home size={16} className="text-slategray" />
      </Button>
    </div>
  );
};

export default SelectTeachingLocations;
