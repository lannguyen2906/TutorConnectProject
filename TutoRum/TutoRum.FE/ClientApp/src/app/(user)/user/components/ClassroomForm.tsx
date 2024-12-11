"use client";
import {
  Form,
  FormControl,
  FormField,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Button, Input, Select } from "antd";
import FormItem from "antd/es/form/FormItem";
import React from "react";
import { useForm } from "react-hook-form";
import SubjectSelect from "../../tutor-requests/components/SubjectSelect";
import PriceInput from "@/components/ui/price-input";
import AddressAutocomplete from "../settings/user-profile/components/AddressAutocomplete";
import SelectFreeSchedule from "../settings/user-profile/components/SelectFreeSchedule";
import { useAppContext } from "@/components/provider/app-provider";
import { useClassroomContext } from "@/components/provider/classroom-provider";

const ClassroomForm = ({ disabled }: { disabled?: boolean }) => {
  const form = useForm();
  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(console.log)}>
        <div className="flex gap-10">
          {/* Tên lớp */}
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem className="w-full md:w-1/2">
                <FormLabel required>Tên lớp</FormLabel>
                <Input {...field} placeholder="Tên lớp" disabled={disabled} />
                <FormMessage />
              </FormItem>
            )}
          />

          {/* Học viên */}
          <FormField
            control={form.control}
            name="learner"
            render={({ field }) => (
              <FormItem className="w-full md:w-1/2">
                <FormLabel required>Học viên</FormLabel>
                <Select
                  {...field}
                  placeholder="Học viên"
                  options={[
                    { value: "1", label: "Học sinh A" },
                    { value: "2", label: "Học sinh B" },
                  ]}
                  allowClear
                  showSearch
                  optionFilterProp="label"
                  disabled={disabled}
                />
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        <div className="flex gap-10">
          {/* Môn học */}
          <FormField
            control={form.control}
            name="subject"
            render={({ field }) => (
              <FormItem className="w-full md:w-1/2">
                <FormLabel required>Môn học</FormLabel>
                <SubjectSelect field={field} disabled={disabled} />
                <FormMessage />
              </FormItem>
            )}
          />

          {/* Giá/1h */}
          <FormField
            control={form.control}
            name="fee"
            render={({ field }) => (
              <FormItem className="w-full md:w-1/2">
                <div className="flex-col flex gap-1 ">
                  <FormLabel required>Giá/1h</FormLabel>
                  <PriceInput field={field} disabled={disabled} />
                </div>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        {/* Địa chỉ */}
        <div className="w-full space-y-4">
          <FormLabel required>Địa chỉ</FormLabel>
          <AddressAutocomplete form={form} disabled={disabled} />
        </div>

        <div className="mt-5">
          {/* Khung giờ học */}
          <FormField
            control={form.control}
            name="schedule"
            render={({ field }) => (
              <FormItem className="w-full">
                <FormLabel required>Khung giờ học</FormLabel>
                <FormControl>
                  <SelectFreeSchedule disabled={disabled} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        {!disabled && (
          <div className="text-end">
            <Button type="primary" htmlType="submit">
              Tạo lớp
            </Button>
          </div>
        )}
      </form>
    </Form>
  );
};

export default ClassroomForm;
