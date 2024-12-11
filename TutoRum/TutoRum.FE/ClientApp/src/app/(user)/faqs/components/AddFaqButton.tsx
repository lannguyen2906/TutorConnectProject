"use client";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { FaqSchema, FaqSchemaType } from "@/utils/schemaValidations/faq.schema";
import { tConnectService } from "@/utils/services/tConnectService";
import { zodResolver } from "@hookform/resolvers/zod";
import MDEditor from "@uiw/react-md-editor";
import { Button, Modal } from "antd";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

export function AddFaqButton() {
  const form = useForm<FaqSchemaType>({
    resolver: zodResolver(FaqSchema),
  });
  const [open, setOpen] = useState(false);
  async function onSubmit(values: FaqSchemaType) {
    try {
      console.log(values);
      setOpen(false);
    } catch (err) {
      toast("Có lỗi xảy ra");
      console.log(err);
    }
  }

  return (
    <div>
      <button onClick={() => setOpen(true)}>
        <div className="group bg-white h-12 w-12 shadow-xl text-base rounded-full flex items-center justify-center hover:bg-black">
          <Plus className="group-hover:text-white transition-all duration-400" />
        </div>
      </button>
      <Modal
        open={open}
        onCancel={() => setOpen(false)}
        footer={null}
        className="max-w-5xl"
        title="Thêm mới FAQ"
        getContainer={false}
      >
        <Form {...form}>
          <form
            id="forgot-password"
            className="space-y-6 mt-5"
            onSubmit={(e) => {
              e.preventDefault();
              e.stopPropagation();
              form.handleSubmit(onSubmit)(e);
            }}
          >
            <FormField
              control={form.control}
              name="question"
              render={({ field }) => (
                <FormItem>
                  <div className="flex justify-between">
                    <FormLabel required>Câu hỏi</FormLabel>
                    <FormMessage />
                  </div>
                  <FormControl>
                    <Input placeholder="Tôi có thể..." {...field} />
                  </FormControl>
                </FormItem>
              )}
            />
            <div data-color-mode="light" className="w-full">
              <FormField
                control={form.control}
                name="answer"
                render={({ field }) => (
                  <FormItem>
                    <div className="flex justify-between">
                      <FormLabel required>Nội dung</FormLabel>
                      <FormMessage />
                    </div>
                    <FormControl>
                      <MDEditor value={field.value} onChange={field.onChange} />
                    </FormControl>
                  </FormItem>
                )}
              />
            </div>
            <div className="flex justify-end">
              <Button type="primary" htmlType="submit">
                Tạo mới
              </Button>
            </div>
          </form>
        </Form>
      </Modal>
    </div>
  );
}
