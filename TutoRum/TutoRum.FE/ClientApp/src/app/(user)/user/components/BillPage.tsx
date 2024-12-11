"use client";

import React, { useMemo } from "react";
import { Button, Card, Spin, Tag } from "antd";
import { Check, CreditCard } from "lucide-react";
import { useAppContext } from "@/components/provider/app-provider";
import DownloadBillButton from "@/components/ui/download-bill";
import {
  useApproveBill,
  useBillDetailById,
  useBillHtmlById,
} from "@/hooks/use-billing-entry";
import { useTutorLearnerSubjectDetail } from "@/hooks/use-tutor-learner-subject";
import { tConnectService } from "@/utils/services/tConnectService";
import { toast } from "@/hooks/use-toast";
import EmailBillButton from "./EmailBillButton";

const BillPage = ({
  billId,
  classroomId,
}: {
  billId: number;
  classroomId: number;
}) => {
  const { user } = useAppContext();

  const { data: billHtml, isLoading: isHtmlLoading } = useBillHtmlById(billId);
  const { data: classroomData } = useTutorLearnerSubjectDetail(classroomId);
  const { data: billDetail } = useBillDetailById(billId);
  const { mutateAsync: approveBill, isLoading: isApproveLoading } =
    useApproveBill(billId);

  // Memoized conditions
  const isTutor = useMemo(
    () => classroomData?.tutorId === user?.id,
    [classroomData, user]
  );

  const billStatus = useMemo(() => {
    if (!billDetail?.isApprove) return { color: "red", label: "Chưa xác nhận" };
    if (!billDetail?.isPaid)
      return { color: "orange", label: "Chưa thanh toán" };
    return { color: "green", label: "Đã thanh toán" };
  }, [billDetail]);

  console.log(billDetail);

  const handlePayment = async () => {
    try {
      const response = await tConnectService.api.paymentCreatePaymentUrlCreate(
        billId
      );
      const paymentUrl = response.data.data;

      if (paymentUrl) {
        window.open(paymentUrl, "_blank");
      } else {
        throw new Error("Không thể tạo URL thanh toán.");
      }
    } catch (error) {
      console.error(error);
      toast({
        title: "Lỗi thanh toán",
        description: "Không thể tạo URL thanh toán. Vui lòng thử lại sau.",
        variant: "destructive",
      });
    }
  };

  const handleApprove = async () => {
    try {
      const response = await approveBill();
      if (response.status === 201) {
        toast({
          title: "Xác nhận thành công",
          variant: "default",
        });
      }
    } catch (error) {
      console.error(error);
      toast({
        title: "Lỗi xác nhận",
        description: "Không thể xác nhận hóa đơn. Vui lòng thử lại sau.",
        variant: "destructive",
      });
    }
  };

  if (isHtmlLoading) return <Spin />;

  return (
    <div className="space-y-2">
      {/* Header with buttons and status */}
      <div className="flex justify-between items-center">
        {!isTutor ? (
          <>
            {!billDetail?.isApprove ? (
              <Button
                type="primary"
                onClick={handleApprove}
                icon={<Check size={16} />}
                loading={isApproveLoading}
              >
                Xác nhận
              </Button>
            ) : (
              <Button
                type="primary"
                onClick={handlePayment}
                icon={<CreditCard size={16} />}
                disabled={billDetail?.isPaid!}
              >
                {billDetail?.isPaid ? "Đã thanh toán" : "Thanh toán ngay"}
              </Button>
            )}
          </>
        ) : (
          <EmailBillButton
            learnerMail={billDetail?.learnerEmail!}
            billId={billId}
            disabled={billDetail?.isApprove!}
            type="button"
          />
        )}

        <Tag color={billStatus.color}>{billStatus.label}</Tag>
        <DownloadBillButton billId={billId} />
      </div>

      {/* Bill content */}
      <Card>
        <div dangerouslySetInnerHTML={{ __html: billHtml || "" }} />
      </Card>
    </div>
  );
};

export default BillPage;
