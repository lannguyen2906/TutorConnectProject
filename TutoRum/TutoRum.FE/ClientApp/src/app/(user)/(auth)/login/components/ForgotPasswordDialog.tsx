import { AlertDialogHeader } from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
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
import {
  ForgotPasswordSchema,
  ForgotPasswordType,
} from "@/utils/schemaValidations/auth.schema";
import { tConnectService } from "@/utils/services/tConnectService";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader, Loader2 } from "lucide-react";
import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

const ForgotPasswordDialog = () => {
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);

  const form = useForm<ForgotPasswordType>({
    resolver: zodResolver(ForgotPasswordSchema),
  });

  async function onSubmit(values: ForgotPasswordType) {
    setLoading(true); // Bắt đầu loading
    try {
      const { data } = await tConnectService.api.accountsForgotPasswordCreate(
        values
      );

      if (data.status == 200) {
        toast.success(data.message ?? "Vui lòng kiểm tra mail");

        setOpen(false);
      }
    } catch (err) {
      toast.error("Có lỗi xảy ra");
      console.log(err);
    } finally {
      setLoading(false); // Kết thúc loading
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger className="text-sm underline hover:text-slategray">
        Quên mật khẩu?
      </DialogTrigger>
      <DialogContent>
        <AlertDialogHeader>
          <DialogTitle>Quên mật khẩu</DialogTitle>
          <DialogDescription>
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
                <div className="flex justify-end">
                  <Button disabled={loading} type="submit">
                    {loading ? <Loader2 className="animate-spin" /> : "Gửi"}
                  </Button>
                </div>
              </form>
            </Form>
          </DialogDescription>
        </AlertDialogHeader>
      </DialogContent>
    </Dialog>
  );
};

export default ForgotPasswordDialog;
