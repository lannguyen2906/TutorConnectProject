import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { cn } from "@/lib/utils";
import { UserProfileBodyType } from "@/utils/schemaValidations/user.profile.schema";
import axios from "axios";
import { Check, ChevronsUpDown } from "lucide-react";
import { useEffect, useState } from "react";
import { UseFormReturn } from "react-hook-form";

export enum AddressType {
  City = 1,
  District = 2,
  Ward = 3,
}

export enum AddressProvinceType {
  City = "p",
  District = "d",
  Ward = "w",
}

interface AddressApi {
  addressType: AddressType;
  id: string;
}

interface AddressProvinceApi {
  addressType: AddressProvinceType;
  code?: string;
}

export interface AddressResponse {
  id: string;
  name: string;
  full_name: string;
}

export interface AddressProvinceResponse {
  name: string;
  code: string;
  province_code?: string;
  district_code?: string;
  districts?: AddressProvinceResponse[];
  wards?: AddressProvinceResponse[];
}

export const fetchAddress = async (params: AddressApi) => {
  try {
    const { data } = await axios.get(
      `https://esgoo.net/api-tinhthanh/${params.addressType}/${params.id}.htm`
    );
    return data.data || [];
  } catch (error) {
    console.error("Error fetching address data:", error);
  }
};

export const fetchAddress2 = async (params: AddressProvinceApi) => {
  try {
    switch (params.addressType) {
      case AddressProvinceType.City:
        const { data } = await axios.get(`https://provinces.open-api.vn/api/p`);
        return data;
      case AddressProvinceType.District:
        const { data: province } = await axios.get(
          `https://provinces.open-api.vn/api/p/${params.code}?depth=2`
        );
        return province.districts;
      case AddressProvinceType.Ward:
        const { data: district } = await axios.get(
          `https://provinces.open-api.vn/api/d/${params.code}?depth=2`
        );
        return district.wards;
    }
  } catch (error) {
    console.error("Error fetching address data:", error);
  }
};

// Reusable select field component
const AddressSelectField = ({
  label,
  fieldName,
  options,
  form,
  isDisabled,
  placeholder,
}: {
  label: string;
  fieldName: string;
  options: AddressProvinceResponse[];
  form: UseFormReturn;
  isDisabled?: boolean;
  placeholder: string;
}) => {
  const selectedValue = form.watch(fieldName);
  return (
    <FormField
      control={form.control}
      name={fieldName}
      render={({ field }) => (
        <FormItem className="flex flex-col w-full">
          <FormLabel>{label}</FormLabel>
          <Popover>
            <PopoverTrigger asChild>
              <FormControl>
                <Button
                  variant="outline"
                  role="combobox"
                  className={cn(
                    "w-full justify-between",
                    isDisabled && "text-muted-foreground cursor-not-allowed",
                    !field.value && "text-muted-foreground"
                  )}
                  disabled={isDisabled}
                >
                  {field.value
                    ? options?.find((option) => option.code == field.value)
                        ?.name
                    : placeholder}
                  <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </FormControl>
            </PopoverTrigger>
            <PopoverContent align="start" className="w-60 xl:w-96 p-0">
              <Command>
                <CommandInput placeholder={`Tìm ${label.toLowerCase()}...`} />
                <CommandList>
                  <CommandEmpty>{`Không tìm thấy ${label.toLowerCase()}.`}</CommandEmpty>
                  <CommandGroup>
                    {options?.map((option) => (
                      <CommandItem
                        key={option.code}
                        onSelect={() =>
                          form.setValue(fieldName, option.code.toString())
                        }
                      >
                        <Check
                          className={cn(
                            "mr-2 h-4 w-4",
                            option.code === field.value
                              ? "opacity-100"
                              : "opacity-0"
                          )}
                        />
                        {option.name}
                      </CommandItem>
                    ))}
                  </CommandGroup>
                </CommandList>
              </Command>
            </PopoverContent>
          </Popover>
          <FormMessage />
        </FormItem>
      )}
    />
  );
};

const AddressAutocomplete = ({
  form,
  disabled,
}: {
  form: any;
  disabled?: boolean;
}) => {
  const [cities, setCities] = useState<AddressProvinceResponse[]>([]);
  const [districts, setDistricts] = useState<AddressProvinceResponse[]>([]);
  const [wards, setWards] = useState<AddressProvinceResponse[]>([]);

  const selectedCity = form.watch("city");
  const selectedDistrict = form.watch("district");
  const selectedWard = form.watch("ward");

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

    if (selectedDistrict) {
      const fetchWards = async () => {
        const data = await fetchAddress2({
          addressType: AddressProvinceType.Ward,
          code: selectedDistrict,
        });
        setWards(data);
      };
      fetchWards();
    }
  }, [selectedCity, selectedDistrict]);

  return (
    <div className="w-full flex flex-col xl:flex-row justify-between gap-10">
      <div className="flex justify-between gap-5 w-full xl:w-1/2">
        {/* City Selector */}
        <AddressSelectField
          label="Thành phố"
          fieldName="city"
          options={cities}
          form={form}
          placeholder="Chọn thành phố"
          isDisabled={disabled}
        />

        {/* District Selector */}
        <AddressSelectField
          label="Quận/ Huyện"
          fieldName="district"
          options={districts}
          form={form}
          isDisabled={!selectedCity || disabled}
          placeholder="Chọn Quận/Huyện"
        />
      </div>

      <div className="flex justify-between gap-5 w-full xl:w-1/2">
        {/* Ward Selector */}
        <AddressSelectField
          label="Phường/ Xã"
          fieldName="ward"
          options={wards}
          form={form}
          isDisabled={!selectedDistrict || disabled}
          placeholder="Chọn Phường/Xã"
        />

        {/* Address Detail */}
        <FormField
          control={form.control}
          name="address"
          render={({ field }) => (
            <FormItem className="flex flex-col w-full">
              <FormLabel>Địa chỉ chi tiết</FormLabel>
              <FormControl>
                <Input
                  disabled={!selectedWard || disabled}
                  placeholder="Nhà số 0"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </div>
  );
};

export default AddressAutocomplete;
