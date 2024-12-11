import { Select } from "antd";
import React, { useEffect, useState } from "react";
import { Controller, useFormContext } from "react-hook-form";
import {
  AddressResponse,
  AddressType,
  fetchAddress,
} from "../../user/settings/user-profile/components/AddressAutocomplete";

const SelectLocation = () => {
  const { watch, setValue } = useFormContext();
  const [cities, setCities] = useState<AddressResponse[]>([]);
  const [districts, setDistricts] = useState<AddressResponse[]>([]);
  const selectedCity = watch("city"); // Watching the selected city

  // Fetch cities and districts based on the selected city
  useEffect(() => {
    const fetchCities = async () => {
      const data = await fetchAddress({
        addressType: AddressType.City,
        id: "0",
      });
      setCities(data);
    };
    fetchCities();

    if (selectedCity) {
      const fetchDistricts = async () => {
        const data = await fetchAddress({
          addressType: AddressType.District,
          id: selectedCity,
        });
        setDistricts(data);
      };
      fetchDistricts();
    } else {
      setDistricts([]); // Reset districts if no city is selected
    }
  }, [selectedCity]);

  return (
    <div className="flex gap-2">
      <Controller
        name="city" // Name for the city
        render={({ field }) => (
          <Select
            {...field}
            options={cities.map((city) => ({
              label: city.full_name, // Display name
              value: city.id, // Value
            }))}
            showSearch
            optionFilterProp="label"
            placeholder="Chọn thành phố"
            allowClear
            onChange={(value) => {
              field.onChange(value); // Update the selected city
              setDistricts([]); // Reset districts when city changes
              setValue("districts", []);
            }}
          />
        )}
      />
      <Controller
        name="district" // Name for the districts
        render={({ field }) => (
          <Select
            {...field}
            options={districts.map((district) => ({
              label: district.full_name, // Display name
              value: district.id, // Value
            }))}
            placeholder="Chọn quận huyện"
            allowClear
            showSearch
            optionFilterProp="label"
            onChange={(value) => {
              field.onChange(value); // Update selected districts
            }}
            disabled={!selectedCity}
          />
        )}
      />
    </div>
  );
};

export default SelectLocation;
