"use client";

import { useAppContext } from "@/components/provider/app-provider";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import PriceInput from "@/components/ui/price-input";
import { useCreatePaymentRequest } from "@/hooks/use-payment-request";
import { formatNumber } from "@/utils/other/formatter";
import { CreatePaymentRequestDTO } from "@/utils/services/Api";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input, Modal, Select } from "antd";
import Image from "next/image";
import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { z } from "zod";

type Bank = {
  bin: string;
  code: string;
  id: number;
  isTransfer: number;
  logo: string;
  lookupSupported: number;
  name: string;
  shortName: string;
  short_name: string;
  support: number;
  swift_code: string;
  transferSupported: number;
};

const PaymentRequestButton = () => {
  const [isModalOpen, setIsModalOpen] = React.useState(false);
  const [banks, setBanks] = React.useState<Bank[]>([]);
  const { mutateAsync, isLoading } = useCreatePaymentRequest();
  const { user } = useAppContext();

  // Schema validation với Zod
  const withdrawalSchema = z.object({
    fullName: z.string().min(2, "Họ và tên phải có ít nhất 2 ký tự"),
    bankCode: z.string().min(1, "Vui lòng chọn ngân hàng"),
    accountNumber: z
      .string()
      .regex(/^\d+$/, "Số tài khoản chỉ được chứa số")
      .min(6, "Số tài khoản phải có ít nhất 6 chữ số"),
    amount: z
      .number({ invalid_type_error: "Số tiền phải là một số hợp lệ" })
      .min(50000, "Số tiền tối thiểu để rút là 50,000 VNĐ")
      .max(
        user?.balance ?? 0,
        "Số tiền rút vượt quá số dư hiện tại " +
          formatNumber(user?.balance.toString() ?? "0") +
          " VND"
      ),
  });
  type WithdrawalFormValues = z.infer<typeof withdrawalSchema>;

  const form = useForm<WithdrawalFormValues>({
    resolver: zodResolver(withdrawalSchema),
  });
  // Fetch danh sách ngân hàng
  useEffect(() => {
    const fetchBanks = async () => {
      try {
        const response = await fetch("https://api.vietqr.io/v2/banks");
        const data = await response.json();
        setBanks(data.data);
      } catch (error) {
        console.error("Error fetching banks:", error);
      }
    };
    fetchBanks();
  }, []);

  const onSubmit = async (values: WithdrawalFormValues) => {
    const request: CreatePaymentRequestDTO = {
      accountNumber: values.accountNumber,
      amount: values.amount,
      bankCode: values.bankCode,
      fullName: values.fullName,
    };

    try {
      const response = await mutateAsync(request);
      if (response.status === 201) {
        toast.success("Yêu cầu rút tiền thành công");
        setIsModalOpen(false);
        form.reset();
      }
    } catch (error) {
      toast.error("Lỗi khi tạo yêu cầu rút tiền");
    } finally {
    }
  };

  return (
    <div>
      <Button onClick={() => setIsModalOpen(true)} type="primary">
        Yêu cầu rút tiền
      </Button>
      <Modal
        title="Yêu cầu rút tiền"
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={[
          <Button key="cancel" onClick={() => setIsModalOpen(false)}>
            Hủy
          </Button>,
          <Button
            key="submit"
            type="primary"
            onClick={form.handleSubmit(onSubmit)}
            loading={isLoading}
          >
            Xác nhận
          </Button>,
        ]}
      >
        <Form {...form}>
          <form className="space-y-4">
            {/* Ngân hàng */}
            <FormField
              control={form.control}
              name="bankCode"
              render={({ field }) => (
                <FormItem>
                  <FormLabel required>Ngân hàng</FormLabel>
                  <div className="w-full">
                    <Select
                      style={{ width: "100%" }}
                      placeholder="Gõ code ngân hàng để tìm"
                      {...field}
                      onChange={field.onChange}
                      showSearch
                      allowClear
                      options={banks.map((bank) => ({
                        value: bank.code,
                        label: (
                          <div
                            style={{ display: "flex", alignItems: "center" }}
                          >
                            <Image
                              src={bank.logo}
                              alt={bank.name}
                              width={20}
                              height={20}
                              style={{
                                marginRight: 8,
                                width: "auto",
                                height: "auto",
                              }}
                            />
                            {bank.name}
                          </div>
                        ),
                      }))}
                    />
                  </div>

                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Số tài khoản */}
            <FormField
              control={form.control}
              name="accountNumber"
              render={({ field }) => (
                <FormItem>
                  <FormLabel required>Số tài khoản</FormLabel>
                  <Input placeholder="Nhập số tài khoản" {...field} />
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Họ và tên */}
            <FormField
              control={form.control}
              name="fullName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel required>Họ và tên</FormLabel>
                  <Input placeholder="Nhập họ và tên" {...field} />
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Số tiền */}
            <FormField
              control={form.control}
              name="amount"
              render={({ field }) => (
                <FormItem>
                  <div className="flex justify-between">
                    <FormLabel required>Số tiền muốn rút</FormLabel>
                    <span className="text-sm font-semibold text-Blueviolet">
                      Số dư: {formatNumber(user?.balance.toString() ?? "0")} VND
                    </span>
                  </div>
                  <PriceInput field={field} addonAfter="VND" />
                  <FormMessage />
                </FormItem>
              )}
            />
          </form>
        </Form>
      </Modal>
    </div>
  );
};

export default PaymentRequestButton;
