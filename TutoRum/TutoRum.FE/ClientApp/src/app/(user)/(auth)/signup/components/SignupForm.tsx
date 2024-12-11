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
import { tConnectService } from "@/utils/services/tConnectService";
import { useState } from "react";
import { useRouter } from "next/navigation";
import {
  SignupBodySchema,
  SignupBodyType,
} from "@/utils/schemaValidations/auth.schema";
import { Button } from "antd";
import { toast } from "react-toastify";

export const SignupForm = () => {
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  const form = useForm<SignupBodyType>({
    resolver: zodResolver(SignupBodySchema),
  });

  async function onSubmit(values: SignupBodyType) {
    setLoading(true); // Bắt đầu loading
    try {
      const data = await tConnectService.api.accountsSignUpCreate(values);

      if (data.data.status == 200) {
        toast.success("Đăng ký thành công, vui lòng xác nhận qua email");
        router.push("/login");
      }
    } catch (err) {
      toast.error("Đăng ký không thành công");
      console.log(err);
    } finally {
      setLoading(false); // Kết thúc loading
    }
  }

  return (
    <Form {...form}>
      <form className="space-y-5" onSubmit={form.handleSubmit(onSubmit)}>
        <div className="text-center text-3xl font-bold">Đăng ký</div>
        <FormField
          control={form.control}
          name="fullName"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Họ và tên</FormLabel>
              <FormControl>
                <Input placeholder="Nguyễn Văn A" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
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
        <Button
          type="primary"
          htmlType="submit"
          className="w-full"
          loading={loading}
        >
          Đăng ký
        </Button>
      </form>
    </Form>
  );
};
