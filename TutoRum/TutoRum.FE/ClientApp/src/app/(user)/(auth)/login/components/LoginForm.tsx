"use client";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { PasswordInput } from "@/components/ui/pasword-input";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  clientService,
  tConnectService,
} from "@/utils/services/tConnectService";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { useAppContext } from "@/components/provider/app-provider";
import {
  LoginBodySchema,
  LoginBodyType,
} from "@/utils/schemaValidations/auth.schema";
import ForgotPasswordDialog from "./ForgotPasswordDialog";
import { Button } from "antd";
import { Separator } from "@/components/ui/separator";
import { toast } from "react-toastify";

export const LoginForm = () => {
  const [loading, setLoading] = useState(false);
  const { refetch } = useAppContext();
  const router = useRouter();

  const form = useForm<LoginBodyType>({
    resolver: zodResolver(LoginBodySchema),
  });

  async function onSubmit(values: LoginBodyType) {
    setLoading(true); // Bắt đầu loading
    try {
      const { data } = await tConnectService.api.accountsSignInCreate(values);

      if (data.status == 200) {
        await fetch("/api/auth/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            sessionToken: data.data?.roles?.includes("admin")
              ? "admin"
              : "user",
          }), // Gửi token vào body
        });
        toast.success("Đăng nhập thành công");
      }
      refetch();
      router.push("/");
      router.refresh();
    } catch (err) {
      toast.error("Đăng nhập không thành công");
      console.log(err);
    } finally {
      setLoading(false); // Kết thúc loading
    }
  }

  return (
    <Form {...form}>
      <form className="space-y-5" onSubmit={form.handleSubmit(onSubmit)}>
        <div className="text-center text-3xl font-bold">Đăng nhập</div>
        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Email</FormLabel>
              <FormControl>
                <Input placeholder="example@gmail.com" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="password"
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
        <div className="text-right">
          <ForgotPasswordDialog />
        </div>
        <Button
          type="primary"
          htmlType="submit"
          className="w-full"
          loading={loading}
        >
          Đăng nhập
        </Button>
      </form>
      <div className="flex my-5 items-center gap-5 mx-7 text-muted-foreground text-sm">
        <Separator className="flex-1" />
        Hoặc
        <Separator className="flex-1" />
      </div>
      <Button
        onClick={() => router.push("/signup")}
        htmlType="button"
        className="w-full"
      >
        Đăng ký
      </Button>
    </Form>
  );
};
