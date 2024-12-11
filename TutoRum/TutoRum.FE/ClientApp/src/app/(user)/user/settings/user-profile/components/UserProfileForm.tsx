"use client";
import { Calendar } from "@/components/ui/calendar";
import {
  FormMessage,
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils";
import {
  UserProfileBodyType,
  UserProfileSchema,
} from "@/utils/schemaValidations/user.profile.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Dayjs } from "dayjs";
import { CalendarIcon } from "lucide-react";
import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { vi } from "date-fns/locale";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { FaMale, FaFemale } from "react-icons/fa";
import AddressAutocomplete from "./AddressAutocomplete";
import { useAppContext } from "@/components/provider/app-provider";
import { UpdateUserDTO } from "@/utils/services/Api";
import { useUpdateProfile } from "@/hooks/use-profile";
import { Button, DatePicker } from "antd";
import dayjs from "dayjs";
import { toast } from "react-toastify";

const UserProfileForm = () => {
  const [loading, setLoading] = useState(false);
  const { user } = useAppContext();
  const form = useForm<UserProfileBodyType>({
    resolver: zodResolver(UserProfileSchema),
    defaultValues: { ...user },
  });

  const { mutateAsync: updateProfile, isLoading: updateProfileLoading } =
    useUpdateProfile();
  const selectedGender = form.watch("gender");

  // Reset form when the user data changes
  useEffect(() => {
    form.reset({ ...user });
  }, [form, user]);

  async function onSubmit(values: UserProfileBodyType) {
    setLoading(true);

    const profileDto: UpdateUserDTO = {
      addressDetail: values.address,
      cityId: values.city,
      districtId: values.district,
      wardId: values.ward,
      dob: values.dateOfBirth,
      fullname: values.fullName,
      gender: values.gender === "male",
      phoneNumber: values.phoneNumber,
    };

    try {
      const { data } = await updateProfile(profileDto);
      if (data.status === 200) {
        toast.success("Cập nhật thành công");
      }
    } catch (err) {
      toast.error("Cập nhật không thành công");
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  return (
    <Form {...form}>
      <form className="space-y-8" onSubmit={form.handleSubmit(onSubmit)}>
        <div className="flex flex-col xl:flex-row w-full justify-between gap-10">
          <FormField
            control={form.control}
            name="fullName"
            render={({ field }) => (
              <FormItem className="w-full xl:w-1/2">
                <FormLabel required>Họ và tên</FormLabel>
                <FormControl>
                  <Input placeholder="Nguyễn Văn A" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="phoneNumber"
            render={({ field }) => (
              <FormItem className="w-full xl:w-1/2">
                <FormLabel>Số điện thoại</FormLabel>
                <FormControl>
                  <Input placeholder="0987654321" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        <div className="flex flex-col xl:flex-row w-full justify-between gap-10">
          <FormField
            control={form.control}
            name="email"
            render={({ field }) => (
              <FormItem className="w-full xl:w-1/2">
                <FormLabel required>Email</FormLabel>
                <FormControl>
                  <Input
                    disabled={!!user}
                    placeholder="vidu@gmail.com"
                    {...field}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <div className="flex flex-col xl:flex-row w-full xl:w-1/2 justify-between gap-5">
            <FormField
              control={form.control}
              name="gender"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel>Giới tính</FormLabel>
                  <FormControl>
                    <RadioGroup
                      onValueChange={field.onChange}
                      defaultValue={field.value}
                      className="flex"
                    >
                      <FormItem>
                        <FormLabel
                          className={cn(
                            "font-normal border-2 rounded-lg p-2 flex gap-1 cursor-pointer",
                            { "bg-black text-white": selectedGender === "male" }
                          )}
                        >
                          <span>Nam</span>
                          <FaMale
                            color={
                              selectedGender === "male" ? "white" : "black"
                            }
                          />
                        </FormLabel>
                        <FormControl>
                          <RadioGroupItem value="male" hidden={true} />
                        </FormControl>
                      </FormItem>

                      <FormItem>
                        <FormLabel
                          className={cn(
                            "font-normal border-2 rounded-lg p-2 flex gap-1 cursor-pointer",
                            {
                              "bg-black text-white":
                                selectedGender === "female",
                            }
                          )}
                        >
                          <span>Nữ</span>
                          <FaFemale
                            color={
                              selectedGender === "female" ? "white" : "black"
                            }
                          />
                        </FormLabel>
                        <FormControl>
                          <RadioGroupItem value="female" hidden={true} />
                        </FormControl>
                      </FormItem>
                    </RadioGroup>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="dateOfBirth"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2 flex flex-col gap-2">
                  <FormLabel>Ngày sinh</FormLabel>
                  <FormControl>
                    <DatePicker
                      placeholder="Chọn ngày sinh"
                      onChange={(date: Dayjs, dateString) =>
                        field.onChange(dateString)
                      }
                      value={field.value ? dayjs(field.value) : null}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        </div>

        <AddressAutocomplete form={form} />

        <div className="w-full flex justify-end">
          <Button
            htmlType="submit"
            type="primary"
            disabled={!form.formState.isDirty || updateProfileLoading}
            loading={updateProfileLoading}
          >
            Cập nhật thông tin
          </Button>
        </div>
      </form>
    </Form>
  );
};

export default UserProfileForm;
