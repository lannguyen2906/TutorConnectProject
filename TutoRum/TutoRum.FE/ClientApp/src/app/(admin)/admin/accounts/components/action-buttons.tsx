import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  useAccountList,
  useAssignRole,
  useBlockUser,
  useUnblockUser,
} from "@/hooks/admin/use-accounts";
import {
  AssignRoleSchema,
  AssignRoleType,
} from "@/utils/schemaValidations/account.schema";
import { AssignRoleAdminDto } from "@/utils/services/Api";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, DatePicker, Input, Modal, Select } from "antd";
import { formatDate } from "date-fns";
import dayjs, { Dayjs } from "dayjs";
import { Eye, Shield, UserCog } from "lucide-react";
import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

export const DetailButton = () => {
  return (
    <button title="Chi tiết">
      <Eye className="text-Blueviolet" />
    </button>
  );
};

export const AssignRoleButton = ({ id }: { id: string }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { mutateAsync: assignRole, isLoading } = useAssignRole();
  const { data: accounts } = useAccountList();
  const form = useForm<AssignRoleType>({
    resolver: zodResolver(AssignRoleSchema),
    defaultValues: {
      hireDate: dayjs().toISOString(),
    },
  });

  const showModal = () => {
    setIsModalOpen(true);
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  async function onSubmit(values: AssignRoleType) {
    const profileDto: AssignRoleAdminDto = {
      userId: id,
      hireDate: values.hireDate,
      position: values.position,
      salary: Number.parseInt(values.salary),
      supervisorId: 0,
    };

    try {
      const { data } = await assignRole(profileDto);
      if (data.status === 200) {
        toast.success("Cập nhật thành công");
      }
    } catch (err) {
      toast.error("Cập nhật không thành công");
      console.log(err);
    }
  }

  return (
    <>
      <button title="Cấp quyền" onClick={showModal}>
        <UserCog className="text-slategray" />
      </button>
      <Modal
        title="Cấp quyền cho người dùng"
        open={isModalOpen}
        onCancel={handleCancel}
        footer={[
          <Button
            onClick={form.handleSubmit(onSubmit)}
            key="submit"
            form="assign-role-form"
            disabled={isLoading}
            type="primary"
          >
            Cấp quyền
          </Button>,
        ]}
      >
        <Form {...form}>
          <form
            id="assign-role-form"
            className="space-y-8"
            onSubmit={form.handleSubmit(onSubmit)}
          >
            <FormField
              control={form.control}
              name="position"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Chức danh</FormLabel>
                  <FormControl>
                    <Input placeholder="Quản lý nhân sự" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="flex gap-5">
              <FormField
                control={form.control}
                name="hireDate"
                render={({ field }) => (
                  <FormItem className="w-1/2">
                    <FormLabel>Ngày tuyển dụng</FormLabel>
                    <FormControl>
                      <DatePicker
                        className="w-full"
                        placeholder="Chọn ngày tuyển dụng"
                        onChange={(date: Dayjs, dateString) =>
                          field.onChange(dateString)
                        }
                        value={dayjs(field.value)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="salary"
                render={({ field }) => (
                  <FormItem className="w-1/2">
                    <FormLabel>Mức lương/ tháng</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        placeholder="300000"
                        {...field}
                        addonAfter="VNĐ"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <FormField
              control={form.control}
              name="supervisorId"
              render={({ field }) => (
                <FormItem className="w-1/2">
                  <FormLabel>Người giám sát</FormLabel>
                  <FormControl>
                    <Select
                      className="w-full"
                      showSearch
                      placeholder="Chọn người giám sát"
                      optionFilterProp="label"
                      onChange={field.onChange}
                      options={accounts?.map((item) => ({
                        value: item.userId,
                        label: item.fullname,
                      }))}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </form>
        </Form>
      </Modal>
    </>
  );
};

export const BlockUserButton = ({
  id,
  isBlocked,
}: {
  id: string;
  isBlocked: boolean;
}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { mutateAsync: blockUser, isLoading: blockLoading } = useBlockUser();
  const { mutateAsync: unBlockUser, isLoading: unblockLoading } =
    useUnblockUser();

  const showModal = () => {
    setIsModalOpen(true);
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  async function handleBlockUser() {
    try {
      const { data } = isBlocked ? await unBlockUser(id) : await blockUser(id);
      if (data?.status === 200) {
        toast.success(
          data.message || isBlocked
            ? "Kích hoạt người dùng thành công"
            : "Chặn người dùng thành công"
        );

        setIsModalOpen(false);
      }
    } catch (err) {
      toast.error("Chặn người dùng không thành công");
      console.error(err);
    }
  }

  return (
    <>
      <button title={isBlocked ? "Kích hoạt người dùng" : "Chặn người dùng"}>
        <Shield className="text-red" onClick={showModal} />
      </button>
      <Modal
        title={isBlocked ? "Kích hoạt người dùng" : "Chặn người dùng"}
        open={isModalOpen}
        onCancel={handleCancel}
        footer={[
          <Button
            onClick={handleBlockUser}
            key="submit"
            form="assign-role-form"
            disabled={blockLoading || unblockLoading}
            type="primary"
            danger={!isBlocked}
          >
            {isBlocked ? "Kích hoạt" : "Chặn"}
          </Button>,
        ]}
      >
        <p>
          {" "}
          Bạn có chắc chắn muốn {isBlocked ? "kích hoạt" : "chặn"} người dùng
          này?
        </p>
      </Modal>
    </>
  );
};
