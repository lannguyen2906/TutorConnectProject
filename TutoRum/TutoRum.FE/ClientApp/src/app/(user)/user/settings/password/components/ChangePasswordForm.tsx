"use client";
import ForgotPasswordDialog from "@/app/(user)/(auth)/login/components/ForgotPasswordDialog";
import { useAppContext } from "@/components/provider/app-provider";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { PasswordInput } from "@/components/ui/pasword-input";
import {
  ChangePasswordBodySchema,
  ChangePasswordType,
} from "@/utils/schemaValidations/auth.schema";
import { tConnectService } from "@/utils/services/tConnectService";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

const ChangePasswordForm = () => {
  const { refetch, remove } = useAppContext();
  const [loading, setLoading] = useState(false);
  const router = useRouter();
  const form = useForm<ChangePasswordType>({
    resolver: zodResolver(ChangePasswordBodySchema),
  });

  async function onSubmit(values: ChangePasswordType) {
    setLoading(true); // Bắt đầu loading
    try {
      const { data } = await tConnectService.api.accountsChangePasswordCreate(
        values
      );

      if (data.status == 200) {
        toast.success(data.message || "Đổi mật khẩu thành công");
      }
      await tConnectService.api.accountsSignOutCreate();
      remove();
      refetch();
      router.push("/login");
      router.refresh();
    } catch (err) {
      toast.error("Đổi mật khẩu không thành công");
      console.log(err);
    } finally {
      setLoading(false); // Kết thúc loading
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          control={form.control}
          name="currentPassword"
          render={({ field }) => (
            <FormItem>
              <FormLabel className="flex justify-between">
                <span>Mật khẩu hiện tại</span>
                <ForgotPasswordDialog />
              </FormLabel>
              <FormControl>
                <PasswordInput {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="newPassword"
          render={({ field }) => (
            <FormItem>
              <FormLabel required>Mật khẩu mới</FormLabel>
              <FormControl>
                <PasswordInput {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="confirmPassword"
          render={({ field }) => (
            <FormItem>
              <FormLabel required>Nhập lại mật khẩu mới</FormLabel>
              <FormControl>
                <PasswordInput {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button disabled={loading} type="submit">
          Thay đổi mật khẩu
        </Button>
      </form>
    </Form>
  );
};

export default ChangePasswordForm;
