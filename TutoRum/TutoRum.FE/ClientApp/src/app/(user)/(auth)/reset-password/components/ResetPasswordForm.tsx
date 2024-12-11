"use client";

import { Input } from "@/components/ui/input";
import React from "react";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { PasswordInput } from "@/components/ui/pasword-input";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { tConnectService } from "@/utils/services/tConnectService";
import { Button } from "@/components/ui/button";
import { useEffect } from "react";

import {
  ResetPasswordBodySchema,
  ResetPasswordType,
} from "@/utils/schemaValidations/auth.schema";
import { useAppContext } from "@/components/provider/app-provider";
import { toast } from "react-toastify";
const ResetPasswordForm = () => {
  const [loading, setLoading] = useState(false);
  const router = useRouter();
  const { refetch, remove } = useAppContext();
  const form = useForm<ResetPasswordType>({
    resolver: zodResolver(ResetPasswordBodySchema),
  });
  // Cập nhật giá trị email vào form khi trang load
  useEffect(() => {
    // Get query from URL using window.location
    const params = new URLSearchParams(window.location.search);
    const token = params.get("token");
    const email = params.get("email");
    if (token) {
      form.setValue("token", token);
    }
    if (email) {
      form.setValue("email", email);
    }
  }, [form]);

  async function onSubmit(values: ResetPasswordType) {
    setLoading(true); // Bắt đầu loading
    try {
      const { data } = await tConnectService.api.accountsResetPasswordCreate(
        values
      );
      if (data.status == 200) {
        toast.success(data?.message ?? "Đổi mật khẩu thành công");
        await tConnectService.api.accountsSignOutCreate();
        remove();
        refetch();
        router.push("/login");
      }
    } catch (err) {
      toast.error("Đổi mật khẩu không thành công");
      console.log(err);
    } finally {
      setLoading(false); // Kết thúc loading
    }
  }
  return (
    <Form {...form}>
      <form
        className="w-1/2 space-y-5 p-28"
        onSubmit={form.handleSubmit(onSubmit)}
      >
        <div className="text-center text-3xl font-bold">Đổi mật khẩu</div>
        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem className="hidden">
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="token"
          render={({ field }) => (
            <FormItem className="hidden">
              <FormControl>
                <Input {...field} />
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
              <FormLabel>Mật khẩu</FormLabel>
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
              <FormLabel>Nhập lại Mật khẩu</FormLabel>
              <FormControl>
                <PasswordInput {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <Button className="w-full" disabled={loading}>
          {loading ? "Đổi mật khẩu..." : "Đổi mật khẩu"}
        </Button>
      </form>
    </Form>
  );
};

export default ResetPasswordForm;
